using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaldonServer
{
    public struct ContainerItem
    {
        public byte Location;
        public IItem Item;
    }

    public struct LearnedSpell
    {
        public int Id;
        public int Level;
    }

    public struct Skill
    {
        public byte Id;
        public double Level;
    }

    public struct Stats
    {
        public int Strength;
        public int Defence;
        public int Consititution;
        public int Intelligence;
        public int Magic;
        public int Stamina;
    }

    public struct SpawnLocation
    {
        public Map Map;
        public Point3D Location;

        public SpawnLocation(Map map, Point3D location)
        {
            Map = map;
            Location = location;
        }
    }

    public struct MailMessage
    {
        public int Id;
        public bool Read;
    }
}
