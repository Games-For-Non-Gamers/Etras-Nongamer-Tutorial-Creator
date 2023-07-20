using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutableAbilitySelection : MonoBehaviour
{
    public GameObject prefabToDuplicate;
    public GameObject scrollableList;

    // Start is called before the first frame update
    void Start()
    {
        //Destory all initial children
        int childCount = scrollableList.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = scrollableList.transform.GetChild(i);
            Destroy(child.gameObject);
        }




    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
