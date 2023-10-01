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

        private bool canExitInventory = true;
        private bool cursorHasObject = false;

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
                hotbarImages[i].sprite = itemManager.usableItems[i].script.inventoryImage;
                MaxAlpha(hotbarImages[i]);
            }

            for (int i = 0; i < hotbarImages.Length; i++)
            {
                if (hotbarImages[i].sprite == null)
                {
                    RemoveAlpha(hotbarImages[i]);
                }
            }
        }

        void UpdateInvReaminingImages()
        {
            for (int i = 0; i < itemManager.inventory.Length; i++)
            {
                inventoryImages[i].sprite = itemManager.inventory[i].script.inventoryImage;
                MaxAlpha(inventoryImages[i]);
            }

            for (int i = 0; i < inventoryImages.Length; i++)
            {
                if (inventoryImages[i].sprite == null)
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
                if (!cursorHasObject)
                {
                    toggleInvetory();
                }
            }

            if (cursorHasObject)
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
            //Place down object
            if (cursorHasObject)
            {
                if (hotbarImages[num].sprite == null)
                {
                    Sprite temp = cursorImage.sprite;
                    cursorImage.sprite = null;
                    cursorImage.enabled = false;
                    hotbarImages[num].sprite = temp;
                    MaxAlpha(hotbarImages[num]);
                    cursorHasObject = false;
                    placeItem(WhatArray.hotbar, num);
                }
                else
                {
                    placeItem(WhatArray.hotbar, num);
                    Sprite mouseImageSaved = cursorImage.sprite;
                    PickUpObjectHotbar(num);
                    hotbarImages[num].sprite = mouseImageSaved;
                    MaxAlpha(hotbarImages[num]);
                }

            }
            //Pick up object
            else if (hotbarImages[num].sprite != null && !cursorHasObject)
            {
                PickUpObjectHotbar(num);
            }
        }

        void PickUpObjectHotbar(int num)
        {
            Sprite temp = hotbarImages[num].sprite;
            RemoveAlpha(hotbarImages[num]);
            hotbarImages[num].sprite = null;
            cursorImage.sprite = temp;
            cursorImage.enabled = true;
            cursorHasObject = true;

            arrayMouseItemIsFrom = WhatArray.hotbar;
            arrayMouseItemIndex = num;
        }

        public void ButtonPressInventory(int num)
        {
            //Place down object
            if (cursorHasObject)
            {
                if (inventoryImages[num].sprite == null)
                {
                    Sprite temp = cursorImage.sprite;
                    cursorImage.sprite = null;
                    cursorImage.enabled = false;
                    inventoryImages[num].sprite = temp;
                    MaxAlpha(inventoryImages[num]);
                    cursorHasObject = false;
                    placeItem(WhatArray.inventory, num);
                }
                else
                {
                    placeItem(WhatArray.inventory, num);
                    Sprite mouseImageSaved = cursorImage.sprite;
                    pickUpObjectInventory(num);
                    inventoryImages[num].sprite = mouseImageSaved;
                    MaxAlpha(inventoryImages[num]);
                }

            }
            //Pick up object
            else if (inventoryImages[num].sprite != null && !cursorHasObject)
            {
                pickUpObjectInventory(num);
            }
        }

        void pickUpObjectInventory(int num)
        {
            Sprite temp = inventoryImages[num].sprite;
            RemoveAlpha(inventoryImages[num]);
            inventoryImages[num].sprite = null;
            cursorImage.sprite = temp;
            cursorImage.enabled = true;
            cursorHasObject = true;

            arrayMouseItemIsFrom = WhatArray.inventory;
            arrayMouseItemIndex = num;
        }

        void placeItem(WhatArray destinationArray, int destinationIndex )
        {

            if (arrayMouseItemIsFrom == WhatArray.hotbar)
            {
                itemManager.swapItems(itemManager.usableItems, arrayMouseItemIndex, destinationArray == WhatArray.hotbar ? itemManager.usableItems : itemManager.inventory, destinationIndex);
            }
            else if (arrayMouseItemIsFrom == WhatArray.inventory)
            {
                itemManager.swapItems(itemManager.inventory, arrayMouseItemIndex, destinationArray == WhatArray.hotbar ? itemManager.usableItems : itemManager.inventory, destinationIndex);
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
