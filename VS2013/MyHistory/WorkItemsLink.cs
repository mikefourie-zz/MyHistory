// <copyright file="WorkItemsLink.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.TeamFoundation.Controls;
    using Microsoft.VisualStudio.Shell;

    [TeamExplorerNavigationLink(WorkItemsLink.LinkId, MyHistoryNavigationItem.LinkId, 10)]
    public class WorkItemsLink : TeamExplorerBaseNavigationLink
    {
        public const string LinkId = "13BC3C19-ED66-4423-BC87-D28B9471A20E";

        [ImportingConstructor]
        public WorkItemsLink([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.Text = "Work Items";
            this.IsVisible = true;
            this.IsEnabled = true;
        }

        public override void Execute()
        {
            try
            {
                ITeamExplorer teamExplorer = GetService<ITeamExplorer>();
                if (teamExplorer != null)
                {
                    teamExplorer.NavigateToPage(new Guid(WorkItemsPage.PageId), null);
                }
            }
            catch (Exception ex)
            {
                this.ShowNotification(ex.Message, NotificationType.Error);
            }
        }

        public override void Invalidate()
        {
            base.Invalidate();
            this.IsEnabled = true;
            this.IsVisible = true;
        }
    }
}