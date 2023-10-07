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
        public Image[] imagesToFadeIn;
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

            if (hideUiAtStart)
            {
                hideUi();
            }

            itemManager.uiItemSelectionChange.AddListener(ItemSwap);
            itemManager.uiItemSwap.AddListener(SetItemImages);
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
                itemSlotsImages[i].enabled = true;
                if (itemManager.usableItems[i].script != null)
                {
                    if (itemManager.usableItems[i].script.inventoryImage != null)
                    {
                        //Use normal image
                        itemSlotsImages[i].sprite = itemManager.usableItems[i].script.inventoryImage;
                    }
                    else
                    {
                        //Use no icon found
                        Texture2D temp = (Texture2D)Resources.Load("IconNullItem");
                        itemSlotsImages[i].sprite = Sprite.Create(temp, new Rect(0, 0, temp.width, temp.height), Vector2.zero);
                    }
                }
            }

            for (int i = 0; i < itemSlotsImages.Length; i++)
            {
                if (itemSlotsImages[i].sprite == null || itemSlotsImages[i].sprite == itemManager.defaultNullItem.script.inventoryImage)
                {
                    itemSlotsImages[i].enabled = false;
                }
            }
        }

        public override void fadeInUi(float time)
        {
            foreach (Image image in imagesToFadeIn)
            {
                image.enabled = true;
                LeanTween.value(0, 1, time).setOnUpdate((float alphaValue) => { Color newColor = image.color; newColor.a = alphaValue; image.color = newColor; }).setEaseInOutSine();
            }
        }

    }
}
