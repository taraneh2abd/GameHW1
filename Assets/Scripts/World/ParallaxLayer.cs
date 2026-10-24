using UnityEngine;

/// Infinite horizontal scroller: lays out `segmentCount` copies of `sprite`
/// side by side and recycles whichever one scrolls fully past the camera's
/// left edge back onto the right end of the strip.
public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private float speedScale = 1f;
    [SerializeField] private int sortingOrder;
    [SerializeField] private int segmentCount = 3;

    private Transform[] segments;
    private float segmentWidth;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        BuildSegments();
    }

    private void BuildSegments()
    {
        if (sprite == null)
        {
            Debug.LogError($"{name}: ParallaxLayer has no sprite assigned.");
            enabled = false;
            return;
        }

        segmentWidth = sprite.bounds.size.x;
        segments = new Transform[segmentCount];

        for (int i = 0; i < segmentCount; i++)
        {
            var segment = new GameObject($"Segment_{i}");
            segment.transform.SetParent(transform, false);
            segment.transform.localPosition = new Vector3(i * segmentWidth, 0f, 0f);

            var spriteRenderer = segment.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = sortingOrder;

            segments[i] = segment.transform;
        }
    }

    private void Update()
    {
        if (segments == null || mainCamera == null)
        {
            return;
        }

        float delta = GameSpeed.Current * speedScale * Time.deltaTime;
        float leftEdgeLocalX = GetCameraLeftEdgeLocalX();

        foreach (var segment in segments)
        {
            segment.localPosition += Vector3.left * delta;

            if (segment.localPosition.x + segmentWidth * 0.5f < leftEdgeLocalX)
            {
                float rightmostLocalX = GetRightmostLocalX();
                segment.localPosition = new Vector3(rightmostLocalX + segmentWidth, segment.localPosition.y, segment.localPosition.z);
            }
        }
    }

    private float GetCameraLeftEdgeLocalX()
    {
        float halfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float cameraLeftWorldX = mainCamera.transform.position.x - halfWidth;
        return transform.InverseTransformPoint(new Vector3(cameraLeftWorldX, 0f, 0f)).x;
    }

    private float GetRightmostLocalX()
    {
        float max = float.NegativeInfinity;
        foreach (var segment in segments)
        {
            if (segment.localPosition.x > max)
            {
                max = segment.localPosition.x;
            }
        }
        return max;
    }
}
