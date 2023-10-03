using Etra.StarterAssets.Input;
using Etra.StarterAssets.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Etra.StarterAssets.Items.EtraFPSUsableItemManager;

namespace Etra.StarterAssets
{
    public class MineInventoryUi : MonoBehaviour
    {
        public GameObject activeParent;
        public Image[] hotbarImages;
        public Image[] inventoryImages; //topLefttobottomleft
        public Image tint;
        public Image cursorImage;

        usableItemScriptAndPrefab cursorHeldItem = null;

        EtraFPSUsableItemManager itemManager;


        public enum WhatArray
        {
            hotbar,
            inventory
        }


        WhatArray arrayMouseItemIsFrom;
        int arrayMouseItemIndex;
        StarterAssetsInputs starterAssetsInputs;
        // Start is called before the first frame update
        void Start()
        {
            activeParent.SetActive(false);
            inventoryOpen = false;
            cursorImage.enabled = false;
            //Set Item Manager
            itemManager = EtraCharacterMainController.Instance.etraFPSUsableItemManager;
            if (itemManager == null)
            {
                Debug.LogError("Cannot Use Hotbar without usable Item Manager");
                this.enabled = false;
                return;
            }


            starterAssetsInputs = EtraCharacterMainController.Instance.GetComponent<StarterAssetsInputs>();

            cursorHeldItem = itemManager.defaultNullItem;

            //itemManager.uiItemSelectionChange.AddListener(ItemSwap);
            //Will need an event trigger to set item images
            UpdateItemImages();//<---Run whenever menu opened

        }




        // Start is called before the first frame update
        void UpdateItemImages()
        {
            UpdateInvHotbarImages();
            UpdateInvReaminingImages();
        }


        void UpdateInvHotbarImages()
        {
            for (int i = 0; i < itemManager.usableItems.Length; i++)
            {
                if (itemManager.usableItems[i].script.inventoryImage != null)
                {
                    //Use normal image
                    hotbarImages[i].sprite = itemManager.usableItems[i].script.inventoryImage;
                }
                else
                {
                    //Use no icon found
                    hotbarImages[i].sprite = (Sprite)Resources.Load("IconNullItem");
                }
                MaxAlpha(hotbarImages[i]);
            }

            for (int i = 0; i < hotbarImages.Length; i++)
            {
                if (hotbarImages[i].sprite == null || hotbarImages[i].sprite == itemManager.defaultNullItem.script.inventoryImage)
                {
                    RemoveAlpha(hotbarImages[i]);
                }
            }
        }

        void UpdateInvReaminingImages()
        {
            for (int i = 0; i < itemManager.inventory.Length; i++)
            {
                if (itemManager.inventory[i].script.inventoryImage != null)
                {
                    //Use normal image
                    inventoryImages[i].sprite = itemManager.inventory[i].script.inventoryImage;
                }
                else
                {
                    //Use no icon found
                    inventoryImages[i].sprite = (Sprite)Resources.Load("IconNullItem");
                }
                MaxAlpha(inventoryImages[i]);
            }

            for (int i = 0; i < inventoryImages.Length; i++)
            {
                if (inventoryImages[i].sprite == null || inventoryImages[i].sprite == itemManager.defaultNullItem.script.inventoryImage)
                {
                    RemoveAlpha(inventoryImages[i]);
                }
            }
        }

        void RemoveAlpha(Image img)
        {
            SetAlpha(img, 0);
        }

        void MaxAlpha(Image img)
        {
            SetAlpha(img, 255);
        }

        void SetAlpha(Image img, float alphaValue)
        {
            Color temp = img.color;
            img.color = new Color(temp.r, temp.g, temp.b, alphaValue);
        }

        // Update is called once per frame
        void Update()
        {

            if (starterAssetsInputs.inventory)
            {
                starterAssetsInputs.inventory = false;
                if (cursorHeldItem == itemManager.defaultNullItem)
                {
                    toggleInvetory();
                }
            }

            if (cursorHeldItem != itemManager.defaultNullItem)
            {

                Vector2 mousePosition = Mouse.current.position.ReadValue();
                cursorImage.transform.position = mousePosition;
            }

        }


        void toggleInvetory()
        {
            if (!inventoryOpen)
            {
                openInventory();
            }
            else
            {
                closeInventory();
            }
        }

        bool inventoryOpen = false;
        void openInventory()
        {
            EtraCharacterMainController.Instance.disableAllActiveAbilities();
            itemManager.disableFPSItemInputs();
            starterAssetsInputs.SetCursorState(false);
            activeParent.SetActive(true);
            inventoryOpen = true;
        }

        void closeInventory()
        {
            EtraCharacterMainController.Instance.enableAllActiveAbilities();
            itemManager.enableFPSItemInputs();
            starterAssetsInputs.SetCursorState(true);
            activeParent.SetActive(false);
            inventoryOpen = false;
        }

