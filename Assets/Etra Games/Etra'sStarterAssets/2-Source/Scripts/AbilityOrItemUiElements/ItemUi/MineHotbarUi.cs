using Etra.StarterAssets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Etra.StarterAssets
{
    public class MineHotbarUi : EtraAbilityOrItemUi
    {
        public Transform[] itemSlotPos;
        public Image[] itemSlotsImages;
        public GameObject selectionImage;

        EtraFPSUsableItemManager itemManager;

        // Start is called before the first frame update
        void Start()
        {
            //Set Item Manager
            itemManager = EtraCharacterMainController.Instance.etraFPSUsableItemManager;
            if (itemManager == null)
            {
                Debug.LogError("Cannot Use Hotbar without usable Item Manager");
                this.enabled= false;
                return;
            }

            itemManager.uiItemSelectionChange.AddListener(ItemSwap);
            //Will need an event trigger to set item images
            SetItemImages();


        }

        // Update is called once per frame
        void ItemSwap()
        {
            selectionImage.transform.localPosition = itemSlotPos[itemManager.targetItemUiNum].localPosition;
        }

        private void SetItemImages()
        {
            //Set InitialItemImages
            for (int i = 0; i < itemManager.usableItems.Length; i++)
            {
                itemSlotsImages[i].sprite = itemManager.usableItems[i].script.inventoryImage;
            }

            for (int i = 0; i < itemSlotsImages.Length; i++)
            {
                if (itemSlotsImages[i].sprite == null)
                {
                    itemSlotsImages[i].enabled = false;
                }
            }
        }

    }
}
