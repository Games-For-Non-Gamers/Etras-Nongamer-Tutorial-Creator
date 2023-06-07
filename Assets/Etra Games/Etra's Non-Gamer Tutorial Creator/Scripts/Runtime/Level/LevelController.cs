using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Etra.NonGamerTutorialCreator.Level
{
    public class LevelController : MonoBehaviour
    {
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
        /// 


        public void ResetAllChunksPositions()
        {
            destroyBridgesImmediate();
            chooseBridgeOption();
        }

        void chooseBridgeOption()
        {
            switch (bridgeOptions)
            {
                case BridgeOptions.NoBridges:
                    placeChunks(chunks);
                    break;

                case BridgeOptions.FlatBridges:
                    List<LevelChunkObject> chunksPlusBridges = addBridges(chunks, flatBridgeName);
                    placeChunks(chunksPlusBridges);
                    break;

                case BridgeOptions.AngledBridges:
                    List<LevelChunkObject> chunksPlusBridges1 = addBridges(chunks, angledBridgeName);
                    placeChunks(chunksPlusBridges1);
                    break;
            }
        }

        void placeChunks(List<LevelChunkObject> chunkList)
        {
            Vector3 offset = Vector3.zero;
            foreach (var chunk in chunkList)
            {
                offset -= chunk.startConnectionPoint;
                chunk.transform.localPosition = offset;
                offset += chunk.endConnectionPoint;
            }
        }

        List<LevelChunkObject> addBridges(List<LevelChunkObject> initialChunks, string bridgeName)
        {
            List<LevelChunkObject> returnedChunksPlusBridges = new List<LevelChunkObject>();

            for (int i = 0; i < initialChunks.Count; i++)
            {
                GameObject loadedBridge = Resources.Load<GameObject>(bridgeName);

                returnedChunksPlusBridges.Add(initialChunks[i]); // add initial chunk
                if (i != chunks.Count - 1)
                {
                    GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(loadedBridge, this.transform);
                    prefab.name = bridgeName;
                    returnedChunksPlusBridges.Add(prefab.GetComponent<LevelChunkObject>());
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


    }
}