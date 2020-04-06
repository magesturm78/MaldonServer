using MaldonServer.Network;
using MaldonServer.Network.ServerPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaldonServer
{

    public interface IMobile : IPoint3D
    {
        String Name { get; set; }
        int Level { get; set; }
        double Experience { get; set; }
        byte Gender { get; set; }
        byte AttackRating { get; set; }
        short HairID { get; set; }
        Map Map { get; set; }
        SpawnLocation SpawnLocation { get; set; }
        ContainerItem[] Backpack { get; set; }
        IItem Weapon { get; set; }
        IItem ChestArmor { get; set; }
        IItem ShieldArmor { get; set; }
        IItem HeadArmor { get; set; }
        IItem Ring1 { get; set; }
        IItem Ring2 { get; set; }
        IItem HandArmor { get; set; }
        IItem Boots { get; set; }
        LearnedSpell[] Spells { get; set; }
        Skill[] Skills { get; set; }
        byte GuildHallId { get; set; }
        byte HouseId { get; set; }
        MailMessage[] Mail { get; set; }

        int AvailablePoints { get; set; }
        Stats RawStats { get; set; }

        int Health { get; set; }
        int HealthMax { get; set; }
        int Mana { get; set; }
        int ManaMax { get; set; }
        int Energy { get; set; }
        int EnergyMax { get; set; }

        int MeleeDamageMin { get; set; }
        int MeleeDamageMax { get; set; }

        IAccount Account { get; set; }
        PlayerSocket PlayerSocket { get; set; }

        void SetLocation(IPoint3D location, bool teleport);
        void LocalMessage(MessageType msgType, string message);
        void SendEverything();
    }
}
