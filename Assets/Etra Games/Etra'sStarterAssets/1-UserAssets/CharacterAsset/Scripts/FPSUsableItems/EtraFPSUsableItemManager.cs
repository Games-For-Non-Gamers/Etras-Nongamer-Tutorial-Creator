using UnityEngine;
using System.Collections;
using UnityEditor;
using Etra.StarterAssets.Input;
using Etra.StarterAssets.Source;
using EtrasStarterAssets;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using UnityEngine.Events;

namespace Etra.StarterAssets.Items
{
    public class EtraFPSUsableItemManager : MonoBehaviour
    {
        //Visible in Inspector
        public bool playEquipAnims = true;
        public bool playUnequipAnims = true;
        [SerializeField] private bool lockNumberHotbarSwap = false;
        [SerializeField] private bool lockScrollHotbarItemSwap = false;
        [HideInInspector] public bool weaponInitHandledElsewhere = false;

        [Header("Ovverride Default Null Item")]
        public usableItemScriptAndPrefab defaultNullItem;
        public bool fillEmptySlotsWithDefaultItem = false;


        private const int NUMBER_OF_HOTBAR_SLOTS = 9;
        [Header("The Items Will Be Selected In This Order:")]
        public usableItemScriptAndPrefab[] usableItems;

        [Header("Item Inventory")]
        private const int NUMBER_OF_INVENTORY_SLOTS = 27;
        public usableItemScriptAndPrefab[] inventory = new usableItemScriptAndPrefab[NUMBER_OF_INVENTORY_SLOTS];

        usableItemScriptAndPrefab mostRecentItem;
        int mostRecentItemIndex;
        [HideInInspector] public UnityEvent uiItemSelectionChange;
        [HideInInspector] public UnityEvent uiItemSwap;

        #region Functions to update The usableItems Array
        //Run this function whenever an item is added

        bool addInHotbar = false;

        public void SetLockNumberHotbarSwap(bool state)
        {
            lockNumberHotbarSwap = state;
        }
        public void SetLockScrollHotbarItemSwap(bool state) {
            lockScrollHotbarItemSwap = state;
        }

        [ContextMenu("updateUsableItemsArray")]
        public void updateUsableItemsArray(bool addInHotbar)
        {
            if (addInHotbar)
            {
                this.addInHotbar = true;
            }
            else
            {
                this.addInHotbar = false;
            }
            updateUsableItemsArray();
        }

