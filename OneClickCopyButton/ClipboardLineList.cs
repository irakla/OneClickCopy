using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneClickCopy
{
    public class ClipboardLineList
    {
        private List<OwnCopyLinePanel> clipboardLineList = new List<OwnCopyLinePanel>();
        private int minimumLine = 1;

        public List<OwnCopyLinePanel> ClipboardLines { get => clipboardLineList; }

        public ClipboardLineList() =>
            InitializeList();

        private void InitializeList()
        {
            for (int i = 0; i < minimumLine; i++)
                clipboardLineList.Add(new OwnCopyLinePanel());
        }
    }
}