using MaldonServer.Network.ServerPackets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaldonServer.Network
{
    public class PacketHandlers
    {
        public static PacketHandler[] Handlers { get; private set; }

        static PacketHandlers()
        {
            Handlers = new PacketHandler[byte.MaxValue];

            //Client initiailization packets
            Register(0x32, false, new OnPacketReceive(Stage1Responce));
            Register(0x6D, false, new OnPacketReceive(Stage2));
            Register(0x29, false, new OnPacketReceive(Stage2Responce));//Unknown

            //Account packets
            Register(0x63, false, new OnPacketReceive(AccountPackets));
            Register(0x02, false, new OnPacketReceive(LoginCharacter));
            Register(0x03, false, new OnPacketReceive(CreateCharacter));

            //Misc Packets
            Register(0x16, false, new OnPacketReceive(Empty));//Ping
            Register(0x34, true, new OnPacketReceive(Empty));//not sure what Packet 0x34 does send every few seconds

            //Player Packets
            Register(0x01, true, new OnPacketReceive(TextCommand));
            Register(0x04, true, new OnPacketReceive(MovementReq));
            Register(0x0C, true, new OnPacketReceive(AddStatPoint));
            Register(0x0F, true, new OnPacketReceive(CastSpell));
            Register(0x10, true, new OnPacketReceive(UseSkill));
            Register(0x1E, true, new OnPacketReceive(RunReq));//Run key down
            Register(0x1F, true, new OnPacketReceive(StopRunReq));//Run key up
            Register(0x47, true, new OnPacketReceive(RespawnRequest));

            Register(0x07, true, new OnPacketReceive(GetMobileName));
            Register(0x33, true, new OnPacketReceive(GetMobileLook));
            Register(0x08, true, new OnPacketReceive(Process08));
            Register(0x0A, true, new OnPacketReceive(AttackMobile));
            Register(0x37, true, new OnPacketReceive(GetNPCMobileInfo));

            Register(0x0B, true, new OnPacketReceive(InteractItem));
            Register(0x27, true, new OnPacketReceive(InteractBank));
            Register(0x2E, true, new OnPacketReceive(InteractNPC));
            Register(0x38, true, new OnPacketReceive(DialogInteract));
            Register(0x39, true, new OnPacketReceive(ShopInteract));

            Register(0x3B, true, new OnPacketReceive(GetDoorStatus));//door status return 5B encrypted

            Register(0x45, true, new OnPacketReceive(InteractGuild));//Guild Stuff

            Register(0x5C, true, new OnPacketReceive(GetMapPatch));
            Register(0x5E, true, new OnPacketReceive(CompletePatchMap));
            Register(0x5D, true, new OnPacketReceive(PatchMap));//set to false to use external map editor
            Register(0x60, true, new OnPacketReceive(GetSectorName));

            Register(0x64, true, new OnPacketReceive(Unknown64));

            Register(0x35, true, new OnPacketReceive(ScriptUpload));
            Register(0x36, true, new OnPacketReceive(ScriptDownload));

            Register(0x1C, true, new OnPacketReceive(GetEditNPCInfo));//Create NPC/Upload NPC Script
            Register(0x1D, true, new OnPacketReceive(CreateMobInfo));//Create Mob
        }

        #region Client initialization packets
        public static void Stage1Responce(PlayerSocket socket, Packet packet)
        {
            socket.Send(new Stage1Reply());
        }

        public static void Stage2(PlayerSocket socket, Packet packet)
        {
            socket.Send(new Stage2());
        }

        public static void Stage2Responce(PlayerSocket socket, Packet packet)
        {
            socket.Send(new Stage2Reply());
        }
        #endregion

        private static void AccountPackets(PlayerSocket socket, Packet packet)
        {
            byte accountAction = packet.ReadByte();
            string username;
            string password;
            string email;
            switch (accountAction)
            {
                case 0x00://Create account
                    username = packet.ReadString(15).Trim();
                    password = packet.ReadString(15).Trim();
                    email = packet.ReadString(30).Trim();

                    World.AccountManager.CreateAccount(socket, username, password, email);
                    break;
                case 0x01://Login account
                    username = packet.ReadString(15).Trim();
                    password = packet.ReadString(15).Trim();

                    World.AccountManager.LoginAccount(socket, username, password);
                    break;
                case 0x03://Get character List
                    socket.Account.GetCharacterList();
                    break;
                default:
                    Console.WriteLine("Login: {0}: Unknow Account Action {1}", socket, accountAction);
                    break;
            }
        }

        public static void CreateCharacter(PlayerSocket socket, Packet packet)
        {
            string name = packet.ReadString(12).Trim();
            string password = packet.ReadString(12).Trim();

            packet.ReadByte(); packet.ReadByte(); packet.ReadByte(); packet.ReadByte();// 0x00 0x14 0x14 0x14 
            packet.ReadByte(); packet.ReadByte(); packet.ReadByte(); packet.ReadByte();// 0x14 0x14 0x14 0x00
            byte gender = packet.ReadByte();

            string name2 = packet.ReadUnicodeString(12);
            packet.ReadByte(); packet.ReadByte(); packet.ReadByte();// 0x04 0x00 0x00
            byte hairID = packet.ReadByte();

            socket.Account.CreateCharacter(name, name2, password, gender, hairID);
        }

        public static void LoginCharacter(PlayerSocket socket, Packet packet)
        {
            string name = packet.ReadString(12).Trim();
            string password = packet.ReadString(12).Trim();
            socket.Account.LoginCharacter(name, password);
        }

        public static void TextCommand(PlayerSocket socket, Packet packet)
        {
            string text = packet.ReadNullString();
            socket.Mobile.ProcessText(text);

        }

        public static void MovementReq(PlayerSocket socket, Packet packet)
        {
            byte zz = packet.ReadByte();//Direction and facing?
            int X = packet.ReadCompressed();
            int Y = packet.ReadCompressed();

            socket.Mobile.ProcessMovement(new Point3D(X, Y, (byte)(zz % 8)), (byte)(zz/8));
        }

        public static void AddStatPoint(PlayerSocket socket, Packet packet)
        {
            packet.ReadByte();//??
            StatType statType = (StatType)packet.ReadByte();
            socket.Mobile.AddStat(statType);
        }

        public static void CastSpell(PlayerSocket socket, Packet packet)
        {
            byte castSpellID = packet.ReadByte();

            IMobile m = socket.Mobile;
            ISpell spell = World.GetSpellByCastID(castSpellID);

            if (spell.SpellTargetType == SpellTargetType.Location)
            {
                int X = packet.ReadInt16();
                int Y = packet.ReadInt16();
                byte Z = packet.ReadByte();
                m.CastSpell(spell, new Point3D(X, Y, Z));
            }
            else
            if (spell.SpellTargetType == SpellTargetType.Player)
            {
                byte playerType = packet.ReadByte();
                int playerID = packet.ReadInt16();
                IMobile target = null;
                switch (playerType) {
                    case 0: //NPC/Mob
                        target = m.Map.GetMobile(playerID);
                        break;
                    case 1: //Players
                        if (Listener.Instance.PlayerSockets[playerID] != null)
                            target = Listener.Instance.PlayerSockets[playerID].Mobile;
                        break;
                }
                m.CastSpell(spell, target);
            }
        }

        public static void UseSkill(PlayerSocket socket, Packet packet)
        {
            byte skillUseID = packet.ReadByte();

            ISkill skill = World.GetSkillByUseID(skillUseID);

            byte targetType;
            int targetID;
            if (skill == null)
            {
                Console.WriteLine("Cannot find Skill {0}", skillUseID);
            }
            switch (skill.TargetType)
            {
                case SkillTargetType.None://not set up correctly....
                    Console.WriteLine("Unhandled Packet 0x10");
                    break;
                case SkillTargetType.Location:
                    int X = packet.ReadInt16();
                    int Y = packet.ReadInt16();
                    byte Z = packet.ReadByte();
                    socket.Mobile.UseSkill(skill, new Point3D(X, Y, Z));
                    break;
                case SkillTargetType.Trade:
                    targetType = packet.ReadByte();
                    targetID = packet.ReadByte();
                    if (targetType != 0)
                        Console.WriteLine("recieved trade type = {0} {1}", targetType, targetID);
                    break;
                case SkillTargetType.ByteID:
                    targetID = packet.ReadByte();
                    socket.Mobile.UseSkill(skill, targetID);
                    break;
                case SkillTargetType.ShortID:
                    targetID = packet.ReadInt16();
                    socket.Mobile.UseSkill(skill, targetID);
                    break;
                case SkillTargetType.NPCorPlayer:
                    targetType = packet.ReadByte();
                    targetID = packet.ReadInt16();
                    IMobile target = null;
                    switch (targetType)
                    {
                        case 0: //NPC/Mob
                            target = socket.Mobile.Map.GetMobile(targetID);
                            break;
                        case 1: //Players
                            if (Listener.Instance.PlayerSockets[targetID] != null)
                                target = Listener.Instance.PlayerSockets[targetID].Mobile;
                            break;
                    }
                    socket.Mobile.UseSkill(skill, target);
                    break;
            }
        }

        public static void RunReq(PlayerSocket socket, Packet packet)
        {
            socket.Mobile.SetRun(true);
        }

        public static void StopRunReq(PlayerSocket socket, Packet packet)
        {
            socket.Mobile.SetRun(false);
        }

        public static void RespawnRequest(PlayerSocket socket, Packet packet)
        {
            socket.Mobile.Respawn();
        }

        public static void GetMobileName(PlayerSocket socket, Packet packet)
        {
            byte playerID = packet.ReadByte();
            if (playerID == 0) return;
            if (Listener.Instance.PlayerSockets[playerID] != null)
            {
                IMobile m = Listener.Instance.PlayerSockets[playerID].Mobile;
                if (m != null)
                    socket.Send(new GePlayerNameReplyPacket(m));
            }
        }

        public static void GetMobileLook(PlayerSocket socket, Packet packet)
        {
            byte playerID = packet.ReadByte();
            if (playerID == 0) return;
            if (Listener.Instance.PlayerSockets[playerID] != null)
            {
                IMobile m = Listener.Instance.PlayerSockets[playerID].Mobile;
                if (m != null)
                    socket.Send(new GetPlayerLookPacket(m));
            }
        }

        public static void Process08(PlayerSocket socket, Packet packet)
        {
            byte unk1 = packet.ReadByte();
            byte dir = packet.ReadByte();
            if (unk1 == 0) //arrow
            {
                int x = packet.ReadInt16();
                int y = packet.ReadInt16();
                byte z = packet.ReadByte();
                socket.Mobile.Map.AddProjectile(ProjectTileType.Arrow, new Point3D(x, y, z), dir);
            }
            else //attack player
            {
                if (Listener.Instance.PlayerSockets[unk1] != null)
                {
                    IMobile target = Listener.Instance.PlayerSockets[unk1].Mobile;
                    if (target != null)
                    {
                        socket.Mobile.Attack(target, dir);
                    }
                }
            }

        }

        public static void AttackMobile(PlayerSocket socket, Packet packet)
        {
            int mobID = packet.ReadInt16();
            byte dir = packet.ReadByte();
            IMobile target = socket.Mobile.Map.GetMobile(mobID);
            if (target != null)
                socket.Mobile.Attack(target, dir);
        }

        public static void GetNPCMobileInfo(PlayerSocket socket, Packet packet)
        {
            byte mapID = packet.ReadByte();
            int mobileID = packet.ReadInt16();
            IMap map = World.GetMapByID(mapID);
            if (map == null) return;

            IMobile m = map.GetMobile(mobileID);
            //if (m != null && m.Name != "-vendor-")
            if (m != null)
                socket.Send(new NPCLookPacket(m));
        }

        public static void InteractItem(PlayerSocket socket, Packet packet)
        {
            byte interactionType = packet.ReadByte();
            IMobile player = socket.Mobile;
            switch (interactionType)
            {
                case 0x42://Drop Item
                    byte dropInvID = packet.ReadByte();
                    player.DropItem(dropInvID);
                    break;
                case 0x43://Use Inventory Item from location
                    byte useInvID = packet.ReadByte();
                    player.UseItem(useInvID);
                    break;
                case 0x44://Unequip Item
                    byte equipSlotID = packet.ReadByte();
                    player.UnequipItem(equipSlotID);
                    break;
                case 0x45://Drop Item x Amount
                    byte dropInvID2 = (byte)packet.ReadByte();
                    int largeDropAmount = (byte)packet.ReadByte();

                    int dropInvIDAmount = packet.ReadInt16();
                    dropInvIDAmount += largeDropAmount * 32768;
                    player.DropItem(dropInvID2, dropInvIDAmount);
                    break;
                case 0x59://move Item
                    byte equipFromSlotID = (byte)packet.ReadByte();
                    byte equipToSlotID = (byte)packet.ReadByte();
                    player.MoveItem(equipFromSlotID, equipToSlotID);
                    break;
                case 0x5A://Pickup Item from ground
                    int x = packet.ReadInt16();
                    int y = packet.ReadInt16();
                    player.PickupItem(new Point3D(x, y, socket.Mobile.Z));
                    break;
                default://Unhandled
                    Console.WriteLine("Client: {0}: Unhandled packet 0x{1:X2}", socket, interactionType);
                    break;
            }
        }

        public static void InteractBank(PlayerSocket socket, Packet packet)
        {
            byte interactionType = packet.ReadByte();
            IMobile player = socket.Mobile;
            switch (interactionType)
            {
                case 0x44://Deposit Item
                    byte invSlotID = (byte)packet.ReadByte();
                    packet.ReadByte();//??
                    int amount = packet.ReadInt16();
                    player.BankStoreItem(invSlotID, amount);
                    break;
                case 0x57://Withdraw Item
                    byte bankSlotID = packet.ReadByte();
                    player.BankWithdrawItem(bankSlotID);
                    break;
                default://Unhandled
                    Console.WriteLine("Client: {0}: Unhandled packet 0x{1:X2}", socket, interactionType);
                    break;
            }
        }

        public static void InteractNPC(PlayerSocket socket, Packet packet)
        {
            int mobileID = packet.ReadInt16();
            IMap map = socket.Mobile.Map;
            IMobile mob = map.GetMobile(mobileID);
            if (mob != null)
                socket.Mobile.InteractNPC(mob);
        }

        public static void DialogInteract(PlayerSocket socket, Packet packet)
        {
            int mobileID = packet.ReadInt16();
            byte buttonID = packet.ReadByte();
            IMobile mob = socket.Mobile.Map.GetMobile(mobileID);
            if (mob != null)
                socket.Mobile.InteractDialog(mob, buttonID);
        }

        public static void ShopInteract(PlayerSocket socket, Packet packet)
        {
            byte itemLocationID = packet.ReadByte();
            //byte unknown = packet.ReadByte();
            socket.Mobile.InteractShop(itemLocationID);
        }

        public static void GetDoorStatus(PlayerSocket socket, Packet packet)
        {
            byte mapId = packet.ReadByte();
            int X = packet.ReadInt16();
            int Y = packet.ReadInt16();
            byte Z = packet.ReadByte();

            IMap map = World.GetMapByID(mapId);
            map.GetDoorStatus(socket.Mobile, new Point3D(X, Y, Z));
        }

        public static void GetSectorName(PlayerSocket socket, Packet packet)
        {
            short loc1 = packet.ReadInt16();

            socket.Mobile.Map.GetMapSector(socket.Mobile, loc1);
        }

        public static void InteractGuild(PlayerSocket socket, Packet packet)
        {
            byte interaction = packet.ReadByte();
            int guildID;
            IGuild guild;
            IMobile player = socket.Mobile;

            switch (interaction)
            {
                case 0://ShowGuildList
                    player.ShowGuildList();
                    break;
                case 1://View Guild
                    guildID = packet.ReadInt16();
                    guild = World.GetGuildByID(guildID);
                    player.ShowGuild(guild);
                    break;
                case 2://Apply to Guild
                    guildID = packet.ReadInt16();
                    guild = World.GetGuildByID(guildID);
                    player.ApplyGuild(guild);
                    break;
                case 4://Guild Applicants
                    guildID = packet.ReadInt16();
                    guild = World.GetGuildByID(guildID);

                    interaction = packet.ReadByte();
                    byte applicantID;
                    switch (interaction)
                    {
                        case 0://Accept Applicant
                            applicantID = packet.ReadByte();
                            guild.AcceptApplicant(player, applicantID);
                            break;
                        case 1:
                            applicantID = packet.ReadByte();
                            guild.DenyApplicant(player, applicantID);
                            break;
                        case 2://show guild applicants
                            guild.ShowGuildApplicants(player, guild);
                            break;
                    }
                    //state.Send(new ShowGuildApplicants(guild));
                    break;
                case 5://Guild Member Ranking/Kick
                    guildID = packet.ReadInt16();
                    guild = World.GetGuildByID(guildID);

                    interaction = packet.ReadByte();
                    byte memberID;
                    switch (interaction)
                    {
                        case 0://Update member
                            memberID = packet.ReadByte();
                            byte founder = packet.ReadByte();//Founder/Lord
                            byte memType = packet.ReadByte();
                            guild.UpdateMember(player, memberID, founder, (MemberType)memType);
                            break;
                        case 1:
                            memberID = packet.ReadByte();
                            guild.KickMember(player, memberID);
                            break;
                    }
                    player.ShowGuild(guild);//refresh the guild screen

                    break;
                case 6://Guild Decrees
                    guildID = packet.ReadInt16();
                    guild = World.GetGuildByID(guildID);

                    interaction = packet.ReadByte();
                    byte decreeType;
                    switch (interaction)
                    {
                        case 0://Add Decree
                            decreeType = packet.ReadByte();
                            guildID = packet.ReadInt16();
                            guild.AddDecree(player, decreeType, guildID);
                            player.ShowGuild(guild);//refresh the guild screen
                            break;
                        case 1:
                            byte decreeID = packet.ReadByte();
                            guild.RemoveDecree(player, decreeID);
                            player.ShowGuild(guild);//refresh the guild screen
                            break;
                        case 2://show guild decrees
                            player.ShowGuildDecrees(guild);
                            break;
                    }
                    break;
                case 8://Found Guild
                    string guildName = packet.ReadNullString();
                    player.CreateGuild(guildName);
                    break;
                case 9://Guild Hall
                    guildID = packet.ReadInt16();
                    guild = World.GetGuildByID(guildID);

                    interaction = packet.ReadByte();
                    switch (interaction)
                    {
                        case 0://Purchase Guild Hall
                            World.ShowAvailableGuildHalls(player);
                            break;
                        case 1:
                            byte guildHallID = packet.ReadByte();
                            player.BuyGuildHall(guild, guildHallID);
                            break;
                        case 2://Remove Guild Hall
                            player.SellGuildHall(guild);
                            break;
                    }
                    break;
                default:
                    Console.WriteLine(String.Format("Unknown guild Interaction"));
                    break;
            }
        }

        public static void GetMapPatch(PlayerSocket socket, Packet packet)
        {
            byte mapID = packet.ReadByte();
            short sector = packet.ReadInt16();

            socket.Mobile.GetMapPatch(mapID, sector);

        }

        public static void CompletePatchMap(PlayerSocket socket, Packet packet)
        {
            socket.Send(new Unk51Packet());
        }

        public static void PatchMap(PlayerSocket socket, Packet packet)
        {
            byte MapID = packet.ReadByte();
            short sector = packet.ReadInt16();

            byte[] data = packet.ReadBytesToEOF();

            IMap map = World.GetMapByID(MapID);
            map.UpdateData(sector, data);
            map.RecomputeCheckSum(sector);
        }

        public static void Unknown64(PlayerSocket socket, Packet packet)
        {
            byte val1 = packet.ReadByte();
            byte marketTab;
            byte itemID;
            int mailMessageID = 0;
            switch (val1)
            {
                case 0x00://Mail
                    {
                        socket.Mobile.SendMailList();
                        break;
                    }
                case 0x01://Read Message
                    {
                        mailMessageID += packet.ReadByte() * 32768;
                        mailMessageID += packet.ReadByte();
                        mailMessageID += packet.ReadByte() * 256;
                        socket.Mobile.ShowMailMessage(mailMessageID);
                        break;
                    }
                case 0x02://GetItem from Message
                    {
                        mailMessageID += packet.ReadByte() * 32768;
                        mailMessageID += packet.ReadByte();
                        mailMessageID += packet.ReadByte() * 256;
                        byte itemNum = packet.ReadByte();
                        socket.Mobile.GetItemFromMail(mailMessageID, itemNum);
                        break;
                    }
                case 0x03://Compose new Mail Message
                    {
                        List<int> mailItems = new List<int>(4);
                        for (int i = 0; i < 4; i++)
                        {
                            byte locationID = packet.ReadByte();//Item1
                            mailItems[i] = locationID;
                        }
                        byte subjectLen = packet.ReadByte();
                        byte toLen = packet.ReadByte();
                        int contentLen = packet.ReadInt16();
                        string subject = packet.ReadString(subjectLen);
                        string toName = packet.ReadString(toLen);
                        string content = packet.ReadString(contentLen);
                        socket.Mobile.SendMail(toName, subject, content, mailItems);
                        break;
                    }
                case 0x20://Market
                    {
                        marketTab = packet.ReadByte();
                        //state.Mobile.SendPopupMessage(String.Format("Open Market {0}",marketTab));
                        socket.Mobile.ShowMarket(marketTab);
                        break;
                    }
                case 0x21://Sell Item in Market
                    {
                        marketTab = packet.ReadByte();
                        int itemLocation = packet.ReadByte();
                        int totalCost = packet.ReadInt32();
                        socket.Mobile.SellItemOnMarket(marketTab, itemLocation, totalCost);
                        break;
                    }
                case 0x22://Buy Market
                    {
                        marketTab = packet.ReadByte();
                        byte unk1 = packet.ReadByte();//0x07 might be map ID???
                        itemID = packet.ReadByte();
                        socket.Mobile.BuyItemOnMarket(marketTab, itemID, unk1);
                        break;
                    }
                case 0x44:
                    //state.Mobile.SendPopupMessage("64 Packet 44");
                    socket.Send(new Unk64ReplyPacket(socket.Mobile));
                    break;
                default:
                    Console.WriteLine("Client: {0}: Unhandled packet 0x64 sub 0x{1:X2}", socket, val1);
                    break;
            }
            //byte val2 = pvSrc.ReadByte();
        }

        public static void ScriptUpload(PlayerSocket socket, Packet packet)
        {
            try
            {
                NPCSpawnInfo spawnInfo = new NPCSpawnInfo();
                spawnInfo.ScriptName = packet.ReadString(16).Trim();
                spawnInfo.NpcName = packet.ReadString(16).Trim();
                spawnInfo.Picture = packet.ReadInt16();//Picture
                spawnInfo.Weapon = packet.ReadInt16();//Weapon
                spawnInfo.Shield = packet.ReadInt16();//Shield
                spawnInfo.Helmet = packet.ReadInt16();//Helmet
                spawnInfo.Gauntlet = packet.ReadInt16();//Gauntlet
                spawnInfo.Boots = packet.ReadInt16();//Boots
                spawnInfo.Armour = packet.ReadInt16();//Armour
                spawnInfo.Gender = packet.ReadByte();//Gender

                byte[] data = packet.ReadBytesToEOF();
                if (data.Length > 0)
                {
                    try
                    {
                        spawnInfo.Script = Encoding.UTF8.GetString(data, 0, data.Length);
                    } catch
                    {
                        Console.WriteLine("Error Converting Uploaded Script to string");
                    }
                }
                World.ServerManager.UploadScript(socket, spawnInfo);
            }
            catch
            {

            }
        }

        public static void ScriptDownload(PlayerSocket socket, Packet packet)
        {
            string filename = packet.ReadString(16).Trim();
            World.ServerManager.DownloadScript(socket, filename);
        }

        public static void GetEditNPCInfo(PlayerSocket socket, Packet packet)
        {
            byte mapID = packet.ReadByte();
            int mobileID = packet.ReadInt16();
            World.ServerManager.GetSpawnInfo(socket, mapID, mobileID);
        }

        public static void CreateMobInfo(PlayerSocket socket, Packet packet)
        {
            MobileSpawn ms = new MobileSpawn();
            ms.MapID = packet.ReadByte();
            ms.MobileID = packet.ReadInt16();
            ms.MobType = packet.ReadByte();
            ms.SpawnDelay = packet.ReadInt16();
            Rect bounds = new Rect();
            bounds.X = packet.ReadInt16();
            bounds.Width = packet.ReadInt16();
            bounds.Y = packet.ReadInt16();
            bounds.Height = packet.ReadInt16();
            ms.Bounds = bounds;
            int X = packet.ReadInt16();
            int Y = packet.ReadInt16();
            byte Z = socket.Mobile.Z;
            ms.SpawnLocation = new Point3D(X,Y,Z);
            ms.ScriptName = packet.ReadString(16);
            World.ServerManager.AddSpawn(socket, ms);
        }

        public static void Empty(PlayerSocket socket, Packet packet)
        {

        }

        #region Class functions
        private static void Register(int packetID, bool ingame, OnPacketReceive onReceive)
        {
            Handlers[packetID] = new PacketHandler(packetID, ingame, onReceive);
        }

        internal static PacketHandler GetHandler(byte packetID)
        {
            return Handlers[packetID];
        }
        #endregion
    }
}
