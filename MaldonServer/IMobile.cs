using MaldonServer.Network;
using MaldonServer.Network.ServerPackets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaldonServer
{

    public interface IMobile : IPoint3D
    {
        Point3D Location { get; }
        byte Speed { get; }
        byte Direction { get; }
        int NPCId { get;  }
        int NameID { get; }
        Body Body { get; }
        String Name { get; }
        int Level { get; }
        int Experience { get; }
        byte Gender { get; }
        byte AttackRating { get; }
        short HairID { get; }
        IMap Map { get; }
        SpawnLocation SpawnLocation { get;  }
        List<ContainerItem> Backpack { get;  }
        BankBox Bank { get;  }
        IItem Weapon { get;  }
        IItem ChestArmor { get;  }
        IItem ShieldArmor { get;  }
        IItem HeadArmor { get;  }
        IItem Ring1 { get;  }
        IItem Ring2 { get;  }
        IItem HandArmor { get;  }
        IItem Boots { get;  }
        List<LearnedSpell> Spells { get;  }
        Skill[] Skills { get; }
        byte GuildHallId { get; }
        byte HouseId { get; set; }
        List<MailMessage> Mail { get; }

        int AvailablePoints { get;  }
        Stats RawStats { get; }

        int Health { get;  }
        int HealthMax { get;  }
        int Mana { get;  }
        int ManaMax { get; }
        int Energy { get;  }
        int EnergyMax { get; }

        int MeleeDamageMin { get; }
        int MeleeDamageMax { get; }
        bool Player { get; }
        byte ReligionId { get; }
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
        void SendMailList();
        void ShowMailMessage(int mailMessageID);
        void GetItemFromMail(int mailMessageID, byte itemNum);
        void SendMail(string toName, string subject, string content, List<int> mailItems);
        void ShowMarket(byte marketTab);
        void SellItemOnMarket(byte marketTab, int itemLocation, int totalCost);
        void BuyItemOnMarket(byte marketTab, byte itemLocation, byte additionalData);
        void GetMapPatch(byte mapId, short sector);
        void WarpToLocation(IMap targetMap, Point3D targetLocation);
        void UnsubscribeToMessage(MessageSubscriptionType messageType);
        void SendMessage(MessageSubscriptionType messageType, Packet p);
        void OpenBank();
    }
}
