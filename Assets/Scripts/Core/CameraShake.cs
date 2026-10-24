using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Coroutine running;
    private Vector3 restPosition;

    private void Awake()
    {
        restPosition = transform.localPosition;
    }

    public void Shake(float duration, float magnitude)
    {
        if (running != null)
        {
            StopCoroutine(running);
            transform.localPosition = restPosition;
        }
        running = StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            Vector2 offset = Random.insideUnitCircle * magnitude;
            transform.localPosition = restPosition + new Vector3(offset.x, offset.y, 0f);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        transform.localPosition = restPosition;
        running = null;
    }
}
