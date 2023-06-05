using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Source;
using Etra.StarterAssets;
using EtrasStarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NonGamerTutorialPickup : MonoBehaviour, ISerializationCallbackReceiver
{
    public static List<string> TMPList;
    [HideInInspector] public List<string> abilityShortenedNames;
    [ListToPopup(typeof(NonGamerTutorialPickup), "TMPList")]
    public string Ability_To_Activate;
    private List<AbilityScriptAndNameHolder> abilityAndSubAbilities;
    AbilityScriptAndNameHolder selectedAbility;
    EtraAbilityBaseClass abilityScriptOnCharacter;

    public bool showTutorialPopup = true;

    //Set the correct selected ability
    private void Start()
    {
        updateAbilities();

        foreach (AbilityScriptAndNameHolder abil in abilityAndSubAbilities)
        {
            if (abil.shortenedName == Ability_To_Activate)
            {
                selectedAbility = abil;
            }
        }


        //If the ability is not on the player, it cannot be activated or deactivated
        if ((EtraAbilityBaseClass)EtraCharacterMainController.Instance.etraAbilityManager.GetComponent(selectedAbility.script.GetType()) == null)
        {
            Debug.LogWarning("PickupAbility.cs cannot activate the " + Ability_To_Activate + " ability on your character because your character does not have the " + Ability_To_Activate + " script attached to its ability manager.");
        }
        else
        {
            abilityScriptOnCharacter = (EtraAbilityBaseClass)EtraCharacterMainController.Instance.etraAbilityManager.GetComponent(selectedAbility.script.GetType());
        }

    }

    private void Reset()
    {
        updateAbilities();
    }

    //If the player collides with the pickup...
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;
            GetComponent<AudioManager>().Play("AbilityGet");
            //enable the ability and destroy the pickup
            if (showTutorialPopup)
            {
                GameObject.Find(getUiObjectName(selectedAbility.shortenedName)).GetComponent<AbilityOrItemUI>().runUiEvent(selectedAbility);
            }
            else
            {
                abilityScriptOnCharacter.unlockAbility(selectedAbility.name);
            }
            StartCoroutine(waitToDestroy());
        }

    }

    private string getUiObjectName(string baseName)
    {
        return baseName += " UI";
    }

    //so sfx can play
    IEnumerator waitToDestroy()
    {

        yield return new WaitForSeconds(1);
    }


    //Update the list every frame on editor selection "functionally"
    [ContextMenu("Update Abilities")]
    public void updateAbilities()
    {
        abilityAndSubAbilities = EtrasResourceGrabbingFunctions.GetAllAbilitiesAndSubAbilities();
        abilityShortenedNames = new List<string>();
        foreach (var ability in abilityAndSubAbilities)
        {
            abilityShortenedNames.Add(ability.shortenedName);
        }

    }

    public void OnBeforeSerialize()
    {
        //abilityShortenedNames = GetAllAbilities();
        TMPList = abilityShortenedNames;
    }

    public void OnAfterDeserialize()
    {

    }

}
