using tablegen2.logic;

namespace tablegen2
{
    public static class AppData
    {
        public static MainWindow MainWindow = null;
        public static TableGenConfig Config = null;

        static AppData()
        {
        }

        public static void loadConfig()
        {
            Config = JsonConfig.readFromFile<TableGenConfig>("config.json") ?? new TableGenConfig();
            //修正值
            if (string.IsNullOrEmpty(Config.SheetNameForField))
                Config.SheetNameForField = "def";
            if (string.IsNullOrEmpty(Config.SheetNameForData))
                Config.SheetNameForData = "data";
        }
        public static void saveConfig()
        {
            JsonConfig.writeToFile("config.json", Config);
        }
    }
}
