using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaldonServer
{
    public interface IAccount
    {
        string UserName { get; set; }
        /// <summary>
        /// List of Characters for this account
        /// </summary>
		IMobile[] Mobiles { get; set; }
    }
}
