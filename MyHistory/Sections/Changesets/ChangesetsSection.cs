// <copyright file="ChangesetsSection.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using EnvDTE80;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.Controls;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.TeamFoundation.VersionControl;

    [TeamExplorerSection(ChangesetsSection.SectionId, MyHistoryPage.PageId, 20)]
    public class ChangesetsSection : TeamExplorerBaseSection
    {
        public const string SectionId = "97D86431-C756-49E8-9E4E-1DF55DF06778";
        private ObservableCollection<Changeset> changesets = new ObservableCollection<Changeset>();

        public ChangesetsSection()
        {
            this.Title = "Changesets";
            this.IsVisible = true;
            this.IsExpanded = true;
            this.IsBusy = false;
            this.SectionContent = new ChangesetsSectionView();
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

        protected ChangesetsSectionView View
        {
            get { return this.SectionContent as ChangesetsSectionView; }
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

        public void ViewChangesetDetails(int changesetId)
        {
            ITeamExplorer teamExplorer = this.GetService<ITeamExplorer>();
            if (teamExplorer != null)
            {
                teamExplorer.NavigateToPage(new Guid(TeamExplorerPageIds.ChangesetDetails), changesetId);
            }
        }

        public async override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);

            // If the user navigated back to this page, there could be saved context information that is passed in
            var sectionContext = e.Context as ChangesSectionContext;
            if (sectionContext != null)
            {
                // Restore the context instead of refreshing
                ChangesSectionContext context = sectionContext;
                this.Changesets = context.Changesets;
            }
            else
            {
                // Kick off the refresh
                await this.RefreshAsync();
            }
        }

        /// <summary>
        /// Refresh override.
        /// </summary>
        public async override void Refresh()
        {
            base.Refresh();
            await this.RefreshAsync();
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
                    string path = "$/" + context.TeamProjectName;
                    string user;
                    int maxCount;
                    this.GetHistoryParameters(vcs, out user, out maxCount);

                    VersionControlExt vc = GetVersionControlExt(this.ServiceProvider);
                    if (vc != null)
                    {
                        vc.History.Show(path, VersionSpec.Latest, 0, RecursionType.Full, user, null, null, int.MaxValue, true);
                    }
                }
            }
        }

        /// <summary>
        /// Save contextual information about the current section state.
        /// </summary>
        public override void SaveContext(object sender, SectionSaveContextEventArgs e)
        {
            base.SaveContext(sender, e);

            // Save our current so when the user navigates back to the page the content is restored rather than requeried
            ChangesSectionContext context = new ChangesSectionContext { Changesets = this.Changesets };
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
                await this.RefreshAsync();
            }
        }

        /// <summary>
        /// Get the parameters for the history query.
        /// </summary>
        private void GetHistoryParameters(VersionControlServer vcs, out string user, out int maxCount)
        {
            user = vcs.AuthorizedUser;
            maxCount = 10;
        }

        private async Task RefreshAsync()
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
                            GetHistoryParameters(vcs, out user, out maxCount);

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
    }
}
