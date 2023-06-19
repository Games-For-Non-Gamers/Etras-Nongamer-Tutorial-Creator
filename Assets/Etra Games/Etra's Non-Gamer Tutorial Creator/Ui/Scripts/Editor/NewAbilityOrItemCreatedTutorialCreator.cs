using Etra.StarterAssets.Abilities;
using Etra.StarterAssets.Interactables;
using Etra.StarterAssets.Items;
using UnityEditor;
using UnityEngine;

namespace Etra.NonGamerTutorialCreator.Editor
{
    public class NewAbilityOrItemCreatedTutorialCreator : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string assetPath in importedAssets)
            {
                if (assetPath.EndsWith(".cs")) // Check if it's a C# script
                {
                    MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
                    if (script != null && script.GetClass() != null)
                    {
                        bool changed = false;
                        if (script.GetClass().IsSubclassOf(typeof(EtraAbilityBaseClass))) // Check if it's a child of the desired class
                        {
                            PickupAbility[] foundScripts = GameObject.FindObjectsOfType<PickupAbility>(true);
                            foreach (PickupAbility pickupScript in foundScripts)
                            {
                                pickupScript.updateAbilities();
                            }
                            changed = true;
                        }
                        if (script.GetClass().IsSubclassOf(typeof(EtraFPSUsableItemBaseClass))) // Check if it's a child of the desired class
                        {
                            PickupFPSUsableItem[] foundScripts = GameObject.FindObjectsOfType<PickupFPSUsableItem>(true);
                            foreach (PickupFPSUsableItem pickupScript in foundScripts)
                            {
                                pickupScript.updateItems();
                            }
                            changed = true;
                        }

                        if (changed)
                        {
                            AbilityOrItemPickup[] foundScripts = GameObject.FindObjectsOfType<AbilityOrItemPickup>(true);
                            foreach (AbilityOrItemPickup pickup in foundScripts)
                            {
                                pickup.updateAbilitiesAndItems();
                            }

                            AbilityOrItemUIGenerator[] foundScripts1 = GameObject.FindObjectsOfType<AbilityOrItemUIGenerator>(true);
                            foreach (AbilityOrItemUIGenerator generator in foundScripts1)
                            {
                                generator.generateAbilityAndItemUiObjects();
                            }
                        }
                    }
                }
            }
        }

    }
}
