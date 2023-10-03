using UnityEngine;

namespace Etra.StarterAssets.Items
{
    public class USABLEITEM_FPS_Default_Null_Item : EtraFPSUsableItemBaseClass
    {
        public override string getNameOfPrefabToLoad() { return "FPSDefaultNullItemGroup"; }
        public override string getEquipSfxName() { return "NoSound"; }

        private void Awake()
        {
            Texture2D temp = (Texture2D)Resources.Load("IconNullItem");
            inventoryImage = Sprite.Create(temp, new Rect(0, 0, temp.width, temp.height), Vector2.zero);
            enabled = false;
        }

    }
}
