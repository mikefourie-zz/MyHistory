// <copyright file="ProjectsAndSolutionsSection.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>

namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using Microsoft.TeamFoundation.Controls;

    [TeamExplorerSection(ProjectsAndSolutionsSection.SectionId, MyHistoryPage.PageId, 40)]
    public class ProjectsAndSolutionsSection : TeamExplorerBaseSection
    {
        public const string SectionId = "8682B106-FBCC-4A15-BD3A-284016D29955";

        public ProjectsAndSolutionsSection()
        {
            this.Title = "Projects and Solutions";
            this.SectionContent = new ProjectsAndSolutionsSectionView();
        }

        public void ShowError(string message)
        {
            this.ShowNotification(message, NotificationType.Error);
        }
    }
}
