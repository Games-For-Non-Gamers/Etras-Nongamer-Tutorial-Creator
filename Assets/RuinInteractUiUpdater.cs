using UnityEngine;
using UnityEngine.UI;

public class RuinInteractUiUpdater : MonoBehaviour
{
    public GameObject controllerUi;
    [SerializeField] public Image leftFaceUi;

    Image self;

    private void Start()
    {
        self = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controllerUi.gameObject.activeInHierarchy)
        {
            self.enabled = true;
            self.sprite = leftFaceUi.sprite;
        }
        else
        {
            self.enabled = false;
        }
    }
}
