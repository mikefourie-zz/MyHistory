// <copyright file="SummarySectionView.xaml.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.Framework.Client;
    using Microsoft.TeamFoundation.Framework.Common;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    /// <summary>
    /// Changesets Section View
    /// </summary>
    public partial class SummarySectionView
    {
        public static readonly DependencyProperty ParentSectionProperty = DependencyProperty.Register("ParentSection", typeof(SummarySection), typeof(SummarySectionView));

        public SummarySectionView()
        {
            this.InitializeComponent();
        }

        public SummarySection ParentSection
        {
            get
            {
                return (SummarySection)GetValue(ParentSectionProperty);
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

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            this.PerformSearch();
        }

        private void TextBoxSearch_KeyUp(object sender, KeyEventArgs e)
        {
            this.TextBoxSearch.Foreground = Brushes.Black;

            if (e.Key == Key.Enter)
            {
                this.PerformSearch();
            }
        }

        private void PerformSearch()
        {
            this.TextBoxSearch.Foreground = Brushes.Black;

            if (!string.IsNullOrWhiteSpace(this.TextBoxSearch.Text))
            {
                ITeamFoundationContext context = this.ParentSection.GetContext();
                IIdentityManagementService ims = context.TeamProjectCollection.GetService<IIdentityManagementService>();

                // First try search by AccountName 
                TeamFoundationIdentity userIdentity = ims.ReadIdentity(IdentitySearchFactor.AccountName, this.TextBoxSearch.Text, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties);
                if (userIdentity == null)
                {
                    // Next we try search by DisplayName
                    userIdentity = ims.ReadIdentity(IdentitySearchFactor.DisplayName, this.TextBoxSearch.Text, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties);
                    if (userIdentity == null)
                    {
                        this.TextBoxSearch.Foreground = Brushes.DarkRed;
                        return;
                    }
                }

                this.ParentSection.UserAccountName = this.TextBoxSearch.Text;
                this.ParentSection.UserDisplayName = userIdentity.DisplayName;
            }
            else
            {
                this.ParentSection.UserAccountName = "@Me";
                this.ParentSection.UserDisplayName = string.Empty;
            }

            this.ParentSection.Refresh();
        }

        private void HistoryLink_Click(object sender, RoutedEventArgs e)
        {
            this.ParentSection.ViewHistory();
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
