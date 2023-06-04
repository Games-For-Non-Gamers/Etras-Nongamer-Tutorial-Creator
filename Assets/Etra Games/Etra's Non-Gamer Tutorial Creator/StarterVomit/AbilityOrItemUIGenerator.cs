using Etra.StarterAssets;
using Etra.StarterAssets.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AbilityOrItemUIGenerator : MonoBehaviour
{
    //I should run when a new item or ability script is made


    [ContextMenu("Re-Generate Ability And Item Ui Objects")]
    void generateAbilityAndItemUiObjects()
    {
        generateAbilityUiObjects();
    }

    private List<AbilityScriptAndNameHolder> abilityAndSubAbilities;
    public void generateAbilityUiObjects()
    {

        abilityAndSubAbilities = EtrasResourceGrabbingFunctions.GetAllAbilitiesAndSubAbilities();
        foreach (var ability in abilityAndSubAbilities)
        {

            if (!transform.Find(getUiObjectName(ability.shortenedName).ToString()))
            {
                //make objects and add them to 
                GameObject addedObject = new GameObject(getUiObjectName(ability.shortenedName));
                // Instantiate(addedObject, Vector3.zero, Quaternion.identity);
                #if UNITY_EDITOR
                PrefabUtility.InstantiatePrefab(addedObject);
                #endif

                addedObject.AddComponent<AbilityOrItemUI>();

                addedObject.transform.parent = this.transform;
            }

        }
    }

    private string getUiObjectName(string baseName)
    {
        return baseName += " UI";
    }



}

