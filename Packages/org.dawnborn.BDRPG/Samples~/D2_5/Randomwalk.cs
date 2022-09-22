using System.Collections;
using BDUtil;
using UnityEngine;

namespace BDRPG
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Randomwalk : MonoBehaviour
    {
        public Vector2 SpeedRange = new(3f, 6f);
        public Vector2 VirtDistance = new(.25f, 6f);
        public Vector2 PauseTime = new(.5f, 2.5f);
        new Rigidbody2D rigidbody;
        void Awake() => rigidbody = GetComponent<Rigidbody2D>();

        IEnumerator Start()
        {
            while (true)
            {
                yield return Coroutines.Fixed;
                float speed = Random.Range(SpeedRange.x, SpeedRange.y);
                float distance = Random.Range(VirtDistance.x, VirtDistance.y);
                Vector2 target = Random.rotationUniform * (speed * Vector3.right);
                Vector2 prevError = default, cumError = default;
                for (float start = Time.fixedTime, elapsed = 0f, max = distance / speed; elapsed < max; elapsed = Time.fixedTime - start)
                {
                    rigidbody.AddForce(BDUtil.Math.PID.Default.Apply(Time.fixedDeltaTime, target - rigidbody.velocity, ref prevError, ref cumError));
                    yield return Coroutines.Fixed;
                }
                yield return new WaitForSeconds(Random.Range(PauseTime.x, PauseTime.y));
            }
        }
    }
}
