using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float distance, height, smoothSpeed;

    private void Update()
    {
        // Assuming you have a reference to the target object and the camera
        float targetObjectSize = player.GetComponent<Collider>().bounds.size.magnitude;
        float desiredDistance = targetObjectSize * distance; // Adjust the multiplier as needed
        float desiredHeight = targetObjectSize * height; // Adjust the multiplier as needed

        Vector3 targetObjectPosition = player.transform.position;
        Vector3 cameraTargetPosition = targetObjectPosition - transform.forward * desiredDistance;
        cameraTargetPosition.y = targetObjectPosition.y + desiredHeight;

        // Set the camera position using smooth damping
        transform.position = Vector3.Lerp(transform.position, cameraTargetPosition, smoothSpeed * Time.deltaTime);
    }
}
