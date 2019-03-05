#addin nuget:?package=Cake.Json
#addin nuget:?package=Newtonsoft.Json&version=9.0.1

const string BuildConfiguration = "Release";
const string SolutionFilePath = "./src/EFCore.Toolbox.sln";

const string MigratorProjectPath = "./src/Migrator";
const string MigratorOutputPath = MigratorProjectPath + "/Migrator.Core/bin/Release/netcoreapp2.1/publish";
const string ToolboxExtensionProjectPath = "./src/Toolbox.Extension";

const string ArtifactsDir = "./src/temp";
const string OutputDir = "./output";

string[] MigratorFileNames = new [] {
  "Migrator.Core.deps.json",
  "Migrator.Core.dll",
  "Migrator.Core.runtimeconfig.json",
  "Migrator.Logic.dll"
};
string[] MigratorLibsFileNames = new [] {
  "hostfxr.dll",
  "hostpolicy.dll",
  "sni.dll",
  "System.Text.Encoding.CodePages.dll"
};

class ManifestFileItem
{
  public string fileName { get; set; }
  public string sha256 => null;
}

class BuildSteps
{
  public const string Clean = "Clean";
  public const string RestoreNuget = "RestoreNuget";
  public const string BuildSolution = "BuildSolution";
  public const string MoveArtifactsToTempDir = "MoveArtifactsToTempDir";
  public const string UpdateManifest = "UpdateManifest";
  public const string CreateExtensionPackage = "CreateExtensionPackage";
}

Task(BuildSteps.Clean)
  .Does(() =>
{
  CleanDirectory(ArtifactsDir);
  CleanDirectory(OutputDir);

  CleanDirectories(MigratorProjectPath + "/**/bin");
  CleanDirectories(MigratorProjectPath + "/**/obj");

  CleanDirectories(ToolboxExtensionProjectPath + "/bin");
  CleanDirectories(ToolboxExtensionProjectPath + "/obj");
});

Task(BuildSteps.RestoreNuget)
  .IsDependentOn(BuildSteps.Clean)
  .Does(() => NuGetRestore(SolutionFilePath));

Task(BuildSteps.BuildSolution)
  .IsDependentOn(BuildSteps.RestoreNuget)
  .Does(() =>
{
  DotNetCorePublish(MigratorProjectPath + "/Migrator.Core/Migrator.Core.csproj", new DotNetCorePublishSettings
  {
    Framework = "netcoreapp2.1",
    Configuration = BuildConfiguration,
    SelfContained = false,
    Runtime = "win-x64",
    OutputDirectory = MigratorOutputPath
  });

  MSBuild(ToolboxExtensionProjectPath + "/Toolbox.Extension.csproj", settings => settings
    .SetConfiguration(BuildConfiguration)
    .SetPlatformTarget(PlatformTarget.MSIL)
    .SetMSBuildPlatform(MSBuildPlatform.x86)
    .UseToolVersion(MSBuildToolVersion.VS2017)
    .WithTarget("Build")
    .WithProperty("DeployExtension", "false"));
});

Task(BuildSteps.MoveArtifactsToTempDir)
  .IsDependentOn(BuildSteps.BuildSolution)
  .Does(() => Unzip(ToolboxExtensionProjectPath + $"/bin/{BuildConfiguration}/Toolbox.Extension.vsix", ArtifactsDir))
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
      ArtifactsDir,
      $"{OutputDir}/Toolbox.Extension.zip",
      GetFiles($"{ArtifactsDir}/*.*"))
  );

Teardown(context => DeleteDirectory(ArtifactsDir, new DeleteDirectorySettings
  {
    Recursive = true,
    Force = true
  })
);

RunTarget(BuildSteps.CreateExtensionPackage);