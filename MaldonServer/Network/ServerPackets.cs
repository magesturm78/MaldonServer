using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MaldonServer.Network.ServerPackets
{
    #region Client Initialization Packets
    public sealed class Stage1 : Packet
    {
        public Stage1() : base(0x52, 6)
        {
            Write((byte)Utility.Random(1, 254));
            Write((byte)Utility.Random(1, 254));
            Write((byte)Utility.Random(1, 254));
            Write((byte)Utility.Random(1, 254));
            Write((byte)Utility.Random(1, 254));
            Write((byte)Utility.Random(1, 254));
        }
    }

    public sealed class Stage1Reply : Packet
    {
        public Stage1Reply() : base(0x35, 6)
        {
            Write((byte)(0x30 + Utility.Random(0, 9)));
            Write((byte)(0x30 + Utility.Random(0, 9)));
            Write((byte)(0x30 + Utility.Random(0, 9)));
            Write((byte)(0x30 + Utility.Random(0, 9)));
            Write((byte)(0x30 + Utility.Random(0, 9)));
            Write((byte)(0x30 + Utility.Random(0, 9)));
        }
    }

    public sealed class Stage2 : Packet
    {
        public Stage2() : base(0x3F, 0)
        {
        }
    }

    public sealed class Stage2Reply : Packet
    {
        public Stage2Reply() : base(0xBF, 0)
        {
        }
    }
    #endregion

    #region account packets
    public enum ALRReason : byte
    {
        AccountExists   = 0x00,
        Blocked         = 0x01,//notused
        AccountCreated  = 0x02,
        InvalidAccount  = 0x03,
        BadPass         = 0x04,
        CorrectPassword = 0x05,
        CharExist       = 0x08,
        CharCreated     = 0x09,
        CharInvPw       = 0x21,
        CharInUse       = 0x6A,
    }

    public sealed class AccountCreateLoginReplyPacket : Packet
    {
        public AccountCreateLoginReplyPacket(ALRReason reason) : base(0x65, 1)
        {
            Write((byte)reason);
        }
    }

    public sealed class CharacterLoginReplyPacket : Packet
    {
        public CharacterLoginReplyPacket(ALRReason reason) : base(0x02, 1)
        {
            Write((byte)reason);
        }
        public CharacterLoginReplyPacket(IMobile m) : base(0x02, 9)
        {
            Write((byte)94);//need to be 94 for it to connect
            Write((short)m.X);
            Write((short)m.Y);
            Write((byte)m.Map.MapID); //Map

            Write((byte)0x05);//02 A0 39 
            Write((byte)0x67);//
            Write((byte)0x72);// 
        }
    }

    public sealed class CharacterCreateReplyPacket : Packet
    {
        public CharacterCreateReplyPacket(ALRReason reason) : base(0x02, 1)
        {
            Write((byte)reason);
        }
    }

    public sealed class CharacterListPacket : Packet
    {
        public CharacterListPacket(IAccount a) : base(0x65)
        {
            int length = 1;
            for (int i = 0; i < a.Characters.Count; ++i)
            {
                IMobile m = a.Characters[i];

                if (m != null)
                {
                    length += m.Name.Length + 1;
                }
            }
            EnsureCapacity(length);

            Write((byte)1);
            for (int i = 0; i < a.Characters.Count; ++i)
            {
                IMobile m = a.Characters[i];

                if (m != null)
                {
                    Write((byte)m.Name.Length);
                    WriteAsciiNull(m.Name);
                }
            }

        }
    }

    #endregion

    //InGame Packets

    public enum MessageType
    {
        Misc = 0x00,
        System = 0x01,
        Yell = 0x02,
        Broadcast = 0x03,
        Guild = 0x04,
        Tell = 0x05,
        Whisper = 0x06
    }

    public sealed class HardCodedMessagePacket : Packet
    {
        //TODO: update to correct values???
        public HardCodedMessagePacket(byte messageID) : base(0x41, 3)
        {
            Write((byte)0x64);
            Write((byte)messageID);
            Write((byte)0x00);
        }
        public HardCodedMessagePacket(byte messageID, byte number) : base(0x41, 6)
        {
            Write((byte)0x01);
            Write((byte)messageID);
            Write((byte)0x00);
            Write((byte)0x69);
            Write((byte)number);
            Write((byte)0x00);
        }
        public HardCodedMessagePacket(byte messageID, string message) : base(0x41)
        {
            EnsureCapacity(message.Length + 5);
            Write((byte)0x01);
            Write((byte)messageID);
            Write((byte)0x00);
            Write((byte)0x73);
            Write((byte)message.Length);
            WriteAsciiNull(message);
        }
    }

    public sealed class TextMessagePacket : Packet
    {
        public TextMessagePacket(MessageType type, string text) : base(0x01)
        {
            if (text == null)
                text = "";

            this.EnsureCapacity(1 + text.Length);

            Write((byte)type);
            WriteAsciiNull(text);
        }
    }

    public sealed class SayMessagePacket : Packet
    {
        public SayMessagePacket(IMobile m, string message) : base(0x17)
        {

            EnsureCapacity(m.Name.Length + message.Length + 3);
            Write((byte)m.PlayerSocket.SocketID);
            WriteAsciiNull(m.Name);
            WriteAsciiNull(": ");
            WriteAsciiNull(message);
        }
    }

    public sealed class PopupMessagePacket : Packet
    {
        public PopupMessagePacket(string message) : base(0x2A)
        {
            EnsureCapacity(message.Length);
            WriteAsciiNull(message);
        }
        public PopupMessagePacket(byte messageNum) : base(0x2A, 3)
        {
            Write((byte)0x04);
            Write((byte)messageNum);
            Write((byte)0x00);
        }
    }

    public sealed class EncodedMessagePacket : Packet
    {
        public EncodedMessagePacket(string text, string text2) : base(0x53)
        {
            if (text == null)
                text = "";
            if (text2 == null)
                text2 = "";
            int size = 7 + text.Length + text2.Length;

            this.EnsureCapacity(size);
            Write((byte)3);
            Write((byte)1);
            Write((byte)0);
            Write((byte)115);
            Write((byte)text.Length);//size of first string
            WriteAsciiNull(text);
            Write((byte)115);
            Write((byte)text2.Length);//size of first string
            WriteAsciiNull(text2);
            Encrypt();
        }
    }

    public sealed class NumberPlayersPacket : Packet
    {
        public NumberPlayersPacket() : base(0x41, 6)
        {
            int numPlayers = Listener.ConnectedCount;

            Write((byte)1); //1 == number of players message
            //10 = person, 11 == people
            if (numPlayers == 1)
                Write((byte)10);
            else
                Write((byte)11);

            Write((byte)0);//??
            Write((byte)105);//?? needs to be 105
            Write((short)numPlayers); //number of current players
        }
    }

    public sealed class WhoIsOnlinePacket : Packet
    {
        public WhoIsOnlinePacket() : base(0x38)
        {
            int size = 0;
            foreach (PlayerSocket ps in Listener.Instance.PlayerSockets)
            {
                if (ps.Mobile != null)
                    size += ps.Mobile.Name.Length + 1;
            }
            EnsureCapacity(size);
            foreach (PlayerSocket ps in Listener.Instance.PlayerSockets)
            {
                if (ps.Mobile != null)
                {
                    int value = ps.Mobile.Name.Length * 16;
                    if (ps.Account.AccessLevel > AccessLevel.Peasant)
                        value += 1;
                    Write((byte)value);
                    WriteAsciiNull(ps.Mobile.Name);
                }
            }
        }
    }

    public sealed class TutorialPacket : Packet
    {
        public TutorialPacket(string message) : base(0x58)
        {
            EnsureCapacity(message.Length + 1);
            WriteAsciiNull(message);
            Write((byte)0x01);
        }
    }

    #region Player Mobile Packets
    public sealed class ReligionPacket : Packet
    {
        public ReligionPacket(IMobile m) : base(0xA6, 2)
        {
            Write((byte)93);//0x5d = 93   Religion Name Packet
            Write((byte)m.ReligionId);
        }
    }

    public sealed class PlayerInventoryPacket : Packet
    {
        public PlayerInventoryPacket(IMobile m) : base(0x96)
        {
            MemoryStream ms = new MemoryStream();

            foreach (ContainerItem contItem in m.Backpack)
            {
                //Encoding 0 = max amount = 8388607     pre/suf     dura
                //Encoding 1 = max amount = 8388607                 dura
                //Encoding 2 = max amount = 32767       pre/suf     dura
                //Encoding 3 = max amount = 32767                   dura
                //Encoding 4 = max amount = 8388607     pre/suf
                //Encoding 5 = max amount = 8388607 
                //Encoding 6 = max amount = 32767       pre/suf
                //Encoding 7 = max amount = 32767

                IItem item = contItem.Item;
                byte iLoc = contItem.LocationID;
                int itemEnoding = 7;

                if (item.Prefix != 0 || item.Suffix != 0)
                    itemEnoding -= 1;

                if (item.Amount > 32768)
                    itemEnoding -= 2;

                if (item.DuraMax != item.DuraMin)
                    itemEnoding -= 4;

                ms.WriteByte((byte)((itemEnoding * 32) + iLoc));

                if (item.ItemID < 128)
                {
                    ms.WriteByte((byte)(0x80 + item.ItemID));
                }
                else
                {
                    ms.WriteByte((byte)(item.ItemID / 256));
                    ms.WriteByte((byte)(item.ItemID % 256));
                }

                int amount = item.Amount;

                if (amount > 8388607) amount = 8388607;

                int value = (amount % 32768);
                if (value < 128)
                {
                    ms.WriteByte((byte)(0x80 + value));
                }
                else
                {
                    ms.WriteByte((byte)(value / 256));
                    ms.WriteByte((byte)(value % 256));
                }

                if (amount > 32768)
                {
                    if (amount >= 65536)
                    {
                        value = (amount / 65536);
                        if (value > 127) value = 127;
                        ms.WriteByte((byte)value);
                        if ((amount % 65536) >= 32768)
                            ms.WriteByte((byte)0x80);
                        else
                            ms.WriteByte((byte)0x00);
                    }
                    else if (amount >= 32768)
                    {
                        ms.WriteByte((byte)0x00);// < 128 = (item Amount / 65,536) > unknown
                        ms.WriteByte((byte)0x80);// < 128 = (item Amount / 65,536) > unknown
                    }
                    else
                    {
                        ms.WriteByte((byte)0x80);// > 128 = item Amount > 32768
                    }
                }

                if (item.Prefix != 0 || item.Suffix != 0)
                {
                    ms.WriteByte((byte)item.Suffix);
                    sbyte[] data = new sbyte[1] { (sbyte)(item.Prefix * 2) };
                    byte[] equivalentData = (byte[])(object)data;
                    ms.WriteByte(equivalentData[0]);
                }

                if (item.DuraMax != item.DuraMin)
                {
                    ms.WriteByte((byte)(item.DuraMax / 2));
                    ms.WriteByte((byte)(item.DuraMin + ((item.DuraMax % 2) * 128)));
                }
            }
            EnsureCapacity((int)ms.Length);
            Write(ms.ToArray());
        }
    }

    public sealed class PlayerEquipmentPacket : Packet
    {
        public PlayerEquipmentPacket(IMobile m) : base(0x47, 52)
        {
            IItem weapon = (IItem)m.Weapon;
            if (weapon != null)
            {
                Write((short)weapon.ItemID);//weapon/right hand
                Write((byte)0x00);
                Write((byte)0x00);
                Write((byte)weapon.Prefix);//prefix
                Write((byte)weapon.Suffix);//postfix
                Write((byte)weapon.DuraMin);//min dura
                Write((byte)weapon.DuraMax);//max dura
            }
            else
            {
                Write((short)0);//weapon/right hand
                Write((byte)0x00);
                Write((byte)0x00);
                Write((byte)0x00);//prefix
                Write((byte)0x00);//postfix
                Write((byte)0x00);//min dura
                Write((byte)0x00);//max dura

            }

            if (m.ChestArmor != null)
            {
                Write((short)m.ChestArmor.ItemID);//Armor
                Write((byte)m.ChestArmor.Prefix);//prefix
                Write((byte)m.ChestArmor.Suffix);//postfix
                Write((byte)m.ChestArmor.DuraMin);//min dura
                Write((byte)m.ChestArmor.DuraMax);//max dura
            }
            else
            {
                Write((short)0);//Armor
                Write((byte)0);//prefix
                Write((byte)0);//postfix
                Write((byte)0);//total dura
                Write((byte)0);//current dura
            }

            if (m.ShieldArmor != null)
            {
                Write((short)m.ShieldArmor.ItemID);
                Write((byte)m.ShieldArmor.Prefix);//prefix
                Write((byte)m.ShieldArmor.Suffix);//postfix
                Write((byte)m.ShieldArmor.DuraMin);//min dura
                Write((byte)m.ShieldArmor.DuraMax);//max dura
            }
            else
            {
                Write((short)0);//Armor
                Write((byte)0);//prefix
                Write((byte)0);//postfix
                Write((byte)0);//total dura
                Write((byte)0);//current dura
            }

            if (m.HeadArmor != null)
            {
                Write((short)m.HeadArmor.ItemID);
                Write((byte)m.HeadArmor.Prefix);//prefix
                Write((byte)m.HeadArmor.Suffix);//postfix
                Write((byte)m.HeadArmor.DuraMin);//min dura
                Write((byte)m.HeadArmor.DuraMax);//max dura
            }
            else
            {
                Write((short)m.HairID);//Armor
                Write((byte)0);//prefix
                Write((byte)0);//postfix
                Write((byte)0);//total dura
                Write((byte)0);//current dura
            }

            if (m.Ring1 != null)
            {
                Write((short)m.Ring1.ItemID);
                Write((byte)m.Ring1.Prefix);//prefix
                Write((byte)m.Ring1.Suffix);//postfix
                Write((byte)m.Ring1.DuraMin);//min dura
                Write((byte)m.Ring1.DuraMax);//max dura
            }
            else
            {
                Write((short)0);//Armor
                Write((byte)0);//prefix
                Write((byte)0);//postfix
                Write((byte)0);//total dura
                Write((byte)0);//current dura
            }

            if (m.HandArmor != null)
            {
                Write((short)m.HandArmor.ItemID);
                Write((byte)m.HandArmor.Prefix);//prefix
                Write((byte)m.HandArmor.Suffix);//postfix
                Write((byte)m.HandArmor.DuraMin);//min dura
                Write((byte)m.HandArmor.DuraMax);//max dura
            }
            else
            {
                Write((short)0);//Armor
                Write((byte)0);//prefix
                Write((byte)0);//postfix
                Write((byte)0);//total dura
                Write((byte)0);//current dura
            }

            if (m.Ring2 != null)
            {
                Write((short)m.Ring2.ItemID);
                Write((byte)m.Ring2.Prefix);//prefix
                Write((byte)m.Ring2.Suffix);//postfix
                Write((byte)m.Ring2.DuraMin);//min dura
                Write((byte)m.Ring2.DuraMax);//max dura
            }
            else
            {
                Write((short)0);//Armor
                Write((byte)0);//prefix
                Write((byte)0);//postfix
                Write((byte)0);//total dura
                Write((byte)0);//current dura
            }

            if (m.Boots != null)
            {
                Write((short)m.Boots.ItemID);
                Write((byte)m.Boots.Prefix);//prefix
                Write((byte)m.Boots.Suffix);//postfix
                Write((byte)m.Boots.DuraMin);//min dura
                Write((byte)m.Boots.DuraMax);//max dura
            }
            else
            {
                Write((short)0);//Armor
                Write((byte)0);//prefix
                Write((byte)0);//postfix
                Write((byte)0);//total dura
                Write((byte)0);//current dura
            }

            Write((byte)m.Gender);

            Write((byte)m.AttackRating);//Attack rating

            Encrypt();
        }
    }

    public sealed class PlayerSpellListPacket : Packet
    {
        public PlayerSpellListPacket(IMobile m) : base(0x11)
        {
            int spellCount = m.Spells.Count;

            this.EnsureCapacity(spellCount * 2);

            foreach (LearnedSpell sp in m.Spells)
            {
                Write((byte)sp.Id);
                Write((byte)sp.Level);
            }
        }
    }

    public sealed class PlayerSkillListPacket : Packet
    {
        public PlayerSkillListPacket(IMobile m) : base(0x16)
        {
            int skillCount = m.Skills.Length;
            this.EnsureCapacity((skillCount * 3));

            for (int i = 0; i < skillCount; i++)
            {
                Skill sk = m.Skills[i];
                Write((byte)(sk.Id));//Skill ID
                double skillLevel = sk.Level;//only calc once
                byte value = (byte)Math.Truncate(skillLevel);
                byte subValue = (byte)((skillLevel - Math.Truncate(skillLevel)) * 100.0);
                Write((byte)value);//Skill Level 
                Write((byte)subValue);//Skill Level 
            }
        }
    }

    public sealed class PlayerNamePacket : Packet
    {
        public PlayerNamePacket(IMobile m) : base(0x04)
        {
            string name = m.Name;

            if (name == null) name = "";

            this.EnsureCapacity(name.Length + 2);

            Write((byte)76);
            Write((byte)m.PlayerSocket.SocketID);
            WriteAsciiNull(name);
        }
    }

    public sealed class PlayerIncomingPacket : Packet
    {
        public PlayerIncomingPacket(IMobile m) : base(0x04)
        {
            string name = m.Name;

            if (name == null) name = "";

            this.EnsureCapacity(name.Length + 2);

            Write((byte)0x2b);
            Write((byte)m.PlayerSocket.SocketID);
            WriteAsciiNull(name);
        }
    }

    public sealed class PlayerHouseingPacket : Packet
    {
        public PlayerHouseingPacket(IMobile m) : base(0x2F, 2)
        {
            Write((byte)m.GuildHallId);//guild hall number
            Write((byte)m.HouseId);//house number
        }
    }

    public sealed class PlayerTeleportPacket : Packet
    {
        public PlayerTeleportPacket(byte Map, int X, int Y, byte Z) : base(0x18, 7)
        {
            Write((byte)Map); //Map
            Write((byte)0x00);
            Write((short)X);
            Write((short)Y);
            Write((byte)Z);
        }
    }

    public sealed class PlayerHMEPacket : Packet
    {
        public PlayerHMEPacket(IMobile m) : base(0x48, 6)
        {
            //26 24 47
            int size = 6;
            if (m.HealthMax >= 128) size++;
            if (m.Health >= 128) size++;
            if (m.ManaMax >= 128) size++;
            if (m.Mana >= 128) size++;
            if (m.EnergyMax >= 128) size++;
            if (m.Energy >= 128) size++;

            this.EnsureCapacity(size);

            WriteCompressed((short)m.HealthMax);
            WriteCompressed((short)m.Health);

            WriteCompressed((short)m.ManaMax);
            WriteCompressed((short)m.Mana);

            WriteCompressed((short)m.EnergyMax);
            WriteCompressed((short)m.Energy);

            Encrypt();
        }
    }

    public sealed class PlayerStatsPacket : Packet
    {
        public PlayerStatsPacket(IMobile m) : base(0x0D, 14)
        {
            Write((short)m.AvailablePoints);
            Write((short)m.RawStats.Strength);
            Write((short)m.RawStats.Defence);
            Write((short)m.RawStats.Consititution);
            Write((short)m.RawStats.Intelligence);
            Write((short)m.RawStats.Magic);
            Write((short)m.RawStats.Stamina);
        }
    }

    public sealed class PlayerLevelPacket : Packet
    {
        public PlayerLevelPacket(IMobile m) : base(0x21, 6)
        {
            Write((ushort)m.Level);
            Write((double)m.Experience);
        }
    }

    public sealed class PlayerMinMaxDamagePacket : Packet
    {
        public PlayerMinMaxDamagePacket(IMobile m) : base(0xB5)
        {
            int size = 2;
            int min = m.MeleeDamageMin;
            int max = m.MeleeDamageMax;
            if (min >= 128) size++;
            if (max >= 128) size++;
            EnsureCapacity(size);
            WriteCompressed((short)min);//Min Damage
            WriteCompressed((short)max);//Max Damage
        }
    }

    public sealed class PlayerDeathDisplayPacket : Packet
    {
        public PlayerDeathDisplayPacket(byte dialogID) : base(0x0C, 1)
        {
            Write((byte)dialogID);
        }
    }

    public sealed class PlayerResurrectPacket : Packet
    {
        public PlayerResurrectPacket(IMobile m) : base(0x6C, 2)
        {
            Write((byte)0x44);
            Write((byte)m.PlayerSocket.SocketID);
        }
    }

    public sealed class PlayerBankPacket : Packet
    {
        public PlayerBankPacket(IMobile m) : base(0x26)
        {
            int size = 5;

            foreach (ContainerItem item in m.Bank.Items)
            {
                size += 2;
                if (item.Item.ItemID >= 128) size++;
            }

            EnsureCapacity(size);
            Write((byte)0x01);
            Write((byte)(m.Bank.Gold / 8000000));
            int remGold = (m.Bank.Gold % 8000000);
            Write((byte)(remGold / 32768));
            Write((byte)(remGold % 256));
            Write((byte)((remGold % 32768) / 256));

            foreach (ContainerItem item in m.Bank.Items)
            {
                Write((byte)item.LocationID);//Position
                WriteCompressed((short)item.Item.ItemID);//Compressed short (itemid)
            }
        }
    }

    public sealed class PlayerRemove : Packet
    {
        public PlayerRemove(IMobile m) : base(0x04, 2)
        {
            Write((byte)0x2D);
            Write((byte)m.PlayerSocket.SocketID);
        }
    }

    public sealed class PlayerAttackAnimationPacket : Packet
    {
        public PlayerAttackAnimationPacket(IMobile m) : base(0x2C)
        {
            int playerID = m.PlayerSocket.SocketID;
            int size = 2;

            EnsureCapacity(size);
            Write((byte)playerID);
            Write((byte)m.Direction);
        }
    }

    public sealed class GePlayerNameReplyPacket : Packet
    {
        public GePlayerNameReplyPacket(IMobile m) : base(0x03)//sent from all mobiles
        {
            this.EnsureCapacity(m.Name.Length + 3);

            Write((byte)0x62);
            Write((byte)0x21);//0x21
            Write((byte)m.PlayerSocket.SocketID); //character id
            WriteAsciiNull(m.Name);
        }
    }

    public sealed class GetPlayerLookPacket : Packet
    {
        public GetPlayerLookPacket(IMobile m) : base(0x4A)
        {
            int size = 15;
            int armorID = 0;
            int helmetID = m.HairID;
            int shieldID = 0;
            int glovesID = 0;
            int bootsID = 0;
            int weaponID = 0;

            if (m.ChestArmor != null) armorID = m.ChestArmor.ItemID;
            if (m.HeadArmor != null) helmetID = m.HeadArmor.ItemID;
            if (m.ShieldArmor != null) shieldID = m.ShieldArmor.ItemID;
            if (m.HandArmor != null) glovesID = m.HandArmor.ItemID;
            if (m.Boots != null) bootsID = m.Boots.ItemID;
            if (m.Weapon != null) weaponID = m.Weapon.ItemID;

            if (armorID >= 128) size++;
            if (helmetID >= 128) size++;
            if (shieldID >= 128) size++;
            if (glovesID >= 128) size++;
            if (bootsID >= 128) size++;
            if (weaponID >= 128) size++;
            if (m.HealthMax >= 128) size++;
            if (m.ManaMax >= 128) size++;

            EnsureCapacity(size);
            Write((byte)0xC3);//0xC3  //??
            Write((byte)0x04);//0x04  //??
            Write((byte)m.PlayerSocket.SocketID);

            //if (m.BodyMod != null)
            //    Write((byte)m.BodyMod.BodyID);//0x00  (val%128) == mob to look like
            //else
                Write((byte)0x00);//0x00  (val%128) == mob to look like

            Write((byte)0x00);//0x00  //Spell around player

            //if (m.Skills.Highest.BaseFixedPoint == 1000)
                Write((byte)0xFF);        //1-33 Skill Title 0 = the Person
            //else
            //    Write((byte)(m.Skills.Highest.SkillID + 1));

            WriteCompressed((short)m.HealthMax);         //Compressed short Mana Perc 148 = max > 148 = sub from max;
            WriteCompressed((short)m.ManaMax);         //Compressed short Mana Perc 148 = max > 148 = sub from max;
            WriteCompressed((short)armorID);//Compressed short Armor
            WriteCompressed((short)helmetID);//Compressed short Helmet
            WriteCompressed((short)shieldID);//Compressed short Shield
            WriteCompressed((short)glovesID);//Compressed short Gloves
            WriteCompressed((short)bootsID);//Compressed short boots
            WriteCompressed((short)weaponID);//Compressed short Weapon
            byte titles = 0;

            //if (m.Criminal)
            //    titles = 4;
            /*
                 1 = Ally
                 2 = Enemy
                 3 = Fellow
                 4 = Criminal
                 8 = Murderer
                12 = Villian
                16 = Believer
                32 = Pagan
                48 = Priest
                64 = High Priest
                80 = Initiate
             */

            Write((byte)titles);//0x00 (val%128) = religion title
            Encrypt();
        }
    }

    public sealed class PlayerLocationPacket : Packet
    {
        public PlayerLocationPacket(IMobile m) : base(0x03, 7)
        {
            Write((byte)0x62);
            Write((byte)0x4A);
            Write((byte)m.PlayerSocket.SocketID);
            Write((ushort)m.X);
            Write((ushort)m.Y);
        }
    }

    public sealed class PlayerDeathBroadcastPacket : Packet
    {
        public PlayerDeathBroadcastPacket(IMobile m) : base(0x41)
        {
            string name = m.Name;

            if (name == null) name = "";

            this.EnsureCapacity(name.Length + 5);

            Write((byte)0x64);
            Write((byte)0x05);
            Write((byte)0);
            Write((byte)0x73);
            Write((byte)name.Length);
            WriteAsciiNull(name);
        }
    }

    public sealed class SnoopingPacket : Packet
    {
        public SnoopingPacket(IItem[] items) : base(0x31)
        {
            int size = items.Length * 7;
            EnsureCapacity(size);

            foreach (IItem item in items)
            {
                Write((short)0);
                Write((short)item.ItemID);
                Write((byte)0);
                Write((short)item.Amount);
            }
        }
    }

    public sealed class ParalyzedPacket : Packet
    {
        public ParalyzedPacket() : base(0x20, 0)
        {
        }
    }

    #endregion

    #region Unknown Packets 

    public sealed class Unk51Packet : Packet
    {
        public Unk51Packet() : base(0x51, 1)
        {
            Write((byte)0x00);
        }
    }

    public sealed class Unk03Packet : Packet
    {
        public Unk03Packet(IMobile m) : base(0x03, 3) //Only sent to mobile
        {
            Write((byte)0x62);
            Write((byte)0x4C);
            Write((byte)m.PlayerSocket.SocketID);
        }
    }

    public sealed class Unk03_1Packet : Packet
    {
        public Unk03_1Packet(IMobile m) : base(0x03, 6)//only sent to mobile
        {
            Write((byte)0x62);
            Write((byte)0x4A);
            Write((byte)m.PlayerSocket.SocketID);
            Write((byte)0x00);
            Write((byte)0x0B); //0x0B
            Write((byte)0x00);
        }
    }

    public sealed class Unk55Packet : Packet
    {
        public Unk55Packet() : base(0x55, 6)
        {
            Write((byte)0x10);//0-21
            Write((byte)0x00);
            Write((byte)0x00);
            Write((byte)0x00);
            Write((byte)0x00);
            Write((byte)0x00);
        }
    }

    public sealed class Unk64ReplyPacket : Packet
    {
        public Unk64ReplyPacket(IMobile m) : base(0x64, 3)
        {
            Write((byte)41);
            Write((byte)0);
            Write((byte)m.Mail.Count);
        }
    }

    #endregion

    #region Map Packets
    public sealed class BrightnessPacket : Packet
    {
        public BrightnessPacket(IMap m) : base(0x09, 1)
        {
            Write((byte)(0x80 + m.Brightness));
        }
    }

    public sealed class WeatherPacket : Packet
    {
        //TODO figure out different weather systems
        public WeatherPacket(byte val1, byte val2, byte val3) : base(0x50, 3)
        {
            Write((byte)val1);
            Write((byte)val2);
            Write((byte)val3);
        }
    }

    public sealed class AddGroundItemPacket : Packet
    {
        public AddGroundItemPacket(GroundItem groundItem) : base(0x8D)
        {
            int size = 5;
            Point3D loc = groundItem.Location;
            int x = loc.X;
            int y = loc.Y;
            int z = loc.Z;

            if (groundItem.LocationID >= 128)
                size++;

            if (groundItem.Item.ItemID >= 128)
                size++;

            EnsureCapacity(size);
            unchecked
            {
                WriteCompressed((short)groundItem.LocationID);
                WriteCompressed((short)groundItem.Item.ItemID);

                //int val3 = (((y % 64) * 2) + (x / 256));
                //int val4 = (x % 256);
                //int val5 = ((z * 8) + (y / 64));
                //Console.WriteLine("{0} {1} {2} {3} {4} {5} {6} {7}", item.LocationID, item.ItemID, x, y, z, val3, val4, val5);
                Write((byte)(((y % 64) * 2) + (x / 256)));      //Y = (val/2)
                Write((byte)(x % 256));
                Write((byte)((z * 8) + (y / 64)));//y += ((val%8)*64)
                //m_Stream.Write((byte)(y / 64));//y += ((val%8)*64)
            }
        }
    }

    public sealed class RemoveGroundItem : Packet
    {
        public RemoveGroundItem(GroundItem groundItem) : base(0x0A, 3)
        {
            Write((byte)0x2D);
            Write((short)groundItem.LocationID);
        }
    }

    public sealed class SpellEffectPacket : Packet
    {
        public SpellEffectPacket(IMobile m, int spellEffect) : base(0x89)
        {
            int size = 3;
            //int spellEffect = m.TempValue;
            int mobID = m.PlayerSocket.SocketID;

            if (mobID >= 128) size++;
            EnsureCapacity(size);

            if (m.Player)
                Write((byte)0x01);
            else
                Write((byte)0x00);

            Write((byte)spellEffect);
            WriteCompressed((short)mobID);

            Encrypt();

        }
    }

    public sealed class PingMiniMapLocationPacket : Packet
    {
        public PingMiniMapLocationPacket(Point3D Location) : base(0x97)
        {
            int size = 3;
            if (Location.X >= 128) size++;
            if (Location.Y >= 128) size++;

            EnsureCapacity(size);

            Write((byte)Location.Z);//??
            WriteCompressed((short)Location.X);
            WriteCompressed((short)Location.Y);
        }
    }

    public enum DoorStatus
    {
        Locked = 0,
        Close = 1,
        Open = 2,
    }

    public sealed class DoorStatusPacket : Packet
    {
        public DoorStatusPacket(IMap map, Point3D location, DoorStatus status) : base(0x5B, 8)
        {
            Write((byte)map.MapID);
            Write((byte)location.Z);
            Write((byte)(location.Y % 256));
            Write((byte)(location.Y / 256));
            Write((byte)(location.X % 256));
            Write((byte)(location.X / 256));

            switch (status)
            {
                case DoorStatus.Close:
                    Write((byte)0x01);//0 or 1
                    Write((byte)0x00);//0 or 1
                    break;
                case DoorStatus.Open:
                    Write((byte)0x00);//0 or 1
                    Write((byte)0x01);//0 or 1
                    break;
                default:
                    Write((byte)0x00);//0 or 1
                    Write((byte)0x00);//0 or 1
                    break;
            }
            Encrypt();
        }
    }

    public sealed class SectorNamePacket : Packet
    {
        //TODO: update to correct values???
        public SectorNamePacket(IMap map, int sector) : base(0x60)
        {
            string SectorName = String.Format("Map {0} Sector {1}", map.MapID, sector);
            EnsureCapacity(SectorName.Length + 2);
            Write((short)sector);
            WriteAsciiNull(SectorName);
            Encrypt();
        }
    }

    public sealed class NPCAttackAnimationPacket : Packet
    {
        public NPCAttackAnimationPacket(IMobile m) : base(0x5F)
        {
            int npcID = m.NPCId;
            int size = 2;

            if (npcID >= 128)
                size++;

            EnsureCapacity(size);
            WriteCompressed((short)npcID);
            Write((byte)m.Direction);
            Encrypt();
        }
    }

    public sealed class NPCMovementPacket : Packet
    {
        public NPCMovementPacket(IMobile m) : base(0x54, 9)
        {
            int dir = (int)m.Direction;
            int mana = 0;
            int bodyID = m.Body.BodyID;//32
            int mapID = m.Map.MapID;
            int npcID = m.NPCId;
            int nameID = m.NameID;//10 for -vendor-
            if (npcID >= 8192) return;//max id of 8191
            int healthPerc = (int)(128.0 * ((double)m.Health / (double)m.HealthMax));

            int size = 9;
            if (npcID >= 32)
                size++;

            if ((healthPerc < 128) || (m.Z > 0))
                size++;

            EnsureCapacity(size);

            Write((byte)mapID);

            if (npcID >= 32)
            {
                Write((byte)(npcID / 64));
                Write((byte)((npcID * 4) % 256));//0 = Location+look, 1 = unk, 2 = location only, 3 = unknown 
            }
            else
            {
                Write((byte)(0x80 + (npcID * 4)));//0 = Location+look, 1 = unk, 2 = location only, 3 = unknown
            }
            //val2
            Write((byte)(((nameID % 2) * 128) + mana));// mana perc = 0 - 128

            //val3
            Write((byte)(((nameID % 256) / 2) + ((bodyID % 2) * 128)));//5

            //val4
            Write((byte)(((dir / 4) * 128) + (bodyID / 2)));
            //val6
            Write((byte)(dir % 4));
            //max health = 128 

            if (m.Health > m.HealthMax) m.Health = m.HealthMax;

            if ((healthPerc < 128) || (m.Z > 0))
            {
                healthPerc = 128 - healthPerc;
                Write((byte)((m.Z * 2) + (healthPerc > 64 ? 1 : 0)));//Z
                Write((byte)(m.X % 256));//X
                Write((byte)(((m.Y % 128) * 2) + (m.X / 256)));//(Y*2)+(X/256)
                Write((byte)(((healthPerc % 64) * 4) + (m.Y / 128)));
            }
            else
            {
                Write((byte)(0x80 + (m.Y / 128)));//128 // y = (v/4)*128
                Write((byte)(m.X % 256));//X
                Write((byte)(((m.Y % 128) * 2) + (m.X / 256)));//(Y*2)+(X/256)
            }
            Encrypt();

        }
    }

    public sealed class NPCLookPacket : Packet
    {
        public NPCLookPacket(IMobile m) : base(0x4D, 34)
        {
            int armorID = 0;
            int helmetID = m.HairID;
            int shieldID = 0;
            int glovesID = 0;
            int bootsID = 0;
            int weaponID = 0;

            if (m.ChestArmor != null) armorID = m.ChestArmor.ItemID;
            if (m.HeadArmor != null) helmetID = m.HeadArmor.ItemID;
            if (m.ShieldArmor != null) shieldID = m.ShieldArmor.ItemID;
            if (m.HandArmor != null) glovesID = m.HandArmor.ItemID;
            if (m.Boots != null) bootsID = m.Boots.ItemID;
            if (m.Weapon != null) weaponID = m.Weapon.ItemID;

            Write((byte)m.Map.MapID);
            Write((short)m.NPCId);
            WriteAsciiFixed(m.Name, 16);
            Write((byte)m.Body.BodyType1);//body
            Write((byte)m.Body.BodyType2);//body

            Write((short)weaponID);
            Write((short)shieldID);
            Write((short)helmetID);
            Write((short)glovesID);
            Write((short)bootsID);
            Write((short)armorID);

            Write((byte)m.Gender);
        }
    }

    public sealed class NPCMessagePacket : Packet
    {
        public NPCMessagePacket(IMobile m, string message) : base(0x1C)
        {
            int npcID = m.NPCId;
            int size = message.Length + 1;
            if (npcID >= 128) size++;

            EnsureCapacity(size);

            WriteCompressed((short)npcID);
            WriteAsciiNull(message);
        }
    }

    public sealed class NPCAnimationPacket : Packet
    {
        public NPCAnimationPacket(IMobile m) : base(0x54)
        {
            int npcID = m.NPCId;
            int size = 4;

            if (npcID >= 32)
                size++;

            EnsureCapacity(size);

            if (npcID >= 32)
            {
                Write((byte)(npcID / 64));
                Write((byte)(((npcID * 4) % 256) + 2));//0 = Location+look, 1 = unk, 2 = animation, 3 = unknown 
            }
            else
            {
                Write((byte)(0x80 + (npcID * 4) + 2));//0 = Location+look, 1 = unk, 2 = animation, 3 = unknown
            }
            Write((byte)0x0A);
            Write((byte)(0xA0 + m.Direction));
            Write((byte)0x0C);
            Encrypt();
        }
    }

    public sealed class NPCRemovePacket : Packet
    {
        public NPCRemovePacket(IMobile m) : base(0x5E)
        {
            int npcID = m.NPCId;
            if (npcID >= 128)
                EnsureCapacity(2);
            else
                EnsureCapacity(1);
            WriteCompressed((short)npcID);
            Encrypt();
        }
    }

    public sealed class NPCDialogPacket : Packet
    {
        public NPCDialogPacket(NPCDialog dialog) : base(0x4E)
        {
            int size = 15;
            size += dialog.Title.Length;//null terminated
            size += dialog.Text.Length;//null terminated

            foreach (string str in dialog.Options)
                size += str.Length + 3;

            EnsureCapacity(size);

            Write((short)(dialog.Owner.NPCId));
            Write((byte)dialog.Owner.Map.MapID);
            Write((short)dialog.Text.Length);
            WriteAsciiNull(dialog.Text);

            Write((byte)dialog.Options.Length);
            Write((byte)0x00);
            Write((byte)0x00);
            Write((byte)0x00);
            Write((byte)0x00);
            Write((byte)0x00);
            Write((byte)0x00);
            Write((byte)0x00);
            Write((byte)0x00);
            Write((byte)0x00);

            foreach (String str in dialog.Options)
            {
                Write((short)(str.Length + 1));
                WriteAsciiNull(str);
                Write((byte)0x00);
            }

            WriteAsciiNull(dialog.Title);

            Encrypt();
        }
    }

    public sealed class VendorDialogPacket : Packet
    {
        public VendorDialogPacket(NPCVendor vendor) : base(0x4F)
        {
            int size = 4;
            size += vendor.VendorItems.Length * 13;

            EnsureCapacity(size);

            Write((byte)vendor.Owner.Map.MapID);

            Write((short)vendor.Owner.NPCId);

            Write((byte)vendor.Sale);//Buy 0 / Sell = 1

            foreach (VendorItem vendorItem in vendor.VendorItems)
            {
                double buyPrice = vendorItem.Price;
                IItem item = vendorItem.Item;
                Write((byte)vendorItem.LocationID);
                Write((short)item.ItemID);
                Write((byte)item.Prefix);
                Write((byte)item.Suffix);
                Write((double)item.Amount);
                Write((double)buyPrice);
            }

            Encrypt();
        }
    }

    public enum ProjectTileType
    {
        Fireball = 1,
        MagicMissile = 2,
        Arrow = 3,
        IceBolt = 4,
        DarkTouch = 5,
        Lightning = 6,
        MagicMissile2 = 7,
        ThrowingStar = 8,
        ThrowingStar2 = 9,
        ThrowingStar3 = 10,
        FlamingArrow = 12,
        Spear = 13,
        Spear2 = 14,
        Arrow2 = 15
    }

    public sealed class Projectile : Packet
    {
        public Projectile(byte typeID, Point3D startLoc, Point3D endLoc, int speed = 6) : base(0x5D)
        {
            //336, 91
            int size = 7;
            if (startLoc.X >= 128) size++;
            if (startLoc.Y >= 128) size++;
            double angle = Utility.GetAngle(startLoc, endLoc);

            this.EnsureCapacity(size);

            Write((byte)speed);
            WriteCompressed((short)startLoc.X);
            WriteCompressed((short)startLoc.Y);

            Write((byte)startLoc.Z);
            Write((byte)(angle % 180));
            Write((byte)typeID);
            Write((byte)0x7F);//projectileLife????

            Encrypt();
        }
    }

    //public sealed class MapPatch : Packet
    //{
    //    public MapPatch(Map map, Point2D Location) : base(0x56)
    //    {
    //        int sector = ((Location.X / 16) * 32) + (Location.Y / 16);

    //        MemoryStream ms = new MemoryStream();
    //        ms.WriteByte(0x07);//unknown

    //        int checksum = map.GetCheckSum((short)sector);

    //        ms.WriteByte((byte)(checksum % 256));
    //        ms.WriteByte((byte)(checksum / 256));

    //        ms.WriteByte((byte)map.MapID);
    //        ms.WriteByte((byte)(sector % 256));
    //        ms.WriteByte((byte)(sector / 256));
    //        for (int x = Location.X; x < Location.X + 16; x++)
    //        {
    //            Tile[] lastTiles = new Tile[0];
    //            for (int y = Location.Y; y < Location.Y + 16; y++)
    //            {
    //                Tile[] tiles = map.Tiles.GetLandBlock(x, y);

    //                Tile t = tiles[0];
    //                int id = 0;

    //                bool hasNextzLevel = false;
    //                for (int i = 1; i < tiles.Length; i++)
    //                    if (tiles[i].icon1 != 0 || tiles[i].item1 != 0 || tiles[i].item2 != 0 ||
    //                        !String.IsNullOrEmpty(tiles[i].ScriptName) || !String.IsNullOrEmpty(tiles[i].MapCommand))
    //                        hasNextzLevel = true;

    //                if (t.icon2 != 0) id += 2;
    //                if (t.item1 != 0 || t.item2 != 0 || !String.IsNullOrEmpty(t.ScriptName) || !String.IsNullOrEmpty(t.MapCommand) || hasNextzLevel) id += 4;

    //                if (lastTiles.Length != 0)
    //                {
    //                    bool lthasNextzLevel = false;
    //                    for (int i = 1; i < lastTiles.Length; i++)
    //                        if (lastTiles[i].icon1 != 0 || lastTiles[i].item1 != 0 || lastTiles[i].item2 != 0 ||
    //                            !String.IsNullOrEmpty(lastTiles[i].ScriptName) || !String.IsNullOrEmpty(lastTiles[i].MapCommand))
    //                            lthasNextzLevel = true;

    //                    if (lastTiles[0].icon1 == t.icon1 && lastTiles[0].icon2 == t.icon2 && lastTiles[0].item1 == t.item1 && lastTiles[0].item2 == t.item2 &&
    //                        lastTiles[0].ScriptName == t.ScriptName && lastTiles[0].MapCommand == t.MapCommand && lthasNextzLevel == hasNextzLevel)
    //                        id = 0x01;//duplicate tile
    //                }

    //                //0x03?
    //                if (id == 0x01)
    //                    ms.WriteByte((byte)0x03);
    //                else
    //                    ms.WriteByte((byte)id);

    //                if (id != 0x01)
    //                {
    //                    ms.WriteByte((byte)(t.icon1 % 256));
    //                    ms.WriteByte((byte)(t.icon1 / 256));

    //                    if (t.icon2 != 0)
    //                    {
    //                        ms.WriteByte((byte)(t.icon2 % 256));
    //                        ms.WriteByte((byte)(t.icon2 / 256));
    //                    }
    //                }

    //                lastTiles = tiles;
    //            }
    //        }

    //        for (int zIndex = 0; zIndex < map.Tiles.MaxLayers; zIndex++)
    //        {
    //            //item1
    //            for (int x = Location.X; x < Location.X + 16; x++)
    //            {
    //                Tile[] lastTiles = new Tile[0];
    //                for (int y = Location.Y; y < Location.Y + 16; y++)
    //                {
    //                    Tile[] tiles = map.Tiles.GetLandBlock(x, y);

    //                    Tile t = tiles[zIndex];
    //                    Tile t2 = new Tile();

    //                    if (zIndex + 1 < tiles.Length)
    //                        t2 = tiles[zIndex + 1];

    //                    int id = 0;

    //                    bool hasItem = (t.item1 != 0);
    //                    bool hasItemIndex = (t.item1Index != 0);
    //                    bool hasItem2 = (t.item2 != 0);
    //                    bool hasnzIcon = (t2.icon1 != 0);
    //                    bool hasScript = !String.IsNullOrEmpty(t.ScriptName);
    //                    bool hasMapCommand = !String.IsNullOrEmpty(t.MapCommand);
    //                    bool hasNextzLevel = false;

    //                    for (int i = 1; i < tiles.Length; i++)
    //                        if (tiles[i].icon1 != 0 || tiles[i].item1 != 0 || tiles[i].item2 != 0 ||
    //                            !String.IsNullOrEmpty(tiles[i].ScriptName) || !String.IsNullOrEmpty(tiles[i].MapCommand))
    //                            hasNextzLevel = true;

    //                    if (hasnzIcon)
    //                        id += 2;
    //                    if (hasNextzLevel)
    //                        id += 4;
    //                    //if (hasItem2)
    //                    //    id += 8;
    //                    if (hasItem)
    //                        id += 0x10;
    //                    if (hasItemIndex)
    //                        id += 0x20;
    //                    if (hasItem2 || hasScript || hasMapCommand)
    //                        id += 8;

    //                    //id += 0x40;//rotate 90 degrees
    //                    //id += 0x80;//shift haft tile close to camera

    //                    if (lastTiles.Length != 0)
    //                    {
    //                        Tile lt = lastTiles[zIndex];
    //                        Tile lt2 = new Tile();

    //                        if (zIndex + 1 < lastTiles.Length)
    //                            lt2 = lastTiles[zIndex + 1];

    //                        //bool lthasNextzLevel = false;
    //                        //for (int i = 1; i < lastTiles.Length; i++)
    //                        //    if (lastTiles[i].icon1 != 0 || lastTiles[i].item1 != 0 || lastTiles[i].item2 != 0 || 
    //                        //        !String.IsNullOrEmpty(lastTiles[i].ScriptName) || !String.IsNullOrEmpty(lastTiles[i].MapCommand))
    //                        //        lthasNextzLevel = true;
    //                        //if ((lt.icon1 == t.icon1 && lt.icon2 == t.icon2 && lt.item1 == t.item1 && lt.item1Index == t.item1Index && lt.item2 == t.item2 && lt.item2Index == t.item2Index) &&
    //                        //    (lt2.icon1 == t2.icon1 && lt2.icon2 == t2.icon2 && lt2.item1 == t2.item1 && lt2.item1Index == t2.item1Index && lt2.item2 == t2.item2 && lt2.item2Index == t2.item2Index) &&
    //                        //    lthasNextzLevel == hasNextzLevel)
    //                        //    id = id;// 0x01;//duplicate tile
    //                    }

    //                    if (id != 0)
    //                    {
    //                        ms.WriteByte((byte)id);
    //                        if (id != 0x01)
    //                        {
    //                            if (hasnzIcon)
    //                            {
    //                                ms.WriteByte((byte)(t2.icon1 % 256));
    //                                ms.WriteByte((byte)(t2.icon1 / 256));
    //                            }

    //                            if (hasItem)
    //                            {
    //                                ms.WriteByte((byte)(t.item1 % 256));
    //                                ms.WriteByte((byte)(t.item1 / 256));

    //                                if (hasItemIndex) ms.WriteByte((byte)(t.item1Index));
    //                            }

    //                            if (hasItem2)
    //                            {
    //                                if (t.item2Index == 0)
    //                                    ms.WriteByte((byte)0x02);
    //                                else
    //                                    ms.WriteByte((byte)0x03);

    //                                ms.WriteByte((byte)0x26);
    //                                ms.WriteByte((byte)(t.item2 % 256));
    //                                ms.WriteByte((byte)(t.item2 / 256));

    //                                if (t.item2Index != 0) ms.WriteByte((byte)t.item2Index);
    //                            }
    //                            else if (hasScript)
    //                            {
    //                                ms.WriteByte((byte)(0xE0 + t.ScriptName.Length));
    //                                foreach (byte b in t.ScriptName.ToCharArray())
    //                                    ms.WriteByte(b);
    //                            }
    //                            else if (hasMapCommand)
    //                            {
    //                                if (t.MapCommand.StartsWith("W"))
    //                                {
    //                                    string[] tmpArray = t.MapCommand.Split(' ');
    //                                    ms.WriteByte((byte)0xF6);
    //                                    for (int i = 1; i < 7; i++)
    //                                    {
    //                                        byte value = Byte.Parse(tmpArray[i]);
    //                                        ms.WriteByte((byte)value);
    //                                    }
    //                                }
    //                                else if (t.MapCommand.StartsWith("G"))
    //                                {
    //                                    string[] tmpArray = t.MapCommand.Split(' ');
    //                                    byte val = Byte.Parse(tmpArray[1]);
    //                                    if (val == 0)
    //                                    {
    //                                        ms.WriteByte((byte)0xD0);
    //                                    }
    //                                    else
    //                                    {
    //                                        ms.WriteByte((byte)0xD1);
    //                                        ms.WriteByte((byte)val);
    //                                    }
    //                                }
    //                                else if (t.MapCommand.StartsWith("H"))
    //                                {
    //                                    string[] tmpArray = t.MapCommand.Split(' ');
    //                                    byte val = Byte.Parse(tmpArray[1]);
    //                                    if (val == 0)
    //                                    {
    //                                        ms.WriteByte((byte)0xC0);
    //                                    }
    //                                    else
    //                                    {
    //                                        ms.WriteByte((byte)0xC1);
    //                                        ms.WriteByte((byte)val);
    //                                    }
    //                                }
    //                                else if (t.MapCommand.StartsWith("x"))
    //                                {
    //                                    string[] tmpArray = t.MapCommand.Split(' ');
    //                                    byte val = Byte.Parse(tmpArray[1]);
    //                                    if (val == 0)
    //                                    {
    //                                        ms.WriteByte((byte)0xB0);
    //                                    }
    //                                    else
    //                                    {
    //                                        ms.WriteByte((byte)0xB1);
    //                                        ms.WriteByte((byte)Byte.Parse(tmpArray[1]));
    //                                    }
    //                                }
    //                                else
    //                                {
    //                                    string[] tmpArray = t.MapCommand.Split(' ');
    //                                    if (tmpArray[2] == "0")
    //                                    {
    //                                        ms.WriteByte((byte)0x01);
    //                                        ms.WriteByte((byte)(tmpArray[0].ToCharArray()[0]));
    //                                        ms.WriteByte((byte)Byte.Parse(tmpArray[1]));
    //                                    }
    //                                    else
    //                                    {
    //                                        ms.WriteByte((byte)0x02);
    //                                        ms.WriteByte((byte)(tmpArray[0].ToCharArray()[0]));
    //                                        ms.WriteByte((byte)Byte.Parse(tmpArray[1]));
    //                                        ms.WriteByte((byte)Byte.Parse(tmpArray[2]));
    //                                    }
    //                                }
    //                            }
    //                        }
    //                        lastTiles = tiles;
    //                    }
    //                }
    //            }
    //        }

    //        ms.WriteByte(0x66);//unknown //version???
    //        EnsureCapacity((int)ms.Length);
    //        m_Stream.Write(ms.ToArray(), 0, (int)ms.Length);
    //        m_Stream.Encrypt();
    //    }
    //}

    #endregion

    #region Market Packets
    public sealed class CloseBuyMarketPacket : Packet
    {
        public CloseBuyMarketPacket(IMarket market) : base(0x64, 2)
        {
            Write((byte)0x22);
            Write((byte)market.MarketID);
        }
    }

    public sealed class MarketPacket : Packet
    {
        public MarketPacket(IMarket market) : base(0x64)
        {
            EnsureCapacity(((market.MarketItems.Length - 2) * 14) + 2);

            Write((byte)0x20);
            Write((byte)market.MarketID);

            foreach (MarketItem marketItem in market.MarketItems)
            {
                if (marketItem.Price > 0)
                {
                    double goldPrice = marketItem.Price;

                    Write((byte)0x03);//unknown
                    Write((byte)0x96);//
                    Write((byte)marketItem.LocationID);//ID
                    Write((byte)0x6C);//
                    Write((byte)0x6E);
                    Write((byte)marketItem.Item.Prefix);
                    Write((byte)marketItem.Item.Suffix);
                    Write((byte)0x00);
                    Write((byte)(goldPrice / 80000.0));// 80000.0 gold per
                    goldPrice = (goldPrice % 80000.0);
                    Write((byte)(goldPrice / 327.68)); //327.68 gold per
                    goldPrice = (goldPrice % 327.68);
                    Write((byte)(goldPrice % 2.56));// 0.01 gold per
                    Write((byte)(goldPrice / 2.56));//2.56 gold per
                    Write((byte)0x05);
                    Write((byte)0x05);
                }
            }
        }
    }

    #endregion

    #region Mail Packets
    public sealed class MailList : Packet
    {
        public MailList(IMobile m) : base(0x64)
        {
            MemoryStream ms = new MemoryStream();
            ms.WriteByte((byte)01);
            foreach (MailMessage mailmessage in m.Mail)
            {
                int mailNum = mailmessage.Id;

                if (mailNum >= 32768)
                {
                    ms.WriteByte((byte)(mailNum / 65536));
                    ms.WriteByte((byte)((mailNum % 65536) / 256));
                    ms.WriteByte((byte)(mailNum % 256));
                }
                else
                {
                    ms.WriteByte((byte)(0x80 + (mailNum / 256)));
                    ms.WriteByte((byte)(mailNum % 256));
                }

                ms.WriteByte((byte)mailmessage.From.Name.Length);
                ms.WriteByte((byte)mailmessage.Subject.Length);
                foreach (char c in mailmessage.From.Name.ToCharArray())
                    ms.WriteByte((byte)c);
                foreach (char c in mailmessage.Subject.ToCharArray())
                    ms.WriteByte((byte)c);
            }
            EnsureCapacity((int)ms.Length);
            Write(ms.ToArray());
        }
    }

    public sealed class MailMessagePacket : Packet
    {
        public MailMessagePacket(MailMessage mm) : base(0x64)
        {
            MemoryStream ms = new MemoryStream();
            ms.WriteByte((byte)0x02);
            ms.WriteByte((byte)mm.From.Name.Length);
            ms.WriteByte((byte)mm.Subject.Length);
            ms.WriteByte((byte)(mm.Contents.Length / 256));
            ms.WriteByte((byte)(mm.Contents.Length % 256));
            //Items
            for (int i = 0; i < 4; i++)
            {
                if (mm.Items.Length > i)
                {
                    IItem item = (IItem)mm.Items[i].Item;
                    ms.WriteByte((byte)(item.ItemID % 256));
                    ms.WriteByte((byte)(item.ItemID / 256));
                }
                else
                {
                    ms.WriteByte((byte)0x00);
                    ms.WriteByte((byte)0x00);
                }

            }

            foreach (char c in mm.From.Name.ToCharArray())
                ms.WriteByte((byte)c);
            foreach (char c in mm.Subject.ToCharArray())
                ms.WriteByte((byte)c);
            foreach (char c in mm.Contents.ToCharArray())
                ms.WriteByte((byte)c);
            ms.WriteByte((byte)0x00);//unknown
            ms.WriteByte((byte)0x00);//unknown
            ms.WriteByte((byte)0x90);//unknown

            EnsureCapacity((int)ms.Length);
            Write(ms.ToArray());
        }
    }

    public sealed class SendMailResultPacket : Packet
    {
        public SendMailResultPacket(byte status) : base(0x64, 1)
        {
            Write((byte)status);
        }
    }

    #endregion

    #region Guild Packets
    public sealed class ShowGuildListPacket : Packet
    {
        public ShowGuildListPacket(IGuild[] guilds) : base(0x45, 1)
        {
            int size = 1;
            foreach (IGuild guild in guilds)
                size += 3 + guild.Name.Length;

            EnsureCapacity(size);
            Write((byte)0x00);
            foreach (IGuild guild in guilds)
            {
                Write((short)guild.Id);
                Write((byte)guild.Name.Length);
                WriteAsciiNull(guild.Name);
            }
        }
    }

    public sealed class ShowGuildPacket : Packet
    {
        public ShowGuildPacket(IMobile m, IGuild guild) : base(0x45, 1)
        {
            int size = 10;
            size += guild.Name.Length;
            if (guild.GuildHall.Id == 0)
                size += 1;
            else
                size += guild.GuildHall.Name.Length;

            size += guild.Members.Length * 3;
            foreach (GuildMember gm in guild.Members)
                size += gm.Member.Name.Length;

            if (m == guild.Owner)
                size++;

            size += guild.Decrees.Length * 3;
            foreach (GuildDecree gd in guild.Decrees)
                size += gd.Guild.Name.Length;

            EnsureCapacity(size);

            Write((byte)0x01);//show guild screen

            if (m == guild.Owner)
                Write((byte)0x05);

            Write((byte)0x04);

            //0x00 (Guild Name Length) (Guild Name)
            Write((byte)0x00);//0 = View, 1= Administer
            Write((byte)guild.Name.Length);
            //name
            WriteAsciiNull(guild.Name);

            //0x01 (Hall Name Length) (Hall Name)
            Write((byte)0x01);
            if (guild.GuildHall.Id == 0)
            {
                Write((byte)1);
                WriteAsciiNull("-");
            }
            else
            {
                Write((byte)guild.GuildHall.Name.Length);
                WriteAsciiNull(guild.GuildHall.Name);
            }

            //02 guild alliance/war section
            Write((byte)0x02);
            Write((byte)guild.Decrees.Length);//Num Items
            foreach (GuildDecree gd in guild.Decrees)
            {
                Write((byte)gd.DecreeID);//ID
                Write((byte)gd.DecreeType);//Type 1=War, 2=Piece
                Write((byte)gd.Guild.Name.Length);//Name Length
                WriteAsciiNull(gd.Guild.Name);
            }

            //03 members section
            Write((byte)0x03);
            Write((byte)guild.Members.Length);
            foreach (GuildMember gm in guild.Members)
            {
                Write((byte)gm.MemberID);
                Write((byte)gm.MemberType);
                Write((byte)gm.Member.Name.Length);
                WriteAsciiNull(gm.Member.Name);
            }
        }
    }

    public sealed class ShowGuildApplicantsPacket : Packet
    {
        public ShowGuildApplicantsPacket(IGuild guild) : base(0x45)
        {
            //4   new applicants
            //9   purchase guild hall
            //15  don't have enough money in bank to purchase guild halll.
            //16  you have purchase guild hall
            //17  you have sold guild hall
            //18  you already own a guild hall
            //110 war / peace decrees

            int size = 1;

            size += guild.Applicants.Length * 2;
            foreach (GuildApplicant ga in guild.Applicants)
            {
                size += ga.Applicant.Name.Length;
            }
            EnsureCapacity(size);
            Write((byte)4);//show guild applicants screen

            foreach (GuildApplicant ga in guild.Applicants)
            {
                Write((byte)ga.ApplicantID);
                Write((byte)ga.Applicant.Name.Length);
                WriteAsciiNull(ga.Applicant.Name);
            }
        }
    }

    public sealed class ShowGuildHallPurchaseResultPacket : Packet
    {
        public ShowGuildHallPurchaseResultPacket(byte result) : base(0x45, 1)
        {
            //15  don't have enough money in bank to purchase guild halll.
            //16  you have purchase guild hall
            //17  you have sold guild hall
            //18  you already own a guild hall
            Write((byte)result);
        }
    }

    public sealed class ShowGuildHallsPacket : Packet
    {
        public ShowGuildHallsPacket(IGuildHall[] availableghs) : base(0x45)
        {
            //110 war / peace decrees

            int size = 2;

            size += availableghs.Length * 5;
            foreach (IGuildHall gh in availableghs)
                size += gh.Name.Length;

            EnsureCapacity(size);
            Write((byte)9);
            Write((byte)availableghs.Length);

            foreach (IGuildHall gh in availableghs)
            {
                Write((byte)gh.Id);//ID
                Write((byte)gh.Name.Length);//name length
                WriteAsciiNull(gh.Name);//name
                Write((byte)(gh.Price / 32768));           //price/32768
                Write((byte)(gh.Price % 256));             //price
                Write((byte)((gh.Price % 32768) / 256));   //price/256
            }
        }
    }

    public sealed class ShowGuildDecreesPacket : Packet
    {
        public ShowGuildDecreesPacket(IGuild[] availablegs) : base(0x45)
        {
            //110 war / peace decrees

            int size = 1;

            size += availablegs.Length * 3;
            foreach (IGuild gh in availablegs)
                size += gh.Name.Length;

            EnsureCapacity(size);

            Write((byte)110);
            //m_Stream.Write((byte)(availablegs.Count-1));

            foreach (IGuild g in availablegs)
            {
                Write((UInt16)g.Id);//ID
                Write((byte)g.Name.Length);//name length
                WriteAsciiNull(g.Name);//name
                //m_Stream.Write((byte)0);
            }
        }
    }

    #endregion 

    public sealed class EditNPCPacket : Packet
    {
        public EditNPCPacket(MobileSpawn m) : base(0x2D)
        {
            if (m.NameID == 0)
            {
                EnsureCapacity(33);
                for (int i = 0; i < 33; i++)
                    Write((byte)0);
            }
            else
            {
                EnsureCapacity(33);

                Write((byte)m.NameID);
                Write((byte)(m.SpawnDelay % 256));//SpawnDelay 0 - 2000
                //4 Region values -2 - 77
                //X1
                Write((byte)(m.Bounds.X % 256));
                Write((byte)(m.Bounds.X / 256));
                //X2
                Write((byte)((m.Bounds.X + m.Bounds.Width) % 256));
                Write((byte)((m.Bounds.X + m.Bounds.Width) / 256));
                //Y1
                Write((byte)(m.Bounds.Y % 256));
                Write((byte)(m.Bounds.Y / 256));
                //Y2
                Write((byte)((m.Bounds.Y + m.Bounds.Height) % 256));
                Write((byte)((m.Bounds.Y + m.Bounds.Height) / 256));
                //X
                Write((byte)(m.SpawnLocation.X % 256));
                Write((byte)(m.SpawnLocation.X / 256));
                //Y
                Write((byte)(m.SpawnLocation.Y % 256));
                Write((byte)(m.SpawnLocation.Y / 256));

                //16 char script name
                if (!string.IsNullOrEmpty(m.ScriptName))
                {
                    char[] script = m.ScriptName.ToCharArray();
                    for (int i = 0; i < 17; i++)
                    {
                        if (script.Length > i)
                            Write((byte)script[i]);
                        else
                            Write((byte)0);
                    }
                }
                else
                {
                    for (int i = 0; i < 17; i++)
                        Write((byte)0);
                }

                //flags
                for (int i = 0; i < 2; i++)
                    Write((byte)0);

            }
        }
    }

    public sealed class ScriptDownloadPacket : Packet
    {
        public ScriptDownloadPacket(NPCSpawnInfo spawnInfo) : base(0x4C, 49)
        {
            EnsureCapacity(spawnInfo.Script.Length + 49);
            WriteAsciiFixed(spawnInfo.ScriptName, 16);
            WriteAsciiFixed(spawnInfo.NpcName, 16);
            Write((UInt16)spawnInfo.Picture);//Pic
            Write((UInt16)spawnInfo.Weapon);//Weapon
            Write((UInt16)spawnInfo.Shield);//Shield
            Write((UInt16)spawnInfo.Helmet);//Helmet
            Write((UInt16)spawnInfo.Gauntlet);//Gauntlet
            Write((UInt16)spawnInfo.Boots);//Boots
            Write((UInt16)spawnInfo.Armour);//Armour
            Write((byte)spawnInfo.Gender);//Gender
            Write((UInt16)spawnInfo.Script.Length);//FileLength???
            WriteAsciiNull(spawnInfo.Script);//FileContents...
        }
    }

}
