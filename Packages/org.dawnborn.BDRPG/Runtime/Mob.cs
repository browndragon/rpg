using System.Collections.Generic;

namespace BDRPG
{
    /// An active entity which can take deeds.
    public class Mob : Ob
    {
        public List<BDRPG.Deeds.Deed> Deeds = new();
    }
}
