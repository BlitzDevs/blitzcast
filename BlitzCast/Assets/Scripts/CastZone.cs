using UnityEngine;

public class CastZone : MonoBehaviour {

    [SerializeField] private GameObject targetObject;
    [SerializeField] private Transform castingSlot;

    public GameObject GetTargetObject()
    {
        return targetObject;
    }

    public Transform GetCastingSlot()
    {
        return castingSlot;
    }
}
