using UnityEngine;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCollisionHandler : MonoBehaviour
{
    [System.Serializable]
    public class TaggedObjectEvent : UnityEvent<GameObject> { }

    [Header("Tagged Object Events")]

    //private string targetTag = "Player"; // Set the tag you want to detect collisions with in the Inspector.
    public TaggedObjectEvent onObjectEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag== "Player")
        {
            Debug.Log("Player Enter");
            onObjectEnter.Invoke(other.gameObject);

        }
    }
}
