using UnityEngine;

namespace Etra.NonGamerTutorialCreator.Level
{
    public class PlayerSpawn : MonoBehaviour
    {
        public void teleportPlayerInitial()
        {
            if (GameObject.FindGameObjectWithTag("Player"))
            {
                GameObject.FindGameObjectWithTag("Player").transform.position = this.transform.position;
            }
        }
        void Awake()
        {
            GameObject.FindGameObjectWithTag("Player").transform.position = this.transform.position;
        }

    }
}
