namespace pnyx.net.fluent
{
    public class SettingsHome : ISettingsFactory
    {
        public static ISettingsFactory settingsFactory { get; set; }

        static SettingsHome()
        {
            settingsFactory = new SettingsHome();
        }
        
        private readonly Settings settings = new Settings();
        
        public Settings buildSettings()
        {
            return settings;
        }
    }
}