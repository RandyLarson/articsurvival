using System.Collections;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public static float FramesPerSec { get; protected set; }

    [SerializeField] private float frequency = 0.5f;

    private void Start()
    {
        StartCoroutine(FPS());
    }

    private IEnumerator FPS()
    {
        for ( ; ; )
        {
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(frequency);

            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;

            FramesPerSec = frameCount / timeSpan;
        }
    }
}