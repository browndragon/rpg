using BDUtil;
using UnityEngine;
using UnityEngine.Events;

namespace BDRPG
{
    public class ButtonScript : MonoBehaviour
    {
        public Timer DelayOn = .25f;
        [Tooltip("If this is infinity, it can't get switched back off...")]
        public Timer DelayOff = float.PositiveInfinity;
        public UnityEvent<bool> Publish;
        Animator animator;

        void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            DelayOn.Halt();
            DelayOff.Halt();
        }
        void OnTriggerEnter2D(Collider2D other)
        {
            bool becomingOn = !animator.GetBool("IsOn");
            if (becomingOn && DelayOn.IsRunning || !becomingOn && DelayOff.IsRunning) return;
            animator.SetBool("IsOn", becomingOn);
            Publish?.Invoke(becomingOn);
            Debug.Log($"Button {this} is {(becomingOn ? "on" : "off")}", this);
            if (becomingOn) DelayOn.Reset();
            else DelayOff.Reset();
        }
    }
}
