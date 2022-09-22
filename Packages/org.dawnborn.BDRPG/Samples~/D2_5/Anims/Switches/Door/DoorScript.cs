using UnityEngine;

namespace BDRPG
{
    public class DoorScript : MonoBehaviour
    {
        [SerializeField] bool isLocked;
        public bool IsLocked
        {
            get => isLocked;
            set
            {
                isLocked = value;
                // If we're open when we lock, slam shut rudely.
                if (isLocked && IsOpen) IsOpen = false;
            }
        }
        public bool IsUnlocked
        {
            get => !IsLocked;
            set => IsLocked = !value;
        }
        public bool IsOpen
        {
            get => Animator.GetBool("IsOn");
            set
            {
                Body.enabled = !value;
                Animator.SetBool("IsOn", value);
            }
        }

        public Collider2D Body;  // !isTrigger
        public Collider2D OpenIf;  // Radius of approach at which to open. isTrigger.
        public ContactFilter2D Filter = new()
        {
            useTriggers = true,
            useDepth = true,
        };
        static readonly Collider2D[] results = { null };
        Animator Animator;

        void Awake()
        {
            Filter.SetDepth(transform.position.z - .5f, transform.position.z + .5f);
            Animator = GetComponentInChildren<Animator>();
            foreach (Collider2D collider in GetComponents<Collider2D>())
            {
                if (!collider.isTrigger) Body = collider;
                else OpenIf = collider;
            }
        }
        void OnTriggerEnter2D(Collider2D collision) => IsOpen = !isLocked;
        void OnTriggerStay2D(Collider2D collision) => IsOpen = !isLocked;
        void OnTriggerExit2D(Collider2D collision) => IsOpen = false;
    }
}
