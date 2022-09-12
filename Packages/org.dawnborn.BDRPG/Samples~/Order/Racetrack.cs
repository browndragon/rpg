using System.Collections;
using System.Collections.Generic;
using BDUtil;
using BDUtil.Math;
using BDUtil.Pubsub;
using BDUtil.Raw;
using BDUtil.Serialization;
using UnityEngine;

namespace BDRPG
{
    public class Racetrack : MonoBehaviour
    {

        public new RectTransform transform => (RectTransform)base.transform;
        public ObjsDeque Queue;
        public Ref<Clone> TrackProto;
        readonly Dictionary<int, Clone> TrackMembers = new();

        readonly Disposes.All unsubscribe = new();
        void OnEnable()
        {
            unsubscribe.Add(Queue.Subscribe(OnQUpdate));
        }
        void OnDisable()
        {
            foreach (Clone member in TrackMembers.Values) Clone.Release(member);
            TrackMembers.Clear();
            unsubscribe.Dispose();
        }
        void OnQUpdate(Observable.Update update)
        {
            Debug.Log($"Observing queue update {update}", this);
            HashSet<int> keep = new();
            for (int i = 0; i < Queue.Collection.Count; ++i)
            {
                GameObject target = Queue.Collection[i];
                int targetId = target.GetInstanceID();
                Vector3 position = i * 64 * Vector3.right;
                if (!TrackMembers.TryGetValue(targetId, out Clone racer))
                {
                    racer = TrackMembers[targetId] = Clone.Acquire(TrackProto);
                    racer.transform.SetParent(transform);
                    racer.GetComponentInChildren<TMPro.TMP_Text>().text = target.name;
                    racer.transform.localPosition = (Queue.Collection.Count + 1) * 64 * Vector3.right;
                }
                racer.StopAllCoroutines();
                racer.StartCoroutine(LerpTo(racer.transform, position));
                keep.Add(targetId);
            }
            List<int> kill = new();
            foreach (int id in TrackMembers.Keys) if (!keep.Contains(id)) kill.Add(id);
            foreach (int id in kill) Clone.Release(TrackMembers.PopKeyOrDefault(id));
        }
        IEnumerator LerpTo(Transform transform, Vector3 position)
        {
            float start = Time.time;
            Vector3 init = transform.localPosition;
            float upscale = position.x < init.x ? -.25f : +1.5f;
            float duration = .25f;
            for (float elapsed = Time.time - start; elapsed < duration; elapsed = Time.time - start)
            {
                Vector3 now = Vector3.Lerp(init, position, Easings.Impl.InOutSine(elapsed / duration));
                now.y = 64f * Mathf.Lerp(0, upscale, Easings.Impl.OutSine((2 * elapsed - duration) / (2f * duration)));
                transform.localPosition = now;
                yield return null;
            }
            transform.localPosition = position;
        }
    }
}
