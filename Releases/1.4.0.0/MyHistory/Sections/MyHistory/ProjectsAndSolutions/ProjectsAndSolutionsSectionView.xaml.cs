// <copyright file="ProjectsAndSolutionsSectionView.xaml.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Input;
    using EnvDTE;
    using Microsoft.Win32;

    /// <summary>
    /// ProjectsAndSolutionsSectionView
    /// </summary>
    public partial class ProjectsAndSolutionsSectionView
    {
        public static readonly DependencyProperty ParentSectionProperty = DependencyProperty.Register("ParentSection", typeof(ProjectsAndSolutionsSection), typeof(ProjectsAndSolutionsSectionView));
        private ObservableCollection<VSMruList> mruList = new ObservableCollection<VSMruList>();

        public ProjectsAndSolutionsSectionView()
        {
            this.InitializeComponent();
            this.GetMRU();
        }

        /// <summary>
        /// Parent section.
        /// </summary>
        public ProjectsAndSolutionsSection ParentSection
        {
            get
            {
                return (ProjectsAndSolutionsSection)GetValue(ParentSectionProperty);
            }

            set
            {
                SetValue(ParentSectionProperty, value);
            }
        }

        private void GetMRU()
        {
            RegistryKey userkey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);
            RegistryKey mruKey = userkey.OpenSubKey(@"Software\Microsoft\VisualStudio\11.0\ProjectMRUList");
            if (mruKey != null)
            {
                foreach (string tmpKeyIndex in mruKey.GetValueNames())
                {
                    string tmpProjectPath = mruKey.GetValue(tmpKeyIndex).ToString();
                    VSMruList tmpMRU = new VSMruList { ProjectPath = Regex.Match(Environment.ExpandEnvironmentVariables(tmpProjectPath), @"[^|]{1,}").Value, ProjectName = System.IO.Path.GetFileName(Regex.Match(Environment.ExpandEnvironmentVariables(tmpProjectPath), @"[^|]{1,}").Value) };
                    this.mruList.Add(tmpMRU);
                }

                mruKey.Close();
                this.mruList.BubbleSort();
                this.ListViewMru.ItemsSource = this.mruList;
            }
        }

        private void ListViewMru_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && this.ListViewMru.SelectedItems.Count == 1)
            {
                this.LoadProject();
            }
        }

        private void ListViewMru_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && this.ListViewMru.SelectedItems.Count == 1)
            {
                this.LoadProject();
            }
        }

        private void LoadProject()
        {
            VSMruList hi = ListViewMru.SelectedItems[0] as VSMruList;
            if (hi != null)
            {
                if (File.Exists(hi.ProjectPath))
                {
                    EnvDTE80.DTE2 dte2 = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE)) as EnvDTE80.DTE2;
                    if (dte2 != null)
                    {
                        try
                        {
                            dte2.Solution.Open(hi.ProjectPath);
                        }
                        catch (Exception ex)
                        {
                            this.ParentSection.ShowError(ex.Message);
                        }
                    }
                }

                this.ParentSection.ShowError("File not found");
            }
        }
    }
}
