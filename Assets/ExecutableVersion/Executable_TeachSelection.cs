using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Executable_TeachSelection : MonoBehaviour
{
    public GameObject prefabToDuplicateStandard;
    public GameObject prefabToDuplicateOffOnly;
    public GameObject entryParent;
    [HideInInspector]public bool abilityChoicesChanged;

    public List<string> abilitiesWithTeachingBlocks = new List<string>();

   public  List<string> selectedAbilities = new List<string>();
    public List<string> selectedItems = new List<string>();

    public List<string> taughtAbilities = new List<string>();
    public List<string> taughtItems = new List<string>();

    private ExecutableNewLevelDataHolder dataHolder;

    private List<ToggleStringHolder> allToggles = new List<ToggleStringHolder>();

    // Start is called before the first frame update

    void OnEnable()
    {

        //Conditions where not to rebuild the list
        if (allToggles.Count > 0 && abilityChoicesChanged == false)
        {
            return;
        }
        //e
        abilityChoicesChanged = false;

        selectedAbilities = new List<string>();
        selectedItems = new List<string>();
        taughtAbilities = new List<string>();
        taughtItems = new List<string>();


        dataHolder = GetComponentInParent<ExecutableNewLevelDataHolder>();
        selectedAbilities = dataHolder.tempSelectedAbilities;
        selectedItems = dataHolder.tempSelectedItems;
        taughtAbilities = selectedAbilities.Intersect(abilitiesWithTeachingBlocks).ToList();
        taughtItems = selectedItems.Intersect(abilitiesWithTeachingBlocks).ToList();


        //Destory all initial children
        int childCount = entryParent.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = entryParent.transform.GetChild(i);
            Destroy(child.gameObject);
        }


        foreach (string str in selectedAbilities)
        {
            if (taughtAbilities.Contains(str))
            {
                GameObject entry = Instantiate(prefabToDuplicateStandard, entryParent.transform, false);
                Toggle t = entry.GetComponentInChildren<Toggle>();
                t.onValueChanged.AddListener(OnToggleChange);
                entry.GetComponentInChildren<TextMeshProUGUI>().text = "ABILITY: " + str;
                allToggles.Add(new ToggleStringHolder(str, t));
            }
        }

        foreach (string str in selectedItems)
        {
            if (taughtItems.Contains(str))
            {
                GameObject entry = Instantiate(prefabToDuplicateStandard, entryParent.transform, false);
                Toggle t = entry.GetComponentInChildren<Toggle>();
                t.onValueChanged.AddListener(OnToggleChange);
                entry.GetComponentInChildren<TextMeshProUGUI>().text = "ITEM: " + str;
                allToggles.Add(new ToggleStringHolder(str, t));
            }
        }

        foreach (string str in selectedAbilities)
        {
            if (!taughtAbilities.Contains(str))
            {
                GameObject entry = Instantiate(prefabToDuplicateOffOnly, entryParent.transform, false);
                entry.GetComponentInChildren<TextMeshProUGUI>().text = "ABILITY: " + str;
            }
        }

        foreach (string str in selectedItems)
        {
            if (!taughtItems.Contains(str))
            {
                GameObject entry = Instantiate(prefabToDuplicateOffOnly, entryParent.transform, false);
                entry.GetComponentInChildren<TextMeshProUGUI>().text = "ITEM: " + str;
            }
        }


        OnToggleChange(true);

    }

    void OnToggleChange(bool value)
    {

        List<string> AbilitiesToEnable = new List<string>();
        foreach (ToggleStringHolder t in allToggles)
        {
            if (t.toggle.isOn == false)
            {
                AbilitiesToEnable.Add(t.abilityName);
            }
        }

        dataHolder.abilitiesToActivate = AbilitiesToEnable;
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

        public ToggleStringHolder(string n, Toggle t)
        {
            abilityName = n;
            toggle = t;
        }
    }
}
