using UnityEngine;
using UnityEngine.UI;

public class EtraOnOffToggle : MonoBehaviour
{
    public GameObject targetObject;
    private Toggle toggle;

    private void Start()
    {
        // Get reference to the Toggle component attached to this GameObject
        toggle = GetComponent<Toggle>();

        // Add listener to the Toggle's state change event
        toggle.onValueChanged.AddListener(OnToggleValueChanged);

        if (targetObject != null)
        {
            targetObject.SetActive(toggle.isOn);
        }
    }

    private void OnToggleValueChanged(bool isOn)
    {
        // Toggle the targetObject on and off based on the state of the Toggle
        if (targetObject != null)
        {
            targetObject.SetActive(isOn);
        }
    }

    private void OnValidate()
    {
        toggle = GetComponent<Toggle>();
        if (targetObject != null)
        {
            targetObject.SetActive(toggle.isOn);
        }
    }
}
