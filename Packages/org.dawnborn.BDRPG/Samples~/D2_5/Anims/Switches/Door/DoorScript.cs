using BDUtil;
using UnityEngine;

namespace BDRPG
{
    public class DoorScript : LeverScript
    {
        public LeverScript IsUnlocked;
        readonly Disposes.All unsubscribe = new();

        Collider2D Body;  // !isTrigger
        Collider2D OpenIf;  // Radius of approach at which to open. isTrigger.

        public ContactFilter2D Filter = new()
        {
            useTriggers = true,
            useDepth = true,
        };
        static readonly Collider2D[] results = { null };

        protected override void Awake()
        {
            base.Awake();
            Filter.SetDepth(transform.position.z - .5f, transform.position.z + .5f);
            foreach (Collider2D collider in GetComponents<Collider2D>())
            {
                if (!collider.isTrigger) Body = collider;
                else OpenIf = collider;
            }
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            if (IsUnlocked != null) unsubscribe.Add(IsUnlocked.IsOn.Topic.Subscribe(unlocked => Body.enabled = !unlocked));
        }
        protected override void OnDisable()
        {
            unsubscribe.Dispose();
            base.OnDisable();
        }
        protected override void OnTriggerEnter2D(Collider2D collision) => SetIsOn(true);
        void OnTriggerStay2D(Collider2D collision) => SetIsOn(true);
        void OnTriggerExit2D(Collider2D collision) => SetIsOn(false);
        public override void SetIsOn(bool isOn)
        {
            if (Delay.IsRunning) return;
            if (IsOn.Value == isOn) return;
            if (isOn)
            {
                if (IsUnlocked != null && !IsUnlocked.IsOn.Value) return;
            }
            base.SetIsOn(isOn);
        }
    }
}
