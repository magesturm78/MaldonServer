using System;
using System.Collections.Generic;
using MaldonServer;
using MaldonServer.Network;
using MaldonServer.Network.ServerPackets;

namespace MaldonServer.Scripts
{
    public class PlayerMobile : Mobile
    {
        public int ID { get; private set; }

        private string password;

        public PlayerMobile(int id, string name, string password, int level, int experience,
                            byte gender, byte hair, byte spawnMap, int spawnX, int spawnY, byte spawnZ,
                            int availablePoints, int strength, int defence, int consititution, int intelligence,
                            int magic, int stamina, byte religion) : base()
        {
            this.Map = MapManager.Internal;
            this.Speed = 10;

            this.ID = id;
            this.Name = name;
            this.password = password;
            this.Level = level;
            this.Experience = experience;
            this.Gender = gender;
            this.HairID = hair;
            this.Map = MapManager.Internal;
            this.SpawnLocation = new SpawnLocation(World.GetMapByID(spawnMap), new Point3D(spawnX, spawnY, spawnZ));
            this.AvailablePoints = availablePoints;

            Stats stats = new Stats();
            stats.Strength = strength;
            stats.Defence = defence;
            stats.Consititution = consititution;
            stats.Intelligence = intelligence;
            stats.Magic = magic;
            stats.Stamina = stamina;
            this.RawStats = stats;

            this.ReligionId = religion;
        }

        public bool ValidPassword(string password)
        {
            return (this.password == password);
        }

        public override void Spawn()
        {
            base.Spawn();
            Map = SpawnLocation.Map;
            SetLocation(SpawnLocation.Location, true);
        }

        public override void ProcessMovement(Point3D location, byte direction)
        {
            base.ProcessMovement(location, direction);
            PlayerSocket.Send(new PlayerMovementPacket(this));
        }

        public override void WarpToLocation(IMap targetMap, Point3D targetLocation)
        {
            base.WarpToLocation(targetMap, targetLocation);
            PlayerSocket.Send(new PlayerTeleportPacket(Map.MapID, X, Y, Z));
        }

        public override void SendMessage(MessageSubscriptionType messageType, Packet p) 
        {
            base.SendMessage(messageType, p);
            PlayerSocket.Send(p);
        }

        public override void OpenBank() 
        {
            PlayerSocket.Send(new PlayerBankPacket(this));
        }

    }
}