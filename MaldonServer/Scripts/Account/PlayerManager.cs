using System;
using System.Collections;
using MaldonServer;
using MaldonServer.Network;

namespace MaldonServer.Scripts.Accounting
{
	public class PlayerManager
	{
		private static Hashtable Players;

		static PlayerManager()
        {
            Players = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
		}

		public static bool PlayerExists(string name)
		{
			return (Players[name] != null);
		}
	}
}