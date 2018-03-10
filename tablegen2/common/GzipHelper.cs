using System;
using System.IO;
using System.IO.Compression;

namespace tablegen2
{
    internal static class GzipHelper
    {
        public static byte[] processGZipEncode(byte[] content)
        {
            return processGZipEncode(content, content.Length);
        }

        public static byte[] processGZipEncode(byte[] content, int length)
        {
            var ms = new MemoryStream();
            ms.Seek(0, SeekOrigin.Begin);
            var zip = new GZipStream(ms, CompressionMode.Compress, true);
            zip.Write(content, 0, length);
            zip.Close();

            var r = new byte[ms.Length];
            Array.Copy(ms.GetBuffer(), r, ms.Length);

            ms.Close();

            return r;
        }

        public static byte[] processGZipDecode(byte[] content)
        {
            return processGZipDecode(content, content.Length);
        }

        public static byte[] processGZipDecode(byte[] content, int length)
        {
            var wms = new MemoryStream();

            var ms = new MemoryStream(content, 0, length);
            ms.Seek(0, SeekOrigin.Begin);
            var zip = new GZipStream(ms, CompressionMode.Decompress, true);

            var buf = new byte[4096];
            int n;
            while ((n = zip.Read(buf, 0, buf.Length)) != 0)
            {
                wms.Write(buf, 0, n);
            }

            zip.Close();
            ms.Close();

            var r = new byte[wms.Length];
            Array.Copy(wms.GetBuffer(), r, wms.Length);
            return r;
        }
    }
}
