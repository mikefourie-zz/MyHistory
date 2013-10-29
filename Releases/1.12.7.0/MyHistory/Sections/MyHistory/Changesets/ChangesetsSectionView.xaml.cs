// <copyright file="ChangesetsSectionView.xaml.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.TeamFoundation.VersionControl.Client;

    /// <summary>
    /// Changesets Section View
    /// </summary>
    public partial class ChangesetsSectionView
    {
        public static readonly DependencyProperty ParentSectionProperty = DependencyProperty.Register("ParentSection", typeof(ChangesetsSection), typeof(ChangesetsSectionView));

        public ChangesetsSectionView()
        {
            this.InitializeComponent();
        }

        public ChangesetsSection ParentSection
        {
            get
            {
                return (ChangesetsSection)GetValue(ParentSectionProperty);
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

        protected void GetHistoryParameters(VersionControlServer vcs, out string user, out int maxCount)
        {
            user = vcs.AuthorizedUser;
            maxCount = 15;
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
    }
}
