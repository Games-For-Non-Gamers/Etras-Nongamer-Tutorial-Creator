using Etra.NonGamerTutorialCreator;
using Etra.StarterAssets.Source;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NonGamerTutorialOcean : MonoBehaviour
{
    public GameObject upperLayer;
    public GameObject lowerLayer;
    public Material toonMaterial;
    public Material pixelMaterial;

    public enum OceanType
    {
        LightOcean,
        DarkOcean,
        MixedToon,
        MixedPixel,
        ToonOnly,
        PixelOnly
    }

    private void disableAll()
    {
        upperLayer.GetComponent<Renderer>().enabled = false;
        lowerLayer.GetComponent<Renderer>().enabled = false;

        if (transform.Find("CrestOcean"))
        {
            GameObject crestOcean = transform.Find("CrestOcean").gameObject;
            PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(crestOcean);

            if (prefabAssetType == PrefabAssetType.Regular || prefabAssetType == PrefabAssetType.Variant)
            {
                PrefabUtility.UnpackPrefabInstance(this.transform.gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction); 
            }
            DestroyImmediate(transform.Find("CrestOcean").gameObject);

        }
        if (transform.Find("PostProcessUnderwater"))
        {
            GameObject postProcess = transform.Find("PostProcessUnderwater").gameObject;
            PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(postProcess);

            if (prefabAssetType == PrefabAssetType.Regular || prefabAssetType == PrefabAssetType.Variant)
            {
                PrefabUtility.UnpackPrefabInstance(this.transform.gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
            }
            DestroyImmediate(transform.Find("PostProcessUnderwater").gameObject);
        }
    }

    public void changeOcean(OceanType oceanType, bool postProcessOn)
    {
        disableAll();


        if (postProcessOn)
        {
            EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("PostProcessUnderwater", this.transform, false, new Vector3 (0f, -9.4f,248f), Quaternion.identity, new Vector3(46f, 15.23f, 600));
        }

        switch (oceanType)
        {
            case OceanType.LightOcean:
                EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("CrestOcean", this.transform, false, new Vector3(0f, -2, 0));
                lowerLayer.GetComponent<Renderer>().enabled = true;
                break;

            case OceanType.DarkOcean:
                EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("CrestOcean", this.transform, false, new Vector3(0f, -2, 0));
                break;

            case OceanType.MixedToon:
                EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("CrestOcean", this.transform, false, new Vector3(0f, -2, 0));
                upperLayer.GetComponent<Renderer>().enabled = true;
                upperLayer.GetComponent<Renderer>().material = toonMaterial;
                upperLayer.GetComponent<ScrollTexture>().textureScrollSpeedY = 0.05f;
                break;

            case OceanType.MixedPixel:
                EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("CrestOcean", this.transform, false, new Vector3(0f, -2, 0));
                upperLayer.GetComponent<Renderer>().enabled = true;
                upperLayer.GetComponent<Renderer>().material = pixelMaterial;
                upperLayer.GetComponent<ScrollTexture>().textureScrollSpeedY = 0.25f;
                break;

            case OceanType.ToonOnly:
                upperLayer.GetComponent<Renderer>().enabled = true;
                upperLayer.GetComponent<Renderer>().material = toonMaterial;
                upperLayer.GetComponent<ScrollTexture>().textureScrollSpeedY = 0.05f;
                break;

            case OceanType.PixelOnly:
                upperLayer.GetComponent<Renderer>().enabled = true;
                upperLayer.GetComponent<Renderer>().material = pixelMaterial;
                upperLayer.GetComponent<ScrollTexture>().textureScrollSpeedY = 0.25f;
                break;

        }

    }


}
