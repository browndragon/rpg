using System;
using BDUtil;
using UnityEngine;

namespace BDRPG
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayRandomSound : MonoBehaviour
    {
        [Serializable]
        public struct Clip
        {
            [Tooltip("==0 means ==1; <0 means disabled.")]
            [SerializeField] float chance;
            public Vector2 Delay;
            public AudioClip AudioClip;
            public float Chance => chance == default ? 1f : chance < 0 ? 0f : chance;
        }
        public Clip[] Clips;
        AudioSource Source;
        Timer delay;
        float Odds;
        void Awake()
        {
            Source = GetComponent<AudioSource>();
            Odds = 0f;
            foreach (Clip clip in Clips)
            {
                Odds += clip.Chance;
            }
            delay.Halt();
        }
        void Update()
        {
            if (Source.isPlaying) return;
            if (delay.IsRunning) return;
            float pick = UnityEngine.Random.Range(0, Odds);
            foreach (Clip clip in Clips)
            {
                if ((pick -= clip.Chance) <= 0f)
                {
                    delay = UnityEngine.Random.Range(clip.Delay.x, clip.Delay.y);
                    if (clip.AudioClip != null)
                    {
                        Source.clip = clip.AudioClip;
                        Source.Play();
                        delay = delay.Length + clip.AudioClip.length;
                    }
                    delay.Restart();
                    break;
                }
            }
        }
    }
}
