using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MaldonServer
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CallPriorityAttribute : Attribute
    {
        private int priority;

        public int Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        public CallPriorityAttribute(int priority)
        {
            this.priority = priority;
        }
    }

    public class CallPriorityComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            MethodInfo a = x as MethodInfo;
            MethodInfo b = y as MethodInfo;

            if (a == null && b == null)
                return 0;

            if (a == null)
                return 1;

            if (b == null)
                return -1;

            return GetPriority(a) - GetPriority(b);
        }

        private int GetPriority(MethodInfo mi)
        {
            object[] objs = mi.GetCustomAttributes(typeof(CallPriorityAttribute), true);

            if (objs == null)
                return 0;

            if (objs.Length == 0)
                return 0;

            CallPriorityAttribute attr = objs[0] as CallPriorityAttribute;

            if (attr == null)
                return 0;

            return attr.Priority;
        }
    }

    public class ScriptCompiler
    {
        public static Assembly CompiledAssembly;

        private static ArrayList additionalReferences = new ArrayList();

        public static string[] GetReferenceAssemblies()
        {
            ArrayList list = new ArrayList();

            string path = Path.Combine(Core.BaseDirectory, "Data/Assemblies.cfg");

            if (File.Exists(path))
            {
                using (StreamReader ip = new StreamReader(path))
                {
                    string line;

                    while ((line = ip.ReadLine()) != null)
                    {
                        if (line.Length > 0 && !line.StartsWith("#"))
                            list.Add(line);
                    }
                }
            }
            list.Add(Core.ExePath.Replace(".vshost", ""));

            list.AddRange(additionalReferences);

            return (string[])list.ToArray(typeof(string));
        }

        private static void DeleteFiles(string mask)
        {
            try
            {
                string[] files = Directory.GetFiles(Path.Combine(Core.BaseDirectory, "Scripts/Output"), mask);

                foreach (string file in files)
                {
                    try { File.Delete(file); }
                    catch { }
                }
            }
            catch
            {
            }
        }

        private static CompilerResults CompileCSScripts()
        {
            return CompileCSScripts(false);
        }

        private static CompilerResults CompileCSScripts(bool debug)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            //CodeDomProvider compiler = CodeDomProvider.CreateProvider("C#");
            //ICodeCompiler compiler = provider.CreateCompiler();

            Console.Write("Scripts: Compiling C# scripts...");
            string[] files = GetScripts("*.cs");

            if (files.Length == 0)
            {
                Console.WriteLine("no files found.");
                return null;
            }

            string path = GetUnusedPath("Scripts.CS");

            CompilerParameters parms = new CompilerParameters(GetReferenceAssemblies(), path, debug);

            if (!debug)
                parms.CompilerOptions = "/debug- /optimize+"; // doesn't seem to have any effect

            CompilerResults results = provider.CompileAssemblyFromFile(parms, files);
            //CompilerResults results = compiler.CompileAssemblyFromFileBatch( parms, files );

            additionalReferences.Add(path);

            if (results.Errors.Count > 0)
            {
                int errorCount = 0, warningCount = 0;

                foreach (CompilerError e in results.Errors)
                {
                    if (e.IsWarning)
                        ++warningCount;
                    else
                        ++errorCount;
                }

                if (errorCount > 0)
                    Console.WriteLine("failed ({0} errors, {1} warnings)", errorCount, warningCount);
                else
                    Console.WriteLine("done ({0} errors, {1} warnings)", errorCount, warningCount);

                foreach (CompilerError e in results.Errors)
                {
                    Console.WriteLine(" - {0}: {1}: {2}: (line {3}, column {4}) {5}", e.IsWarning ? "Warning" : "Error", e.FileName, e.ErrorNumber, e.Line, e.Column, e.ErrorText);
                }
            }
            else
            {
                Console.WriteLine("done (0 errors, 0 warnings)");
            }

            return results;
        }

        private static string GetUnusedPath(string name)
        {
            string path = Path.Combine(Core.BaseDirectory, String.Format("Scripts/Output/{0}.dll", name));

            for (int i = 2; File.Exists(path) && i <= 1000; ++i)
                path = Path.Combine(Core.BaseDirectory, String.Format("Scripts/Output/{0}.{1}.dll", name, i));

            return path;
        }

        public static bool Compile()
        {
            return Compile(false);
        }

        public static bool Compile(bool debug)
        {
            EnsureDirectory("Scripts/");
            EnsureDirectory("Scripts/Output/");

            DeleteFiles("Scripts.CS*.dll");
            DeleteFiles("Scripts*.dll");

            if (additionalReferences.Count > 0)
                additionalReferences.Clear();

            CompilerResults csResults = null;

            csResults = CompileCSScripts(debug);
            if (csResults != null && !csResults.Errors.HasErrors)
            {
                CompiledAssembly = csResults.CompiledAssembly;

                Console.Write("Scripts: Verifying...");
                Core.VerifySerialization();
                Console.WriteLine("done ({0} items, {1} mobiles)", Core.ItemCount, Core.MobileCount);

                ArrayList invoke = new ArrayList();

                Type[] types = CompiledAssembly.GetTypes();

                for (int i = 0; i < types.Length; ++i)
                {
                    MethodInfo m = types[i].GetMethod("Configure", BindingFlags.Static | BindingFlags.Public);

                    if (m != null)
                        invoke.Add(m);
                }

                invoke.Sort(new CallPriorityComparer());

                for (int i = 0; i < invoke.Count; ++i)
                    ((MethodInfo)invoke[i]).Invoke(null, null);

                invoke.Clear();

                //World.Load();

                types = CompiledAssembly.GetTypes();

                for (int i = 0; i < types.Length; ++i)
                {
                    MethodInfo m = types[i].GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public);

                    if (m != null)
                        invoke.Add(m);
                }

                invoke.Sort(new CallPriorityComparer());

                for (int i = 0; i < invoke.Count; ++i)
                    ((MethodInfo)invoke[i]).Invoke(null, null);


                return true;
            }
            return false;
        }

        private static void EnsureDirectory(string dir)
        {
            string path = Path.Combine(Core.BaseDirectory, dir);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private static string[] GetScripts(string type)
        {
            ArrayList list = new ArrayList();

            GetScripts(list, Path.Combine(Core.BaseDirectory, "Scripts"), type);

            return (string[])list.ToArray(typeof(string));
        }

        private static void GetScripts(ArrayList list, string path, string type)
        {
            foreach (string dir in Directory.GetDirectories(path))
                GetScripts(list, dir, type);

            list.AddRange(Directory.GetFiles(path, type));
        }
    }
}
