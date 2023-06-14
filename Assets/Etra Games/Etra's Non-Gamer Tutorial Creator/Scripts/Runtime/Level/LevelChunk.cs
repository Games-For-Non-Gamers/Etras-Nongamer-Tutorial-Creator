using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Linq;
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

        [Tooltip("If the object is recommended it will automatically be added to level builder if it is usable")] public bool recommended;
        [Tooltip("The higher the priority, the closter to the star this chunk will automatically be placed in. Star priority = 500")] public int orderPriority; 

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
            //get the tested abilities and add them to the list
            List<string> testedAbilitiesOrdering = new List<string>();

            if (testedAbilityOne!= "" && testedAbilityOne != null)
            {
                testedAbilitiesOrdering.Add(testedAbilityOne);
            }
            if (testedAbilityTwo != "" && testedAbilityTwo != null)
            {
                testedAbilitiesOrdering.Add(testedAbilityTwo);
            }
            if (testedAbilityThree != "" && testedAbilityThree != null)
            {
                testedAbilitiesOrdering.Add(testedAbilityThree);
            }
            if (testedAbilityFour != "" && testedAbilityFour != null)
            {
                testedAbilitiesOrdering.Add(testedAbilityFour);
            }
            if (testedAbilityFive != "" && testedAbilityFive != null)
            {
                testedAbilitiesOrdering.Add(testedAbilityFive);
            }


            List<string> taughtAbilitiesOrdering = new List<string>();

            if (taughtAbilityOne != "" && taughtAbilityOne != null)
            {
                taughtAbilitiesOrdering.Add(taughtAbilityOne);
            }
            if (taughtAbilityTwo != "" && taughtAbilityTwo != null)
            {
                taughtAbilitiesOrdering.Add(taughtAbilityTwo);
            }
            if (taughtAbilityThree != "" && taughtAbilityThree != null)
            {
                taughtAbilitiesOrdering.Add(taughtAbilityThree);
            }

            taughtAbilities = new string[taughtAbilitiesOrdering.Count];

            for (int i = 0; i < taughtAbilitiesOrdering.Count; i++)
            {
                taughtAbilities[i] = taughtAbilitiesOrdering[i];
            }



            //we can do something here like re add the learned abilities to tested abilities to make sure they pop up
            testedAbilities = new string[testedAbilitiesOrdering.Count];

            for (int i = 0; i < testedAbilitiesOrdering.Count; i++)
            {
                testedAbilities[i] = testedAbilitiesOrdering[i];
            }




            OnAssetValidation?.Invoke(this);
        }
#endif
    }
}