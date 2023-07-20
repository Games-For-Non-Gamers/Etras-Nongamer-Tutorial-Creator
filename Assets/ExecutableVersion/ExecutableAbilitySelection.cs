using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExecutableAbilitySelection : MonoBehaviour
{
    public GameObject prefabToDuplicate;
    public GameObject entryParent;


    [HideInInspector]public List<string> standardAbilities = new List<string>();
    [HideInInspector] public List<string> allFpsAbilities = new List<string>();
    [HideInInspector] public List<string> allFpsItems = new List<string>();
    [HideInInspector] public List<string> allTpsAbilities = new List<string>();

    private ExecutableNewLevelDataHolder dataHolder;

    private List<ToggleStringHolder> allToggles = new List<ToggleStringHolder>();
    private Etra.StarterAssets.EtraCharacterMainController.GameplayType savedGameplayType = Etra.StarterAssets.EtraCharacterMainController.GameplayType.FirstPerson;

    // Start is called before the first frame update

    void OnEnable()
    {
        dataHolder = GetComponentInParent<ExecutableNewLevelDataHolder>();
        //Conditions where not to rebuild the list
        if (savedGameplayType == dataHolder.gameplayType && allToggles.Count > 0)
        {
            return;
        }

        //Destory all initial children
        int childCount = entryParent.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = entryParent.transform.GetChild(i);
            Destroy(child.gameObject);
        }


        foreach (string str in standardAbilities)
        {
            GameObject entry = Instantiate(prefabToDuplicate, entryParent.transform, false);
            Toggle t = entry.GetComponentInChildren<Toggle>();
            t.onValueChanged.AddListener(OnToggleChange);
            entry.GetComponentInChildren<TextMeshProUGUI>().text = "ABILITY: " + str;
            allToggles.Add(new ToggleStringHolder(str, t));
        }


        //First Person
        if (dataHolder.gameplayType == Etra.StarterAssets.EtraCharacterMainController.GameplayType.FirstPerson)
        {
            foreach (string str in allFpsAbilities)
            {
                GameObject entry = Instantiate(prefabToDuplicate, entryParent.transform, false);
                Toggle t = entry.GetComponentInChildren<Toggle>();
                t.onValueChanged.AddListener(OnToggleChange);
                entry.GetComponentInChildren<TextMeshProUGUI>().text = "ABILITY: " + str;
                allToggles.Add(new ToggleStringHolder(str, t));
            }

            foreach (string str in allFpsItems)
            {
                GameObject entry = Instantiate(prefabToDuplicate, entryParent.transform, false);
                Toggle t = entry.GetComponentInChildren<Toggle>();
                t.onValueChanged.AddListener(OnToggleChange);
                entry.GetComponentInChildren<TextMeshProUGUI>().text = "ITEM: " + str;
                allToggles.Add(new ToggleStringHolder(str, t));
            }
        }

        //Third person
        else if (dataHolder.gameplayType == Etra.StarterAssets.EtraCharacterMainController.GameplayType.ThirdPerson)
        {
            foreach (string str in allTpsAbilities)
            {
                GameObject entry = Instantiate(prefabToDuplicate, entryParent.transform, false);
                Toggle t = entry.GetComponentInChildren<Toggle>();
                t.onValueChanged.AddListener(OnToggleChange);
                entry.GetComponentInChildren<TextMeshProUGUI>().text = "ABILITY: " + str;
                allToggles.Add(new ToggleStringHolder(str, t));
            }
        }

        OnToggleChange(true);

    }

    void OnToggleChange(bool value)
    {

        List<string> activatedAbilities = new List<string>(); 
        foreach (ToggleStringHolder t in allToggles)
        {
            if (t.toggle.isOn)
            {
                activatedAbilities.Add(t.abilityName);
            }
        }

        dataHolder.tempSelectedAbilities = activatedAbilities;
    }

    public void AllOn()
    {
        foreach (ToggleStringHolder t in allToggles)
        {
            t.toggle.isOn = true;
        }

        OnToggleChange(true);
    }

    public void AllOff()
    {
        foreach (ToggleStringHolder t in allToggles)
        {
            t.toggle.isOn = false;
        }

        OnToggleChange(true);
    }


    class ToggleStringHolder
    {
        public string abilityName;
        public Toggle toggle;

        public ToggleStringHolder(string n , Toggle t)
        {
            abilityName = n;
            toggle = t;
        }
    }
}
