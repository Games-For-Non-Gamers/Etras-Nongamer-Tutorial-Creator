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

    public enum TexturePack
    {
        [InspectorName("(Default)-Winthor64x")]
        Winthor64,
        [InspectorName("(Realistic)-Cubed128x")]
        Cubed128,
        [InspectorName("(Pixel)-Pixels16x")]
        Pixels16,
        [InspectorName("(Toon)-Acme128x")]
        Acme128,
        [InspectorName("Custom Loaded")]
        Custom 
    }

    public TexturePack selectedTexturePack = TexturePack.Winthor64;
    private string customTexturePackName = ""; // Added custom string input
    public bool normalMapsEnabled = true;
    public FilterMode filterMode = FilterMode.Bilinear;

    private void OnValidate()
    {
        // Perform validation logic here if needed
    }

    public void SwapTextures()
    {
        string addedString;
        if (selectedTexturePack == TexturePack.Custom && !string.IsNullOrEmpty(customTexturePackName))
        {
            addedString = customTexturePackName;
        }
        else
        {
            addedString = selectedTexturePack.ToString();
        }

        string moddedTexturePath = texturePath + addedString;
        Debug.Log("Path" + texturePath);
        materialArray = Resources.LoadAll<Material>(materialPath);
        textureArray = Resources.LoadAll<Texture2D>(moddedTexturePath);

        List<textureSwapData> swapData = new List<textureSwapData>();
        for (int i = 0; i < materialArray.Length; i++)
        {
            Material material = materialArray[i];
            swapData.Add(new textureSwapData(material.name, false));
        }

        for (int i = 0; i < textureArray.Length; i++)
        {
            Texture2D texture = textureArray[i];
            texture.filterMode = filterMode;

            for (int j = 0; j < materialArray.Length; j++)
            {
                Material material = materialArray[j];

                // Swap albedo
                if (material.mainTexture != null)
                {
                    Texture2D newTexture = material.mainTexture as Texture2D;

                    if (newTexture.name == texture.name)
                    {
                        material.mainTexture = texture;
                    }
                }

                // Swap normal
                if (material.GetTexture("_BumpMap") != null)
                {

                    if (normalMapsEnabled)
                    {
                        material.EnableKeyword("_NORMALMAP");
                    }
                    else
                    {
                        material.DisableKeyword("_NORMALMAP");
                    }
                }

                EditorUtility.SetDirty(material);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Update material inspectors to reflect the changes
        foreach (Material material in materialArray)
        {
            EditorUtility.SetDirty(material);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(material));
        }
    }

    internal class textureSwapData
    {
        public string textureName = "";
        public bool normalSwapped = false;

        public textureSwapData(string n, bool s)
        {
            textureName = n;
            normalSwapped = s;
        }
    }

    // Add the following method to display the inspector button and custom string field
#if UNITY_EDITOR
    [CustomEditor(typeof(TextureSwapper))]
    public class TextureSwapperEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            TextureSwapper textureSwapper = (TextureSwapper)target;

            if (textureSwapper.selectedTexturePack == TextureSwapper.TexturePack.Custom)
            {
                textureSwapper.customTexturePackName = EditorGUILayout.TextField("Custom Texture Pack Name", textureSwapper.customTexturePackName);
            }

            if (GUILayout.Button("Swap Textures"))
            {
                textureSwapper.SwapTextures();
            }
        }
    }
#endif
}
