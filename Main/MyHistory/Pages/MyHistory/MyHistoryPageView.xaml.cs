// <copyright file="MyHistoryPageView.xaml.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;
    using EnvDTE80;
    using Microsoft.TeamFoundation.Controls;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TeamFoundation.VersionControl;

    /// <summary>
    /// Changesets Section View
    /// </summary>
    public partial class MyHistoryPageView
    {
        public static readonly DependencyProperty ParentSectionProperty = DependencyProperty.Register("ParentSection", typeof(MyHistoryPage), typeof(MyHistoryPageView));
        public List<string> NamesList = new List<string>();

        public MyHistoryPageView()
        {
            this.InitializeComponent();
        }

        public MyHistoryPage ParentSection
        {
            get { return (MyHistoryPage)GetValue(ParentSectionProperty); }

            set { SetValue(ParentSectionProperty, value); }
        }

        public int SelectedIndex
        {
            get
            {
                return changesetList.SelectedIndex;
            }

            set
            {
                changesetList.SelectedIndex = value;
                changesetList.ScrollIntoView(changesetList.SelectedItem);
            }
        }

        public static VersionControlExt GetVersionControlExt(IServiceProvider serviceProvider)
        {
            if (serviceProvider != null)
            {
                DTE2 dte = serviceProvider.GetService(typeof(SDTE)) as DTE2;
                if (dte != null)
                {
                    return dte.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as VersionControlExt;
                }
            }

            return null;
        }

        private void ColleagueLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ITeamExplorer teamExplorer = this.ParentSection.GetService<ITeamExplorer>();
                if (teamExplorer != null)
                {
                    teamExplorer.NavigateToPage(new Guid(ColleagueHistoryPage.PageId), null);
                }
            }
            catch (Exception ex)
            {
                this.ParentSection.ShowNotification(ex.Message, NotificationType.Error);
            }
        }

        private void HistoryLink_Click(object sender, RoutedEventArgs e)
        {
            this.ParentSection.ViewHistory();
        }

        private void OptionsLink_Click(object sender, RoutedEventArgs e)
        {
            this.ParentSection.ShowNotification("Patience... Options in development ;-)", NotificationType.Information);
        }

        private void WorkItemLink_Click(object sender, RoutedEventArgs e)
        {
            ITeamExplorer teamExplorer = this.ParentSection.GetService<ITeamExplorer>();
            if (teamExplorer != null)
            {
                teamExplorer.NavigateToPage(new Guid(WorkItemsPage.PageId), null);
            }
        }

        private void ShelvesetsLink_Click(object sender, RoutedEventArgs e)
        {
            ITeamExplorer teamExplorer = this.ParentSection.GetService<ITeamExplorer>();
            if (teamExplorer != null)
            {
                teamExplorer.NavigateToPage(new Guid(ShelvesetsPage.PageId), null);
            }
        }

        private void ChangesetList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && changesetList.SelectedItems.Count == 1)
            {
                this.ViewChangesetDetails();
            }
        }

        private void ChangesetList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && changesetList.SelectedItems.Count == 1)
            {
                this.ViewChangesetDetails();
            }
        }

        private void ViewChangesetDetails()
        {
            if (changesetList.SelectedItems.Count == 1)
            {
                Changeset changeset = changesetList.SelectedItems[0] as Changeset;
                if (changeset != null)
                {
                    this.ParentSection.ViewChangesetDetails(changeset.ChangesetId);
                }
            }
        }

        private void ListViewShelvesets_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && ListViewShelvesets.SelectedItems.Count == 1)
            {
                this.ViewShelvesetDetails();
            }
        }

        private void ListViewShelvesets_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && ListViewShelvesets.SelectedItems.Count == 1)
            {
                this.ViewShelvesetDetails();
            }
        }

        private void ViewShelvesetDetails()
        {
            if (ListViewShelvesets.SelectedItems.Count == 1)
            {
                Shelveset shelveset = ListViewShelvesets.SelectedItems[0] as Shelveset;
                if (shelveset != null)
                {
                    this.ParentSection.ViewShelvesetDetails(shelveset);
                }
            }
        }

        private void WorkItemList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && this.workItemList.SelectedItems.Count == 1)
            {
                this.ViewWorkItemDetails();
            }
        }

        private void WorkItemList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && this.workItemList.SelectedItems.Count == 1)
            {
                this.ViewWorkItemDetails();
            }
        }

        private void ViewWorkItemDetails()
        {
            if (this.workItemList.SelectedItems.Count == 1)
            {
                WorkItem wi = this.workItemList.SelectedItems[0] as WorkItem;
                if (wi != null)
                {
                    this.ParentSection.ViewWorkItemDetails(wi.Id);
                }
            }
        }
    }
}
