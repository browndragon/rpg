using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BDUtil;
using UnityEngine;

namespace BDRPG.Flates
{
    /// A game object which (at least in theory) supports round-tripping through a deflate.
    [AddComponentMenu("BDRPG/Inflated")]
    [Tooltip("A gameobject which supports inflation/deflation through a type")]
    public class Inflated : MonoBehaviour, Cameos.IAm
    {
        public Deflated Deflate;

        public Sprite Portrait => Deflate.Portrait;
        public string DisplayName => Deflate.DisplayName;
        public string Description => Deflate.Description;
    }
}
