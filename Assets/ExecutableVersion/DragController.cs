using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    RectTransform currentTransform;
    private GameObject mainContent;
    private Vector3 currentPosition;
    Executable_LevelBuilder levelbuilder;

    public Image icon;
    public TextMeshProUGUI text;
    public GameObject trashButton;

    private int totalChild;

    private void Start()
    {
        currentTransform = this.GetComponent<RectTransform>();
        levelbuilder = GetComponentInParent<Executable_LevelBuilder>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        currentPosition = currentTransform.position;
        mainContent = currentTransform.parent.gameObject;
        totalChild = mainContent.transform.childCount;
    }

    public void OnDrag(PointerEventData eventData)
    {
        currentTransform.position =
            new Vector3(currentTransform.position.x, eventData.position.y, currentTransform.position.z);

        for (int i = 0; i < totalChild; i++)
        {
            if (i != currentTransform.GetSiblingIndex())
            {
                Transform otherTransform = mainContent.transform.GetChild(i);
                int distance = (int) Vector3.Distance(currentTransform.position,
                    otherTransform.position);
                if (distance <= 10)
                {
                    Vector3 otherTransformOldPosition = otherTransform.position;
                    otherTransform.position = new Vector3(otherTransform.position.x, currentPosition.y,
                        otherTransform.position.z);
                    currentTransform.position = new Vector3(currentTransform.position.x, otherTransformOldPosition.y,
                        currentTransform.position.z);
                    currentTransform.SetSiblingIndex(otherTransform.GetSiblingIndex());
                    currentPosition = currentTransform.position;

                    levelbuilder.UpdateListDataFromChildrenPosition();
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        currentTransform.position = currentPosition;
    }


    public void TrashPressed()
    {
        levelbuilder.SaveGameObjectForUndo(this.gameObject, this.transform.GetSiblingIndex());

    }

}