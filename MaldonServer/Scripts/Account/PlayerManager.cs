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

		public static bool CharacterExists(string name)
        {
			return (Players[name] != null);
		}

        public static bool ValidPassword(string name, string password)
        {
            //add check for character passwords //faldon client sends encrypted password, Maldon client sends unencrypted password
            return (Players[name] != null);
        }

        private static PlayerMobile CreateMobile(Account account)
        {
            return new PlayerMobile();
            //return null;
        }

        public static IMobile CreateCharacter(CharacterCreateEventArgs args)
        {
            PlayerMobile newChar = CreateMobile(args.PlayerSocket.Account as Account);

            if (newChar == null)
            {
                Console.WriteLine("Login: {0}: Character creation failed, account full", args.PlayerSocket);
                return null;
            }

            args.Mobile = newChar;

            //newChar.Player = true;
            newChar.Gender = args.Gender;
            switch (args.Hair)
            {
                case 0://black
                    newChar.HairID = 61;
                    break;
                case 1://blond
                    newChar.HairID = 60;
                    break;
                case 2://blue
                    newChar.HairID = 63;
                    break;
                case 3://brown
                    newChar.HairID = 62;
                    break;
                case 4://green
                    newChar.HairID = 64;
                    break;
                case 5://red
                    newChar.HairID = 59;
                    break;
            }
            newChar.Name = args.Name;
            newChar.Password = args.Password;

            args.Accepted = true;

            //newChar.EquipItem(new Candle());
            //newChar.EquipItem(new Robe());

            Point3D pStart = new Point3D(0, 0, 0);
            newChar.MoveToWorld(pStart, MapManager.Internal);

            Console.WriteLine("Login: {0}: New character being created (account={1})", args.PlayerSocket, ((Account)args.PlayerSocket.Account).UserName);
            Console.WriteLine(" - Character: {0} (serial={1})", newChar.Name, newChar.Serial);
            return newChar;
        }
    }
}