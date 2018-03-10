using System.IO;
using System.Text;
using System.Web.Script.Serialization;

namespace tablegen2
{
    internal static class JsonConfig
    {
        public static void writeToFile(string fileName, object target)
        {
            var jss = new JavaScriptSerializer();
            var content = jss.Serialize(target);
            var filePath = Path.Combine(Util.WorkPath, fileName);
            File.WriteAllBytes(filePath, Encoding.UTF8.GetBytes(content));
        }

        public static T readFromFile<T>(string fileName)
        {
            try
            {
                var filePath = Path.Combine(Util.WorkPath, fileName);
                var fileContent = File.ReadAllBytes(filePath);
                var content = Encoding.UTF8.GetString(fileContent);
                var jss = new JavaScriptSerializer();
                return jss.Deserialize<T>(content);
            }
            catch (System.Exception)
            {
            }

            return default(T);
        }
    }
}
