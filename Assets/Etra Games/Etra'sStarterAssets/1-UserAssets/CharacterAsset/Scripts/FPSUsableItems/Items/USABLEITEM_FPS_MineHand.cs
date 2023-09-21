namespace Etra.StarterAssets.Items
{
    public class USABLEITEM_FPS_MineHand : USABLEITEM_FPS_MineBlock
    {
        private new void Awake()
        {
            isHand= true;
            base.Awake();
        }

    }
}
