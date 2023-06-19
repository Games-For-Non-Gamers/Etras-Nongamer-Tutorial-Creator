using Etra.StarterAssets;
using Etra.StarterAssets.Source;
using UnityEngine;

namespace Etra.NonGamerTutorialCreator.Level
{
    public class LevelChunkObject : MonoBehaviour
    {
        [HelpBox("Place the points at the edge of the chunk on the ground")]
        [Space]
        public Vector3 startConnectionPoint = new Vector3(-5f, 0f, 0f);
        public Vector3 endConnectionPoint = new Vector3(5f, 0f, 0f);
        public Vector3 playerSpawnPosition = new Vector3(0f, 0f, 1f);


        //Broke again. Spawned object but not saved. Unstable code place.
        public void makePlayerSpawn()
        {
            if (GameObject.Find("PlayerSpawn"))
            {
                return;
            }
            GameObject playerSpawn;
            playerSpawn = EtrasResourceGrabbingFunctions.addPrefabFromResourcesByName("PlayerSpawn");
            playerSpawn.transform.SetParent(this.transform);
            playerSpawn.transform.localPosition = playerSpawnPosition;
            playerSpawn.GetComponent<PlayerSpawn>().teleportPlayerInitial();
        }

        private void OnDrawGizmos()
        {
            const float pointRadius = 0.1f;

            Gizmos.color = new Color(0f, 0.7f, 1f);
            Gizmos.DrawSphere(transform.position + playerSpawnPosition, pointRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + startConnectionPoint, pointRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + endConnectionPoint, pointRadius);
        }
    }
}