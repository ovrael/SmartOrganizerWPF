using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SmartOrganizerWPF.Interfaces
{
    public interface IExplorerTreeItem
    {
        //public bool IsChecked { get; set; }
        public StackPanel CreateTreeItemContent();
    }
}