        public void ButtonPressHotbar(int num)
        {
            //Place down or swap object
            if (cursorHeldItem != itemManager.defaultNullItem)
            {
                //Image Slot is empty
                if (hotbarImages[num].sprite == null || hotbarImages[num].sprite == itemManager.defaultNullItem.script.inventoryImage )
                {
                    //Set slot image to cursor image
                    hotbarImages[num].sprite = cursorImage.sprite;
                    //Hide/Reset Cursor Image
                    cursorImage.sprite = itemManager.defaultNullItem.script.inventoryImage;
                    cursorImage.enabled = false;
                    //Show slot image
                    MaxAlpha(hotbarImages[num]);
                    //Update proper array
                    itemManager.placeItem(cursorHeldItem, itemManager.usableItems, num);
                    itemManager.disableFPSItemInputs();
                    cursorHeldItem = itemManager.defaultNullItem;
                }
                else
                //Image slot contains image/object
                {
                    //Swap the cursor and slot images
                    Sprite imageToSwapTo = hotbarImages[num].sprite;
                    hotbarImages[num].sprite = cursorImage.sprite;
                    cursorImage.sprite = imageToSwapTo;

                    //Save clicked item
                    usableItemScriptAndPrefab temp = itemManager.usableItems[num];
                    //Place cursor item
                    itemManager.placeItem(cursorHeldItem, itemManager.usableItems, num);
                    itemManager.disableFPSItemInputs();
                    //Getsaved item
                    cursorHeldItem = temp;
                }
            }
            //Pick up object
            else if (hotbarImages[num].sprite != null && cursorHeldItem == itemManager.defaultNullItem)
            {
                //Hide and null slot image
                Sprite temp = hotbarImages[num].sprite;
                RemoveAlpha(hotbarImages[num]);
                hotbarImages[num].sprite = null;
                //Show cursor image
                cursorImage.sprite = temp;
                cursorImage.enabled = true;

                //Add object to cursor slot
                cursorHeldItem = itemManager.usableItems[num];
                //Place cursor item
                itemManager.placeItem(itemManager.defaultNullItem, itemManager.usableItems, num);
                itemManager.disableFPSItemInputs();
            }
        }


        public void ButtonPressInventory(int num)
        {
            //Place down or swap object
            if (cursorHeldItem != itemManager.defaultNullItem)
            {
                //Image Slot is empty
                if (inventoryImages[num].sprite == null || inventoryImages[num].sprite == itemManager.defaultNullItem.script.inventoryImage)
                {
                    //Set slot image to cursor image
                    inventoryImages[num].sprite = cursorImage.sprite;
                    //Hide/Reset Cursor Image
                    cursorImage.sprite = itemManager.defaultNullItem.script.inventoryImage;
                    cursorImage.enabled = false;
                    //Show slot image
                    MaxAlpha(inventoryImages[num]);
                    //Update proper array
                    itemManager.placeItem(cursorHeldItem, itemManager.inventory, num);
                    itemManager.disableFPSItemInputs();
                    cursorHeldItem = itemManager.defaultNullItem;
                }
                else
                //Image slot contains image/object
                {
                    //Swap the cursor and slot images
                    Sprite imageToSwapTo = inventoryImages[num].sprite;
                    inventoryImages[num].sprite = cursorImage.sprite;
                    cursorImage.sprite = imageToSwapTo;

                    //Save clicked item
                    usableItemScriptAndPrefab temp = itemManager.inventory[num];
                    //Place cursor item
                    itemManager.placeItem(cursorHeldItem, itemManager.inventory, num);
                    itemManager.disableFPSItemInputs();
                    //Getsaved item
                    cursorHeldItem = temp;
                }
            }
            //Pick up object
            else if (inventoryImages[num].sprite != null && cursorHeldItem == itemManager.defaultNullItem)
            {
                //Hide and null slot image
                Sprite temp = inventoryImages[num].sprite;
                RemoveAlpha(inventoryImages[num]);
                inventoryImages[num].sprite = null;
                //Show cursor image
                cursorImage.sprite = temp;
                cursorImage.enabled = true;

                //Add object to cursor slot
                cursorHeldItem = itemManager.inventory[num];
                //Place cursor item
                itemManager.placeItem(itemManager.defaultNullItem, itemManager.inventory, num);
                itemManager.disableFPSItemInputs();
            }
        }


        public void ButtonHoverHotbar(int num)
        {
            tint.enabled = true;
            tint.transform.localPosition = hotbarImages[num].transform.localPosition;
        }

        //e
        public void ButtonHoverInventory(int num)
        {
            tint.enabled = true;
            tint.transform.localPosition = inventoryImages[num].transform.localPosition;
        }

        public void DisableTint()
        {
            tint.enabled = false;
        }

    }
}
