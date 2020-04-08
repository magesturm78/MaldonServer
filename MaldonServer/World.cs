using MaldonServer.Network;
using MaldonServer.Network.ServerPackets;
using System;
using System.Collections.Generic;

namespace MaldonServer
{
	public static class World
	{
		private static readonly List<IMap> maps = new List<IMap>();
		private static readonly List<ISpell> spells = new List<ISpell>();
		private static readonly List<ISkill> skills = new List<ISkill>();
		private static readonly List<IGuild> guilds = new List<IGuild>();
		private static readonly List<IGuildHall> guildHalls = new List<IGuildHall>();

		public static IServerManager ServerManager { get; private set; }

		public static IAccountManager AccountManager { get; private set; }

		public static void Broadcast(Packet p)
        {
			Listener.Instance.Broadcast(p);
        }

		public static void AddSpell(ISpell spell)
		{
			spells.Add(spell);
		}

		public static ISpell GetSpellByCastID(int spellCastID)
		{
			foreach (ISpell spell in spells)
			{
				if (spell.CastID == spellCastID)
					return spell;
			}
			return null;
		}

		public static void AddSkill(ISkill skill)
		{
			skills.Add(skill);
		}

		public static ISkill GetSkillByUseID(int skillUseID)
		{
			foreach (ISkill skill in skills)
			{
				if (skill.UseID == skillUseID)
					return skill;
			}
			return null;

		}

		public static void AddMap(IMap map)
		{
			maps.Add(map);
		}

		public static IMap GetMapByID(int mapID)
		{
			foreach (IMap map in maps)
			{
				if (map.MapID == mapID)
					return map;
			}
			return null;

		}

		public static void AddGuild(IGuild guild)
		{
			guilds.Add(guild);
		}

		public static IGuild GetGuildByID(int guildID)
		{
			foreach (IGuild guild in guilds)
			{
				if (guild.Id == guildID)
					return guild;
			}
			return null;

		}

		public static void AddGuildHall(IGuildHall guildHall)
		{
			guildHalls.Add(guildHall);
		}

		public static void ShowAvailableGuildHalls(IMobile player)
		{
			List<IGuildHall> available = new List<IGuildHall>();

			foreach (IGuildHall gh in guildHalls)
				if (gh.Owner == null)
					available.Add(gh);

			player.PlayerSocket.Send(new ShowGuildHallsPacket(available.ToArray()));
		}

		public static void SetServerManager(IServerManager serverManager)
		{
			if (ServerManager == null)
			{
				ServerManager = serverManager;
			} 
			else
			{
				Console.WriteLine("Trying to initialize an Additional Server Manager {0}.", serverManager);
			}
		}

		public static void SetAccountManager(IAccountManager accountManager)
		{
			if (AccountManager == null)
			{
				AccountManager = accountManager;
			}
			else
			{
				Console.WriteLine("Trying to initialize an Additional Account Manager {0}.", accountManager);
			}
		}
	}
}