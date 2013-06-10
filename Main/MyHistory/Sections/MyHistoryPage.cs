// <copyright file="MyHistoryPage.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using Microsoft.TeamFoundation.Controls;

    /// <summary>
    /// MyHistory Page. We are extending Team Explorer by adding a new page and therefore use the TeamExplorerPage attribute and pass in our unique ID
    /// </summary>
    [TeamExplorerPage(MyHistoryPage.PageId)]
    public class MyHistoryPage : TeamExplorerBasePage
    {
        // All Pages must have a unique ID. Use the Tools - Create GUID menu in Visual Studio to create your own GUID
        public const string PageId = "BAC5373E-1BE5-4A10-97F5-AC278CA77EDF";

        public MyHistoryPage()
        {
            // Set the page title
            this.Title = "My History";
        }
    }
}