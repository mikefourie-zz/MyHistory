// <copyright file="MyHistoryPage.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.Controls;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TeamFoundation.VersionControl;
    using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;

    /// <summary>
    /// We are extending Team Explorer by adding a new page and therefore use the TeamExplorerPage attribute and pass in our unique ID
    /// </summary>
    [TeamExplorerPage(MyHistoryPage.PageId)]
    public class MyHistoryPage : TeamExplorerBasePage
    {
        // All Pages must have a unique ID. Use the Tools - Create GUID menu in Visual Studio to create your own GUID
        public const string PageId = "BAC5373E-1BE5-4A10-97F5-AC278CA77EDF";
        private ObservableCollection<Changeset> changesets = new ObservableCollection<Changeset>();
        private ObservableCollection<WorkItem> workItems = new ObservableCollection<WorkItem>();
        private ObservableCollection<Shelveset> shelvesets = new ObservableCollection<Shelveset>();

        public MyHistoryPage()
        {
            // Set the page title
            this.Title = "My History";
            this.PageContent = new MyHistoryPageView();
            this.View.ParentSection = this;
        }

        public ObservableCollection<Changeset> Changesets
        {
            get
            {
                return this.changesets;
            }

            protected set
            {
                this.changesets = value;
                this.RaisePropertyChanged("Changesets");
            }
        }

        public ObservableCollection<Shelveset> Shelvesets
        {
            get
            {
                return this.shelvesets;
            }

            protected set
            {
                this.shelvesets = value;
                this.RaisePropertyChanged("Shelvesets");
            }
        }

        public ObservableCollection<WorkItem> WorkItems
        {
            get
            {
                return this.workItems;
            }

            protected set
            {
                this.workItems = value;
                this.RaisePropertyChanged("WorkItems");
            }
        }

        protected MyHistoryPageView View
        {
            get { return this.PageContent as MyHistoryPageView; }
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

        public ITeamFoundationContext GetContext()
        {
            return this.CurrentContext;
        }

        public IServiceProvider GetServiceProvider()
        {
            return this.ServiceProvider;
        }

        public void ViewChangesetDetails(int changesetId)
        {
            ITeamExplorer teamExplorer = this.GetService<ITeamExplorer>();
            if (teamExplorer != null)
            {
                teamExplorer.NavigateToPage(new Guid(TeamExplorerPageIds.ChangesetDetails), changesetId);
            }
        }

        public void ViewShelvesetDetails(Shelveset shelveset)
        {
            ITeamExplorer teamExplorer = this.GetService<ITeamExplorer>();
            if (teamExplorer != null)
            {
                teamExplorer.NavigateToPage(new Guid(TeamExplorerPageIds.ShelvesetDetails), shelveset);
            }
        }

        public void ViewWorkItemDetails(int workItemId)
        {
            try
            {
                ITeamFoundationContext context = this.CurrentContext;
                EnvDTE80.DTE2 dte2 = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE)) as EnvDTE80.DTE2;
                if (dte2 != null)
                {
                    DocumentService witDocumentService = (DocumentService)dte2.DTE.GetObject("Microsoft.VisualStudio.TeamFoundation.WorkItemTracking.DocumentService");
                    var widoc = witDocumentService.GetWorkItem(context.TeamProjectCollection, workItemId, this);
                    witDocumentService.ShowWorkItem(widoc);
                }
            }
            catch (Exception ex)
            {
                this.ShowNotification(ex.Message, NotificationType.Error);
            }
        }

        public override async void Refresh()
        {
            base.Refresh();
            await this.RefreshAsyncChangesets();
            await this.RefreshAsyncShelveSets();
            await this.RefreshAsyncWorkitems();
        }

        public void ViewHistory()
        {
            ITeamFoundationContext context = this.CurrentContext;
            if (context != null && context.HasCollection && context.HasTeamProject)
            {
                VersionControlServer vcs = context.TeamProjectCollection.GetService<VersionControlServer>();
                if (vcs != null)
                {
                    // Ask the derived section for the history parameters
                    string user;
                    int maxCount;
                    this.GetHistoryParameters(vcs, out user, out maxCount);
                    string path = "$/" + context.TeamProjectName;
                    VersionControlExt vc = GetVersionControlExt(this.ServiceProvider);
                    if (vc != null)
                    {
                        vc.History.Show(path, VersionSpec.Latest, 0, RecursionType.Full, user, null, null, int.MaxValue, true);
                    }
                }
            }
        }

        public async override void Initialize(object sender, PageInitializeEventArgs e)
        {
            base.Initialize(sender, e);

            // If the user navigated back to this page, there could be saved context information that is passed in
            var sectionContext = e.Context as ChangesSectionContext;
            if (sectionContext != null)
            {
                // Restore the context instead of refreshing
                ChangesSectionContext context = sectionContext;
                this.Changesets = context.Changesets;
                this.WorkItems = context.WorkItems;
                this.Shelvesets = context.Shelvesets;
            }
            else
            {
                // Kick off the refresh
                await this.RefreshAsyncChangesets();
                await this.RefreshAsyncShelveSets();
                await this.RefreshAsyncWorkitems();
            }
        }

        /// <summary>
        /// Save contextual information about the current section state.
        /// </summary>
        public override void SaveContext(object sender, PageSaveContextEventArgs e)
        {
            base.SaveContext(sender, e);

            // Save our current so when the user navigates back to the page the content is restored rather than requeried
            ChangesSectionContext context = new ChangesSectionContext { Changesets = this.Changesets, WorkItems = this.WorkItems, Shelvesets = this.Shelvesets };
            e.Context = context;
        }

        /// <summary>
        /// ContextChanged override.
        /// </summary>
        protected override async void ContextChanged(object sender, TeamFoundation.Client.ContextChangedEventArgs e)
        {
            base.ContextChanged(sender, e);

            // If the team project collection or team project changed, refresh the data for this section
            if (e.TeamProjectCollectionChanged || e.TeamProjectChanged)
            {
                await this.RefreshAsyncChangesets();
                await this.RefreshAsyncShelveSets();
                await this.RefreshAsyncWorkitems();
            }
        }

        /// <summary>
        /// Get the parameters for the history query.
        /// </summary>
        private void GetHistoryParameters(VersionControlServer vcs, out string user, out int maxCount)
        {
            maxCount = 12;
            user = vcs.AuthorizedUser;
        }

        /// <summary>
        /// Get the parameters for the shelveset query.
        /// </summary>
        private void GetShelvesetParameters(VersionControlServer vcs, out string user)
        {
            user = vcs.AuthorizedUser;
        }

        private async Task RefreshAsyncChangesets()
        {
            try
            {
                // Set our busy flag and clear the previous data
                this.IsBusy = true;
                this.Changesets.Clear();

                ObservableCollection<Changeset> lchangesets = new ObservableCollection<Changeset>();

                // Make the server call asynchronously to avoid blocking the UI
                await Task.Run(() =>
                {
                    ITeamFoundationContext context = this.CurrentContext;
                    if (context != null && context.HasCollection && context.HasTeamProject)
                    {
                        VersionControlServer vcs = context.TeamProjectCollection.GetService<VersionControlServer>();
                        if (vcs != null)
                        {
                            // Ask the derived section for the history parameters
                            string user;
                            int maxCount;
                            this.GetHistoryParameters(vcs, out user, out maxCount);
                            string path = "$/" + context.TeamProjectName;
                            foreach (Changeset changeset in vcs.QueryHistory(path, VersionSpec.Latest, 0, RecursionType.Full, user, null, null, maxCount, false, true))
                            {
                                lchangesets.Add(changeset);
                            }
                        }
                    }
                });

                // Now back on the UI thread, update the bound collection and section title
                this.Changesets = lchangesets;
            }
            catch (Exception ex)
            {
                this.ShowNotification(ex.Message, NotificationType.Error);
            }
            finally
            {
                // Always clear our busy flag when done
                this.IsBusy = false;
            }
        }

        private async Task RefreshAsyncShelveSets()
        {
            try
            {
                // Set our busy flag and clear the previous data
                this.IsBusy = true;
                this.Shelvesets.Clear();

                ObservableCollection<Shelveset> lshelvesests = new ObservableCollection<Shelveset>();

                // Make the server call asynchronously to avoid blocking the UI
                await Task.Run(() =>
                {
                    ITeamFoundationContext context = this.CurrentContext;
                    if (context != null && context.HasCollection && context.HasTeamProject)
                    {
                        VersionControlServer vcs = context.TeamProjectCollection.GetService<VersionControlServer>();
                        if (vcs != null)
                        {
                            // Ask the derived section for the history parameters
                            string user;
                            GetShelvesetParameters(vcs, out user);
                            foreach (Shelveset shelveset in vcs.QueryShelvesets(null, user))
                            {
                                lshelvesests.Add(shelveset);
                            }
                        }
                    }
                });

                // The shelvesets are not in the right order, lets sort them by date
                var sortedShelvesets = (
                    from ss in lshelvesests
                    orderby ss.CreationDate descending
                    select ss).ToList();

                ObservableCollection<Shelveset> lshelvesests2 = new ObservableCollection<Shelveset>();
                foreach (var s in sortedShelvesets)
                {
                    lshelvesests2.Add(s);

                    // only bring back the last 15
                    if (lshelvesests2.Count >= 15)
                    {
                        break;
                    }
                }

                this.Shelvesets = lshelvesests2;
            }
            catch (Exception ex)
            {
                this.ShowNotification(ex.Message, NotificationType.Error);
            }
            finally
            {
                // Always clear our busy flag when done
                this.IsBusy = false;
            }
        }

        private async Task RefreshAsyncWorkitems()
        {
            try
            {
                // Set our busy flag and clear the previous data
                this.IsBusy = true;
                this.WorkItems.Clear();

                ObservableCollection<WorkItem> lworkItems = new ObservableCollection<WorkItem>();

                // Make the server call asynchronously to avoid blocking the UI
                await Task.Run(() =>
                {
                    ITeamFoundationContext context = this.CurrentContext;
                    if (context != null && context.HasCollection && context.HasTeamProject)
                    {
                        WorkItemStore wis = context.TeamProjectCollection.GetService<WorkItemStore>();
                        if (wis != null)
                        {
                            WorkItemCollection wic = wis.Query("SELECT [System.Id], [System.Title], [System.State] FROM WorkItems WHERE [System.WorkItemType] <> ''  AND  [System.State] <> ''  AND  [System.AssignedTo] EVER @Me ORDER BY [System.ChangedDate] desc");
                            int i = 0;
                            foreach (WorkItem wi in wic)
                            {
                                lworkItems.Add(wi);
                                i++;
                                if (i >= 12)
                                {
                                    break;
                                }
                            }
                        }
                    }
                });

                // Now back on the UI thread, update the bound collection and section title
                this.WorkItems = lworkItems;
            }
            catch (Exception ex)
            {
                this.ShowNotification(ex.Message, NotificationType.Error);
            }
            finally
            {
                // Always clear our busy flag when done
                this.IsBusy = false;
            }
        }
    }
}