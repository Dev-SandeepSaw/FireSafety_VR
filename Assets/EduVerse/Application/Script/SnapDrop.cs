using Autohand;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class SnapDrop : MonoBehaviour
{
    public Transform snapPoint; // The position and rotation where the object should snap.
    public GameObject objectToSnap; // The object to snap.
    public float snapSpeed = 2.0f; // Adjust this value to control the snap animation speed.
    public UnityEvent onSnap;

    private bool isSnapping = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody objectRigidbody; // Reference to the object's Rigidbody.
    private bool objectPlaced;


    private void Start()
    {
        snapPoint = this.transform;
        objectPlaced = false;
        if (objectToSnap != null)
        {
            // Store the initial position and rotation of the object.
            initialPosition = objectToSnap.transform.position;
            initialRotation = objectToSnap.transform.rotation;

            // Get the Rigidbody component of the object.
            objectRigidbody = objectToSnap.GetComponent<Rigidbody>();
        }

    }
    private void Update()
    {
        if (objectToSnap.GetComponent<Grabbable>().isGrabbable)
        {

        }
    }
    void OnTriggerEnter(Collider other)
    {
        

        if (!isSnapping && objectPlaced )
        {
            StartCoroutine(SnapObject());
        }
    }

    public void TriggerPressedObject()
    {
        objectPlaced = true;

    }

    public void TriggerRelesedObject()
    {
        objectPlaced = false;
    }
    IEnumerator SnapObject()
    {
        isSnapping = true;

        objectToSnap.gameObject.GetComponent<Autohand.Grabbable>().ForceHandsRelease();
        objectToSnap.GetComponent<Autohand.Grabbable>().enabled = false;
        objectToSnap.GetComponent<Rigidbody>().isKinematic = true;
        if (objectRigidbody != null)
        {
            objectRigidbody.isKinematic = true;
        }

        float elapsedTime = 0f;
        Vector3 initialObjectPosition = objectToSnap.transform.position;
        Quaternion initialObjectRotation = objectToSnap.transform.rotation;

        while (elapsedTime < 1f)
        {
            onSnap.Invoke();
            // Interpolate the position and rotation smoothly.
            objectToSnap.transform.position = Vector3.Lerp(initialObjectPosition, snapPoint.position, elapsedTime * snapSpeed);
            objectToSnap.transform.rotation = Quaternion.Lerp(initialObjectRotation, snapPoint.rotation, elapsedTime * snapSpeed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure that the object is precisely snapped to the snap point.
        objectToSnap.transform.position = snapPoint.position;
        objectToSnap.transform.rotation = snapPoint.rotation;
        this.gameObject.SetActive(false);
        // Re-enable the object's Rigidbody if it has one.
        if (objectRigidbody != null)
        {
            objectRigidbody.isKinematic = false;
        }
        isSnapping = false;

    }
}
