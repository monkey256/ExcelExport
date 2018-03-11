using System;
using System.IO;
using System.Text;

namespace tablegen2.logic
{
    public static class TableExcelExportDat
    {
        public static void exportExcelFile(TableExcelData data, string filePath)
        {
            const int version = 1;

            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms, Encoding.UTF8);
            bw.Write(version);

            foreach (var hdr in data.Headers)
            {
                bw.WriteUtf8String(hdr.FieldName);
                byte ftype = 0;
                switch (hdr.FieldType)
                {
                    case "int":
                        ftype = 1;
                        break;
                    case "double":
                        ftype = 2;
                        break;
                    case "string":
                        ftype = 3;
                        break;
                    default:
                        throw new Exception(string.Format("无法识别的字段类型 {0} 名称 {1}", hdr.FieldType, hdr.FieldName));
                }
                bw.Write(ftype);
            }
            bw.Write((byte)0);

            foreach (var row in data.Rows)
            {
                for (int i = 0; i < data.Headers.Count; i++)
                {
                    var hdr = data.Headers[i];
                    var val = row.StrList[i];
                    switch (hdr.FieldType)
                    {
                        case "int":
                            {
                                int n = 0;
                                int.TryParse(val, out n);
                                bw.Write(n);
                            }
                            break;
                        case "double":
                            {
                                double n = 0;
                                double.TryParse(val, out n);
                                bw.Write(n);
                            }
                            break;
                        case "string":
                            bw.WriteUtf8String(val);
                            break;
                    }
                }
            }
            
            if (File.Exists(filePath))
                File.Delete(filePath);

            File.WriteAllBytes(filePath, GzipHelper.processGZipEncode(ms.GetBuffer(), (int)ms.Length));

            bw.Close();
            ms.Close();
        }
    }
}
