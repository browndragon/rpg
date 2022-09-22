using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace BDRPG
{
    public class ButtonScript : MonoBehaviour
    {
        public float Debounce = .25f;
        float DebounceUntil = float.NegativeInfinity;
        public UnityEvent<bool> Publish;
        Animator animator;

        void Awake() => animator = GetComponentInChildren<Animator>();
        void OnTriggerEnter2D(Collider2D other)
        {
            if (Time.time < DebounceUntil) return;
            bool isOn = !animator.GetBool("IsOn");
            animator.SetBool("IsOn", isOn);
            Publish?.Invoke(isOn);
            Debug.Log($"Button {this} is {(isOn ? "on" : "off")}", this);
            DebounceUntil = Time.time + Debounce;
        }
    }
}
