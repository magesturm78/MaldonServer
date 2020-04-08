using System;
using System.Collections.Generic;
using MaldonServer;
using MaldonServer.Network;
using MaldonServer.Network.ServerPackets;

namespace MaldonServer.Scripts.Accounting
{

    public class Account : IAccount
    {
        public PlayerSocket PlayerSocket { get; set; }
        public List<IMobile> Characters { get; set; }
        public AccessLevel AccessLevel { get; set; }
        public string UserName { get; set; }
        public bool Banned { get; private set; }
        private string password;
        private string email;

        public Account(string username, string password, string email)
        {
            this.UserName = username;
            this.password = password;
            this.email = email;
            Banned = false;
        }

        public bool ValidPassword(string pw)
        {
            return (string.Compare(password, pw) == 0);
        }

        public bool CanCreateCharacter() 
        { 
            return true; 
        }

        public void LoginCharacter(string name, string password)
        {

        }

        public void GetCharacterList()
        {

        }
        
        public void CreateCharacter(string name, string name2, string password, byte gender, byte hair)
        {
            PlayerMobile newChar = new PlayerMobile();

            if (newChar == null)
            {
                Console.WriteLine("Login: {0}: Character creation failed, account full", PlayerSocket);
                return;
            }
            newChar.Account = this;
            newChar.Gender = gender;
            switch (hair)
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
            newChar.Name = name;
            newChar.Password = password;

            //newChar.EquipItem(new Candle());
            //newChar.EquipItem(new Robe());

            Point3D pStart = new Point3D(0, 0, 0);
            newChar.MoveToWorld(pStart, MapManager.Internal);

            Console.WriteLine("Login: {0}: New character being created (account={1})", PlayerSocket, UserName);
            Console.WriteLine(" - Character: {0}", newChar.Name);
            Characters.Add(newChar);
        }
    }
}