using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaldonServer
{
    public enum SpellTargetType
    {
        Location,
        Player,
        None
    }

    public interface ISpell
    {
        int SpellID { get; }
        int CastID { get;  }
        SpellTargetType SpellTargetType { get; }
    }
}