        public void updateUsableItemsArray()
        {
            if (defaultNullItem == null || defaultNullItem.script == null)
            {
                removeNullItemSlots();
            }

            if (defaultNullItem != null && fillEmptySlotsWithDefaultItem)
            {
                for (int i = 0; i < usableItems.Length; i++)
                {
                    if (usableItems[i] == null)
                    {

                    }
                    else
                    if (usableItems[i].script == null)
                    {
                        usableItems[i] = null;
                    }
                }
                for (int i = 0; i < inventory.Length; i++)
                {
                    if (inventory[i] == null)
                    {
                        inventory[i] = null;
                    }
                    else
                    if (inventory[i] == null)
                    {
                        inventory[i] = null;
                    }
                }
            }


            //I understand this is Big O^2 however, it only runs on validate. What's more important is navigation of the final structure (an array) is as fast as possible.
            EtraFPSUsableItemBaseClass[] grabbedUsableItems;
            int lastAddedItemIndex = -1;
            usableItemScriptAndPrefab savedOldItem = null;

            if (usableItems.Length>0)
            {
                if (activeItemNum< usableItems.Length)
                {
                    savedOldItem = usableItems[activeItemNum];
                }
            }
            
            if (this == null)
            {
                return;
            }

            if (GetComponent<EtraFPSUsableItemBaseClass>() != null)
            {
                grabbedUsableItems = GetComponents<EtraFPSUsableItemBaseClass>();
            }
            else
            {
                grabbedUsableItems = new EtraFPSUsableItemBaseClass[0];
            }


            foreach (var item in grabbedUsableItems)
            {
                bool itemFound = false;

                foreach (usableItemScriptAndPrefab setItem in usableItems)
                {
                    if (setItem != null)
                    {
                        if (setItem.script != null)
                        {
                            if (item.Equals(setItem.script))
                            {
                                itemFound = true;
                            }
                        }
                    }
                }

                foreach (usableItemScriptAndPrefab setItem in inventory)
                {
                    if (setItem != null)
                    {
                        if (setItem.script != null)
                        {
                            if (item.Equals(setItem.script))
                            {
                                itemFound = true;
                            }
                        }
                    }
                }

                if (!itemFound)
                {
                    usableItemScriptAndPrefab newItem = new usableItemScriptAndPrefab(item);
                    increaseAbilityArrayWithNewElement(newItem);
                }
            }

            //Fill empty slots with null
            if (usableItems.Length < NUMBER_OF_HOTBAR_SLOTS && fillEmptySlotsWithDefaultItem && defaultNullItem != null)
            {
                //maybe unnecesary?
                //If we have no item added don't do anything but basic prune
                usableItemScriptAndPrefab[] temp = new usableItemScriptAndPrefab[NUMBER_OF_HOTBAR_SLOTS];
                for (int i = 0; i < usableItems.Length; i++)
                {
                    temp[i] = usableItems[i];
                }
                usableItems = temp;
            }
            else if (usableItems.Length > NUMBER_OF_HOTBAR_SLOTS)
            {

                int itemNum = NUMBER_OF_HOTBAR_SLOTS;
                //add in hotbar if selected
                if (addInHotbar)
                {
                    addInHotbar = false;
                    for (int i = 0; i < NUMBER_OF_HOTBAR_SLOTS; i++)
                    {
                        if (usableItems[i].script == defaultNullItem.script || usableItems[i].script == null)
                        {
                            usableItems[i] = usableItems[itemNum];
                            lastAddedItemIndex = i;
                            if (itemNum!= usableItems.Length-1)
                            {
                                itemNum++;
                            }
                            else
                            {
                                itemNum++;
                                break;
                            }
                        }
                    }
                }

                //Add the rest to inventory
                for (int i = itemNum; i < usableItems.Length; i++)
                {
                    for (int j = 0; j < inventory.Length; j++)
                    {
                        if (inventory[j] == null || inventory[j].script == null || inventory[j].script == defaultNullItem.script)
                        {
                            inventory[j] = usableItems[i];
                            break;
                        }

                        if (j == inventory.Length-1)
                        {
                            Debug.LogWarning("No room left in inventory");
                            break;
                        }
                    }
                }
                //Reduce prune the usableItemsInventory
                usableItemScriptAndPrefab[] temp = new usableItemScriptAndPrefab[NUMBER_OF_HOTBAR_SLOTS];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = usableItems[i];
                }
                usableItems = temp;
            }

            if (lastAddedItemIndex != -1)
            {
                StartCoroutine(equipItemCoroutine(savedOldItem, lastAddedItemIndex, true));
            }


            uiItemSwap.Invoke();


        }

        private void removeNullItemSlots()
        {

            if (usableItems == null)
            {
                return;
            }

            if (usableItems.Length == 0)
            {
                return;
            }

            bool slotsNeedRemoved = true;
            while (slotsNeedRemoved)
            {
                slotsNeedRemoved = false;
                int elementToPass = 0;
                for (int i = 0; i < usableItems.Length; i++)
                {
                    if (usableItems[i] == null || usableItems[i].script == null)
                    {
                        slotsNeedRemoved = true;
                        elementToPass = i;
                        i = usableItems.Length;
                    }
                }
                if (slotsNeedRemoved)
                {
                    removeElementFromArray(elementToPass);
                }

            }


        }

        private void removeElementFromArray(int elementToSkip)
        {
            usableItemScriptAndPrefab[] shortenedArray = new usableItemScriptAndPrefab[usableItems.Length - 1];
            int iterator = 0;
            for (int i = 0; i < usableItems.Length; i++)
            {
                if (i != elementToSkip)
                {
                    shortenedArray[iterator] = usableItems[i];
                    iterator++;
                }
            }

            usableItems = shortenedArray;
        }

