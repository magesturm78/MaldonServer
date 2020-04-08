using System;
using System.Collections.Generic;
using MaldonServer;
using MaldonServer.Network;
using MaldonServer.Network.ServerPackets;

namespace MaldonServer.Scripts
{
    public class PlayerMobile : IMobile
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public byte Z { get; private set; }

        public int NPCId { get { return -1; } }
        public byte Direction { get; set; }
        public int NameID { get; set; }
        public Body Body { get; set; }
        /* 0 == Agnostic
           1 == Initiate,   Believer
           2 == Believer,   Priest
        */
        public byte ReligionId { get; set; }
        public bool Player { get { return true; } }
        public String Name { get; set; }
        public String Password { get; set; }
        public int Level { get; set; }
        public double Experience { get; set; }
        public byte Gender { get; set; }
        public byte AttackRating { get; set; }
        public short HairID { get; set; }
        public IMap Map { get; set; }
        public SpawnLocation SpawnLocation { get; set; }
        public List<ContainerItem> Backpack { get; set; }
        public BankBox Bank { get; set; }
        public IItem Weapon { get; set; }
        public IItem ChestArmor { get; set; }
        public IItem ShieldArmor { get; set; }
        public IItem HeadArmor { get; set; }
        public IItem Ring1 { get; set; }
        public IItem Ring2 { get; set; }
        public IItem HandArmor { get; set; }
        public IItem Boots { get; set; }
        public List<LearnedSpell> Spells { get; set; }
        public Skill[] Skills { get; set; }
        public byte GuildHallId { get; set; }
        public byte HouseId { get; set; }
        public List<MailMessage> Mail { get; set; }

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

        public PlayerMobile()
        {
            Map = MapManager.Internal;
            SpawnLocation = new SpawnLocation(MapManager.GetMap(7), new Point3D(343, 91, 0));
            Backpack = new List<ContainerItem>();
            Mail = new List<MailMessage>();
            Spells = new List<LearnedSpell>();
            Skills = new Skill[33];
        }

        public void Spawn()
        {
            Map = SpawnLocation.Map;
            SetLocation(SpawnLocation.Location, true);
        }

        public void SetLocation(IPoint3D location, bool teleport)
        {
            if (teleport)
            {
                X = location.X;
                Y = location.Y;
                Z = location.Z;
                return;
            }
        }

        public void LocalMessage(MessageType msgType, string message) { }
        public void SendEverything(){ }
        public void ProcessText(string text) { }
        public void ProcessMovement(Point3D location, byte direction) { }
        public void AddStat(StatType statType) { }
        public void CastSpell(ISpell spell, Point3D location) { }
        public void CastSpell(ISpell spell, IMobile target) { }
        public void UseSkill(ISkill skill, IMobile target) { }
        public void UseSkill(ISkill skill, int target) { }
        public void UseSkill(ISkill skill, Point3D location) { }
        public void SetRun(bool run) { }
        public void Respawn() { }
        public void Attack(IMobile mobile, byte dir) { }
        public void DropItem(byte locationID) { }
        public void DropItem(byte locationID, int Amount) { }
        public void UseItem(byte locationID) { }
        public void MoveItem(byte locationID, byte newLocationID) { }
        public void UnequipItem(byte locationID) { }
        public void PickupItem(Point3D location) { }
        public void BankStoreItem(byte locationID, int amount) { }
        public void BankWithdrawItem(byte locationID) { }
        public void InteractNPC(IMobile mob) { }
        public void InteractDialog(IMobile mob, byte buttonID) { }
        public void InteractShop(byte LocationID) { }
        public void ShowGuildList() { }
        public void ShowGuild(IGuild guild) { }
        public void ApplyGuild(IGuild guild) { }
        public void ShowGuildDecrees(IGuild guild) { }
        public void CreateGuild(string guildName) { }
        public void BuyGuildHall(IGuild guild, byte guildHallID) { }
        public void SellGuildHall(IGuild guild) { }
        public void SendMailList() { }
        public void ShowMailMessage(int mailMessageID) { }
        public void GetItemFromMail(int mailMessageID, byte itemNum) { }
        public void SendMail(string toName, string subject, string content, List<int> mailItems) { }
        public void ShowMarket(byte marketTab) { }
        public void SellItemOnMarket(byte marketTab, int itemLocation, int totalCost) { }
        public void BuyItemOnMarket(byte marketTab, byte itemLocation, byte additionalData) { }
        public void GetMapPatch(byte mapId, short sector) { }

        public void MoveToWorld(Point3D newLocation, IMap map)
        {

        }
    }
}