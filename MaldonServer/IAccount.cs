using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        string UserName { get; set; }
        AccessLevel AccessLevel { get; set; }
        /// <summary>
        /// List of Characters for this account
        /// </summary>
		IMobile[] Mobiles { get; set; }
    }
}
