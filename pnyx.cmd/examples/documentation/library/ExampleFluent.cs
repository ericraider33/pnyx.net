using System;
using System.Linq;
using System.Reflection;
using pnyx.net.fluent;

namespace pnyx.cmd.examples.documentation.library
{
    public class ExampleFluent
    {
        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleFluent builder
        public static void builder()
        {
            using (var p = new Pnyx())
                p.readString("a,b,c,d").parseCsv().print("$4|$3|$2|$1").writeStdout();

            // outputs: d|c|b|a           
        }

        public static void pnyxHtml()
        {
            MethodInfo[] methods = typeof(Pnyx).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            String page = "";            
            foreach (MethodInfo mi in methods)
            {
                String html = "<div class=\"method\">\n";
                html += "  <a class=\"anchor\" id=\"" + mi.Name + "\"></a>\n";                
                html += "  <span class=\"method-name\">" + mi.Name + "</span>: ";
                html += "<span class=\"method-description\">_description_</span><br/>\n";

                ParameterInfo[] parameters = mi.GetParameters();
                if (parameters.Length > 0)
                {
                    html += "  <ul>\n";
                    foreach (ParameterInfo pi in parameters)
                    {
                        html += "    <li class=\"parameter\">\n";
                        html += "      <span class=\"parameter-name\">" + pi.Name + "</span> ";
                        html += "<span class=\"parameter-type\">" + typeText(pi.ParameterType) + "</span>";
                        if (pi.HasDefaultValue)
                            html += " default=<span class=\"parameter-default\">" + pi.DefaultValue + "</span>";

                        html += ": ";
                        html += "<span class=\"parameter-description\">_description_</span>\n";
                        html += "    </li>\n";
                    }
                    html += "  </ul>\n";
                }
                html += "</div>\n\n";

                page += html;
            }                    
            
            using (var p = new Pnyx())
                p.readString(page).write(@"c:\dev\ee.txt");
        }

        private static String typeText(Type t)
        {               
            String typeName = t.Name;
            bool nullable = typeName.StartsWith("Nullable") && t.GenericTypeArguments.Length == 1;
            if (nullable)
            {
                t = t.GenericTypeArguments[0];
                typeName = t.Name;
            }
            
            switch (typeName)
            {
                case "Boolean": typeName = "bool"; break;
                case "Int32": typeName = "int"; break;
                case "Int32[]": typeName = "int[]"; break;
            }

            if (typeName.StartsWith("Action`"))
                typeName = "Action";
            
            if (nullable)
            {
                typeName += "?";
            }
            else if (t.GenericTypeArguments.Length > 0)
            {
                typeName += "&lt";
                typeName += String.Join(", ", t.GenericTypeArguments.Select(typeText));
                typeName += "&gt";
            }                       

            return typeName;
        }
    }
}