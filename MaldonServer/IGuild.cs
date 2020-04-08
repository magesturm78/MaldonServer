using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaldonServer
{
    public enum MemberType
    {
        King = 1,
        Queen = 2,
        Count = 3,
        Countess = 4,
        Baron = 5,
        Baroness = 6,
        Sorcerer = 7,
        Sorcereress = 8,
        Knight = 9,
        Merchant = 10,
        Serf = 11
    }

    public interface IGuildHall
    {
        int Id { get; }
        string Name { get; }
        int Price { get; }
        IGuild Owner { get; }
    }

    public interface IGuild
    {
        int Id { get; }
        string Name { get; }
        IMobile Owner { get; }
        IGuildHall GuildHall { get; }
        GuildMember[] Members { get; }
        GuildDecree[] Decrees { get; }
        GuildApplicant[] Applicants { get; }

        void AcceptApplicant(IMobile player, byte applicantID);
        void DenyApplicant(IMobile player, byte applicantID);
        void ShowGuildApplicants(IMobile player, IGuild guild);
        void UpdateMember(IMobile player, byte memberID, byte founder, MemberType memType);
        void KickMember(IMobile player, byte memberID);
        void AddDecree(IMobile player, byte decreeType, int guildID);
        void RemoveDecree(IMobile player, byte decreeID);
    }
}
