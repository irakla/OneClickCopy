using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OneClickCopy
{
    [Serializable]
    class SerializableDataObject : IDataObject
    {
        private DataObject orgDataObject;

        SerializableDataObject(DataObject dataObject) => orgDataObject = dataObject;

        public object GetData(string format) => orgDataObject.GetData(format);
        public object GetData(Type format) => orgDataObject.GetData(format);
        public object GetData(string format, bool autoConvert) => orgDataObject.GetData(format, autoConvert);
        public bool GetDataPresent(string format) => orgDataObject.GetDataPresent(format);
        public bool GetDataPresent(Type format) => orgDataObject.GetDataPresent(format);
        public bool GetDataPresent(string format, bool autoConvert) => orgDataObject.GetDataPresent(format, autoConvert);
        public string[] GetFormats() => orgDataObject.GetFormats();
        public string[] GetFormats(bool autoConvert) => orgDataObject.GetFormats(autoConvert);
        public void SetData(object data) => orgDataObject.SetData(data);
        public void SetData(string format, object data) => orgDataObject.SetData(format, data);
        public void SetData(Type format, object data) => orgDataObject.SetData(format, data);
        public void SetData(string format, object data, bool autoConvert) => orgDataObject.SetData(format, data, autoConvert);
    }
}
