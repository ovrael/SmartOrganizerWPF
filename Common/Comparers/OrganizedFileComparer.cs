using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartOrganizerWPF.Common.Comparers
{
    public class OrganizedFileComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x == null || y == null) return 0;

            if (x.Contains("Other")) return -1;

            return x.CompareTo(y);
        }
    }
}
