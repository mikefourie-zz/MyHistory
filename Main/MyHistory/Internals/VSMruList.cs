// <copyright file="VSMruList.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using System;

    internal class VSMruList : IComparable
    {
        public string ProjectName { get; set; }

        public string ProjectPath { get; set; }

        public int CompareTo(object obj)
        {
            VSMruList tempO = obj as VSMruList;
            if (tempO == null)
            {
                throw new ArgumentException("Object is not VSMruList");
            }

            return string.Compare(this.ProjectName, tempO.ProjectName, StringComparison.Ordinal);
        }
    }
}
