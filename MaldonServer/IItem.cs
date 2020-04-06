using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaldonServer
{
    public interface IItem
    {
        int ItemID { get; set; }
        byte Prefix { get; set; }
        byte Suffix { get; set; }
        int Amount { get; set; }
        int DuraMax { get; set; }
        int DuraMin { get; set; }
    }
}
