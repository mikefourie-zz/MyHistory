// <copyright file="WorkItemsPage.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using Microsoft.TeamFoundation.Controls;

    /// <summary>
    /// We are extending Team Explorer by adding a new page and therefore use the TeamExplorerPage attribute and pass in our unique ID
    /// </summary>
    [TeamExplorerPage(WorkItemsPage.PageId)]
    public class WorkItemsPage : TeamExplorerBasePage
    {
        // All Pages must have a unique ID. Use the Tools - Create GUID menu in Visual Studio to create your own GUID
        public const string PageId = "E47AACBD-4AC1-4A9B-9F96-9D2256D5B1E4";

        public WorkItemsPage()
        {
            // Set the page title
            this.Title = "My History - WorkItems";
        }
    }
}