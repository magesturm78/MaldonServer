using MaldonServer.Network;

namespace MaldonServer.Scripts
{
	public class World
	{
		public static void Initialize()
		{
			//Initialization script
		}

		public static void Broadcast(Packet p)
        {
			Listener.Instance.Broadcast(p);
        }
	}
}