using BDUtil;
using UnityEngine;
using UnityEngine.Events;

namespace BDRPG
{
    public class LeverScript : MonoBehaviour
    {
        public Timer Delay = 0f;
        public float DelayOn = .25f;
        [Tooltip("If this is infinity, it can't get switched back off...")]
        public float DelayOff = .25f;
        [field: SerializeField] public BDUtil.Pubsub.Val<bool> IsOn { get; private set; }
        public UnityEvent<bool> Publish;
        Animator animator;

        protected virtual void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }
        protected virtual void OnEnable()
        {
            IsOn.Topic.AddListener(OnIsOn);
            Delay = 0f;
        }
        protected virtual void OnDisable() => IsOn.Topic.RemoveListener(OnIsOn);
        void OnIsOn()
        {
            animator.SetBool("IsOn", IsOn.Value);
            Publish?.Invoke(IsOn.Value);
            if (IsOn.Value) Delay = new(DelayOff);
            else Delay = new(DelayOn);
        }
        protected virtual void OnTriggerEnter2D(Collider2D other) => SetIsOnTwiddle();

        public void SetIsOnTwiddle() => SetIsOn(!IsOn.Value);
        public virtual void SetIsOn(bool isOn)
        {
            if (Delay.IsRunning) return;
            IsOn.Value = !IsOn.Value;
        }
    }
}
