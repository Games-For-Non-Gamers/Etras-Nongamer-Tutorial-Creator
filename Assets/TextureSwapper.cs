using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TextureSwapper : MonoBehaviour
{

    private string materialPath = "SwappableMaterials"; // The folder path where the textures are located
    private Material[] materialArray;


    private string texturePath = "SwappableTextures/"; // The folder path where the materials are located
    private Texture2D[] textureArray;

    public string[] packs = { "Winthor64", "Acme128", "Cubed128", "Firewolf128", "Invictus64", "Pixels16" };
    public int selected = 0;

    public FilterMode filterMode = FilterMode.Bilinear;

    [ContextMenu("SwapTextures")]
    public void SwapTextures()
    {
        string addedString = packs[selected];
        string moddedTexturePath = texturePath + addedString;
        Debug.Log("Path" + texturePath);
        materialArray = Resources.LoadAll<Material>(materialPath);
        textureArray = Resources.LoadAll<Texture2D>(moddedTexturePath);

        List<textureSwapData> swapData = new List<textureSwapData>();
        foreach (Material material in materialArray)
        {
            swapData.Add(new textureSwapData(material.name, false));
        }

        //Material material in materialArray
        foreach (Texture2D texture in textureArray)
        {
            texture.filterMode = filterMode;
            foreach (Material material in materialArray)
            {
                Material newMaterial = new Material(material);
                //Swap albedo
                if (newMaterial.mainTexture != null)
                {
                    Texture2D newTexture = newMaterial.mainTexture as Texture2D;

                    if (newTexture.name == texture.name )
                    {
                        newMaterial.mainTexture = texture;
                        
                    }

                }

                //Swap normal
                if (newMaterial.GetTexture("_BumpMap") != null)
                {
                    Texture2D normalTexture = newMaterial.GetTexture("_BumpMap") as Texture2D;
                    //If matches swap and turn on normal
                    if(normalTexture.name == texture.name)
                    {
                        newMaterial.SetTexture("_BumpMap", normalTexture);
                        newMaterial.EnableKeyword("_NORMALMAP");

                        textureSwapData data = swapData.Find(swap => swap.textureName == newMaterial.name);
                        if (data != null)
                        {
                            data.normalSwapped = true;
                            EditorUtility.SetDirty(newMaterial);
                        }

                    }
                }
            }

        }


        List<Material> materialList = materialArray.ToList();

        //Go through each mat. If swap happened do nothing. If no swap, set bump map int to 0
        foreach (textureSwapData swapD in swapData)
        {
            if (swapD.normalSwapped == false)
            {
                Material mat = materialList.Find(x => x.name == swapD.textureName);
                Material newMaterial = new Material(mat);
                if (newMaterial != null)
                {
                    if (newMaterial.GetTexture("_BumpMap") != null)
                    {
                        newMaterial.DisableKeyword("_NORMALMAP");
                        EditorUtility.SetDirty(newMaterial);
                    }
                        
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }

    internal class textureSwapData{
        public string textureName = "";
        public bool normalSwapped = false;

        public textureSwapData(string n, bool s)
        {
            textureName = n;
            normalSwapped = s;
        }

    }

}
