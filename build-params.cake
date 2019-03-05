const string BuildConfiguration = "Release";
const string SolutionFilePath = "./src/EFCore.Toolbox.sln";

const string MigratorProjectPath = "./src/Migrator";
const string MigratorOutputPath = MigratorProjectPath + "/Migrator.Core/bin/Release/netcoreapp2.1/publish";
const string ToolboxExtensionProjectPath = "./src/Toolbox.Extension";

const string ArtifactsDir = "./src/temp";
const string OutputDir = "./output";

readonly string[] MigratorFileNames = new [] {
  "Migrator.Core.deps.json",
  "Migrator.Core.dll",
  "Migrator.Core.runtimeconfig.json",
  "Migrator.Logic.dll"
};
readonly string[] MigratorLibsFileNames = new [] {
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