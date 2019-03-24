using System;
using pnyx.net.fluent;

namespace pnyx.cmd.examples.documentation.library
{
    public class ExampleSettings
    {        
        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleSettings instance
        public static void instance()
        {
            const String input = "line one\nline two";
            using (Pnyx p = new Pnyx())
            {
                
                p.setSettings(stdIoDefault: true);    
                p.readString(input);
                p.process(); // automatically writes to STD-OUT
            }     
            // outputs STD-OUT: 
            // line one
            // line two
        }            
        
        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleSettings global
        public static void global()
        {
            SettingsHome.settingsFactory = new SettingsHome(
                new Settings
                {
                    stdIoDefault = true  // turns on globally
                });
            
            const String input = "line one\nline two";
            using (Pnyx p = new Pnyx())
            {                
                p.readString(input);
                p.process(); // automatically writes to STD-OUT
            }     
            // outputs STD-OUT: 
            // line one
            // line two
        }            
                
        public class CustomFactory : ISettingsFactory
        {
            public Settings buildSettings()
            {
                Settings result = new Settings();
                
                // update from DB, file, web.config, etc
                
                return result;
            }
        }
        
        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleSettings factory
        public static void factory()
        {
            SettingsHome.settingsFactory = new CustomFactory();
            
            using (Pnyx p = new Pnyx())
            {
                // uses custom settings
            }     
        }                    
    }
}