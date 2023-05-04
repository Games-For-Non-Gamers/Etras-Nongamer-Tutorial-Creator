using UnityEngine;

namespace Etra.NonGamerTutorialCreator.Level
{
    public class LevelChunkObject : MonoBehaviour
    {
        public Vector3 boundsCenter;
        public Vector3 boundsSize = new Vector3(10f, 10f, 10f);

        [HelpBox("Place the points at the edge of the chunk on the ground")]
        [Space]
        public Vector3 startConnectionPoint = new Vector3(-5f, 0f, 0f);
        public Vector3 endConnectionPoint = new Vector3(5f, 0f, 0f);

        private void OnDrawGizmos()
        {
            const float pointRadius = 0.05f;

            Gizmos.color = new Color(0f, 0.7f, 1f);
            Gizmos.DrawWireCube(transform.position + boundsCenter, boundsSize);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + startConnectionPoint, pointRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + endConnectionPoint, pointRadius);
        }
    }
}