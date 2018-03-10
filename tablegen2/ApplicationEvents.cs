namespace tablegen2
{
    static class ApplicationEvents
    {
        public static void Application_Startup()
        {
            AppData.loadConfig();
        }

        public static void Application_Exit()
        {
        }
    }
}
