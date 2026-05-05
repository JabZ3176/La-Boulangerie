using UnityEngine;

public class BackgroundFollowCamera : MonoBehaviour
{
    public Transform cameraTransform;
    public float zPosition = 10f;

    void LateUpdate()
    {
        Vector3 camPos = cameraTransform.position;
        transform.position = new Vector3(camPos.x, camPos.y, zPosition);
    }
}