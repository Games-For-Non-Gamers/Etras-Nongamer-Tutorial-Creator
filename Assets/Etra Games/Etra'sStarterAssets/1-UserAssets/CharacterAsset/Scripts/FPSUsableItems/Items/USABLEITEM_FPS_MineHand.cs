namespace Etra.StarterAssets.Items
{
    public class USABLEITEM_FPS_MineHand : USABLEITEM_FPS_MineBlock
    {
        private new void Awake()
        {
            isHand= true;
            blockToLoad = null;
            iconName = "IconMineHand";
            base.Awake();
        }

    }
}
