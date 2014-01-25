// <copyright file="ChangesetsView.xaml.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using System;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using Microsoft.TeamFoundation.VersionControl.Client;

    /// <summary>
    /// Changesets Section View
    /// </summary>
    public partial class ChangesetsView
    {
        public static readonly DependencyProperty ParentSectionProperty = DependencyProperty.Register("ParentSection", typeof(ChangesetsPage), typeof(ChangesetsView));

        public ChangesetsView()
        {
            this.InitializeComponent();
        }

        public ChangesetsPage ParentSection
        {
            get
            {
                return (ChangesetsPage)GetValue(ParentSectionProperty);
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

    /// <summary>
    /// Changeset comment converter class.
    /// </summary>
    public class ChangesetCommentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string comment = (value is string) ? (string)value : string.Empty;
            StringBuilder sb = new StringBuilder(comment);
            sb.Replace('\r', ' ');
            sb.Replace('\n', ' ');
            sb.Replace('\t', ' ');

            if (sb.Length > 50)
            {
                sb.Remove(50, sb.Length - 50);
                sb.Append("...");
            }

            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// NameShortener class.
    /// </summary>
    public class NameShortener : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string comment = (value is string) ? (string)value : string.Empty;
            StringBuilder sb = new StringBuilder(comment);
            sb.Replace('\r', ' ');
            sb.Replace('\n', ' ');
            sb.Replace('\t', ' ');

            if (sb.Length > 13)
            {
                sb.Remove(13, sb.Length - 13);
                sb.Append("...");
            }

            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
