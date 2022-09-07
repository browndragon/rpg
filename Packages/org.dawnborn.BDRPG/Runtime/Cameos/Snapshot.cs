using UnityEngine;

namespace BDRPG.Cameos
{
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
}
