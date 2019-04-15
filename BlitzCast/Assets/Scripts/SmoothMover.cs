using UnityEngine;

public class SmoothMover : MonoBehaviour
{
    public bool useLocalPosition;

    private Vector3 targetPosition = Vector3.zero;
    // smaller smoothTime is faster
    private float smoothTime = 0.05f;
    // modified by SmoothDamp
    private Vector3 velocity = Vector3.zero;


    public void SetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void Update()
    {
        if (useLocalPosition)
        {
            transform.localPosition = Vector3Int.RoundToInt(Vector3.SmoothDamp(
                transform.localPosition, targetPosition, ref velocity, smoothTime));
        }
        else
        {
            transform.position = Vector3Int.RoundToInt(Vector3.SmoothDamp(
                transform.position, targetPosition, ref velocity, smoothTime));
        }
    }
}
