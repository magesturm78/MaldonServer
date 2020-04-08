using System;
using System.Collections;
using MaldonServer;
using MaldonServer.Network;

namespace MaldonServer.Scripts.Accounting
{
	public class PlayerManager
	{
		private static Hashtable Players;

		public static void Initialize()
		{
			//Initialization script
		}

		static PlayerManager()
        {
            Players = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
		}

    }
}