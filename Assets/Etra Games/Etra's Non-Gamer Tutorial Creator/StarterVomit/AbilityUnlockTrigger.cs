using Etra.StarterAssets;
using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Interactables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class AbilityUnlockTrigger : MonoBehaviour, ISerializationCallbackReceiver
{

    public GameObject packedPRefabtest;
    [ContextMenu("Create item")]
    public void createItemTest()
    {
       


        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // Set the cube's parent object
        cube.transform.parent = packedPRefabtest.transform;

        // Set the cube's position and rotation relative to the parent
        cube.transform.localPosition = Vector3.zero;
        cube.transform.localRotation = Quaternion.identity;

    }

    public static List<string> TMPList;
    [HideInInspector] public List<string> abilityShortenedNames;
    [ListToPopup(typeof(PickupAbility), "TMPList")]
    public string Ability_To_Activate;
    private List<Ability> generalAbilities;

    public EtraUIEvent uiEvent;



    #region AbilityListDisplay
    public List<string> GetAllAbilities()
    {
        //Get all EtraAbilityBaseClass
        generalAbilities = new List<Ability>();
        generalAbilities = FindAllTypes<EtraAbilityBaseClass>().Select(x => new Ability(x)).ToList();

        List<string> temp = new List<string>();
        foreach (var ability in generalAbilities)
        {
            temp.Add(ability.shortenedName.ToString());
        }
        return temp;
    }

    //Update the list every frame on editor selection "functionally"
    public void OnBeforeSerialize()
    {
        abilityShortenedNames = GetAllAbilities();
        TMPList = abilityShortenedNames;
    }

    public void OnAfterDeserialize()
    {

    }

    //Helper function to find all EtraAbilityBaseClass scripts
    public static IEnumerable<Type> FindAllTypes<T>()
    {
        var type = typeof(T);
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(t => t != type && type.IsAssignableFrom(t));
    }

    //Helper class to find all EtraAbilityBaseClass scripts
    class Ability
    {
        public Ability(Type type)
        {
            this.type = type;
            state = false;
            name = type.Name;
            GenerateName();
        }

        public Type type;
        public string name;
        public string shortenedName;
        public bool state;

        public void GenerateName()
        {
            shortenedName = "";

            string[] splits = type.Name.Split('_');

            if (splits.Length == 2)
            {
                shortenedName = splits[1];
            }
            else
            {
                for (int i = 1; i < splits.Length; i++)
                {
                    shortenedName += splits[i];
                    if (i != splits.Length - 1)
                    {
                        shortenedName += " ";
                    }

                }
            }

            shortenedName = Regex.Replace(shortenedName, "([a-z])([A-Z])", "$1 $2");
        }
    }
    #endregion
}
