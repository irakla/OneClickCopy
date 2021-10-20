using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace OneClickCopy.OwnCopyLines
{
    public class OwnCopyLineListViewModel
    {
        private ResourceManager messageResourceManager =
            new ResourceManager(typeof(OneClickCopy.Properties.MessageResource));

        private ObservableCollection<OwnCopyLineViewModel> _ownCopyCollection = new ObservableCollection<OwnCopyLineViewModel>();
        private int minimumLine = 1;

        private OwnCopyData _latelyPointedCopyData = null;

        public ObservableCollection<OwnCopyLineViewModel> OwnCopyCollection { get => _ownCopyCollection; }

        public ICommand AddNewLineAtTheIndexCommand { get; }
        public ICommand RemoveLineAtTheIndexCommand { get; }
        public ICommand SetLatelyPointedCopyDataCommand { get; }

        public IDataObject LatelyPointedCopyContent { get => _latelyPointedCopyData?.Content; }

        public OwnCopyLineListViewModel()
        {
            AddNewLineAtTheIndexCommand = new RelayCommand<int>(AddNewLineAtTheIndex);
            RemoveLineAtTheIndexCommand = new RelayCommand<int>(RemoveLineAtTheIndex);
            SetLatelyPointedCopyDataCommand = new RelayCommand<int>(SetLatelyPointedCopyData);

            InitializeList();
        }

        private void InitializeList()
        {
            for (int i = 0; i < minimumLine; i++)
            {
                OwnCopyLineViewModel newCopyLineViewModel =
                    GetNewCopyLineViewModel();

                OwnCopyCollection.Add(newCopyLineViewModel);
            }
        }

        private void AddNewLineAtTheIndex(int addingLineIndex)
        {
            OwnCopyCollection.Insert(addingLineIndex, GetNewCopyLineViewModel());
            ToastNotifier.TryToLaunchMessage(messageResourceManager.GetString("PlusButtonNewCopyLineIsAdded"));
        }

        private void RemoveLineAtTheIndex(int removingLineIndex)
        {
            OwnCopyData removingCopyData = OwnCopyCollection[removingLineIndex].OwnCopyData;

            bool isMeaninglessRemoval =
                removingCopyData.IsDefault && OwnCopyCollection.Count == minimumLine;
            
            if (isMeaninglessRemoval)
                return;

            //Toast Message Setting
            string formattedRemoveMessage
                = messageResourceManager.GetString("RemoveButtonTitleLineIsRemoved_Formatted");

            string nowButtonTitle = removingCopyData.Title;
            int titleMaxLength = 10;

            string properButtonTitle = nowButtonTitle.Length <= titleMaxLength ?
                nowButtonTitle : nowButtonTitle.Substring(0, titleMaxLength) + "...";
            string removeMessage =
                string.Format(formattedRemoveMessage, properButtonTitle);

            //Remove
            if (OwnCopyCollection.Count != minimumLine)
                OwnCopyCollection.RemoveAt(removingLineIndex);
            else
                OwnCopyCollection[removingLineIndex].OwnCopyData = new OwnCopyData();

            ToastNotifier.TryToLaunchMessage(removeMessage);
        }

        private void SetLatelyPointedCopyData(int latelyPointedCopyDataIndex)
            => _latelyPointedCopyData = GetCopyDataByIndex(latelyPointedCopyDataIndex);

        private OwnCopyData GetCopyDataByIndex(int index)
            => OwnCopyCollection[index].OwnCopyData;

        private OwnCopyLineViewModel GetNewCopyLineViewModel()
            => new OwnCopyLineViewModel(new OwnCopyData());
    }
}