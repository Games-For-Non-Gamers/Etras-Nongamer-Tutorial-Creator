using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Etra.StarterAssets
{
    public class MineInventoryUi : MonoBehaviour
    {


        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }


        public void ButtonPressHotbar()
        {
            Debug.Log("Pogger");
        }

        public void ButtonHoverHotbar()
        {
            Debug.Log("Pogger");
        }

        public void ButtonPressInventory()
        {
            Debug.Log("Pogger");
        }

        public void ButtonHoverInventory()
        {
            Debug.Log("Pogger");
        }


    }
}
