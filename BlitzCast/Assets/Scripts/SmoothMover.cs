using UnityEngine;


/// <summary>
/// Smoothly move towards the target position.
/// <para>
/// Attach SmoothMover onto the GameObject to be moved.
/// Access this component and use SetPosition(targetPosition).
/// Disable component when object no longer needs to be moved.
/// </para>
/// </summary>
public class SmoothMover : MonoBehaviour
{

    // local position is in relation to the parent
    public bool useLocalPosition;

    // where we want to end up
    private Vector3 targetPosition = Vector3.zero;
    // smaller smoothTime is faster
    private float smoothTime = 0.05f;
    // velocity is value modified by SmoothDamp
    private Vector3 velocity = Vector3.zero;


    /// <summary>
    /// Set the target position.
    /// </summary>
    /// <param name="targetPosition">Target position.</param>
    public void SetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    /// <summary>
    /// Called by Unity every frame.
    /// Move towards position.
    /// </summary>
    private void Update()
    {
        if (useLocalPosition)
        {
            // use local position
            transform.localPosition = Vector3Int.RoundToInt(Vector3.SmoothDamp(
                transform.localPosition, targetPosition, ref velocity, smoothTime));
        }
        else
        {
            // use actual position
            transform.position = Vector3Int.RoundToInt(Vector3.SmoothDamp(
                transform.position, targetPosition, ref velocity, smoothTime));
        }
    }
}
