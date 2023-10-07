using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Source;
using Etra.StarterAssets;
using EtrasStarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Etra.NonGamerTutorialCreator
{
    public class AbilityOrItemPickup : MonoBehaviour, ISerializationCallbackReceiver
    {
        [Header("Ability or Item")]
        public static List<string> TMPList;
        [HideInInspector] public List<string> abilityAndItemShortenedNames;
        [ListToPopup(typeof(AbilityOrItemPickup), "TMPList")]
        public string AbilityOrItem_To_Activate;
        private List<AbilityScriptAndNameHolder> abilityAndSubAbilities;
        AbilityScriptAndNameHolder selectedAbility;
        EtraAbilityBaseClass abilityScriptOnCharacter;
        private List<ItemScriptAndNameHolder> fpsItems;
        ItemScriptAndNameHolder selectedItem;
        bool isAbility = false;
        bool isItem = false;

        public bool showTutorialAnimation = true;
        public bool showUi = true;
        public bool playPickupSfx = true;

        [Header("Rendering")]
        public bool showInEditor = true;
        public bool showInGame = false;


        public void SetShowTutorialAnimation(bool state)
        {
            showTutorialAnimation= state;
        }

        //Set the correct selected ability
        private void Start()
        {

            if (showInGame)
            {
                showRenderers();
            }
            else
            {
                hideRenderers();
            }

            updateAbilitiesAndItems();

            //Check items first
            foreach (ItemScriptAndNameHolder item in fpsItems)
            {
                if (item.shortenedName == AbilityOrItem_To_Activate)
                {
                    selectedItem = item;
                }
            }

            if (selectedItem != null)
            {
                isItem = true;
                isAbility = false;
            }
            //Then check abilities
            else
            {
                isAbility = true;
                isItem = false;

                foreach (AbilityScriptAndNameHolder abil in abilityAndSubAbilities)
                {
                    if (abil.shortenedName == AbilityOrItem_To_Activate)
                    {
                        selectedAbility = abil;
                    }
                }

                //If the ability is not on the player, it cannot be activated or deactivated
                if ((EtraAbilityBaseClass)EtraCharacterMainController.Instance.etraAbilityManager.GetComponent(selectedAbility.script.GetType()) == null)
                {
                   // Debug.LogWarning("PickupAbility.cs cannot activate the " + AbilityOrItem_To_Activate + " ability on your character because your character does not have the " + AbilityOrItem_To_Activate + " script attached to its ability manager.");
                }
                else
                {
                    abilityScriptOnCharacter = (EtraAbilityBaseClass)EtraCharacterMainController.Instance.etraAbilityManager.GetComponent(selectedAbility.script.GetType());
                }
            }



        }

        private void Reset()
        {
            updateAbilitiesAndItems();
        }

        //If the player collides with the pickup...
        private void OnTriggerStay(Collider other) //in case the player spawns in the collider
        {
            if (other.gameObject.tag == "Player")
            {
                GetAbilityOrItem();
            }

        }

        public void GetAbilityOrItem()
        {
            if (isAbility)
            {
                if (GetComponent<MeshRenderer>())
                {
                    GetComponent<MeshRenderer>().enabled = false;
                }

                if (GetComponent<BoxCollider>())
                {
                    GetComponent<BoxCollider>().enabled = false;
                }


                if (playPickupSfx)
                {
                    GetComponent<AudioManager>().Play("AbilityGet");
                }

                //enable the ability and destroy the pickup
                if (showTutorialAnimation)
                {
                    GameObject.Find(getUiObjectName(selectedAbility.shortenedName)).GetComponent<EtraAnimationHolder>().runAnimation(selectedAbility, selectedItem, true);
                }
                else
                {
                    if (showUi)
                    {
                        GameObject.Find(getUiObjectName(selectedAbility.shortenedName)).GetComponent<EtraAnimationHolder>().showAllAnimatedObjects();
                    }
                    abilityScriptOnCharacter.unlockAbility(selectedAbility.name);
                }
                StartCoroutine(waitToDestroy());
            }
            else if (isItem)
            {
                if (showTutorialAnimation)
                {
                    GameObject.Find(getUiObjectName(selectedItem.shortenedName)).GetComponent<EtraAnimationHolder>().runAnimation(selectedAbility, selectedItem, false);

                }
                else
                {
                    if (showUi)
                    {
                        GameObject.Find(getUiObjectName(selectedItem.shortenedName)).GetComponent<EtraAnimationHolder>().showAllAnimatedObjects();
                    }

                    //Add the script to the item manager
                    EtraCharacterMainController.Instance.etraFPSUsableItemManager.gameObject.AddComponent(selectedItem.script.GetType());
                    //Update the items array
                    EtraCharacterMainController.Instance.etraFPSUsableItemManager.updateUsableItemsArray();
                    //Equip the new item
                    EtraCharacterMainController.Instance.etraFPSUsableItemManager.equipNewItem();
                }
                //Destory this pickup
                Destroy(gameObject);
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
        [ContextMenu("Update Abilities And Items")]
        public void updateAbilitiesAndItems()
        {
            abilityAndItemShortenedNames = new List<string>();
            updateAbilities();
            updateItems();
        }
        void updateAbilities()
        {
            abilityAndSubAbilities = EtrasResourceGrabbingFunctions.GetAllAbilitiesAndSubAbilities();
            foreach (var ability in abilityAndSubAbilities)
            {
                abilityAndItemShortenedNames.Add(ability.shortenedName);
            }

        }
        void updateItems()
        {
            fpsItems = EtrasResourceGrabbingFunctions.GetAllItems();
            foreach (ItemScriptAndNameHolder item in fpsItems)
            {
                abilityAndItemShortenedNames.Add(item.shortenedName);
            }

        }


        public void OnBeforeSerialize()
        {
            //abilityShortenedNames = GetAllAbilities();
            TMPList = abilityAndItemShortenedNames;
        }

        public void OnAfterDeserialize()
        {

        }



        private void OnValidate()
        {
            if (showInEditor)
            {
                showRenderers();
            }
            else
            {
                hideRenderers();
            }

        }

        void showRenderers()
        {
            if (GetComponent<MeshRenderer>())
            {
                this.GetComponent<MeshRenderer>().enabled = true;
            }
        }

        void hideRenderers()
        {
            if (GetComponent<MeshRenderer>())
            {
                this.GetComponent<MeshRenderer>().enabled = false;
            }
        }

    }
}
