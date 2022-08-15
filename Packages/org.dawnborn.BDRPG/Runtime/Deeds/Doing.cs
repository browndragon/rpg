using System;

namespace BDRPG.Deeds
{
    public struct Doing
    {
        public Action Before;
        public Action After;
        public object Doer;
        public object[] To;
        public object Done;
    }
}
