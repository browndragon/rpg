using System;
using System.Collections;
using BDUtil;
using BDUtil.Bind;
using BDUtil.Math;
using BDUtil.Pubsub;
using UnityEngine;

namespace BDRPG
{
    [Serializable]
    public struct Damage
    {
        public Vector3 World;
        public float Amount;
        public Damage(Vector3 world, float amount) { World = world; Amount = amount; }
    }
    [CreateAssetMenu(menuName = "BDRPG/DamageTopic")]
    [Impl(typeof(ValueTopic<Damage>))]
    public class DamageTopic : ValueTopic<Damage>
    {
        [Serializable]
        public class UEventDamage : Subscribers.UEventValue<Damage> { }
    }
}
