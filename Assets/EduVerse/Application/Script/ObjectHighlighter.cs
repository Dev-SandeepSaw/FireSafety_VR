using UnityEngine;

public class ObjectHighlighter : MonoBehaviour
{
    private Renderer objectRenderer;
    private Material originalMaterial;
    public Material highlightMaterial;
    public float highlightFrequency = 1.0f;

    private float timeSinceLastHighlight;
    private bool isHighlighted = false;

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material;
        }
        else
        {
            Debug.LogError("Object does not have a Renderer component!");
            enabled = false;
        }

        if (highlightMaterial == null)
        {
            Debug.LogError("Highlight material is not assigned in the inspector!");
            enabled = false;
        }
    }

    private void Update()
    {
        timeSinceLastHighlight += Time.deltaTime;

        if (timeSinceLastHighlight >= 1.0f / highlightFrequency)
        {
            ToggleHighlight();
            timeSinceLastHighlight = 0f;
        }
    }

    private void OnEnable()
    {
        if (!isHighlighted)
        {
            objectRenderer.material = originalMaterial;
        }
    }

    private void OnDisable()
    {
        objectRenderer.material = originalMaterial;
    }

    private void ToggleHighlight()
    {
        isHighlighted = !isHighlighted;

        if (isHighlighted)
        {
            objectRenderer.material = highlightMaterial;
        }
        else
        {
            objectRenderer.material = originalMaterial;
        }
    }
}

