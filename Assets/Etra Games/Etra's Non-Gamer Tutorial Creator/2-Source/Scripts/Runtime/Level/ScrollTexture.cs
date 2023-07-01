using UnityEngine;

namespace Etra.NonGamerTutorialCreator
{
    public class ScrollTexture : MonoBehaviour
    {
        public float textureScrollSpeedX = 0.5f;
        public float textureScrollSpeedY = 0.5f;
        private Renderer objectRenderer;
        //Every frame update the treadmill texture animation
        private void Start()
        {
            objectRenderer = GetComponent<Renderer>();
        }

        void Update()
        {
            float offsetX = Time.time * textureScrollSpeedX;
            float offsetY = Time.time * textureScrollSpeedY;
            objectRenderer.material.mainTextureOffset = new Vector2(offsetX, offsetY);
        }
    }
}
