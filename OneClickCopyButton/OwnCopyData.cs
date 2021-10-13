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
        private string _title = "Empty";
        private DataObject _content = null;

        public string Title { get => _title; set => _title = value; }
        public DataObject Content { get => _content; set => _content = value; }
    }
}
