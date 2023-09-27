using UnityEngine;

public class Collectible : MonoBehaviour
{
    public bool rotate;
    public Vector3 rotationOnSpawn;    
    public Vector3 rotationSpeed;

    public string onCollectedNotif;

    private void Start()
    {
        if(rotate)
        {
            transform.rotation = Quaternion.Euler(rotationOnSpawn);
        }
    }

    private void Update()
    {
        if (rotate)
        {
            transform.Rotate(rotationSpeed.x * Time.deltaTime, rotationSpeed.y * Time.deltaTime, rotationSpeed.z * Time.deltaTime);
        }
    }
}
