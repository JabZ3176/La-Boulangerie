using UnityEngine;

[DefaultExecutionOrder(10000)]
public class BackgroundFollower : MonoBehaviour
{
    private enum ReferenceMode
    {
        MainCamera,
        CustomTarget
    }

    #region REFERENCES
    [Header("Reference")]
    [Tooltip("Use MainCamera for the most stable background. This stops the background from reacting to the player's landing bounce.")]
    [SerializeField] private ReferenceMode referenceMode = ReferenceMode.MainCamera;

    [Tooltip("Only used when Reference Mode is Custom Target.")]
    [SerializeField] private Transform customTarget;
    #endregion

    #region FOLLOW SETTINGS
    [Header("Follow Settings")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = false;

    [Tooltip("1 = move exactly with the reference. 0.5 = parallax. 0 = stay still on that axis.")]
    [SerializeField] private Vector2 followMultiplier = new Vector2(1f, 0f);

    [SerializeField] private bool keepOriginalZ = true;
    #endregion

    #region OPTIONS
    [Header("Stability Options")]
    [Tooltip("Recommended ON. Recaptures the start positions on the first LateUpdate so the camera/player has finished its initial setup.")]
    [SerializeField] private bool captureOnFirstLateUpdate = true;

    [Tooltip("Only use this for pixel art shimmer. Leave OFF for normal backgrounds.")]
    [SerializeField] private bool snapToPixelGrid = false;
    [SerializeField] private float pixelsPerUnit = 16f;
    #endregion

    #region PRIVATE VARIABLES
    private Transform referenceTarget;
    private Vector3 startingBackgroundPosition;
    private Vector3 startingReferencePosition;
    private bool captured;
    private bool capturedInLateUpdate;
    #endregion

    #region UNITY
    private void Start()
    {
        if (!captureOnFirstLateUpdate)
            CaptureStartingPositions();
    }

    private void LateUpdate()
    {
        if (captureOnFirstLateUpdate && !capturedInLateUpdate)
        {
            CaptureStartingPositions();
            capturedInLateUpdate = true;
        }

        FollowReferenceExactly();
    }

    private void OnValidate()
    {
        if (pixelsPerUnit <= 0f)
            pixelsPerUnit = 1f;
    }
    #endregion

    #region FOLLOW
    private void CaptureStartingPositions()
    {
        FindReferenceTarget();

        startingBackgroundPosition = transform.position;
        startingReferencePosition = referenceTarget != null ? referenceTarget.position : Vector3.zero;
        captured = true;
    }

    private void FollowReferenceExactly()
    {
        FindReferenceTarget();
        if (referenceTarget == null) return;

        if (!captured)
            CaptureStartingPositions();

        Vector3 referenceDelta = referenceTarget.position - startingReferencePosition;
        Vector3 nextPosition = startingBackgroundPosition;

        if (followX)
            nextPosition.x += referenceDelta.x * followMultiplier.x;

        if (followY)
            nextPosition.y += referenceDelta.y * followMultiplier.y;

        if (!keepOriginalZ)
            nextPosition.z = referenceTarget.position.z;

        if (snapToPixelGrid)
            nextPosition = SnapToPixels(nextPosition);

        transform.position = nextPosition;
    }

    private void FindReferenceTarget()
    {
        if (referenceMode == ReferenceMode.CustomTarget)
        {
            referenceTarget = customTarget;
            return;
        }

        if (Camera.main != null)
            referenceTarget = Camera.main.transform;
    }

    private Vector3 SnapToPixels(Vector3 position)
    {
        position.x = Mathf.Round(position.x * pixelsPerUnit) / pixelsPerUnit;
        position.y = Mathf.Round(position.y * pixelsPerUnit) / pixelsPerUnit;
        return position;
    }

    public void RecenterFromCurrentPositions()
    {
        CaptureStartingPositions();
    }
    #endregion
}
