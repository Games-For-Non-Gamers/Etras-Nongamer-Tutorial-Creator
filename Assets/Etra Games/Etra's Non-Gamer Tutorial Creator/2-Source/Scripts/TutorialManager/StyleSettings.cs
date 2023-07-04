using Etra.NonGamerTutorialCreator.Level;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class StyleSettings : MonoBehaviour
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
    [Header("Textures")]
    public TexturePack selectedTexturePack = TexturePack.Winthor64;

    private string customTexturePackName = ""; // Added custom string input
    public bool normalMapsEnabled = true;
    public FilterMode filterMode = FilterMode.Bilinear;

    [Header("Ocean")]
    public NonGamerTutorialOcean.OceanType oceanType = NonGamerTutorialOcean.OceanType.LightOcean;
    public bool underwaterPostProcessAndSplashParticle = true;

#if UNITY_EDITOR
    public void changeStyle()
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

        transform.Find("EnvironmentalObjects").Find("NongamerTutorialOcean").GetComponent<NonGamerTutorialOcean>().changeOcean(oceanType, underwaterPostProcessAndSplashParticle);


    }
#endif
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
    [CustomEditor(typeof(StyleSettings))]
    public class TextureSwapperEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            StyleSettings textureSwapper = (StyleSettings)target;

            if (textureSwapper.selectedTexturePack == StyleSettings.TexturePack.Custom)
            {
                textureSwapper.customTexturePackName = EditorGUILayout.TextField("Custom Texture Pack Name", textureSwapper.customTexturePackName);
            }

            if (GUILayout.Button("Change Style"))
            {
                textureSwapper.changeStyle();
            }
        }
    }
#endif
}
