using System.Collections.Generic;
using UnityEngine;

namespace Etra.NonGamerTutorialCreator.Level
{
    public class LevelController : MonoBehaviour
    {
        public bool isPreview;

        public List<LevelChunkObject> chunks = new List<LevelChunkObject>();

        public bool ooooga;

        /// <summary>Forces all chunks to connect properly</summary>
        public void ResetAllChunksPositions()
        {
            Vector3 offset = Vector3.zero;
            foreach (var chunk in chunks)
            {
                offset -= chunk.startConnectionPoint;
                chunk.transform.localPosition = offset;
                offset += chunk.endConnectionPoint;
            }
        }
    }
}