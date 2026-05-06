using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    #region SETTINGS
    public float speed = 2f;
    public Transform[] points;
    #endregion

    #region PRIVATE VARIABLES
    private int i;
    private Rigidbody2D rb;
    #endregion

    #region START
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.position = points[0].position;
    }
    #endregion

    #region FIXED UPDATE
    void FixedUpdate()
    {
        if (Vector2.Distance(rb.position, points[i].position) < 0.1f)
        {
            i = (i + 1) % points.Length;
        }

        Vector2 newPos = Vector2.MoveTowards(
            rb.position,
            points[i].position,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPos);
    }
    #endregion
}
