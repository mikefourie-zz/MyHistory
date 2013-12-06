// <copyright file="ShelvesetsLink.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.TeamFoundation.Controls;
    using Microsoft.VisualStudio.Shell;

    [TeamExplorerNavigationLink(ShelvesetsLink.LinkId, MyHistoryNavigationItem.LinkId, 30)]
    public class ShelvesetsLink : TeamExplorerBaseNavigationLink
    {
        public const string LinkId = "e49a882b-1677-46a9-93b4-db290943bbcd";

        [ImportingConstructor]
        public ShelvesetsLink([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.Text = "Shelvesets";
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
                    teamExplorer.NavigateToPage(new Guid(ShelvesetsPage.PageId), null);
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