using MaldonServer.Network;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace MaldonServer
{
    class Core
    {
        private static Assembly m_Assembly;
        //private static Process m_Process;
        private static Thread m_Thread;
        private static bool m_Closing;
        private static Listener serverListener;

        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                Start();
            }
        }

        static void Start()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            m_Thread = Thread.CurrentThread;
            //m_Process = Process.GetCurrentProcess();
            m_Assembly = Assembly.GetEntryAssembly();

            if (m_Thread != null)
                m_Thread.Name = "Core Thread";

            //Timer.TimerThread ttObj = new Timer.TimerThread();
            //timerThread = new Thread(new ThreadStart(ttObj.TimerMain));
            //timerThread.Name = "Timer Thread";

            Version ver = m_Assembly.GetName().Version;

            // Added to help future code support on forums, as a 'check' people can ask for to it see if they recompiled core or not
            Console.WriteLine("MagesServer - Version {0}.{1}.{3}, Build {2}", ver.Major, ver.Minor, ver.Revision, ver.Build);
            
            int port = Int32.Parse(ConfigurationManager.AppSettings["Port"].ToString());
            serverListener = new Listener(port);

            try
            {
                while (!m_Closing)
                {
                    Thread.Sleep(1);

                    //Mobile.ProcessDeltaQueue();
                    //Item.ProcessDeltaQueue();

                    //Timer.Slice();
                    serverListener.Slice();

                    //NetState.FlushAll();
                    //NetState.ProcessDisposedQueue();

                    //if (Slice != null)
                    //    Slice();
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
            if (m_Closing)
                return;

            m_Closing = true;

            Console.Write("Exiting...");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine(e.IsTerminating ? "Error:" : "Warning:");
			Console.WriteLine(e.ExceptionObject);
            if (e.IsTerminating)
            {
                //m_Crashed = true;
                m_Closing = true;
            }
        }
    }
}
