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
            static float OrValue(float a, float b = 0f) => a == default ? 1f : a < b ? b : a;
            public float Chance => OrValue(chance);
            [SerializeField] Vector2 delay;
            public float MinDelay => OrValue(delay.x);
            public float MaxDelay => OrValue(delay.y, MinDelay);
            [SerializeField] Vector2 volume;
            public float MinVolume => OrValue(volume.x);
            public float MaxVolume => OrValue(volume.y, MinVolume);
            public AudioClip AudioClip;
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
            if (delay.IsRunning) return;
            if (Source.isPlaying) return;

            float pick = UnityEngine.Random.Range(0, Odds);
            foreach (Clip clip in Clips)
            {
                if ((pick -= clip.Chance) <= 0f)
                {
                    delay = UnityEngine.Random.Range(clip.MinDelay, clip.MaxDelay);
                    if (clip.AudioClip != null)
                    {
                        Source.clip = clip.AudioClip;
                        Source.volume = UnityEngine.Random.Range(clip.MinVolume, clip.MaxVolume);
                        Source.Play();
                        delay = delay.Length + clip.AudioClip.length;
                    }
                    delay = delay.Restart();
                    break;
                }
            }
            throw new NotImplementedException($"Ran through all {this}'s options and didn't choose any?!");
        }
    }
}
