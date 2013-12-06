// <copyright file="WorkItemsFullSectionView.xaml.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    /// <summary>
    /// WorkItemsSectionView
    /// </summary>
    public partial class WorkItemsFullSectionView
    {
        public static readonly DependencyProperty ParentSectionProperty = DependencyProperty.Register("ParentSection", typeof(WorkItemsFullSection), typeof(WorkItemsFullSectionView));

        public WorkItemsFullSectionView()
        {
            this.InitializeComponent();
        }

        public int SelectedIndex
        {
            get
            {
                return workItemList.SelectedIndex;
            }

            set
            {
                workItemList.SelectedIndex = value;
                workItemList.ScrollIntoView(workItemList.SelectedItem);
            }
        }

        public WorkItemsFullSection ParentSection
        {
            get
            {
                return (WorkItemsFullSection)GetValue(ParentSectionProperty);
            }

            set
            {
                SetValue(ParentSectionProperty, value);
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
