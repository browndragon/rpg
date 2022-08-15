using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BDUtil;
using UnityEngine;

namespace BDRPG.Flates
{
    [CreateAssetMenu(menuName = "BDRPG/Deflated")]
    [Tooltip("A flatpacked description of a type of game object ('a sword' or 'a goblin')")]
    public class Deflated : Cameos.ScriptableObject
    {
        /// The stateless game object that this Deflate can be turned into.
        public Inflated Inflate;

        public virtual Inflated Acquire() => Instantiate(Inflate);
        public virtual void Release(Inflated inflate) => Destroy(inflate.gameObject);
    }
}
