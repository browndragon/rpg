using System;
using System.Collections.Generic;
using BDUtil.Math;
using UnityEngine;

namespace BDRPG
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayRandomSound : MonoBehaviour
    {
        public bool Loop = true;
        [Serializable]
        public struct Clip
        {
            public int Category;
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
        public Timer Delay;
        readonly Dictionary<int, float> Odds = new();
        public int Category = 0;
        void Awake()
        {
            Source = GetComponent<AudioSource>();

            foreach (Clip clip in Clips) Odds[clip.Category] = Odds.GetValueOrDefault(clip.Category) + clip.Chance;
            Delay.Halt();
        }
        public void StopLoop() => Loop = false;
        public void LoopRandom(int category)
        {
            Category = category;
            Loop = true;
        }
        public void PlayRandom(int category)
        {
            float pick = UnityEngine.Random.Range(0, Odds[category]);
            foreach (Clip clip in Clips)
            {
                if (clip.Category != category) continue;
                if ((pick -= clip.Chance) > 0f) continue;
                PlayClip(clip);
                return;
            }
        }
        public void PlayClipIndex(int i) => PlayClip(Clips[i]);
        void PlayClip(Clip clip)
        {
            Delay = UnityEngine.Random.Range(clip.MinDelay, clip.MaxDelay);
            if (clip.AudioClip != null)
            {
                Source.clip = clip.AudioClip;
                Source.volume = UnityEngine.Random.Range(clip.MinVolume, clip.MaxVolume);
                Source.Play();
                Delay = Delay.Length + clip.AudioClip.length;
            }
            Delay = Delay.Restart();
        }
        public void PlayAudioClipAtFullVolume(AudioClip clip)
        {
            Source.clip = clip;
            Source.volume = 1f;
            Source.Play();
        }
        public void PlayRandomAudioClip(Clip[] clips) { }
        void Update()
        {
            if (!Loop) return;
            if (Delay.IsRunning) return;
            if (Source.isPlaying) return;
            PlayRandom(Category);
        }
    }
}
