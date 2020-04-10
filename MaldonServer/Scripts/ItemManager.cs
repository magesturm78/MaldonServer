using System;
using System.IO;
using System.Collections.Generic;
using MaldonServer;
using MaldonServer.Network;
using MaldonServer.Network.ServerPackets;

namespace MaldonServer.Scripts
{
    public enum ItemType : byte
    {
        None = 0,
        Weapon = 1,
        Ring = 2,
        LeftHand = 3,
        Helm = 4,
        Armor = 5,
        Consumable = 8,
        Book = 9,
        GMRUNE = 10,
        Projectile = 13,
        Gloves = 14,
        Hair = 15,
        Boot = 16,
        Plantable = 17,
        Skull = 18,
        Bones_Ball = 19,
        Scroll = 20,
        Unk2 = 21,
        Unk3 = 22,
        Pants = 23
    }

    public struct ItemDefinition
    {
        public Int16 ItemID;
        public string Name;
        public string PluralName;
        public Int16 Weight;
        public ItemType ItemType;
        public Int16 StrReq;
        public Int16 IntReq;
        public Int16 StamReq;
        public Int16 Armor;
        public Int16 Health;
        public byte SkillID;
        public bool isaBow;
        public bool isSilver;
        public byte DamageMin;
        public byte DamageMax;
        public sbyte StamMod;
        public sbyte IntMod;
        public sbyte StrMod;
        public sbyte DefMod;
        public sbyte ConMod;
        public sbyte MagMod;
        public byte Dura1;
        public byte Dura2;
        public bool isLightArmor;
        public bool isHeavyArmor;
        public bool isTwoHanded;
        public Int16 DefReq;
        public Int16 MagReq;
        public Int16 LevelReq;
        public byte BaseBuyPrice;
        public Int16 BaseSellPrice;
        public Int16 ResLightning;
        public Int16 ResFire;
        public Int16 ResMagic;
        public Int16 ResPoison;
        public Int16 ResHolyMagic;
        public Int16 ResParalysis;
        public bool isArtifact;
    }

    public class ItemManager
    {
        private static ItemDefinition[] items;

        public static void Configure()
        {
            LoadItems();
        }

