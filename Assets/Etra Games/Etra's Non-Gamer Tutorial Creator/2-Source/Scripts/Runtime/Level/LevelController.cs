using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Etra.NonGamerTutorialCreator.Level
{
    public class LevelController : MonoBehaviour
    {
        public GameObject flatBridge;
        public GameObject angledBridge;



        string flatBridgeName = "FlatBridgeChunk";
        string angledBridgeName = "AngledBridgeChunk";
        public enum BridgeOptions
        {
            NoBridges,
            FlatBridges,
            AngledBridges
        }

        [HideInInspector]
        public BridgeOptions bridgeOptions = BridgeOptions.AngledBridges;

        [HideInInspector]
        public bool isPreview;
        [HideInInspector]
        public List<LevelChunkObject> chunks = new List<LevelChunkObject>();


        /// <summary>Forces all chunks to connect properly</summary>
        public void ResetAllChunksPositions(List<LevelChunkObject> passedChunks)
        {
            chunks = passedChunks;
            ResetAllChunksPositions();
        }

        public void ResetAllChunksPositions()
        {
            destroyBridgesImmediate();
            chooseBridgeOption();
            resetPlayerSpawn();
        }

        private void Reset()
        {
            BridgeReferenceSetup();
        }

        private void OnValidate()
        {
            BridgeReferenceSetup();

        }

        void BridgeReferenceSetup()
        {
            angledBridge = Resources.Load<GameObject>(angledBridgeName);
            flatBridge = Resources.Load<GameObject>(flatBridgeName);
        }

        void chooseBridgeOption()
        {
            switch (bridgeOptions)
            {
                case BridgeOptions.NoBridges:
                    List<LevelChunkObject> chunksNoBridges = maybeBridge(chunks, flatBridgeName, false);
                    placeChunks(chunksNoBridges);
                    break;

                case BridgeOptions.FlatBridges:
                    List<LevelChunkObject> chunksPlusBridges = maybeBridge(chunks, flatBridgeName, true);
                    placeChunks(chunksPlusBridges);
                    break;

                case BridgeOptions.AngledBridges:
                    List<LevelChunkObject> chunksPlusBridges1 = maybeBridge(chunks, angledBridgeName, true);
                    placeChunks(chunksPlusBridges1);
                    break;
            }
        }

        void placeChunks(List<LevelChunkObject> chunkList)
        {
            List<LevelChunkObject> flippedList = chunkList;
            flippedList.Reverse();
            Vector3 offset = Vector3.zero;
            foreach (var chunk in flippedList)
            {
                offset -= chunk.startConnectionPoint;
                chunk.transform.localPosition = offset;
                offset += chunk.endConnectionPoint;
            }
        }

        List<LevelChunkObject> maybeBridge(List<LevelChunkObject> initialChunks, string bridgeName, bool addBridges)
        {
            List<LevelChunkObject> returnedChunksPlusBridges = new List<LevelChunkObject>();

            for (int i = 0; i < initialChunks.Count; i++)
            {
                GameObject loadedBridge = null;


                if (addBridges)
                {
                    if (bridgeName == flatBridgeName)
                    {
                        loadedBridge = flatBridge;
                    }

                    if (bridgeName == angledBridgeName)
                    {
                        loadedBridge = angledBridge;
                    }



                }

                GameObject bridgesRoot;
                if (GameObject.Find("Bridges"))
                {
                    bridgesRoot = GameObject.Find("Bridges");
                }
                else
                {
                    bridgesRoot = new GameObject("Bridges");
                }
                bridgesRoot.transform.parent = this.transform;
                bridgesRoot.transform.SetAsLastSibling();



                returnedChunksPlusBridges.Add(initialChunks[i]); // add initial chunk

                if (addBridges)
                {
                    if (i != chunks.Count - 1)
                    {
                    #if UNITY_EDITOR
                        GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(loadedBridge, this.transform);

#else
 GameObject prefab = Instantiate(loadedBridge, this.transform);
#endif


                        prefab.name = bridgeName;
                        prefab.transform.parent = bridgesRoot.transform;
                        returnedChunksPlusBridges.Add(prefab.GetComponent<LevelChunkObject>());
                    }
                }

            }

            return returnedChunksPlusBridges;

        }

        void destroyBridgesImmediate()
        {
            BridgeIdentifier[] myItems = FindObjectsOfType(typeof(BridgeIdentifier)) as BridgeIdentifier[];
            foreach (BridgeIdentifier item in myItems)
            {
                DestroyImmediate(item.gameObject);
            }
        }

        void resetPlayerSpawn()
        {
            chunks[chunks.Count - 1].deletePlayerSpawn();
            chunks[chunks.Count - 1].makePlayerSpawn();
        }

    }
}