        private void increaseAbilityArrayWithNewElement(usableItemScriptAndPrefab abilityToAdd)
        {
            if (abilityToAdd.script == defaultNullItem.script)
            {
                return;
            }
            mostRecentItem = abilityToAdd;
            mostRecentItemIndex = usableItems.Length;
            usableItemScriptAndPrefab[] temp = new usableItemScriptAndPrefab[usableItems.Length + 1];

            for (int i = 0; i < usableItems.Length; i++)
            {
                temp[i] = usableItems[i];
            }

            temp[usableItems.Length] = abilityToAdd;


            usableItems = temp;
        }


        //Editor exclusive functions
#if UNITY_EDITOR
        EtraFPSUsableItemManager()
        {
            ObjectFactory.componentWasAdded -= HandleComponentAdded;
            ObjectFactory.componentWasAdded += HandleComponentAdded;

            EditorApplication.quitting -= OnEditorQuiting;
            EditorApplication.quitting += OnEditorQuiting;
        }
        private void HandleComponentAdded(Component obj)
        {
            updateUsableItemsArray();
            if (Application.isPlaying)
            {
                Debug.LogWarning("In the EDITOR PLAY MODE, you can add items by adding components to the FPS Usable Item Manager.\n" +
                    "However, this will not work in a built game witout an additional step. Everytime you add a component to\n" +
                    "the item manager, run the updateUsableItemsArray() function to add the new item to the correct array.\n");
            }
        }

        private void OnEditorQuiting()
        {
            ObjectFactory.componentWasAdded -= HandleComponentAdded;
            EditorApplication.quitting -= OnEditorQuiting;
        }

#endif

        #endregion

        //Not in Inspector
        public  int activeItemNum = 0;
        [HideInInspector] public GameObject activeItemPrefab;
        [HideInInspector] bool isEquipping = false;

        //References
        StarterAssetsInputs starterAssetsInputs;
        GameObject cameraRoot;
        private bool inputsLocked = false;
        AudioManager fpsItemAudioManager;

        [System.Serializable]
        public class usableItemScriptAndPrefab
        {
            public EtraFPSUsableItemBaseClass script;
            [HideInInspector] public GameObject prefab;

            public usableItemScriptAndPrefab(EtraFPSUsableItemBaseClass passedScript)
            {
                script = passedScript;
                prefab = Resources.Load<GameObject>(script.getNameOfPrefabToLoad()) ;
            }
        }
        public static GameObject getPrefabFromResourcesByName(string prefabName)
        {
            GameObject foundObject;
            if (Resources.Load<GameObject>(prefabName) == null)
            {
                Debug.LogError(prefabName + " not found in assets. Please restore the prefab.");
                return null;
            }
            foundObject = Resources.Load<GameObject>(prefabName);
            return foundObject;
        }

#if UNITY_EDITOR
        //Reset is ran by the character creator adding this component
        public void Reset()
        {
            updateUsableItemsArray();
            //Add usable FPS item camera if it does not exist
            if (GameObject.Find("FPSUsableItemsCamera"))
            {
                DestroyImmediate(GameObject.Find("FPSUsableItemsCamera"));
            }


            var FPSUsableItemsCamera = EtrasResourceGrabbingFunctions.addPrefabFromAssetsByName("FPSUsableItemsCamera", GameObject.Find("EtraPlayerCameraRoot").transform, false, Vector3.zero);
            //Add usable FPS item camera script to the camera to check for the FPSUsableItem Layer
            if (FPSUsableItemsCamera != null && FPSUsableItemsCamera.GetComponent<FPS_Item_Cam_Checks>() == null) { FPSUsableItemsCamera.AddComponent<FPS_Item_Cam_Checks>(); }
        }


        private void OnValidate() //ON COMPONENT ADD
        {
            updateUsableItemsArray();
            //go through usable items.
            //load all items in game object prefabs so searching does not have to be done.
        }
#endif

