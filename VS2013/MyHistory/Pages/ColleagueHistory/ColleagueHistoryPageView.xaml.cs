// <copyright file="ColleagueHistoryPageView.xaml.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.Controls;
    using Microsoft.TeamFoundation.Framework.Client;
    using Microsoft.TeamFoundation.Framework.Common;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    /// <summary>
    /// Changesets Section View
    /// </summary>
    public partial class ColleagueHistoryPageView
    {
        public static readonly DependencyProperty ParentSectionProperty = DependencyProperty.Register("ParentSection", typeof(ColleagueHistoryPage), typeof(ColleagueHistoryPageView));
        public List<string> NamesList = new List<string>();
        
        public ColleagueHistoryPageView()
        {
            this.InitializeComponent();
        }

        public ColleagueHistoryPage ParentSection
        {
            get
            {
                return (ColleagueHistoryPage)GetValue(ParentSectionProperty);
            }

            set
            {
                SetValue(ParentSectionProperty, value);
            }
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

        private void NamesCombo_KeyUp(object sender, KeyEventArgs e)
        {
            this.NamesCombo.Foreground = Brushes.Black;

            if (e.Key == Key.Enter)
            {
                this.PerformSearch();
                return;
            }

            if (this.NamesList.Count == 0)
            {
                this.NamesList = this.GetRecentUsers();
                this.NamesCombo.ItemsSource = this.NamesList;
            }
        }

        private List<string> GetRecentUsers()
        {
            List<string> names = new List<string>();
            ITeamFoundationContext context = this.ParentSection.GetContext();
            if (context != null && context.HasCollection && context.HasTeamProject)
            {
                // first we get the recent users who committed code changes
                VersionControlServer vcs = context.TeamProjectCollection.GetService<VersionControlServer>();
                if (vcs != null)
                {
                    // Ask the derived section for the history parameters
                    string path = "$/" + context.TeamProjectName;
                    foreach (Changeset changeset in vcs.QueryHistory(path, VersionSpec.Latest, 0, RecursionType.Full, string.Empty, null, null, 250, false, true, false).Cast<Changeset>().Where(changeset => !names.Contains(changeset.CommitterDisplayName)))
                    {
                        names.Add(changeset.CommitterDisplayName);
                    }
                }

                // next we  get the users who recently committed work item changes
                WorkItemStore wis = context.TeamProjectCollection.GetService<WorkItemStore>();
                if (wis != null)
                {
                    WorkItemCollection wic = wis.Query("SELECT [System.Id] FROM WorkItems WHERE [System.WorkItemType] <> ''  AND  [System.State] <> '' AND [System.TeamProject] = '" + context.TeamProjectName + "' ORDER BY [System.ChangedDate] desc");
                    
                    int i = 0;
                    foreach (WorkItem wi in wic)
                    {
                        if (!names.Contains(wi.ChangedBy))
                        {
                            names.Add(wi.ChangedBy);
                        }

                        i++;
                        if (i >= 250)
                        {
                            break;
                        }
                    }
                }

                names.Sort();
            }

            return names;
        }

        private void PerformSearch()
        {
            if (!string.IsNullOrWhiteSpace(this.NamesCombo.Text))
            {
                ITeamFoundationContext context = this.ParentSection.GetContext();
                IIdentityManagementService ims = context.TeamProjectCollection.GetService<IIdentityManagementService>();

                try
                {
                    // First try search by AccountName 
                    TeamFoundationIdentity userIdentity = ims.ReadIdentity(IdentitySearchFactor.AccountName, this.NamesCombo.Text, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties);
                    if (userIdentity == null)
                    {
                        // Next we try search by DisplayName
                        userIdentity = ims.ReadIdentity(IdentitySearchFactor.DisplayName, this.NamesCombo.Text, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties);
                        if (userIdentity == null)
                        {
                            this.NamesCombo.Foreground = Brushes.DarkRed;
                            return;
                        }
                    }
                    
                    if (!string.IsNullOrWhiteSpace(this.NamesCombo.Text))
                    {
                        this.SaveNameToCache();
                    }

                    this.ParentSection.UserAccountName = this.NamesCombo.Text;
                    this.ParentSection.UserDisplayName = userIdentity.DisplayName;
                }
                catch (Exception)
                {
                    this.NamesCombo.Foreground = Brushes.DarkRed;
                    return;
                }
            }
            else
            {
                this.ParentSection.UserAccountName = string.Empty;
                this.ParentSection.UserDisplayName = string.Empty;
            }

            this.ParentSection.Refresh();
        }

        private void SaveNameToCache()
        {
            this.NamesList.Add(this.NamesCombo.Text);
            this.NamesList.Sort();
        }
        
        private void MyLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ITeamExplorer teamExplorer = this.ParentSection.GetService<ITeamExplorer>();
                if (teamExplorer != null)
                {
                    teamExplorer.NavigateToPage(new Guid(MyHistoryPage.PageId), null);
                }
            }
            catch (Exception ex)
            {
                this.ParentSection.ShowNotification(ex.Message, NotificationType.Error);
            }
        }

        private void ReportLink_Click(object sender, RoutedEventArgs e)
        {
            this.ParentSection.ShowNotification("Patience... Feature in development ;-)", NotificationType.Information);
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

        private void NamesCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.PerformSearch();
        }
    }
}
