// <copyright file="ChangesSectionContext.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using System.Collections.ObjectModel;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    /// <summary>
    /// Class to preserve the contextual information for this section.
    /// </summary>
    internal class ChangesSectionContext
    {
        public ObservableCollection<Changeset> Changesets { get; set; }

        public ObservableCollection<WorkItem> WorkItems { get; set; }

        public ObservableCollection<Shelveset> Shelvesets { get; set; }
    }
}
