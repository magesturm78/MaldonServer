using MaldonServer.Network;
using System.Collections.Generic;

namespace MaldonServer
{
    public enum AccessLevel
    {
        Peasant = 0,
        Citizen = 1,
        Moderator = 2,
        GameMaster = 3,
        Developer = 4,
        Administrator = 5
    }

    public interface IAccount
    {
        PlayerSocket PlayerSocket { get; set; }
        string UserName { get; }
        AccessLevel AccessLevel { get; }
		List<IMobile> Characters { get; }
        void CreateCharacter(string name, string name2, string password, byte gender, byte hair);
        void LoginCharacter(string name, string password);
        void GetCharacterList();
    }
}
