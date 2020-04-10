using MaldonServer.Network;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace MaldonServer
{
    public class Core
    {
        private static Assembly assembly;
        //private static Process m_Process;
        private static Thread thread;

        private static bool closing;
        private static bool crashed;

        private static Listener serverListener;

        public static int ItemCount { get; private set; }
        public static int MobileCount { get; private set; }

        static void Main(/*string[] args*/)
        {
            if (Environment.UserInteractive)
            {
                Start();
            }
        }

        public static void VerifySerialization()
        {
            ItemCount = 0;
            MobileCount = 0;

            VerifySerialization(Assembly.GetCallingAssembly());
            VerifySerialization(ScriptCompiler.CompiledAssembly);
        }

        private static void VerifySerialization(Assembly a)
        {
            if (a == null) return;

            foreach (Type t in a.GetTypes())
            {
                bool isItem = false;// t.IsSubclassOf(typeof(Item));

                if (isItem || t.IsSubclassOf(typeof(IMobile)))
                {
                    if (isItem)
                        ItemCount++;
                    else
                        MobileCount++;
                }
            }
        }

        static void Start()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            thread = Thread.CurrentThread;
            assembly = Assembly.GetEntryAssembly();

            if (thread != null)
                thread.Name = "Core Thread";

            Version ver = assembly.GetName().Version;

            // Added to help future code support on forums, as a 'check' people can ask for to it see if they recompiled core or not
            Console.WriteLine("MaldonServer - Version {0}.{1}.{3}, Build {2}", ver.Major, ver.Minor, ver.Revision, ver.Build);

            while (!ScriptCompiler.Compile())
            {
                Console.WriteLine("Scripts: One or more scripts failed to compile or no script files were found.");
                Console.WriteLine(" - Press return to exit, or R to try again.");

                string line = Console.ReadLine();
                if (line == null || line.ToLower() != "r")
                    return;
            }

            int port = Int32.Parse(ConfigurationManager.AppSettings["Port"].ToString());
            serverListener = new Listener(port);

            World.ServerManager.Start();

            try
            {
                while (!closing)
                {
                    //Thread.Sleep(1);
                    
                    //Mobile.ProcessDeltaQueue();
                    //Item.ProcessDeltaQueue();

                    //Timer.Slice();
                    serverListener.Slice();
                }
            }
            catch (Exception e)
            {
                CurrentDomain_UnhandledException(null, new UnhandledExceptionEventArgs(e, true));
            }
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            HandleClosed();
        }

        private static void HandleClosed()
        {
            if (closing)
                return;

            closing = true;
            Console.Write("Exiting...");

            if (!crashed)
                World.ServerManager.Shutdown();

            Console.Write("done.");
        }

        private static string exePath;
        private static string baseDirectory;

        public static string ExePath
        {
            get
            {
                if (exePath == null)
                    exePath = Process.GetCurrentProcess().MainModule.FileName;

                return exePath;
            }
        }

        public static string BaseDirectory
        {
            get
            {
                if (baseDirectory == null)
                {
                    try
                    {
                        baseDirectory = ExePath;

                        if (baseDirectory.Length > 0)
                            baseDirectory = Path.GetDirectoryName(baseDirectory);
                    }
                    catch
                    {
                        baseDirectory = "";
                    }
                }

                return baseDirectory;
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine(e.IsTerminating ? "Error:" : "Warning:");
			Console.WriteLine(e.ExceptionObject);
            if (e.IsTerminating)
            {
                crashed = true;
                closing = true;
            }
        }
    }
}
