using UnityEngine;

public class BackgroundFollowCamera : MonoBehaviour
{
    #region REFERENCES
    public Transform cameraTransform;
    #endregion

    #region SETTINGS
    public float zPosition = 10f;
    public float parallaxStrength = 0.05f;
    #endregion

    #region PRIVATE VARIABLES
    private Vector3 lastCameraPosition;
    #endregion

    #region START
    void Start()
    {
        if (cameraTransform != null)
            lastCameraPosition = cameraTransform.position;
    }
    #endregion

    #region LATE UPDATE
    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // calculate how much the camera moved this frame
        Vector3 cameraDelta = cameraTransform.position - lastCameraPosition;

        // move the background by a fraction of that delta
        transform.position += new Vector3(
            cameraDelta.x * parallaxStrength,
            cameraDelta.y * parallaxStrength,
            0f
        );

        lastCameraPosition = cameraTransform.position;
    }
    #endregion
}