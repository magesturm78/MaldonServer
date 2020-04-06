using System;
using MaldonServer;
using MaldonServer.Network;
using MaldonServer.Network.ServerPackets;

namespace MaldonServer.Scripts
{
    public class PlayerMobile : IMobile
    {
        public Serial Serial { get; set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }

        public String Name { get; set; }
        public String Password { get; set; }
        public int Level { get; set; }
        public double Experience { get; set; }
        public byte Gender { get; set; }
        public byte AttackRating { get; set; }
        public short HairID { get; set; }
        public Map Map { get; set; }
        public SpawnLocation SpawnLocation { get
            {
                return new SpawnLocation(MapManager.GetMap(7), new Point3D(343, 91, 0));
            }
            set { } }
        public ContainerItem[] Backpack { get; set; }
        public IItem Weapon { get; set; }
        public IItem ChestArmor { get; set; }
        public IItem ShieldArmor { get; set; }
        public IItem HeadArmor { get; set; }
        public IItem Ring1 { get; set; }
        public IItem Ring2 { get; set; }
        public IItem HandArmor { get; set; }
        public IItem Boots { get; set; }
        public LearnedSpell[] Spells { get; set; }
        public Skill[] Skills { get; set; }
        public byte GuildHallId { get; set; }
        public byte HouseId { get; set; }
        public MailMessage[] Mail { get; set; }

        public int AvailablePoints { get; set; }
        public Stats RawStats { get; set; }

        public int Health { get; set; }
        public int HealthMax { get; set; }
        public int Mana { get; set; }
        public int ManaMax { get; set; }
        public int Energy { get; set; }
        public int EnergyMax { get; set; }

        public int MeleeDamageMin { get; set; }
        public int MeleeDamageMax { get; set; }

        public IAccount Account { get; set; }
        public PlayerSocket PlayerSocket { get; set; }

        public void SetLocation(IPoint3D location, bool teleport)
        {

        }

        public void LocalMessage(MessageType msgType, string message)
        {

        }

        public void SendEverything()
        {

        }

        public void MoveToWorld(Point3D newLocation, Map map)
        {

        }
    }
}