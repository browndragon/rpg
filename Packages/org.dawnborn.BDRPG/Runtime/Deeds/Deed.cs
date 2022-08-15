using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BDUtil;
using UnityEngine;

namespace BDRPG.Deeds
{
    /// A resource for executing on deeds.
    [CreateAssetMenu(menuName = "BDRPG/Deed")]
    public class Deed : Cameos.ScriptableObject, IDo
    {
        [SerializeReference, Subtype]
        [SuppressMessage("IDE", "IDE0044")]
        IDo Do = new DoLog();

        Type IDo.Doer => Do.Doer;
        IEnumerable<Type> IDo.To => Do.To;
        void IDo.Do(ref Doing doing) => Do.Do(ref doing);

        [Serializable]
        struct DoLog : IDo
        {
            public Type Doer => null;  // Any doer.
            public IEnumerable<Type> To => null;  // Any to.
            public void Do(ref Doing doing)
            {
                doing.Before?.Invoke();
                Debug.Log($"{doing.Doer} logging {To.Summarize()}");
                doing.After?.Invoke();
            }
        }
    }
}
