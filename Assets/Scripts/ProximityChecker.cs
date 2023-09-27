using UnityEngine;

public class ProximityChecker : MonoBehaviour
{
    public string sceneToLoad;
    public int passingDirection = 1; // 1 when passing the portal to load the zone, -1 when going back to unload

    public void ToggleDirection()
    {
        passingDirection *= -1;
    }
}