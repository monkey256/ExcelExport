using System.IO;
using System.Text;

namespace tablegen2
{
    public static class BinaryExtension
    {
        public static void WriteUtf8String(this BinaryWriter bw, string str)
        {
            bw.Write(Encoding.UTF8.GetBytes(str));
            bw.Write((byte)0);
        }

        public static string ReadUtf8String(this BinaryReader br)
        {
            MemoryStream ms = new MemoryStream();
            while (true)
            {
                byte a = br.ReadByte();
                if (a == 0)
                    break;
                ms.WriteByte(a);
            }
            return Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
        }
    }
}
