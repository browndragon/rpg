using UnityEngine;
using UnityEngine.Events;

namespace BDRPG
{
    public class ButtonScript : MonoBehaviour
    {
        public float DelayOn = .25f;
        [Tooltip("If this is infinity, it can't get switched back off...")]
        public float DelayOff = .25f;
        public UnityEvent<bool> Publish;
        float DebounceUntil = float.NegativeInfinity;
        Animator animator;

        void Awake() => animator = GetComponentInChildren<Animator>();
        void OnTriggerEnter2D(Collider2D other)
        {
            if (Time.time < DebounceUntil) return;
            bool isOn = !animator.GetBool("IsOn");
            animator.SetBool("IsOn", isOn);
            Publish?.Invoke(isOn);
            Debug.Log($"Button {this} is {(isOn ? "on" : "off")}", this);
            DebounceUntil = Time.time + (isOn ? DelayOff : DelayOn);
        }
    }
}
