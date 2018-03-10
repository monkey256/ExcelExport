using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tablegen2.logic
{
    public class TableExcelRow
    {
        public List<string> StrList { get; set; }

        public TableExcelRow()
        {
            StrList = new List<string>();
        }
    }
}
