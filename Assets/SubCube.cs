using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCube : MonoBehaviour
{
    public bool isAlive = true;
    public bool nextIsAlive = true;
    private MeshRenderer meshRenderer;
    public Collider myCollider; // Renamed for clarity
    public List<Vector3Int> neighborCoordinates = new List<Vector3Int>(); // Store neighbor coordinates

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        myCollider = GetComponent<Collider>();
        UpdateAppearance(); // Apply initial state
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
}