using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using pnyx.net.fluent;
using pnyx.net.util;

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

        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleFluent pnyxMethods
        public static void pnyxMethods()
        {
            MethodInfo[] methods = typeof(Pnyx).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            
            String names = String.Join("\n", methods.Select(mi => mi.Name));
            using (var p = new Pnyx())
                p.readString(names).write(@"c:\dev\ee.txt");
        }

        public class MethodGrouping
        {
            public String method;
            public String grouping;
            public int order;
            public MethodInfo mi;

            public MethodGrouping(string method, string grouping, int order)
            {
                this.method = method;
                this.grouping = grouping;
                this.order = order;
            }
        }

        // pnyx -e=documentation pnyx.cmd.examples.documentation.library.ExampleFluent pnyxHtml
        public static void pnyxHtml()
        {
            List<MethodGrouping> info = new List<MethodGrouping>();
info.Add(new MethodGrouping("startLine","Input",1));
info.Add(new MethodGrouping("startRow","Input",2));
info.Add(new MethodGrouping("readStreamFactory","Input",3));
info.Add(new MethodGrouping("read","Input",4));
info.Add(new MethodGrouping("readStream","Input",5));
info.Add(new MethodGrouping("readString","Input",6));
info.Add(new MethodGrouping("readStdin","Input",7));
info.Add(new MethodGrouping("cat","Input",8));
info.Add(new MethodGrouping("asCsv","Input",9));
info.Add(new MethodGrouping("head","Input",10));
info.Add(new MethodGrouping("tail","Input",11));
info.Add(new MethodGrouping("tailStream","Input",12));
info.Add(new MethodGrouping("linePart","Line",13));
info.Add(new MethodGrouping("lineFilter","Line",14));
info.Add(new MethodGrouping("lineTransformer","Line",15));
info.Add(new MethodGrouping("lineBuffering","Line",16));
info.Add(new MethodGrouping("lineFilterFunc","Line",17));
info.Add(new MethodGrouping("lineTransformerFunc","Line",18));
info.Add(new MethodGrouping("and","Boolean",19));
info.Add(new MethodGrouping("or","Boolean",20));
info.Add(new MethodGrouping("xor","Boolean",21));
info.Add(new MethodGrouping("not","Boolean",22));
info.Add(new MethodGrouping("parseCsv","Line, Row Conversion",23));
info.Add(new MethodGrouping("parseDelimiter","Line, Row Conversion",24));
info.Add(new MethodGrouping("parseTab","Line, Row Conversion",25));
info.Add(new MethodGrouping("printDelimiter","Line, Row Conversion",26));
info.Add(new MethodGrouping("printTab","Line, Row Conversion",27));
info.Add(new MethodGrouping("lineToRow","Line, Row Conversion",28));
info.Add(new MethodGrouping("rowToLine","Line, Row Conversion",29));
info.Add(new MethodGrouping("print","Line, Row Conversion",30));
info.Add(new MethodGrouping("shimAnd","Row",31));
info.Add(new MethodGrouping("rowPart","Row",32));
info.Add(new MethodGrouping("rowFilter","Row",33));
info.Add(new MethodGrouping("rowTransformer","Row",34));
info.Add(new MethodGrouping("rowFilterFunc","Row",35));
info.Add(new MethodGrouping("rowTransformerFunc","Row",36));
info.Add(new MethodGrouping("rowBuffering","Row",37));
info.Add(new MethodGrouping("columnDefinition","Row",38));
info.Add(new MethodGrouping("swapColumnsAndRows","Row",39));
info.Add(new MethodGrouping("hasColumns","Row",40));
info.Add(new MethodGrouping("widthColumns","Row",41));
info.Add(new MethodGrouping("removeColumns","Row",42));
info.Add(new MethodGrouping("insertColumnsWithPadding","Row",43));
info.Add(new MethodGrouping("insertColumns","Row",44));
info.Add(new MethodGrouping("duplicateColumnsWithPadding","Row",45));
info.Add(new MethodGrouping("duplicateColumns","Row",46));
info.Add(new MethodGrouping("headerNames","Row",47));
info.Add(new MethodGrouping("selectColumns","Row",48));
info.Add(new MethodGrouping("printColumn","Row",49));
info.Add(new MethodGrouping("withColumns","Row",50));
info.Add(new MethodGrouping("grep","Util",51));
info.Add(new MethodGrouping("egrep","Util",52));
info.Add(new MethodGrouping("hasLine","Util",53));
info.Add(new MethodGrouping("sed","Util",54));
info.Add(new MethodGrouping("sedLineNumber","Util",55));
info.Add(new MethodGrouping("sedAppendRow","Util",56));
info.Add(new MethodGrouping("sedAppendLine","Util",57));
info.Add(new MethodGrouping("sedInsert","Util",58));
info.Add(new MethodGrouping("beforeAfterFilter","Util",59));
info.Add(new MethodGrouping("sortLine","Util",60));
info.Add(new MethodGrouping("sortRow","Util",61));
info.Add(new MethodGrouping("setSettings","Util",62));
info.Add(new MethodGrouping("write","Output",63));
info.Add(new MethodGrouping("writeStream","Output",64));
info.Add(new MethodGrouping("writeCsv","Output",65));
info.Add(new MethodGrouping("writeCsvStream","Output",66));
info.Add(new MethodGrouping("writeStdout","Output",67));
info.Add(new MethodGrouping("writeSplit","Output",68));
info.Add(new MethodGrouping("rewrite","Output",69));
info.Add(new MethodGrouping("captureText","Output",70));
info.Add(new MethodGrouping("tee","Output",71));
info.Add(new MethodGrouping("processToString","StateChange",72));
info.Add(new MethodGrouping("process","StateChange",73));
info.Add(new MethodGrouping("compile","StateChange",74));
                        
            MethodInfo[] methods = typeof(Pnyx).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (MethodInfo mi in methods)
            {
                MethodGrouping mg = info.Find(i => i.method == mi.Name);
                if (mg == null)
                {
                    Console.WriteLine("Ignoring method {0}", mi.Name);
                    continue;
                }
                    
                mg.mi = mi;
            }
            
            String page = "";
            String lastGrouping = null;
            foreach (MethodGrouping mg in info)
            {
                if (mg.grouping != lastGrouping)
                {
                    page += "<a class=\"anchor\" id=\"" + anchorText(mg.grouping) + "\"></a>\n";                
                    page += @"<div class=""method-grouping"">" + mg.grouping + "</div>\n";
                    lastGrouping = mg.grouping;
                }
                
                MethodInfo mi = mg.mi;
                String html = "<div class=\"method\">\n";
                html += "  <a class=\"anchor\" id=\"" + anchorText(mi.Name) + "\"></a>\n";                
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

        private static String anchorText(String groupingName)
        {
            groupingName = TextUtil.extractAlphaNumeric(groupingName);
            groupingName = TextUtil.camelToDash(groupingName);
            return groupingName;
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