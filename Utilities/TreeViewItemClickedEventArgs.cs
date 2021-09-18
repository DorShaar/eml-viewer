using System;
using System.Windows.Controls;

namespace Utilities
{
    public class TreeViewItemClickedEventArgs : EventArgs
    {
        public TreeViewItem TreeViewItem { get; set; }
    }
}