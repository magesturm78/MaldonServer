using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaldonServer
{
    public interface IMarket
    {
        byte MarketID { get; set; }
        MarketItem[] MarketItems { get; set; }
    }

    public struct MarketItem
    {
        public byte LocationID;
        public double Price;
        public IItem Item;
    }

    public struct GroundItem
    {
        public byte LocationID;
        public Point3D Location;
        public IItem Item;
    }

    public struct ContainerItem
    {
        public byte LocationID;
        public IItem Item;
    }

    public struct BankBox
    {
        public int Gold;
        public ContainerItem[] Items;
    }

    public struct Body
    {
        public int BodyID;
        public byte BodyType1;
        public byte BodyType2;
    }

    public struct NPCDialog
    {
        public string Title;
        public string Text;
        public string[] Options;
        public IMobile Owner;
    }

    public struct VendorItem
    {
        public byte LocationID;
        public int Price;
        public IItem Item;
    }

    public struct NPCVendor
    {
        public IMobile Owner;
        public byte Sale;
        public VendorItem[] VendorItems;
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
        public IMobile From;
        public IMobile To;
        public string Subject;
        public string Contents;
        public ContainerItem[] Items;
    }


    public struct GuildHall
    {
        public int Id;
        public string Name;
        public int Price;
    }

    public struct GuildDecree
    {
        public byte DecreeID;
        public byte DecreeType;
        public Guild Guild;
    }

    public struct GuildMember
    {
        public byte MemberID;
        public byte MemberType;
        public IMobile Member;
    }
    public struct GuildApplicant
    {
        public byte ApplicantID;
        public IMobile Applicant;
    }

    public struct Guild
    {
        public int Id;
        public string Name;
        public IMobile Owner;
        public GuildHall GuildHall;
        public GuildMember[] Members;
        public GuildDecree[] Decrees;
        public GuildApplicant[] Applicants;
    }

    public struct Rect
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
    }

    public struct MobileSpawn
    {
        public byte NameID;
        public byte SpawnDelay;
        public Rect Bounds;
        public Point3D SpawnLocation;
        public string ScriptName;
    }

    public struct NPCSpawnInfo
    {
        public string Script;
        public string ScriptName;
        public string NpcName;
        public int Picture;//Pic
        public int Weapon;//Weapon
        public int Shield;//Shield
        public int Helmet;//Helmet
        public int Gauntlet;//Gauntlet
        public int Boots;//Boots
        public int Armour;//Armour
        public byte Gender;//Gender
    }
}
