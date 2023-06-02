using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Source;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using static Etra.NonGamerTutorialCreator.TutorialCreator.TutorialCreatorAbilityTreeView;

public class AbilityOrItemUIGenerator : MonoBehaviour
{
    //I should run when a new item or ability script is made


    [ContextMenu("Re-Generate Ability And Item Ui Objects")]
    void generateAbilityAndItemUiObjects()
    {
        generateAbilityUiObjects();
    }
    

    public void generateAbilityUiObjects()
    {
        
        List<Ability> generalAbilities;
        //Get all EtraAbilityBaseClass
        generalAbilities = new List<Ability>();
        generalAbilities = FindAllTypes<EtraAbilityBaseClass>().Select(x => new Ability(x)).ToList();

        List<string> temp = new List<string>();
        foreach (var ability in generalAbilities)
        {

            if (!transform.Find(ability.uiObjectName.ToString()))
            {
                //make objects and add them to 
                temp.Add(ability.uiObjectName.ToString());


                GameObject addedObject = new GameObject(ability.uiObjectName.ToString());
               // Instantiate(addedObject, Vector3.zero, Quaternion.identity);
                #if UNITY_EDITOR
                PrefabUtility.InstantiatePrefab(addedObject);
#endif

                addedObject.AddComponent<AbilityOrItemUI>();

                addedObject.transform.parent = this.transform;
            }

        }
    }



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
        public string uiObjectName;
        public bool state;

        public void GenerateName()
        {
            uiObjectName = "";

            string[] splits = type.Name.Split('_');

            if (splits.Length == 2)
            {
                uiObjectName = splits[1];
            }
            else
            {
                for (int i = 1; i < splits.Length; i++)
                {
                    uiObjectName += splits[i];
                    if (i != splits.Length - 1)
                    {
                        uiObjectName += "";
                    }

                }
            }

            uiObjectName += "_UI";

            //uiObjectName = Regex.Replace(uiObjectName, "([a-z])([A-Z])", "$1 $2");
        }
    }



}

