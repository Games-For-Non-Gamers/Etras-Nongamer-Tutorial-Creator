using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RearrangeableList : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform selectedRectTransform;
    private Vector2 originalPosition;
    private HorizontalOrVerticalLayoutGroup layoutGroup;

    private void Start()
    {
        layoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("e");
        selectedRectTransform = eventData.pointerDrag.GetComponent<RectTransform>();
        originalPosition = selectedRectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        selectedRectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        selectedRectTransform.anchoredPosition = originalPosition;

        // Calculate the new index of the selected item based on its position
        int newIndex = -1;
        float minDistance = float.MaxValue;
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            RectTransform child = transform.GetChild(i) as RectTransform;
            float distance = Vector2.Distance(selectedRectTransform.anchoredPosition, child.anchoredPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                newIndex = i;
            }
        }

        // Reorder the list items based on the new index
        selectedRectTransform.SetSiblingIndex(newIndex);
        layoutGroup.enabled = false;
        layoutGroup.enabled = true; // Update the layout to apply the changes
    }
}
