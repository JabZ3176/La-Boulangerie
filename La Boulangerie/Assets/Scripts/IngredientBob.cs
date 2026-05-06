using UnityEngine;

public class IngredientBobRotate : MonoBehaviour
{
    #region BOB

    [Header("Bob")]
    public float bobHeight = 0.2f;
    public float bobSpeed = 2f;

    #endregion

    #region ROTATION

    [Header("Rotation")]
    public float rotationAmount = 15f;
    public float rotationSpeed = 2f;

    #endregion

    #region GLINT

    [Header("Glint")]
    public Transform glint;
    public float glintSpeed = 2f;
    public float glintMaxScale = 1f;

    #endregion

    #region PRIVATE VARIABLES

    private float startY;
    private float offset;
    private Vector3 glintStartScale;

    #endregion

    #region START

    void Start()
    {
        startY = transform.localPosition.y;
        offset = Random.Range(0f, Mathf.PI * 2f);

        if (glint != null)
        {
            glintStartScale = glint.localScale;
            glint.localScale = Vector3.zero;
        }
    }

    #endregion

    #region UPDATE

    void Update()
    {
        float wave = Mathf.Sin((Time.time * bobSpeed) + offset);

        transform.localPosition = new Vector3(
            transform.localPosition.x,
            startY + wave * bobHeight,
            transform.localPosition.z
        );

        transform.localRotation = Quaternion.Euler(
            0f,
            0f,
            wave * rotationAmount
        );

        if (glint != null)
        {
            float glintWave = Mathf.PingPong((Time.time * glintSpeed) + offset, 1f);
            float scale = Mathf.Sin(glintWave * Mathf.PI) * glintMaxScale;

            glint.localScale = glintStartScale * scale;
        }
    }

    #endregion
}