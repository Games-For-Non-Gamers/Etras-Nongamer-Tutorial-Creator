using UnityEngine;
using System;

namespace Etra.NonGamerTutorialCreator.Level
{
    [CreateAssetMenu(fileName = "New Level Chunk", menuName = "Etra/Non Gamer Tutorial/Level Chunk")]
    public class LevelChunk : ScriptableObject
    {
        public static event Action<LevelChunk> OnAssetValidation;

        public string chunkName;
        public Sprite icon;
        public bool required;

        [Tooltip("Determines if the level can have duplicate chunks")] public bool useSingle;

        [AbilitySelector] public string[] taughtAbilities;
        [AbilitySelector] public string[] abilitiesToTeach;

        public LevelChunkObject chunkObject;

#if UNITY_EDITOR
        private void OnValidate()
        {
            OnAssetValidation?.Invoke(this);
        }
#endif
    }
}