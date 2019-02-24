﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Toolbox.Extension.Logic.Models;
using EnvDTE;
using Project = EnvDTE.Project;

namespace Toolbox.Extension
{
    internal class SolutionProcessor
    {
        private const string ProjectFolderID = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";

        private readonly EnvDTE80.DTE2 _ide;

        public SolutionProcessor(EnvDTE80.DTE2 ide)
        {
            _ide = ide ?? throw new NullReferenceException("IDE");
        }

        //private IEnumerable<ProjectItem> getAllProjectItems(ProjectItems projectItems)
        //{
        //    foreach (ProjectItem item in projectItems)
        //    {
        //        yield return item;

        //        if (item.SubProject != null)
        //        {
        //            foreach (var childItem in getAllProjectItems(item.SubProject.ProjectItems))
        //                yield return childItem;
        //        }
        //        else
        //        {
        //            foreach (var childItem in getAllProjectItems(item.ProjectItems))
        //                yield return childItem;
        //        }
        //    }
        //}

        public IReadOnlyCollection<Logic.Models.Project> GetAllSolutionProjects()
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            var projects = getProjects(_ide.Solution.Projects);
            return projects.OrderBy(p => p.Name).ToList().AsReadOnly();
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

                    return new List<Logic.Models.Project>
                    {
                        new Logic.Models.Project
                        {
                            DisplayName = projectDisplayName,
                            Name = projectItem.Name,
                            Path = projectItem.FullName.Remove(projectItem.FullName.LastIndexOf('\\')),
                            DefaultNamespace = projectItem.Properties.Item("DefaultNamespace").Value.ToString(),
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