        private static void LoadItems()
        {
            string mapPath = Path.Combine(Core.BaseDirectory, "Data", "objects.dat");
            if (!File.Exists(mapPath)) return;

            FileStream fs = new FileStream(mapPath, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            int size = (int)(fs.Length / 499);
            Console.WriteLine("Loading {0} Items...", size);
            items = new ItemDefinition[size];
            for (Int16 i = 0; i < size; i++)
            {
                fs.Seek((499 * i), SeekOrigin.Begin);

                ItemDefinition item = new ItemDefinition();
                item.ItemID = i;

                item.Name= GetNonTerminatedString(br.ReadChars(25));
                item.PluralName = GetNonTerminatedString(br.ReadChars(25));

                //fs.Seek((499 * i) + 149, SeekOrigin.Begin);
                //int iTemp1 = br.ReadByte();//updates to the image1 var
                //int iTemp2 = br.ReadByte();//updates to the image2 var
                //fs.Seek((499 * i) + 50, SeekOrigin.Begin);

                item.Weight = br.ReadInt16();//weigth????
                //int image1 = (iTemp1 * 256) + 
                    br.ReadByte();
                //int image2 = (iTemp2 * 256) + 
                    br.ReadByte();

                item.ItemType = (ItemType)br.ReadByte();
                item.StrReq = br.ReadInt16();
                item.IntReq = br.ReadInt16();
                item.StamReq = br.ReadInt16();

                /* 60 - 66 are only used for scroll */
                fs.Seek((499 * i) + 67, SeekOrigin.Begin);
                item.Armor = br.ReadInt16();

                item.Health = br.ReadInt16();
                item.SkillID = br.ReadByte();
                item.isaBow = br.ReadBoolean();
                item.isSilver = br.ReadBoolean();

                br.ReadBoolean();//unknown

                item.DamageMin = br.ReadByte();
                item.DamageMax = br.ReadByte();

                br.ReadByte();//unknown

                //byte iRed = 
                    br.ReadByte();
                //byte iGreen = 
                    br.ReadByte();
                //byte iBlue = 
                    br.ReadByte();

                item.StamMod = (sbyte)(br.ReadByte() - 127);
                item.IntMod = (sbyte)(br.ReadByte() - 127);
                item.StrMod = (sbyte)(br.ReadByte() - 127);
                item.DefMod = (sbyte)(br.ReadByte() - 127);
                item.ConMod = (sbyte)(br.ReadByte() - 127);
                item.MagMod = (sbyte)(br.ReadByte() - 127);

                item.Dura1 = br.ReadByte();
                item.Dura2 = br.ReadByte();
                //position 89 - 98 = 0x00

                fs.Seek((499 * i) + 99, SeekOrigin.Begin);
                item.isLightArmor = br.ReadBoolean();//Maybe
                br.ReadInt16(); // -1 on some items
                br.ReadByte();//only on 358 quake 15 and 489 Alecto's wrath 4 
                //101 - 111 = 0x00
                fs.Seek((499 * i) + 112, SeekOrigin.Begin);
                br.ReadInt16();//only used on 4 items
                br.ReadInt16();//only used on 3 items
                item.isHeavyArmor = br.ReadBoolean();//Maybe
                br.ReadByte();//unknown
                br.ReadInt16();//only used on 9 items

                br.ReadInt16();
                br.ReadInt16();

                br.ReadInt16();
                br.ReadInt16();//= X,Y spawn adjustment ??
                br.ReadInt16();//= X,Y spawn adjustment ??
                br.ReadInt16();

                br.ReadInt16();
                br.ReadInt16();
                item.isTwoHanded = br.ReadBoolean();

                br.ReadByte();
                br.ReadByte();

                //136 = boolean used for weapon???

                fs.Seek((499 * i) + 139, SeekOrigin.Begin);
                item.DefReq = br.ReadInt16();
                item.MagReq = br.ReadInt16();
                br.ReadByte();//0
                br.ReadByte();//0
                br.ReadByte();//0
                br.ReadByte();//0
                br.ReadByte();//unknown
                br.ReadByte();//0
                br.ReadByte();//added to image1 earlier
                br.ReadByte();//added to image2 earlier
                fs.Seek((499 * i) + 151, SeekOrigin.Begin);
                item.LevelReq = br.ReadInt16();
                br.ReadByte();//unknown only scroll
                br.ReadByte();//0
                br.ReadByte();//0
                br.ReadByte();//0
                fs.Seek((499 * i) + 157, SeekOrigin.Begin);
                item.BaseBuyPrice = br.ReadByte();
                item.BaseSellPrice = br.ReadInt16();
                br.ReadByte();//0

                fs.Seek((499 * i) + 161, SeekOrigin.Begin);
                item.ResLightning = br.ReadInt16();
                item.ResFire = br.ReadInt16();
                item.ResMagic = br.ReadInt16();
                item.ResPoison = br.ReadInt16();
                item.ResHolyMagic = br.ReadInt16();
                item.ResParalysis = br.ReadInt16();

                item.isArtifact = br.ReadBoolean();

                //br.ReadByte();//0

                //fs.Seek((499 * i) + 175, SeekOrigin.Begin);
                //item.genLight = br.ReadBoolean();
                //item.lightAmount = br.ReadInt16();
                //br.ReadInt16();//-255 to +255
                //br.ReadInt16();//-255 to +255
                //br.ReadInt16();//-255 to +255

                items[i] = item;

            }

            br.Close();
            fs.Close();
            Console.WriteLine("Loading Items Completed");

        }

        private static string GetNonTerminatedString(char[] chars)
        {
            string theString = String.Empty;
            theString = new string(chars, 0, chars.Length);
            return theString;
        }
    }
}