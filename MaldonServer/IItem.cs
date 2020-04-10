using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaldonServer
{
    public interface IItem
    {
        int ItemID { get; }
        byte Prefix { get; }
        byte Suffix { get; }
        int Amount { get; }
        int DuraMax { get; }
        int DuraMin { get; }
    }
}
