using Etra.StarterAssets;
using Etra.StarterAssets.Items;
using EtrasStarterAssets;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public class AddItems : MonoBehaviour
{
    EtraFPSUsableItemBaseClass[] itemsToAdd;
    public bool playAudio = true;
    public UnityEvent HitBoxTriggered;

    [Header("Rendering")]
    public bool showInEditor = true;
    public bool showInGame = false;


    private void Start()
    {
        itemsToAdd = GetComponents<EtraFPSUsableItemBaseClass>();

        if (showInGame)
        {
            showRenderers();
        }
        else
        {
            hideRenderers();
        }
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


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach (EtraFPSUsableItemBaseClass item in itemsToAdd)
            {
                object componentToDuplicate = item;
                // Duplicate the component and add it to the target GameObject
                Component newComponent = EtraCharacterMainController.Instance.etraFPSUsableItemManager.gameObject.AddComponent(item.GetType());

                // Get the fields of the original component
                FieldInfo[] fields = componentToDuplicate.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

                // Copy the values from the original component's fields to the new one
                foreach (FieldInfo field in fields)
                {
                    field.SetValue(newComponent, field.GetValue(componentToDuplicate));
                }
                //Update the items array
                EtraCharacterMainController.Instance.etraFPSUsableItemManager.updateUsableItemsArray();//This base function only allows items to be added to inventory, which works for now...
            }
            if (playAudio)
            {
                GetComponent<AudioManager>().Play("AbilityGet");
            }
            HitBoxTriggered.Invoke();
            Destroy(gameObject);
        }
    }

}
