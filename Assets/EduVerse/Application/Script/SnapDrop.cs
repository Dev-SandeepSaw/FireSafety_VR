using UnityEngine;

public class SnapDrop : MonoBehaviour
{
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object has the desired tag (you can use any other condition here)
        if (collision.gameObject.CompareTag("SnapDrop"))
        {
            // Get the transform (position and rotation) of the colliding object
            Transform otherTransform = collision.gameObject.transform;

            // Set the position and rotation of this object to match the colliding object
            rb.position = otherTransform.position;
            rb.rotation = otherTransform.rotation;
        }
    }
}
