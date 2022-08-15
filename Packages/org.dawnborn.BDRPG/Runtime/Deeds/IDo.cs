using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BDRPG.Deeds
{
    public interface IDo
    {
        /// Type expected in Doer.
        public Type Doer { get; }
        /// Types exected in To (length matching expected length).
        public IEnumerable<Type> To { get; }
        /// Implement the actual action. Remember to call Before and After appropriately.
        public void Do(ref Doing doing);
    }
    public interface IDo<TDoer> : IDo
    {
        Type IDo.Doer => typeof(TDoer);
        IEnumerable<Type> IDo.To => Array.Empty<Type>();
    }
    public interface IDo<TDoer, TTo0> : IDo<TDoer>
    {
        IEnumerable<Type> IDo.To => new Type[] { typeof(TTo0) };
    }
    public interface IDo<TDoer, TTo0, TTo1> : IDo<TDoer, TTo0>
    {
        IEnumerable<Type> IDo.To => new Type[] { typeof(TTo0), typeof(TTo1) };
    }
    public interface IDo<TDoer, TTo0, TTo1, TTo2> : IDo<TDoer, TTo0, TTo1>
    {
        IEnumerable<Type> IDo.To => new Type[] { typeof(TTo0), typeof(TTo1), typeof(TTo2) };
    }
    public interface IDo<TDoer, TTo0, TTo1, TTo2, TTo3> : IDo<TDoer, TTo0, TTo1, TTo2>
    {
        IEnumerable<Type> IDo.To => new Type[] { typeof(TTo0), typeof(TTo1), typeof(TTo2), typeof(TTo3) };
    }

    [SuppressMessage("IDE", "IDE0060")]
    public static class IDos
    {
        public static TDoer GetDoer<TDoer>(this IDo<TDoer> thiz, Doing doing) => (TDoer)doing.Doer;
        public static TTo0 GetTo0<TDoer, TTo0>(this IDo<TDoer, TTo0> thiz, Doing doing) => (TTo0)doing.To[0];
        public static TTo1 GetTo1<TDoer, TTo0, TTo1>(this IDo<TDoer, TTo0, TTo1> thiz, Doing doing) => (TTo1)doing.To[1];
        public static TTo2 GetTo2<TDoer, TTo0, TTo1, TTo2>(this IDo<TDoer, TTo0, TTo1, TTo2> thiz, Doing doing) => (TTo2)doing.To[2];
        public static TTo3 GetTo3<TDoer, TTo0, TTo1, TTo2, TTo3>(this IDo<TDoer, TTo0, TTo1, TTo2, TTo3> thiz, Doing doing) => (TTo3)doing.To[3];
    }
}
