using UnityEngine;

public class BackgroundFollowCamera : MonoBehaviour
{
    #region REFERENCES
    public Transform cameraTransform;
    #endregion

    #region SETTINGS
    public float zPosition = 10f;
    #endregion

    #region LATE UPDATE
    void LateUpdate()
    {
        Vector3 camPos = cameraTransform.position;
        transform.position = new Vector3(camPos.x, camPos.y, zPosition);
    }
    #endregion
}
