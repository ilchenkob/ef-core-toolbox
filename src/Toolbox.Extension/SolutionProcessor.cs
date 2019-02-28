using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Project = EnvDTE.Project;
using System.IO;

namespace Toolbox.Extension
{
    internal class SolutionProcessor : IProjectBuilder
    {
        private const string DefaultBuildConfiguration = "Debug";
        private const string ProjectFolderID = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";

        private readonly EnvDTE80.DTE2 _ide;

        public SolutionProcessor(EnvDTE80.DTE2 ide)
        {
            _ide = ide ?? throw new NullReferenceException("IDE");
        }

        public bool Build(string projectName)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            var solutionBuild = _ide.Solution?.SolutionBuild;
            if (solutionBuild != null)
            {
                solutionBuild.BuildProject(
                    solutionBuild.ActiveConfiguration?.Name ?? DefaultBuildConfiguration,
                    projectName,
                    WaitForBuildToFinish: true);

                return solutionBuild.LastBuildInfo == 0; // LastBuildInfo represents the count of projects failed to build
            }

            return false;
        }

        public IReadOnlyCollection<Logic.Models.Project> GetAllSolutionProjects()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            var projects = getProjects(_ide.Solution.Projects);
            return projects.OrderBy(p => p.DisplayName).ToList().AsReadOnly();
        }

        private List<Logic.Models.Project> getProjects(IEnumerable projects, string parentFolder = "")
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            var result = new List<Logic.Models.Project>();
            foreach (var item in projects)
            {
                var currentProject = item is ProjectItem pi ? pi.Object as Project : item as Project;
                result.AddRange(processProjectItem(currentProject, parentFolder));
            }
            return result;
        }

        private IEnumerable<Logic.Models.Project> processProjectItem(Project projectItem, string parentFolder = "")
        {
            try
            {
                Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

                if (projectItem.Kind.Equals(ProjectFolderID))
                {
                    return getProjects(projectItem.ProjectItems, $"{parentFolder}{projectItem.Name}\\");
                }
                if (!string.IsNullOrWhiteSpace(projectItem.Name) && !string.IsNullOrWhiteSpace(projectItem.FullName))
                {
                    var projectDisplayName = string.IsNullOrWhiteSpace(parentFolder)
                                                  ? projectItem.Name
                                                  : $"{parentFolder}{projectItem.Name}";

                    var projRootPath = projectItem.FullName.Remove(projectItem.FullName.LastIndexOf('\\'));
                    var projOutputPath = projectItem.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();

                    return new List<Logic.Models.Project>
                    {
                        new Logic.Models.Project
                        {
                            DisplayName = projectDisplayName,
                            UniqueName = projectItem.UniqueName,
                            Path = projRootPath,
                            DefaultNamespace = projectItem.Properties.Item("DefaultNamespace").Value.ToString(),
                            AssemblyName = projectItem.Properties.Item("AssemblyName").Value.ToString(),
                            AssemblyOutputFullPath = Path.Combine(projRootPath, projOutputPath),
                            IsSelected = false
                        }
                    };
                }
            }
            catch (Exception e)
            {
                // TODO: handle unexpected cases
                var a = e.Message;
            }

            return new List<Logic.Models.Project>();
        }
    }
}
