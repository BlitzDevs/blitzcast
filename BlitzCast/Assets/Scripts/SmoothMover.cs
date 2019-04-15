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

    public void InstantMove()
    {
        //moves to target location instantly
        if (useLocalPosition)
        {
            transform.localPosition = Vector3Int.RoundToInt(targetPosition);
        }
        else
        {
            transform.position = Vector3Int.RoundToInt(targetPosition);
        }
    }

    private void Update()
    {
        //uses Vector3.SmoothDamp so cards move smoothly
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
