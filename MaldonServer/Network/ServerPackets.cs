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
        CharInvPw       = 0x21,
        CharInUse       = 0x6A,
    }

    public sealed class AccountCreateLoginReply : Packet
    {
        public AccountCreateLoginReply(ALRReason reason) : base(0x65, 1)
        {
            Write((byte)reason);
        }
    }

    public sealed class CharacterLoginReply : Packet
    {
        public CharacterLoginReply(ALRReason reason) : base(0x02, 1)
        {
            Write((byte)reason);
        }
        public CharacterLoginReply(IMobile m) : base(0x02, 9)
        {
            Write((byte)94);//need to be 94 for it to connect
            Write((short)m.X);
            Write((short)m.Y);
            Write((byte)7); //Map

            Write((byte)0x05);//02 A0 39 
            Write((byte)0x67);//
            Write((byte)0x72);// 
        }
    }

    public sealed class CharacterCreateReply : Packet
    {
        public CharacterCreateReply(ALRReason reason) : base(0x02, 1)
        {
            Write((byte)reason);
        }
    }

    public sealed class CharacterList : Packet
    {
        public CharacterList(IAccount a) : base(0x65)
        {
            int length = 1;
            for (int i = 0; i < a.Mobiles.Length; ++i)
            {
                IMobile m = a.Mobiles[i];

                if (m != null)
                {
                    length += m.Name.Length + 1;
                }
            }
            EnsureCapacity(length);

            Write((byte)1);
            for (int i = 0; i < a.Mobiles.Length; ++i)
            {
                IMobile m = a.Mobiles[i];

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

    public sealed class HardCodedMessage : Packet
    {
        //TODO: update to correct values???
        public HardCodedMessage(byte messageID) : base(0x41, 3)
        {
            Write((byte)0x64);
            Write((byte)messageID);
            Write((byte)0x00);
        }
        public HardCodedMessage(byte messageID, byte number) : base(0x41, 6)
        {
            Write((byte)0x01);
            Write((byte)messageID);
            Write((byte)0x00);
            Write((byte)0x69);
            Write((byte)number);
            Write((byte)0x00);
        }
        public HardCodedMessage(byte messageID, string message) : base(0x41)
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

    public sealed class NumberPlayers : Packet
    {
        public NumberPlayers() : base(0x41, 6)
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

    public sealed class Brightness : Packet
    {
        public Brightness(IMobile m) : base(0x09, 1)
        {
            Write((byte)(0x80 + m.Map.Brightness));
        }
    }

    public sealed class TextMessage : Packet
    {
        public TextMessage(MessageType type, string text) : base(0x01)
        {
            if (text == null)
                text = "";

            this.EnsureCapacity(1 + text.Length);

            Write((byte)type);
            WriteAsciiNull(text);
        }
    }

    #region Player Mobile Packets
    public sealed class ReligionPacket : Packet
    {
        //TODO: update to correct values???
        public ReligionPacket(IMobile m) : base(0xA6, 2)
        {
            Write((byte)93);//0x5d = 93   Religion Name Packet
            Write((byte)0x02);
            /*
             * 0 == Agnostic
               1 == Initiate,   Believer
               2 == Believer,   Priest
               3 == 
            */
        }
    }

    public sealed class MobileInventory : Packet
    {
        public MobileInventory(IMobile m) : base(0x96)
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
                byte iLoc = contItem.Location;
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

    public sealed class MobileEquipment : Packet
    {
        public MobileEquipment(IMobile m) : base(0x47, 52)
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

    public sealed class MobileSpellList : Packet
    {
        public MobileSpellList(IMobile m) : base(0x11)
        {
            int spellCount = m.Spells.Length;

            this.EnsureCapacity(spellCount * 2);

            foreach (LearnedSpell sp in m.Spells)
            {
                Write((byte)sp.Id);
                Write((byte)sp.Level);
            }
        }
    }

    public sealed class MobileSkillList : Packet
    {
        public MobileSkillList(IMobile m) : base(0x16)
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

    public sealed class MobileName : Packet
    {
        public MobileName(IMobile m) : base(0x04)
        {
            string name = m.Name;

            if (name == null) name = "";

            this.EnsureCapacity(name.Length + 2);

            Write((byte)76);
            Write((byte)m.PlayerSocket.SocketID);
            WriteAsciiNull(name);
        }
    }

    public sealed class MobileIncoming : Packet
    {
        public MobileIncoming(IMobile m) : base(0x04)
        {
            string name = m.Name;

            if (name == null) name = "";

            this.EnsureCapacity(name.Length + 2);

            Write((byte)0x2b);
            Write((byte)m.PlayerSocket.SocketID);
            WriteAsciiNull(name);
        }
    }

    public sealed class HouseingPacket : Packet
    {
        public HouseingPacket(IMobile m) : base(0x2F, 2)
        {
            Write((byte)m.GuildHallId);//guild hall number
            Write((byte)m.HouseId);//house number
        }
    }

    public sealed class TeleportPacket : Packet
    {
        public TeleportPacket(byte Map, int X, int Y, byte Z) : base(0x18, 7)
        {
            Write((byte)Map); //Map
            Write((byte)0x00);
            Write((short)X);
            Write((short)Y);
            Write((byte)Z);
        }
    }

    public sealed class HealthManaEnergyPacket : Packet
    {
        public HealthManaEnergyPacket(IMobile m) : base(0x48, 6)
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

    public sealed class MobileStats : Packet
    {
        public MobileStats(IMobile m) : base(0x0D, 14)
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

    public sealed class MobileLevel : Packet
    {
        public MobileLevel(IMobile m) : base(0x21, 6)
        {
            Write((ushort)m.Level);
            Write((double)m.Experience);
        }
    }

    public sealed class MinMaxDamageDisplay : Packet
    {
        public MinMaxDamageDisplay(IMobile m) : base(0xB5)
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

    #endregion

    #region Unknown Packets 

    public sealed class Unknown51 : Packet
    {
        public Unknown51() : base(0x51, 1)
        {
            Write((byte)0x00);
        }
    }

    public sealed class Unknown03 : Packet
    {
        public Unknown03(IMobile m) : base(0x03, 3) //Only sent to mobile
        {
            Write((byte)0x62);
            Write((byte)0x4C);
            Write((byte)m.PlayerSocket.SocketID);
        }
    }

    public sealed class Unknown03_1 : Packet
    {
        public Unknown03_1(IMobile m) : base(0x03, 6)//only sent to mobile
        {
            Write((byte)0x62);
            Write((byte)0x4A);
            Write((byte)m.PlayerSocket.SocketID);
            Write((byte)0x00);
            Write((byte)0x0B); //0x0B
            Write((byte)0x00);
        }
    }

    public sealed class Unknown03_2 : Packet
    {
        public Unknown03_2(IMobile m) : base(0x03, 7)//sent from all mobiles
        {
            Write((byte)0x62);
            Write((byte)0x43);
            Write((byte)m.PlayerSocket.SocketID);
            Write((short)m.X);
            Write((short)m.Y);
        }
    }

    public sealed class Unknown55 : Packet
    {
        public Unknown55() : base(0x55, 6)
        {
            Write((byte)0x10);//0-21
            Write((byte)0x00);
            Write((byte)0x00);
            Write((byte)0x00);
            Write((byte)0x00);
            Write((byte)0x00);
        }
    }

    #endregion

}
