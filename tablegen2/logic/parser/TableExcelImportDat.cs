using System;
using System.Collections.Generic;
using System.IO;

namespace tablegen2.logic
{
    public static class TableExcelImportDat
    {
        public static TableExcelData importFile(string filePath)
        {
            var content = GzipHelper.processGZipDecode(File.ReadAllBytes(filePath));
            var ms = new MemoryStream(content);
            var br = new BinaryReader(ms);

            if (br.ReadInt32() != 1)
                throw new Exception("无法识别的文件版本号");

            var r = new TableExcelData();
            while (true)
            {
                var fieldName = br.ReadUtf8String();
                if (string.IsNullOrEmpty(fieldName))
                    break;

                var fieldType = string.Empty;
                var ftype = br.ReadByte();
                switch (ftype)
                {
                    case 1:
                        fieldType = "int";
                        break;
                    case 2:
                        fieldType = "double";
                        break;
                    case 3:
                        fieldType = "string";
                        break;
                    default:
                        throw new Exception(string.Format("无法识别的字段类型 fieldName:{0} fieldType:{1}", fieldName, ftype));
                }
                r.Headers.Add(new TableExcelHeader() { FieldName = fieldName, FieldType = fieldType });
            }

            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                var lst = new List<string>();
                for (int i = 0; i < r.Headers.Count; i++)
                {
                    var hdr = r.Headers[i];
                    switch (hdr.FieldType)
                    {
                        case "string":
                            lst.Add(br.ReadUtf8String());
                            break;
                        case "int":
                            lst.Add(br.ReadInt32().ToString());
                            break;
                        case "double":
                            lst.Add(br.ReadDouble().ToString());
                            break;
                    }
                }
                r.Rows.Add(new TableExcelRow() { StrList = lst });
            }

            return r;
        }
    }
}
