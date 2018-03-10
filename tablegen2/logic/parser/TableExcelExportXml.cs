using System.IO;
using System.Text;
using System.Xml;

namespace tablegen2.logic
{
    public static class TableExcelExportXml
    {
        public static void exportExcelFile(TableExcelData data, string filePath)
        {
            var doc = new XmlDocument();
            var root = doc.CreateElement("root");
            doc.AppendChild(root);

            foreach (var row in data.Rows)
            {
                var item = doc.CreateElement("item");
                for (int i = 0; i < data.Headers.Count; i++)
                {
                    var hdr = data.Headers[i];
                    var val = row.StrList[i];
                    item.SetAttribute(hdr.FieldName, val);
                }
                root.AppendChild(item);
            }

            // 保存
            using (FileStream fs = File.Create(filePath))
            {
                var writer = new XmlTextWriter(fs, Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                doc.Save(writer);
                writer.Close();
            }
        }
    }
}
