using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightPriorityHitbox : MonoBehaviour
{
    public GameObject [] groupsToPrioritize;
    public GameObject[] groupsToDePrioritize;

    List<Light> lightsToPrioritize = new List<Light>();
    List<Light> lightsToDePrioritize = new List<Light>();


    // Start is called before the first frame update
    void Start()
    {
        if (showInGame)
        {
            showRenderers();
        }
        else
        {
            hideRenderers();
        }

        foreach (GameObject baseObject in groupsToDePrioritize)
        {
            lightsToDePrioritize.AddRange(baseObject.GetComponentsInChildren<Light>().ToList());
        }

        foreach (GameObject baseObject in groupsToPrioritize)
        {
            lightsToPrioritize.AddRange(baseObject.GetComponentsInChildren<Light>().ToList());
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            foreach (Light light in lightsToDePrioritize)
            {
                light.renderMode = LightRenderMode.Auto;
            }
            foreach (Light light in lightsToPrioritize)
            {
                light.renderMode = LightRenderMode.ForcePixel; //Important?
            }
        }
    }

    [Header("Rendering")]
    public bool showInEditor = true;
    public bool showInGame = false;


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
