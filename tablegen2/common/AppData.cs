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
        }
        public static void saveConfig()
        {
            JsonConfig.writeToFile("config.json", Config);
        }
    }
}
