using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureSwapper : MonoBehaviour
{

    public string materialPath = "Materials"; // The folder path where the textures are located
    private Material[] materialArray;


    public string texturePath = "SwappableTextures/Misa"; // The folder path where the materials are located
    private Texture2D[] textureArray;


    [ContextMenu("SwapTextures")]
    public void SwapTextures()
    {
        materialArray = Resources.LoadAll<Material>(materialPath);
        textureArray = Resources.LoadAll<Texture2D>(texturePath);

        foreach (Texture2D texture in textureArray)
        {
            foreach (Material material in materialArray)
            {


                if (material.mainTexture != null)
                {
                    Texture2D albedoTexture = material.mainTexture as Texture2D;
                   
                    if (albedoTexture.name == texture.name)
                    {
                        material.mainTexture = texture;
                        Debug.Log(albedoTexture.name + " " + texture.name);
                        Debug.Log("Swapped");
                    }

                }
            }
        }
    }

}
