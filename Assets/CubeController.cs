using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

public class CubeController : MonoBehaviour
{
    // Rule parameters
    public int[] survivalRange = { 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26 };
    public int[] birthRange = { 13, 14, 17, 18, 19 };
    public int updateInterval = 1; // Seconds between updates

    // Cube grid properties
    public int cubesPerAxis = 50;
    public int subCubeSize = 1;

    // Efficiency
    private Dictionary<Vector3Int, SubCube> subCubeDict;
    private bool subCubesGenerated = false; 
    // Neighbor Counting Optimization
    private Vector3Int[] neighborOffsets = new Vector3Int[]
    {
        // Each Vector3Int represents a relative offset from the center cell 

        // Layer -1 (the layer "below" the central cube)
        new Vector3Int(-1, -1, -1), new Vector3Int(0, -1, -1), new Vector3Int(1, -1, -1),
        new Vector3Int(-1, 0, -1),  new Vector3Int(0, 0, -1),  new Vector3Int(1, 0, -1),
        new Vector3Int(-1, 1, -1),  new Vector3Int(0, 1, -1),  new Vector3Int(1, 1, -1),

        // Layer 0 (the same layer as the central cube)
        new Vector3Int(-1, -1, 0),  new Vector3Int(0, -1, 0),  new Vector3Int(1, -1, 0),
        new Vector3Int(-1, 0, 0), /* Skip (0, 0, 0) - that's the center cell itself */ new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 1, 0),   new Vector3Int(0, 1, 0),   new Vector3Int(1, 1, 0),

        // Layer +1 (the layer "above" the central cube)
        new Vector3Int(-1, -1, 1),  new Vector3Int(0, -1, 1),  new Vector3Int(1, -1, 1),
        new Vector3Int(-1, 0, 1),   new Vector3Int(0, 0, 1),   new Vector3Int(1, 0, 1),
        new Vector3Int(-1, 1, 1),   new Vector3Int(0, 1, 1),   new Vector3Int(1, 1, 1)
    };

    void Start()
    {
        subCubeDict = new Dictionary<Vector3Int, SubCube>();

        if (!subCubesGenerated)
        {
            CreateSubCubes();
            subCubesGenerated = true;
        }

         StartCoroutine(UpdateGrid());
    }

    void CreateSubCubes()
    {
        for (int x = 0; x < cubesPerAxis; x++)
        {
            for (int y = 0; y < cubesPerAxis; y++)
            {
                for (int z = 0; z < cubesPerAxis; z++)
                {
                    Vector3 position = transform.position + new Vector3(x * subCubeSize, y * subCubeSize, z * subCubeSize);
                    GameObject subCubeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    subCubeObj.transform.position = position; 
                    subCubeObj.transform.localScale = new Vector3(subCubeSize, subCubeSize, subCubeSize); 
                    subCubeObj.transform.parent = transform; // Make the sub-cube a child
                    SubCube subCube = subCubeObj.AddComponent<SubCube>();
                    subCube.isAlive = true; 
                    subCubeDict.Add(new Vector3Int(x, y, z), subCube); 
                }
            }
        }
    }

    IEnumerator UpdateGrid()
    {
        while (true)
        {
            UpdateSubCubeStates();
            yield return new WaitForSeconds(updateInterval);
        }
    }

    void UpdateSubCubeStates()
    {
        // Note: You might implement selective updates instead of updating all sub-cubes

        foreach (var kvp in subCubeDict)
        {
            Vector3Int subCubePos = kvp.Key;
            SubCube subCube = kvp.Value;
            ApplyRules(subCube, subCubePos);
        }
    }

    void ApplyRules(SubCube subCube, Vector3Int subCubePos)
    {
        int liveNeighbors = GetLiveNeighborCount(subCube, subCubePos);

        if (subCube.isAlive)
        {
            subCube.nextIsAlive = survivalRange.Any(x => x == liveNeighbors); 
        }
        else
        {
            // when in proximity of ball it increases
            subCube.nextIsAlive = birthRange.Any(x => x == liveNeighbors);
        }
    }


    int GetLiveNeighborCount(SubCube subCube, Vector3Int subCubePos)
    {
        int liveNeighbors = 0;
        foreach (Vector3Int offset in neighborOffsets)
        {
            Vector3Int neighborPos = subCubePos + offset;

            // Bounds checking: Ensure neighbor position is within the cube dimensions
            if (neighborPos.x >= 0 && neighborPos.x < cubesPerAxis &&
                neighborPos.y >= 0 && neighborPos.y < cubesPerAxis &&
                neighborPos.z >= 0 && neighborPos.z < cubesPerAxis)
            {
                // If valid neighbor, check if it's in the dictionary and alive
                if (subCubeDict.ContainsKey(neighborPos) && subCubeDict[neighborPos].isAlive)
                {
                    liveNeighbors++;
                }
            } 
            // else: neighbor is outside of the cube boundaries - we do nothing
        }

        return liveNeighbors;
    }
}
