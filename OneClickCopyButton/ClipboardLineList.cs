using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneClickCopy
{
    public class ClipboardLineList : ObservableCollection<ClipboardLinePanel>
    {
        private List<ClipboardLinePanel> clipboardLineList = new List<ClipboardLinePanel>();
        private int minimumLine = 1;

        public List<ClipboardLinePanel> ClipboardLines { get => clipboardLineList; }

        public ClipboardLineList() =>
            InitializeList();

        private void InitializeList()
        {
            for (int i = 0; i < minimumLine; i++)
                clipboardLineList.Add(new ClipboardLinePanel());
        }
    }
}