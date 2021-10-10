using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OneClickCopy
{
    public class ClipboardLineList
    {
        private List<OwnCopyLinePanel> clipboardLineList = new List<OwnCopyLinePanel>();
        private int minimumLine = 1;

        public List<OwnCopyLinePanel> ClipboardLines { get => clipboardLineList; }
        public DataObject LatestCopy { get; private set; } = null;

        public ClipboardLineList() =>
            InitializeList();

        private void InitializeList()
        {
            for (int i = 0; i < minimumLine; i++)
            {
                OwnCopyLinePanel nowLinePanel = new OwnCopyLinePanel();
                nowLinePanel.PointedFromClipboard += GetLatestCopy;
                clipboardLineList.Add(nowLinePanel);
            }
        }

        private void GetLatestCopy(object sender, EventArgs e)
        {
            var latestCopyOwner = sender as OwnCopyLinePanel;
            
            if(latestCopyOwner != null)
                LatestCopy = latestCopyOwner.OwnCopy;
        }
    }
}