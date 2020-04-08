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
            Characters = new List<IMobile>();
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
            foreach (PlayerMobile character in Characters)
            {
                if (character.Name == name)
                {
                    if (character.Password != password)
                    {
                        Console.WriteLine("Login: {0}: Invalid Password", PlayerSocket);
                        PlayerSocket.Send(new CharacterLoginReplyPacket(ALRReason.CharInvPw));
                        return;
                    }
                    // Check if anyone is using this Character
                    if (character.Map != MapManager.Internal)
                    {
                        Console.WriteLine("Login: {0}: Account in use", PlayerSocket);
                        PlayerSocket.Send(new CharacterLoginReplyPacket(ALRReason.CharInUse));
                        return;
                    }

                    //if (character.PlayerSocket != null)
                    //    character.PlayerSocket.Dispose();

                    PlayerSocket.Mobile = character;
                    character.PlayerSocket = PlayerSocket;

                    character.Spawn();

                    PlayerSocket.Send(new CharacterLoginReplyPacket(character));
                    DoLogin();
                    return;
                }
            }
            Console.WriteLine("Login: {0}: Character {1} not assigned to account", PlayerSocket, name);
            PlayerSocket.Send(new CharacterLoginReplyPacket(ALRReason.CharInUse));
            PlayerSocket.Dispose();

        }

        public void GetCharacterList()
        {
            PlayerSocket.Send(new CharacterListPacket(this));
        }
        
        public void CreateCharacter(string name, string name2, string password, byte gender, byte hair)
        {
            if (PlayerManager.PlayerExists(name))
            {
                Console.WriteLine("Login: {0}: Character name {1} already exists.", PlayerSocket, name);
                PlayerSocket.Send(new CharacterCreateReplyPacket(ALRReason.CharExist));
                return;
            }

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
            PlayerSocket.Send(new CharacterCreateReplyPacket(ALRReason.CharCreated));
        }

        private void DoLogin()
        {
            PlayerMobile m = PlayerSocket.Mobile as PlayerMobile;
            PlayerSocket.Send(new HardCodedMessagePacket(9, m.Name));//Welcome message
            m.LocalMessage(MessageType.System, "Server Programmed by Magistrate.");

            PlayerSocket.Send(new ReligionPacket(m));//TODO: Fix with correct Values
            PlayerSocket.Send(new PlayerInventoryPacket(m));
            PlayerSocket.Send(new PlayerEquipmentPacket(m));//TODO: Fix with correct Values

            PlayerSocket.Send(new PlayerSpellListPacket(m));
            PlayerSocket.Send(new PlayerSkillListPacket(m));

            PlayerSocket.Send(new NumberPlayersPacket());

            PlayerSocket.Send(new PlayerNamePacket(m));
            World.Broadcast(new HardCodedMessagePacket(12, m.Name));//Player xxx joined broadcast????

            PlayerSocket.Send(new PlayerIncomingPacket(m));

            m.SendEverything();

            PlayerSocket.Send(new Unk51Packet());
            PlayerSocket.Send(new PlayerHouseingPacket(m));
            PlayerSocket.Send(new PlayerTeleportPacket((byte)m.Map.MapID, m.X, m.Y, (byte)m.Z));
            //playerSocket.Send(new GMobileName(m));
            PlayerSocket.Send(new Unk03Packet(m));
            PlayerSocket.Send(new Unk03_1Packet(m));
            PlayerSocket.Send(new PlayerLocationPacket(m));
            //0x56 Packet 
            PlayerSocket.Send(new WeatherPacket(0, 0, 0));

            PlayerSocket.Send(new PlayerHMEPacket(m));
            PlayerSocket.Send(new PlayerStatsPacket(m));
            PlayerSocket.Send(new PlayerLevelPacket(m));
            PlayerSocket.Send(new PlayerMinMaxDamagePacket(m));

            PlayerSocket.Send(new BrightnessPacket(m.Map));

            PlayerSocket.Send(new Unk55Packet());

            int unreadEmail = 0;
            //foreach (MailMessage mm in m.Mail)
            //    if (!mm.Read)
            //        unreadEmail++;

            if (unreadEmail > 0)
                PlayerSocket.Send(new TextMessagePacket(MessageType.System, String.Format("You have {0} unread mail messages.", unreadEmail)));
        }
    }
}