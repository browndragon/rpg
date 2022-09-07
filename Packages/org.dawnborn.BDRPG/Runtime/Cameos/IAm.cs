using UnityEngine;

namespace BDRPG.Cameos
{
    public interface IAm
    {
        public Sprite Portrait { get; }
        public string DisplayName { get; }
        public string Description { get; }
    }
}
