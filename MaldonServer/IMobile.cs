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
        byte Direction { get; }
        int NPCId { get;  }
        int NameID { get; }
        Body Body { get; }
        String Name { get; set; }
        int Level { get; }
        double Experience { get; }
        byte Gender { get; set; }
        byte AttackRating { get; }
        short HairID { get; set; }
        IMap Map { get; }
        SpawnLocation SpawnLocation { get; set; }
        ContainerItem[] Backpack { get; set; }
        BankBox Bank { get; set; }
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
        MailMessage[] Mail { get; }

        int AvailablePoints { get; set; }
        Stats RawStats { get; }

        int Health { get; set; }
        int HealthMax { get;  }
        int Mana { get; set; }
        int ManaMax { get; }
        int Energy { get; set; }
        int EnergyMax { get; }

        int MeleeDamageMin { get; }
        int MeleeDamageMax { get; }
        bool Player { get; }
        byte ReligionId { get; }
        IAccount Account { get; set; }
        PlayerSocket PlayerSocket { get; set; }

        void SetLocation(IPoint3D location, bool teleport);
        void LocalMessage(MessageType msgType, string message);
        void SendEverything();
        void ProcessText(string text);
        void ProcessMovement(Point3D location, byte direction);
        void AddStat(StatType statType);
        void CastSpell(ISpell spell, Point3D location);
        void CastSpell(ISpell spell, IMobile target);
        void UseSkill(ISkill skill, IMobile target);
        void UseSkill(ISkill skill, int target);
        void UseSkill(ISkill skill, Point3D location);
        void SetRun(bool run);
        void Respawn();
        void Attack(IMobile mobile, byte dir);
        void DropItem(byte locationID);
        void DropItem(byte locationID, int Amount);
        void UseItem(byte locationID);
        void MoveItem(byte locationID, byte newLocationID);
        void UnequipItem(byte locationID);
        void PickupItem(Point3D location);
        void BankStoreItem(byte locationID, int amount);
        void BankWithdrawItem(byte locationID);
        void InteractNPC(IMobile mob);
        void InteractDialog(IMobile mob, byte buttonID);
        void InteractShop(byte LocationID);
        void ShowGuildList();
        void ShowGuild(IGuild guild);
        void ApplyGuild(IGuild guild);
        void ShowGuildDecrees(IGuild guild);
        void CreateGuild(string guildName);
        void BuyGuildHall(IGuild guild, byte guildHallID);
        void SellGuildHall(IGuild guild);
    }
}
