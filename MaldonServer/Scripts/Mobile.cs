using System;
using System.Collections.Generic;
using MaldonServer;
using MaldonServer.Network;
using MaldonServer.Network.ServerPackets;

namespace MaldonServer.Scripts
{
    public class Mobile : IMobile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public byte Z { get; set; }

        public byte Speed { get; set; }
        public int NPCId { get; set; }
        public byte Direction { get; set; }
        public int NameID { get; set; }
        public Body Body { get; set; }
        /* 0 == Agnostic
           1 == Initiate,   Believer
           2 == Believer,   Priest
        */
        public byte ReligionId { get; set; }
        public bool Player { get; set; }
        public String Name { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
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
        public Stats RawStats { get; protected set; }
        public int Health { get; protected set; }
        public int HealthMax 
        { 
            get
            {
                return RawStats.Stamina + Level;// + (int)Skills.Tactics.Base;
            }
        }
        public int Mana { get; protected set; }
        public int ManaMax
        {
            get
            {
                return RawStats.Intelligence;// + (int)Skills.Magery.Base;
            }
        }
        public int Energy { get; protected set; }
        public int EnergyMax
        {
            get
            {
                return 40 + (RawStats.Defence - 20) + Level;
            }
        }

        public int MeleeDamageMin { get; protected set; }
        public int MeleeDamageMax { get; protected set; }

        public PlayerSocket PlayerSocket { get; set; }

        public Mobile()
        {
            Backpack = new List<ContainerItem>();
            Mail = new List<MailMessage>();
            Spells = new List<LearnedSpell>();
            Skills = new Skill[33];
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

        public void ProcessText(string text) 
        { 
            if (string.Compare(text, "WHO", true) == 0)
            {
                PlayerSocket.Send(new WhoIsOnlinePacket());
            } else
            {
                Console.WriteLine("{0} sent text {1}", PlayerSocket, text);
            }
        }

        public virtual void ProcessMovement(Point3D location, byte direction) 
        {
            //only allow movement of 5 spaces for testing
            double distance = Utility.GetDistance(new Point3D(X, Y, Z), location);
            if (distance > 5)
            {
                Console.WriteLine("Distance too far {0}", distance);
                return;
            }

            if (Map.CanMove(this,location))
            {
                X = location.X;
                Y = location.Y;
                Z = location.Z;
                Direction = direction;
                Map.ProccessMovement(this, location);
            }
        }

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

        public virtual void WarpToLocation(IMap targetMap, Point3D targetLocation)
        {
            Map = targetMap;
            X = targetLocation.X;
            Y = targetLocation.Y;
            Z = targetLocation.Z;
        }

        public virtual void Spawn()
        {
            Health = HealthMax;
            Mana = ManaMax;
            Energy = EnergyMax;
        }
    }
}