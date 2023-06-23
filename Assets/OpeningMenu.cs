using Etra.NonGamerTutorialCreator;
using Etra.StarterAssets.Input;
using Etra.StarterAssets;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using TMPro;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

public class OpeningMenu : MonoBehaviour
{

    public bool skipMenu;
    public GameObject openingMenuUi;
    OpeningScene opening;
    Button[] allButtons;
    RectTransform[] allRectTransforms;

    private void Start()
    {
        opening = GetComponent<OpeningScene>();
        if (skipMenu)
        {
            disableMenu();

            if (!opening.skipCutscene)
            {
                opening.runOpeningScene();
            }
            return;
        }

        openingMenuUi.SetActive(true);
        allButtons = openingMenuUi.transform.GetComponentsInAllChildren<Button>();
        allRectTransforms = openingMenuUi.transform.GetComponentsInAllChildren<RectTransform>();
        EtraCharacterMainController.Instance.GetComponent<StarterAssetsInputs>().SetCursorState(false);
    }

    public void startPressed()
    {
        EtraCharacterMainController.Instance.GetComponent<StarterAssetsInputs>().SetCursorState(true);
        disableButtons();
        StartCoroutine(closeMenuAnimation());
        
    }

    IEnumerator closeMenuAnimation()
    {
        foreach (RectTransform rect in allRectTransforms)
        {
            if (rect.GetComponent<Image>())
            {
                Image image = rect.GetComponent<Image>();
                LeanTween.value(rect.gameObject, image.color.a, 0, 1f).setOnUpdate((float newAlpha) => { Color e = new Color(image.color.r, image.color.g, image.color.b, newAlpha); rect.GetComponent<Image>().color = e; });
            } else
            if (rect.GetComponent<Text>())
            {
                Text text = rect.GetComponent<Text>();
                LeanTween.colorText(rect, new Color(text.color.r, text.color.g, text.color.b, 0), 1).setEaseInOutSine();
            } else
            if (rect.GetComponent<TextMeshProUGUI>())
            {
                TextMeshProUGUI text = rect.GetComponent<TextMeshProUGUI>();

                LeanTween.value(rect.gameObject, text.color.a, 0, 1f).setOnUpdate((float newAlpha) => { Color e = new Color(text.color.r, text.color.g, text.color.b, newAlpha); rect.GetComponent<TextMeshProUGUI>().color = e; });

            }
        }
        yield return new WaitForSeconds(1);
        disableMenu();
    }

    private void Update()
    {
        
    }
    void disableMenu()
    {
        EtraCharacterMainController.Instance.GetComponent<StarterAssetsInputs>().SetCursorState(true);
        disableButtons();
        openingMenuUi.SetActive(false);
        opening.runOpeningScene();
    }

    void disableButtons()
    {
        foreach (Button b in allButtons)
        {
            b.enabled = false;
        }
    }
}


public static class ComponentExtensions
{
    public static T[] GetComponentsInAllChildren<T>(this Component parent, bool includeInactive = false) where T : Component
    {
        return parent.GetComponentsInChildren<T>(includeInactive);
    }

    public static T[] GetComponentsInAllChildren<T>(this GameObject parent, bool includeInactive = false) where T : Component
    {
        return parent.GetComponentsInChildren<T>(includeInactive);
    }

    public static T[] GetComponentsInAllChildren<T>(this Transform parent, bool includeInactive = false) where T : Component
    {
        var components = parent.GetComponents<T>();

        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            components = components.Concat(child.GetComponentsInAllChildren<T>(includeInactive)).ToArray();
        }

        return components;
    }
}
