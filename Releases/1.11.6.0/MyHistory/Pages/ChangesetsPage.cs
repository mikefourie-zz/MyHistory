// <copyright file="ChangesetsPage.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using Microsoft.TeamFoundation.Controls;

    /// <summary>
    /// We are extending Team Explorer by adding a new page and therefore use the TeamExplorerPage attribute and pass in our unique ID
    /// </summary>
    [TeamExplorerPage(ChangesetsPage.PageId)]
    public class ChangesetsPage : TeamExplorerBasePage
    {
        // All Pages must have a unique ID. Use the Tools - Create GUID menu in Visual Studio to create your own GUID
        public const string PageId = "234C4E71-D513-40CC-8CF0-AAB67DAD2C1E";

        public ChangesetsPage()
        {
            // Set the page title
            this.Title = "My History - Changesets";
        }
    }
}