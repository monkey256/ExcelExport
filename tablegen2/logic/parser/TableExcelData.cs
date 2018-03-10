using System.Collections.Generic;
using System.Linq;

namespace tablegen2.logic
{
    public class TableExcelData
    {
        private List<TableExcelHeader> headers_ = new List<TableExcelHeader>();
        private List<TableExcelRow> rows_ = new List<TableExcelRow>();

        public TableExcelData()
        {
        }

        public TableExcelData(IEnumerable<TableExcelHeader> headers, IEnumerable<TableExcelRow> rows)
        {
            headers_ = headers.ToList();
            rows_ = rows.ToList();
        }

        public List<TableExcelHeader> Headers
        {
            get { return headers_; }
        }

        public List<TableExcelRow> Rows
        {
            get { return rows_; }
        }

        public bool checkUnique(out string errmsg)
        {
            int idx1 = headers_.FindIndex(a => a.FieldName.Equals("Id"));
            int idx2 = headers_.FindIndex(a => a.FieldName.Equals("KeyName"));
            var ids = new HashSet<int>();
            var keys = new HashSet<string>();
            for (int i = 0; i < rows_.Count; i++)
            {
                var row = rows_[i];
                var strId = row.StrList[idx1];
                var strKeyName = row.StrList[idx2];

                int id;
                if (!int.TryParse(strId, out id))
                {
                    errmsg = string.Format("第{0}行Id值非法，须为数字类型：{1}", i + 2, strId);
                    return false;
                }

                if (string.IsNullOrEmpty(strKeyName))
                {
                    errmsg = string.Format("第{0}行KeyName值为空", i + 2);
                    return false;
                }

                if (ids.Contains(id))
                {
                    errmsg = string.Format("第{0}行Id值已存在：{1}", i + 2, strId);
                    return false;
                }

                if (keys.Contains(strKeyName))
                {
                    errmsg = string.Format("第{0}行KeyName值已存在：{1}", i + 2, strKeyName);
                    return false;
                }

                ids.Add(id);
                keys.Add(strKeyName);
            }

            errmsg = string.Empty;
            return true;
        }
    }
}
