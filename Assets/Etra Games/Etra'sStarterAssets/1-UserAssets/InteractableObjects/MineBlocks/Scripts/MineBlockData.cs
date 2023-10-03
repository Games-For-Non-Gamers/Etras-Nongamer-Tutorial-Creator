using UnityEngine;

namespace Etra.StarterAssets
{
    [CreateAssetMenu(fileName = "MineBlockData", menuName = "Etra/Starter Assets/Mine Block Data")]
    public class MineBlockData : ScriptableObject
    {
        public GameObject blockPrefab;
        public Sprite blockIcon;
    }
}
