using System;
using System.Collections.Generic;
using System.Reflection;
using pnyx.net.fluent;
using Xunit;

namespace pnyx.net.test.fluent
{
    public class PynxUniqueNameTest
    {
        [Fact]
        public void uniqueMethodNames()
        {
            HashSet<String> names = new HashSet<String>();
            
            MethodInfo[] methods = typeof(Pnyx).GetMethods(BindingFlags.Instance | BindingFlags.Public);
            foreach (MethodInfo mi in methods)
            {
                if (mi.ReturnType != typeof(Pnyx))
                    continue;

                String methodName = mi.Name;
                Assert.DoesNotContain(methodName, names);

                names.Add(methodName);
            }
        }
    }
}