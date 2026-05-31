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
        if (cameraTransform == null) return;

        transform.position = new Vector3(
            cameraTransform.position.x,
            cameraTransform.position.y,
            zPosition
        );
    }
    #endregion
}