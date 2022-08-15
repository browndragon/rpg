using UnityEngine;

namespace BDRPG
{
    public static class Cameos
    {
        public interface IAm
        {
            public Sprite Portrait { get; }
            public string DisplayName { get; }
            public string Description { get; }
        }
        public struct Snapshot : IAm
        {
            public Sprite Portrait { get; set; }
            public string DisplayName { get; set; }
            public string Description { get; set; }
            public Snapshot(Sprite portrait, string displayName, string description)
            {
                this = default;
                Portrait = portrait;
                DisplayName = displayName;
                Description = description;
            }
            public Snapshot(IAm other) : this(other?.Portrait, other?.DisplayName, other?.Description) { }
        }
        public abstract class ScriptableObject : UnityEngine.ScriptableObject, IAm
        {
            [field: SerializeField] public Sprite Portrait { get; protected set; }
            [field: SerializeField] protected string OverrideDisplayName { get; set; }
            public string DisplayName => OverrideDisplayName?.Length > 0 ? OverrideDisplayName : name;
            [field: SerializeField] public string Description { get; }
        }
    }
}
