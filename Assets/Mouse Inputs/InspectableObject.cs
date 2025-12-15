using UnityEngine;

public class InspectableObject : MonoBehaviour
{
    [HideInInspector]
    public bool inspected = false;

    // Call this when the player inspects the object (e.g., clicks on it)
    public void Inspect()
    {
        inspected = true;

        // Optional: visual feedback
        Debug.Log($"{gameObject.name} inspected!");
    }
}
