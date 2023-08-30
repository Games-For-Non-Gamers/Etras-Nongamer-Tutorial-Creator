using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TvStaticRespawn : MonoBehaviour
{
    public Image image;

    public Sprite[] frames;
    public float changeRate = 0.05f;

    private void Start()
    {
        StartCoroutine(StaticAnimation());
    }

    private void OnEnable()
    {
        StartCoroutine(StaticAnimation());
    }

    private void OnDisable()
    {
        num = 0;
        StopAllCoroutines();
    }

    int num = 0;
    IEnumerator StaticAnimation()
    {
        image.sprite = frames[num];
        yield return new WaitForSeconds(changeRate);
        num++;
        if (frames.Length <= num)
        {
            num = 0;
        }
        StartCoroutine(StaticAnimation());

    }

}
