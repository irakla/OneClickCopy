using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OneClickCopy
{
    public class OwnCopyData
    {
        private static string _defaultTitle = "Empty";

        private string _title = new StringBuilder(_defaultTitle).ToString();
        private DataObject _content = null;

        public static string DefaultTitle
        {
            get => _defaultTitle;
            set
            {
                if (value != null)
                    _defaultTitle = value;
            }
        }

        public bool IsDefault
        {
            get => _title.Equals(_defaultTitle) && _content == null;
        }

        public string Title { get => _title; set => _title = value; }
        public DataObject Content { get => _content; set => _content = value; }
    }
}
