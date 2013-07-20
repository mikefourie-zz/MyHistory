// <copyright file="ShelvesetsFullSectionView.xaml.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.TeamFoundation.VersionControl.Client;

    /// <summary>
    /// Shelvesets Section View
    /// </summary>
    public partial class ShelvesetsFullSectionView
    {
        public static readonly DependencyProperty ParentSectionProperty = DependencyProperty.Register("ParentSection", typeof(ShelvesetsFullSection), typeof(ShelvesetsFullSectionView));

        public ShelvesetsFullSectionView()
        {
            this.InitializeComponent();
        }

        public ShelvesetsFullSection ParentSection
        {
            get
            {
                return (ShelvesetsFullSection)GetValue(ParentSectionProperty);
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
                return ListViewShelvesets.SelectedIndex;
            }

            set
            {
                ListViewShelvesets.SelectedIndex = value;
                ListViewShelvesets.ScrollIntoView(ListViewShelvesets.SelectedItem);
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
    }
}
