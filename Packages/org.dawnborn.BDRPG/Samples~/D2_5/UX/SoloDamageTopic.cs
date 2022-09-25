using System;
using System.Collections;
using BDUtil;
using BDUtil.Bind;
using BDUtil.Math;
using BDUtil.Pubsub;
using UnityEngine;

namespace BDRPG
{
    public class SoloDamageTopic : DamageTopic
    {
        public static SoloDamageTopic main { get; private set; }

        protected override void OnEnable()
        {
            if (!EditorUtils.IsPlayingOrWillChangePlaymode) EditorUtils.InsertPreloadedAsset(this);
            main = this;
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            main = null;
            base.OnDisable();
            if (!EditorUtils.IsPlayingOrWillChangePlaymode) EditorUtils.RemoveEmptyPreloadedAssets();
        }
    }
}
