using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace OneClickCopy.OwnCopyLines
{
    public class OwnCopyLineListViewModel
    {
        private ObservableCollection<OwnCopyLineViewModel> _ownCopyCollection = new ObservableCollection<OwnCopyLineViewModel>();
        private int minimumLine = 2;

        private OwnCopyData _latelyPointedCopyData = null;

        public ObservableCollection<OwnCopyLineViewModel> OwnCopyCollection { get => _ownCopyCollection; }

        public ICommand SetLatelyPointedCopyDataCommand { get; }

        public IDataObject LatelyPointedCopyContent { get => _latelyPointedCopyData?.Content; }

        public OwnCopyLineListViewModel()
        {
            SetLatelyPointedCopyDataCommand = new RelayCommand<int>(SetLatelyPointedCopyData);

            InitializeList();
        }

        private void InitializeList()
        {
            for (int i = 0; i < minimumLine; i++)
            {
                OwnCopyData newCopy = new OwnCopyData();
                OwnCopyLineViewModel newCopyLineViewModel = new OwnCopyLineViewModel(newCopy);

                OwnCopyCollection.Add(newCopyLineViewModel);
            }
        }

        private void SetLatelyPointedCopyData(int latelyPointedCopyDataIndex)
            => _latelyPointedCopyData = GetCopyDataByIndex(latelyPointedCopyDataIndex);

        private OwnCopyData GetCopyDataByIndex(int index)
            => OwnCopyCollection[index].OwnCopyData;
    }
}