        private void Awake()
        {

            if (defaultNullItem == null)
            {
                removeNullItemSlots();
            }


            if (defaultNullItem.script != null)
            {
                defaultNullItem = new usableItemScriptAndPrefab(defaultNullItem.script);
                for (int i = 0; i < usableItems.Length; i++)
                {
                    if (usableItems[i] == null || inventory[i].script == null)
                    {
                        usableItems[i] = defaultNullItem;
                    }
                }

                for (int i = 0; i < inventory.Length; i++)
                {
                    if (inventory[i] == null || inventory[i].script == null)
                    {
                        inventory[i] = defaultNullItem;
                    }
                }
            }

            cameraRoot = GameObject.Find("EtraPlayerCameraRoot");
            starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();

            for (int i = 0; i < usableItems.Length; i++)
            {
                usableItems[i].script.enabled = false;
            }

        }


        private void Start()
        {
            fpsItemAudioManager = GameObject.FindGameObjectWithTag("MainCamera").transform.Find("FPSItemSfx").GetComponent<AudioManager>();
            if (!weaponInitHandledElsewhere)
            {
                instatiateItemAtStart();
            }

        }


        public void instatiateItemAtStart()
        {

            if (usableItems.Length > 0)
            {
                var newItem = Instantiate(usableItems[activeItemNum].prefab);
                newItem.transform.SetParent(cameraRoot.transform);
                newItem.transform.localPosition = Vector3.zero;
                newItem.transform.localRotation = Quaternion.identity;
                activeItemPrefab = newItem;
                usableItems[activeItemNum].script.enabled = true;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (inputsLocked)
            {
                setInputsToDefault();
                return;
            }

            //Some sort of check that were not moving to same item
            if (isEquipping)
            {
                return;
            }

            //Number Keys select
            #region Number Key Item Selection
            if (starterAssetsInputs.item0Select && !lockNumberHotbarSwap)
            {
                if (activeItemNum != 0 && usableItems.Length > 0)
                {
                    StartCoroutine(equipItemCoroutine(0));
                }
            }

            if (starterAssetsInputs.item1Select && !lockNumberHotbarSwap)
            {
                if (activeItemNum != 1 && usableItems.Length > 1)
                {
                    StartCoroutine(equipItemCoroutine(1));
                }
            }

            if (starterAssetsInputs.item2Select && !lockNumberHotbarSwap)
            {
                if (activeItemNum != 2 && usableItems.Length > 2)
                {
                    StartCoroutine(equipItemCoroutine(2));
                }
            }

            if (starterAssetsInputs.item3Select && !lockNumberHotbarSwap)
            {
                if (activeItemNum != 3 && usableItems.Length > 3)
                {
                    StartCoroutine(equipItemCoroutine(3));
                }
            }

            if (starterAssetsInputs.item4Select && !lockNumberHotbarSwap)
            {
                if (activeItemNum != 4 && usableItems.Length > 4)
                {
                    StartCoroutine(equipItemCoroutine(4));
                }
            }

            if (starterAssetsInputs.item5Select && !lockNumberHotbarSwap)
            {
                if (activeItemNum != 5 && usableItems.Length > 5)
                {
                    StartCoroutine(equipItemCoroutine(5));
                }
            }

            if (starterAssetsInputs.item6Select && !lockNumberHotbarSwap)
            {
                if (activeItemNum != 6 && usableItems.Length > 6)
                {
                    StartCoroutine(equipItemCoroutine(6));
                }
            }

            if (starterAssetsInputs.item7Select && !lockNumberHotbarSwap)
            {
                if (activeItemNum != 7 && usableItems.Length > 7)
                {
                    StartCoroutine(equipItemCoroutine(7));
                }
            }

            if (starterAssetsInputs.item8Select && !lockNumberHotbarSwap)
            {
                if (activeItemNum != 8 && usableItems.Length > 8)
                {
                    StartCoroutine(equipItemCoroutine(8));
                }
            }

            #endregion

            if (usableItems.Length <= 1)
            {
                return;
            }

            //Mouse wheel and shoulder button scroll
            if (starterAssetsInputs.usableItemInventoryScroll == 1 && !lockScrollHotbarItemSwap)
            {
                int itemToMoveTo = activeItemNum + 1;

                if (itemToMoveTo >= usableItems.Length)
                {
                    itemToMoveTo = 0;
                }
                StartCoroutine(equipItemCoroutine(itemToMoveTo));

            }

            if (starterAssetsInputs.usableItemInventoryScroll == -1 && !lockScrollHotbarItemSwap)
            {
                int itemToMoveTo = activeItemNum - 1;

                if (itemToMoveTo < 0)
                {
                    itemToMoveTo = usableItems.Length - 1;
                }
                StartCoroutine(equipItemCoroutine(itemToMoveTo));

            }

            if (lockScrollHotbarItemSwap)
            {
                starterAssetsInputs.usableItemInventoryScroll = 0;
            }

            if (lockNumberHotbarSwap)
            {
                starterAssetsInputs.item0Select = false;
                starterAssetsInputs.item1Select = false;
                starterAssetsInputs.item2Select = false;
                starterAssetsInputs.item3Select = false;
                starterAssetsInputs.item4Select = false;
                starterAssetsInputs.item5Select = false;
                starterAssetsInputs.item6Select = false;
                starterAssetsInputs.item7Select = false;
                starterAssetsInputs.item8Select = false;
            }
        }

        public void equipItem(int num)
        {
            if (usableItems.Length > num)
            {
                    StartCoroutine(equipItemCoroutine(num));
            }
        }

        public void equipNewItem()
        {
            if (usableItems.Length>0)
            {
                foreach (usableItemScriptAndPrefab item in usableItems) //Makes sure it is added to hotbar, not inventory
                {
                    if (item == mostRecentItem)
                    {
                        StartCoroutine(equipItemCoroutine(mostRecentItem, mostRecentItemIndex, true));
                        break;
                    }
                }
            }
        }

        [ContextMenu("testSwap")]
        void testSwap()
        {
            swapItems(usableItems, 0, usableItems, 3);
        }

        [ContextMenu("invSwap")]
        void invSwap()
        {
            swapItems(usableItems, 0, inventory, 3);
        }



        public void placeItem(usableItemScriptAndPrefab item, usableItemScriptAndPrefab[] arrayOfTargetItem, int targetIndex)
        {
            usableItemScriptAndPrefab oldItem = usableItems[activeItemNum];
            arrayOfTargetItem[targetIndex] = item;

            if (arrayOfTargetItem == usableItems && targetIndex == activeItemNum)
            {
                StartCoroutine(equipItemCoroutine(oldItem, activeItemNum));
            }
            uiItemSwap.Invoke();
        }


        public void swapItems(usableItemScriptAndPrefab[] array1, int index1, usableItemScriptAndPrefab[] array2, int index2)
        {
            usableItemScriptAndPrefab oldItem = usableItems[activeItemNum];
            usableItemScriptAndPrefab temp = array1[index1];
            array1[index1] = array2[index2];
            array2[index2] = temp;

            if ((array1 == usableItems && index1 == activeItemNum) || (array2 == usableItems && index2 == activeItemNum))
            {
                StartCoroutine(equipItemCoroutine(oldItem, activeItemNum));
            }
            uiItemSwap.Invoke();
        }


        IEnumerator equipItemCoroutine(int newItemNum)
        {
            StartCoroutine(equipItemCoroutine(usableItems[activeItemNum], newItemNum));
            yield return new WaitForSeconds(0.01f);
        }

        IEnumerator equipItemCoroutine(usableItemScriptAndPrefab oldItem, int newItemNum)
        {
            StartCoroutine(equipItemCoroutine(oldItem, newItemNum, false));
            yield return new WaitForSeconds(0.01f);
        }

        public int targetItemUiNum;
        IEnumerator equipItemCoroutine(usableItemScriptAndPrefab oldItem, int newItemNum, bool forceSelect)
        {
            targetItemUiNum = newItemNum;
            uiItemSelectionChange.Invoke();


            if (oldItem == usableItems[newItemNum] && !forceSelect)// avoid swapping items unncessarily if swap to same EXACT item like hand
            {
                activeItemNum = newItemNum;//Change num for ui, but do nothing else
                setInputsToDefault();
            }
            else
            {
                isEquipping = true;
                if (playUnequipAnims && usableItems.Length > 1)
                {
                    oldItem.script.runUnequipAnimation();
                    yield return new WaitForSeconds(oldItem.script.getItemUnequipSpeed());
                }

                oldItem.script.enabled = false;


                Destroy(activeItemPrefab);

                activeItemNum = newItemNum;
                var newItem = Instantiate(usableItems[activeItemNum].prefab);
                newItem.transform.SetParent(cameraRoot.transform);
                newItem.transform.localPosition = Vector3.zero;
                newItem.transform.localRotation = Quaternion.identity;
                activeItemPrefab = newItem;

                if (playEquipAnims)
                {
                    if (usableItems[activeItemNum].script.getEquipSfxName() == "")
                    {
                        fpsItemAudioManager.Play("DefaultEquip");
                    }
                    else
                    {
                        fpsItemAudioManager.Play(usableItems[activeItemNum].script.getEquipSfxName());
                    }

                    activeItemPrefab.transform.localRotation = Quaternion.Euler(usableItems[activeItemNum].script.getItemUnequipRotation());
                    usableItems[activeItemNum].script.runEquipAnimation();
                    yield return new WaitForSeconds(usableItems[activeItemNum].script.getItemEquipSpeed());
                }

                usableItems[activeItemNum].script.enabled = true;
                setInputsToDefault();
                isEquipping = false;
            }

        }
        //For Ruin cutscene lol
        public void ManualTurnOnLight()
        {
            USABLEITEM_FPS_Flashlight light = (USABLEITEM_FPS_Flashlight)usableItems[activeItemNum].script;
            light.ManualTurnOffLight();
        }
        public void ManualTurnOffLight()
        {
            USABLEITEM_FPS_Flashlight light = (USABLEITEM_FPS_Flashlight)usableItems[activeItemNum].script;
            light.ManualTurnOffLight();
        }
        void setInputsToDefault()
        {
            starterAssetsInputs.usableItemInventoryScroll = 0;
            starterAssetsInputs.item0Select = false;
            starterAssetsInputs.item1Select = false;
            starterAssetsInputs.item2Select = false;
            starterAssetsInputs.item3Select = false;
            starterAssetsInputs.item4Select = false;
            starterAssetsInputs.item5Select = false;
            starterAssetsInputs.item6Select = false;
            starterAssetsInputs.item7Select = false;
            starterAssetsInputs.item8Select = false;
        }

        public void disableFPSItemInputs()
        {

            if (usableItems.Length > 0)
            {
                inputsLocked = true;
                usableItems[activeItemNum].script.inputsLocked = true;
                if (EtraCharacterMainController.Instance.GetComponentInChildren<FPSUsableItemSwayAndBobAnimations>())
                {
                    EtraCharacterMainController.Instance.GetComponentInChildren<FPSUsableItemSwayAndBobAnimations>().lockInput = true;
                }
            }

        }

        public void enableFPSItemInputs()
        {

            if (usableItems.Length > 0)
            {
                inputsLocked = false;
                usableItems[activeItemNum].script.inputsLocked = false;
                if (EtraCharacterMainController.Instance.GetComponentInChildren<FPSUsableItemSwayAndBobAnimations>())
                {
                    EtraCharacterMainController.Instance.GetComponentInChildren<FPSUsableItemSwayAndBobAnimations>().lockInput = false;
                }
            }
        }

        public void activateAbilities(List<string> abilitiesShortenedName)
        {

            for (int i = 0; i < usableItems.Length; i++)
            {
                if (!abilitiesShortenedName.Contains(GenerateShortenedName(usableItems[i].script.GetType().ToString())))
                {
                    usableItems[i].script.toDelete = true;
                    usableItems[i].script = null;
                }
            }

            EtraFPSUsableItemBaseClass[] items =GetComponents<EtraFPSUsableItemBaseClass>();

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].toDelete)
                {
                    Destroy(items[i]);
                }
            }

            updateUsableItemsArray();


        }

        public string GenerateShortenedName(string fileName)
        {
            string toReturn = fileName;
            toReturn = toReturn.Split('_').Last();

            toReturn = Regex.Replace(toReturn, "([a-z])([A-Z])", "$1 $2");
            return toReturn;
        }

    }
}
