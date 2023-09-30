using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapshotItem : MonoBehaviour
{
    private SnapshotCamera snapshotCamera;
    public GameObject gameObjectToSnapshot;

    [ContextMenu("Screenshot")]
    void Screenshot()
    {
        snapshotCamera = SnapshotCamera.MakeSnapshotCamera(0);
        Texture2D snapshot = snapshotCamera.TakeObjectSnapshot(gameObjectToSnapshot);
        SnapshotCamera.SavePNG(snapshot);
        //Destroy(snapshotCamera);
    }
}