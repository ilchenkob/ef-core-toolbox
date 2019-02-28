namespace Toolbox.Extension.Logic.Models
{
    public class Project
    {
        public string DisplayName { get; set; }

        public string UniqueName { get; set; }

        public string Path { get; set; }

        public string DefaultNamespace { get; set; }

        public string AssemblyName { get; set; }

        public string AssemblyOutputFullPath { get; set; }

        public bool IsSelected { get; set; }

        public string AssemblyNameWithPath =>
            System.IO.Path.Combine(AssemblyOutputFullPath, $"{AssemblyName}.dll");
    }
}
