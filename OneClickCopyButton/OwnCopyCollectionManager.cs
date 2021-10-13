using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OneClickCopy
{
    public class OwnCopyCollectionManager
    {
        private ObservableCollection<OwnCopyData> _ownCopyCollection = new ObservableCollection<OwnCopyData>();
        private int minimumLine = 1;

        private OwnCopyData _latestCopyData = null;

        public ObservableCollection<OwnCopyData> OwnCopyCollection { get => _ownCopyCollection; }
        public DataObject LatestCopyContent { get => _latestCopyData?.Content; }

        public OwnCopyCollectionManager() =>
            InitializeList();

        private void InitializeList()
        {
            for (int i = 0; i < minimumLine; i++)
            {
                /*OwnCopyLinePanel nowLinePanel = new OwnCopyLinePanel();
                nowLinePanel.PointedFromClipboard += GetLatestCopy;
                _clipboardLineList.Add(nowLinePanel);*/

                OwnCopyData newCopy = new OwnCopyData();
                OwnCopyCollection.Add(newCopy);
            }
        }

        /*private void GetLatestCopy(object sender, EventArgs e)
        {
            var latestCopyOwner = sender as OwnCopyLinePanel;
            
            if(latestCopyOwner != null)
                _latestCopyData = latestCopyOwner.OwnCopy;
        }*/
    }
}