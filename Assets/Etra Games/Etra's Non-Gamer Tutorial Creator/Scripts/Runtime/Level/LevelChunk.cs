using UnityEngine;
using System;
//e
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

        [AbilitySelector] public string[] testedAbilities;
        [AbilitySelector] public string[] taughtAbilities;
        [AbilitySelector] public string testedAbilityOne;
        [AbilitySelector] public string testedAbilityTwo;
        [AbilitySelector] public string testedAbilityThree;
        [AbilitySelector] public string testedAbilityFour;
        [AbilitySelector] public string testedAbilityFive;

        [AbilitySelector] public string taughtAbilityOne;
        [AbilitySelector] public string taughtAbilityTwo;
        [AbilitySelector] public string taughtAbilityThree;



        public LevelChunkObject chunkObject;

#if UNITY_EDITOR
        private void OnValidate()
        {
            //testedAbilities = new string[0];
            // taughtAbilities = new string[0];


            if (testedAbilityOne!= "None")
            {
                testedAbilities = new string[1];
                testedAbilities[0] = testedAbilityOne;
            }

            /*
            testedAbilities = new string[5];
            testedAbilities[0] = testedAbilityOne;
            testedAbilities[1] = testedAbilityTwo;
            testedAbilities[2] = testedAbilityThree;
            testedAbilities[3] = testedAbilityFour;
            testedAbilities[4] = testedAbilityFive;

            taughtAbilities = new string[3];
            taughtAbilities[0] = taughtAbilityOne;
            taughtAbilities[1] = taughtAbilityTwo;
            taughtAbilities[2] = taughtAbilityThree;
            
            */
            OnAssetValidation?.Invoke(this);
        }
#endif
    }
}