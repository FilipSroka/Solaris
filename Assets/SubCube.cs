using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCube : MonoBehaviour
{
    public bool isAlive = true;
    public bool nextIsAlive = true;
    private MeshRenderer meshRenderer;
    public Vector3 position;
    public Collider myCollider; // Renamed for clarity
    public List<Vector3Int> neighborCoordinates = new List<Vector3Int>(); // Store neighbor coordinates

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        myCollider = GetComponent<Collider>();
        UpdateAppearance(); // Apply initial state
        meshRenderer.enabled = true;
    }

    private void Update() {
        isAlive = nextIsAlive;
        UpdateAppearance();
    }

    void UpdateAppearance()
    {
        meshRenderer.enabled = isAlive; // Show/hide the cube
        myCollider.enabled = isAlive;     // Enable/disable collisions
    }

    public void setMeshRenderer(bool b)
    {
        meshRenderer.enabled = b;
    }
}