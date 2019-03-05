#addin nuget:?package=Cake.Json
#addin nuget:?package=Newtonsoft.Json&version=9.0.1

#load build-params.cake

class BuildSteps
{
  public const string Clean = "Clean";
  public const string RestoreNuget = "RestoreNuget";
  public const string BuildSolution = "BuildSolution";
  public const string MoveArtifactsToTempDir = "MoveArtifactsToTempDir";
  public const string UpdateManifest = "UpdateManifest";
  public const string CreateExtensionPackage = "CreateExtensionPackage";
}

string packageName = "";

Setup(context =>
{
  var projectInfo = ParseProject($"{ToolboxExtensionProjectPath}/Toolbox.Extension.csproj");
  packageName = projectInfo.AssemblyName;
});

Task(BuildSteps.Clean)
  .Does(() =>
{
  CleanDirectory(ArtifactsDir);
  CleanDirectory(OutputDir);

  CleanDirectories($"{MigratorProjectPath}/**/bin");
  CleanDirectories($"{MigratorProjectPath}/**/obj");

  CleanDirectories($"{ToolboxExtensionProjectPath}/bin");
  CleanDirectories($"{ToolboxExtensionProjectPath}/obj");
});

Task(BuildSteps.RestoreNuget)
  .IsDependentOn(BuildSteps.Clean)
  .Does(() => NuGetRestore(SolutionFilePath));

Task(BuildSteps.BuildSolution)
  .IsDependentOn(BuildSteps.RestoreNuget)
  .Does(() =>
{
  DotNetCorePublish($"{MigratorProjectPath}/Migrator.Core/Migrator.Core.csproj", new DotNetCorePublishSettings
  {
    Framework = "netcoreapp2.1",
    Configuration = BuildConfiguration,
    SelfContained = false,
    Runtime = "win-x64",
    OutputDirectory = MigratorOutputPath
  });

  MSBuild($"{ToolboxExtensionProjectPath}/Toolbox.Extension.csproj", settings => settings
    .SetConfiguration(BuildConfiguration)
    .SetPlatformTarget(PlatformTarget.MSIL)
    .SetMSBuildPlatform(MSBuildPlatform.x86)
    .UseToolVersion(MSBuildToolVersion.VS2017)
    .WithTarget("Build")
    .WithProperty("DeployExtension", "false"));
});

Task(BuildSteps.MoveArtifactsToTempDir)
  .IsDependentOn(BuildSteps.BuildSolution)
  .Does(() => Unzip($"{ToolboxExtensionProjectPath}/bin/{BuildConfiguration}/{packageName}.vsix", ArtifactsDir))
  .DoesForEach(MigratorFileNames, (file) => CopyFile(MigratorOutputPath + $"/{file}", ArtifactsDir + $"/{file}"))
  .DoesForEach(MigratorLibsFileNames, (file) => CopyFile($"./src/Migrator/libs/{file}", ArtifactsDir + $"/{file}"));

Task(BuildSteps.UpdateManifest)
  .IsDependentOn(BuildSteps.MoveArtifactsToTempDir)
  .Does(() =>
{
  var manifestFilePath = $"{ArtifactsDir}/manifest.json";

  var manifestJson = ParseJsonFromFile(manifestFilePath);
  var files = manifestJson["files"].ToList();

  foreach (var file in MigratorFileNames)
    files.Add(JToken.FromObject(new ManifestFileItem { fileName = $"/{file}" }));
  foreach (var file in MigratorLibsFileNames)
    files.Add(JToken.FromObject(new ManifestFileItem { fileName = $"/{file}" }));

  manifestJson["files"] = JToken.FromObject(files);
  SerializeJsonToFile(manifestFilePath, manifestJson);
});

Task(BuildSteps.CreateExtensionPackage)
  .IsDependentOn(BuildSteps.UpdateManifest)
  .Does(() => Zip(
      ArtifactsDir, $"{OutputDir}/{packageName}.vsix", GetFiles($"{ArtifactsDir}/*.*"))
  );

Teardown(context => DeleteDirectory(ArtifactsDir, new DeleteDirectorySettings
  {
    Recursive = true,
    Force = true
  })
);

RunTarget(BuildSteps.CreateExtensionPackage);