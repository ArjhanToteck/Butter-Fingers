using UnityEngine;

public class FloorRotation : MonoBehaviour
{
    public float rotationSpeed = 250f;
    public Rigidbody2D rb;

    // Update is called once per frame
    void FixedUpdate()
    {
        // controls
        if (Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow))
        {
          rb.SetRotation(rb.rotation + (rotationSpeed * Time.deltaTime));
        }

        if (Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow))
        {
            rb.SetRotation(rb.rotation - (rotationSpeed * Time.deltaTime));
        }
    }
}
