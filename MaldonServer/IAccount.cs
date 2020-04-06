using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaldonServer
{
    public interface IAccount
    {
		int Count { get; }
		Mobile this[int index] { get; set; }
    }
}
