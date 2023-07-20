using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExecutableAbilitySelection : MonoBehaviour
{
    public GameObject prefabToDuplicate;
    public GameObject entryParent;


    public List<string> standardAbilities = new List<string>();
    public List<string> allFpsAbilities = new List<string>();
    public List<string> allFpsItems = new List<string>();
    public List<string> allTpsAbilities = new List<string>();

    private ExecutableNewLevelDataHolder dataHolder;

    public List<string> abilitiesToTeach = new List<string>();
    public List<string> abilitiesToActivate = new List<string>();

    private List<ToggleStringHolder> allToggles = new List<ToggleStringHolder>();
    private Etra.StarterAssets.EtraCharacterMainController.GameplayType savedGameplayType = Etra.StarterAssets.EtraCharacterMainController.GameplayType.FirstPerson;

    // Start is called before the first frame update
    void OnEnable()
    {
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
            GameObject entry = Instantiate(prefabToDuplicate);
            entry.transform.parent = entryParent.transform;
            Toggle t = entry.GetComponentInChildren<Toggle>();
            t.onValueChanged.AddListener(OnToggleChange);
            GetComponentInChildren<TextMeshProUGUI>().text = "ABILITY: " + str;
            allToggles.Add(new ToggleStringHolder(str, t));
        }


        //First Person
        if (dataHolder.gameplayType == Etra.StarterAssets.EtraCharacterMainController.GameplayType.FirstPerson)
        {
            foreach (string str in allFpsAbilities)
            {
                GameObject entry = Instantiate(prefabToDuplicate);
                entry.transform.parent = entryParent.transform;
                Toggle t = entry.GetComponentInChildren<Toggle>();
                t.onValueChanged.AddListener(OnToggleChange);
                GetComponentInChildren<TextMeshProUGUI>().text = "ABILITY: " + str;
                allToggles.Add(new ToggleStringHolder(str, t));
            }

            foreach (string str in allFpsItems)
            {
                GameObject entry = Instantiate(prefabToDuplicate);
                entry.transform.parent = entryParent.transform;
                Toggle t = entry.GetComponentInChildren<Toggle>();
                t.onValueChanged.AddListener(OnToggleChange);
                GetComponentInChildren<TextMeshProUGUI>().text = "ITEM: " + str;
                allToggles.Add(new ToggleStringHolder(str, t));
            }
        }

        //Third person
        else if (dataHolder.gameplayType == Etra.StarterAssets.EtraCharacterMainController.GameplayType.ThirdPerson)
        {
            foreach (string str in allTpsAbilities)
            {
                GameObject entry = Instantiate(prefabToDuplicate);
                entry.transform.parent = entryParent.transform;
                Toggle t = entry.GetComponentInChildren<Toggle>();
                t.onValueChanged.AddListener(OnToggleChange);
                GetComponentInChildren<TextMeshProUGUI>().text = "ABILITY: " + str;
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
