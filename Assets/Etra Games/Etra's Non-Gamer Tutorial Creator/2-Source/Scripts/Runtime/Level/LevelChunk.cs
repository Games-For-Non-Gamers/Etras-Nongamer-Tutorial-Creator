using UnityEngine;
using System;
using System.Collections.Generic;

namespace Etra.NonGamerTutorialCreator.Level
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Level Chunk", menuName = "Etra/Non Gamer Tutorial/Level Chunk")]
    public class LevelChunk : ScriptableObject
    {
       // public static event Action<LevelChunk> OnAssetValidation;

        public string chunkName;
        [SerializeField]public Sprite icon;
        public bool required;

        [Tooltip("Determines if the level can have duplicate chunks")] public bool useSingle;

        [Tooltip("If the object is recommended it will automatically be added to level builder if it is usable")] public bool recommended;
        public bool tempRecommended;
        [Tooltip("The higher the priority, the closter to the star this chunk will automatically be placed in. End Chunk priority = 500")] public int orderPriority;
        [Tooltip("If there are several teaching block options, the one with the highest teaching priority will be chosen.")] public int teachingPriority;

        [AbilitySelector] public string[] testedAbilities;
        [AbilitySelector] public string[] taughtAbilities;

        public LevelChunkObject chunkObject;
    }
}