using System;

namespace tablegen2.logic
{
    public enum TableExportFormat
    {
        Unknown = 0,    //未知
        Dat,            //加密数据格式
        Json,           //Json格式
        Xml,            //Xml格式
        Lua,            //Lua格式
    }
}
