using System.Collections;
using UnityEngine;

namespace BDRPG
{
    /// A live watchdog timer.
    public struct Watchdog
    {
        public float Limit;
        public float Start;
        public Watchdog(float limit)
        {
            Limit = limit;
            Start = 0f;
            Reset();
        }

        public float Now => Time.realtimeSinceStartup;
        public float Elapsed => Now - Start;
        public bool IsOverLimit => Elapsed > Limit;
        public void Reset() => Start = Now;
        public IEnumerable Chunk(IEnumerable underlying)
        {
            foreach (object under in underlying)
            {
                if (under != null || IsOverLimit) yield return under;
                Reset();
            }
        }
    }
}
