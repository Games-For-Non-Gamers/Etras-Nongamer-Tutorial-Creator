using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Etra.StarterAssets.EtraCharacterMainController;

public class ExecutableGameplayTypePage : MonoBehaviour
{
    [Header("Toggles")]
    public Toggle fpsToggle;
    public GameObject fpsGreen;
    public Toggle tpsToggle;
    public GameObject tpsGreen;

    [Header("Gameplay Type")]
    public GameObject fpsOptions;
    public TMP_Dropdown fpsModelDropdown;
    public Image fpsImage;
    public TextMeshProUGUI fpsTextbox;
    public GameObject tpsOptions;
    public TMP_Dropdown tpsModelDropdown;
    public Image tpsImage;
    public TextMeshProUGUI tpsTextbox;

    Model[] fpsModels;
    Model[] allModels;
    ExecutableNewLevelDataHolder levelData;

    private static Dictionary<Model, Sprite> _modelImages = new Dictionary<Model, Sprite>();
    private static Dictionary<Model, string> _modelDescriptions = new Dictionary<Model, string>()
    {
        [Model.DefaultArmature] = "Default humanoid model with animations.",
        [Model.Capsule] = "Default Unity capsule without animations.",
        [Model.Voxel] = "Stylized voxel model with animations.",
        [Model.None] = "No model.",
    };


    // Start is called before the first frame update
    void Start()
    {
        _modelImages = new Dictionary<Model, Sprite>()
        {
            [Model.DefaultArmature] = Resources.Load<Sprite>("CharacterCreatorModelArmature"),
            [Model.Capsule] = Resources.Load<Sprite>("CharacterCreatorModelCapsule"),
            [Model.Voxel] = Resources.Load<Sprite>("CharacterCreatorModelVoxel"),
            [Model.None] = Resources.Load<Sprite>("CharacterCreatorModelNone"),
        };

        levelData = FindObjectOfType<ExecutableNewLevelDataHolder>();
        //Toggle 
        fpsToggle.onValueChanged.AddListener(FpsToggle);
        tpsToggle.onValueChanged.AddListener(TpsToggle);
        //GameplayTypeDropdowns
        //Fps Models
        fpsModels = new Model[] { Model.None, Model.Capsule };
        fpsModelDropdown.ClearOptions();
        List<string> strings = new List<string>();
        foreach (Model model in fpsModels)
        {
            strings.Add(model.ToString());
        }
        fpsModelDropdown.AddOptions(strings);

        //Tps Models
        allModels = (Model[])Enum.GetValues(typeof(Model));
        tpsModelDropdown.ClearOptions();
        List<string> strings1 = new List<string>();
        foreach (Model model in allModels)
        {
            strings1.Add(model.ToString());
        }
        tpsModelDropdown.AddOptions(strings1);

        fpsModelDropdown.onValueChanged.AddListener(UpdateDropdown);
        tpsModelDropdown.onValueChanged.AddListener(UpdateDropdown);
        SetImagesAndText();
        FpsToggle(true);
    }

    void UpdateDropdown(int value)
    {
        SetImagesAndText();
        if (levelData.gameplayType == GameplayType.FirstPerson)
        {
            levelData.characterModel = fpsModels[fpsModelDropdown.value];
        }
        else if (levelData.gameplayType == GameplayType.ThirdPerson)
        {
            levelData.characterModel = allModels[tpsModelDropdown.value];
        }

    }

    void FpsToggle(bool isOn)
    {
        if (isOn)
        {
            ActivateFps();

        }
        else
        {
            ActivateTps();
        }
        UpdateDropdown(0);
    }

    void TpsToggle(bool isOn)
    {
        if (isOn)
        {
            ActivateTps();

        }
        else
        {
            ActivateFps();
        }
        UpdateDropdown(0);
    }

    void ActivateFps()
    {
        //Disable TPS
        tpsGreen.SetActive(false);
        tpsToggle.isOn = false;
        tpsOptions.SetActive(false);

        //Enable FPS
        fpsGreen.SetActive(true);
        fpsToggle.isOn = true;
        fpsOptions.SetActive(true);
        levelData.gameplayType = GameplayType.FirstPerson;
    }

    void ActivateTps()
    {
        //Disable FPS
        fpsGreen.SetActive(false);
        fpsToggle.isOn = false;
        fpsOptions.SetActive(false);

        //Enable TPS
        tpsGreen.SetActive(true);
        tpsToggle.isOn = true;
        tpsOptions.SetActive(true);
        levelData.gameplayType = GameplayType.ThirdPerson;
    }

    void SetImagesAndText()
    {
        //Fps
        fpsImage.sprite = _modelImages[fpsModels[fpsModelDropdown.value]];
        fpsTextbox.text = _modelDescriptions[fpsModels[fpsModelDropdown.value]];

        //Tps
        tpsImage.sprite = _modelImages[allModels[tpsModelDropdown.value]];
        tpsTextbox.text = _modelDescriptions[allModels[tpsModelDropdown.value]];

    }


}
