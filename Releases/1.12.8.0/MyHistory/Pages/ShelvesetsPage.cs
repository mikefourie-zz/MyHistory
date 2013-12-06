// <copyright file="ShelvesetsPage.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using Microsoft.TeamFoundation.Controls;

    /// <summary>
    /// We are extending Team Explorer by adding a new page and therefore use the TeamExplorerPage attribute and pass in our unique ID
    /// </summary>
    [TeamExplorerPage(ShelvesetsPage.PageId)]
    public class ShelvesetsPage : TeamExplorerBasePage
    {
        // All Pages must have a unique ID. Use the Tools - Create GUID menu in Visual Studio to create your own GUID
        public const string PageId = "C5B94742-E81A-4154-BB02-9B1E2E5AAC74";

        public ShelvesetsPage()
        {
            // Set the page title
            this.Title = "My History - Shelvesets";
        }
    }
}