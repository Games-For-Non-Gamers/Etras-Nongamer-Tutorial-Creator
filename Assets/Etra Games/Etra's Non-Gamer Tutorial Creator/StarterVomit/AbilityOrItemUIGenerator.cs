using Etra.StarterAssets;
using Etra.StarterAssets.Source;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AbilityOrItemUIGenerator : MonoBehaviour
{


   // [ContextMenu("Re-Generate Ability And Item Ui Objects")]
    public void generateAbilityAndItemUiObjects()
    {
        generateAbilityUiObjects();
        generateFpsItemUiObjects();


        foreach (Transform child in transform)
        {
            child.position = Vector3.zero;
            child.localPosition = Vector3.zero;
            child.rotation = Quaternion.identity;
            child.localScale = Vector3.one;
        }

        // Save the modified prefab
        string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(this.transform.parent.gameObject);
        PrefabUtility.SaveAsPrefabAssetAndConnect(this.transform.parent.gameObject, prefabPath, InteractionMode.AutomatedAction);
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
                #if UNITY_EDITOR
                PrefabUtility.InstantiatePrefab(addedObject);
                 #endif
                addedObject.AddComponent<AbilityOrItemUI>();
                addedObject.transform.parent = this.transform;
            }

        }
    }

    private List<ItemScriptAndNameHolder> fpsItems;
    public void generateFpsItemUiObjects()
    {
        fpsItems = EtrasResourceGrabbingFunctions.GetAllItems();
        foreach (var item in fpsItems)
        {

            if (!transform.Find(getUiObjectName(item.shortenedName).ToString()))
            {
                //make objects and add them to 
                GameObject addedObject = new GameObject(getUiObjectName(item.shortenedName));
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

