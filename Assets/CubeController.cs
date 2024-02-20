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
    private int iteration = 0;

    // Cube grid properties
    public int cubesPerAxis = 50;
    public int subCubeSize = 1;

    // Efficiency
    private Dictionary<Vector3Int, SubCube> subCubeDict;
    private SubCube[,,] subCubes;

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
        subCubes = new SubCube[cubesPerAxis, cubesPerAxis, cubesPerAxis];

        if (!subCubesGenerated)
        {
            IterateOverSubCubes(CreateSubCubeRule);
            subCubesGenerated = true;
        }
         StartCoroutine(UpdateGrid());
    }

    void IterateOverSubCubes(ApplyRulesDelegate ApplyRules)
    {
        // Note: You might implement selective updates instead of updating all sub-cubes
        for (int x = 0; x < cubesPerAxis; x++)
            for (int y = 0; y < cubesPerAxis; y++)
                for (int z = 0; z < cubesPerAxis; z++)
                    ApplyRules(x, y, z);
    }

    void CreateSubCubeRule(int x,int y,int z)
    {
        Vector3 position = transform.position + new Vector3(x * subCubeSize, y * subCubeSize, z * subCubeSize);
        GameObject subCubeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        subCubeObj.transform.position = position;
        subCubeObj.transform.localScale = new Vector3(subCubeSize, subCubeSize, subCubeSize);
        subCubeObj.transform.parent = transform; // Make the sub-cube a child
        SubCube subCube = subCubeObj.AddComponent<SubCube>();
        subCube.isAlive = true;
        subCubes[x, y, z] = subCube;
    }

    IEnumerator UpdateGrid()
    {
        while (true)
        {
            IterateOverSubCubes(ThreeDCellularAutomataRules);
            Debug.Log(iteration++);
            yield return new WaitForSeconds(updateInterval);
        }
    }


    void ThreeDCellularAutomataRules(int x, int y, int z)
    {
        SubCube subCube = subCubes[x, y, z];
        int liveNeighbors = GetLiveNeighborCount(x, y ,z);

        if (subCube.isAlive)
        {
            subCube.nextIsAlive = survivalRange.Any(x => x == liveNeighbors); 
        }
        else
        {
            subCube.nextIsAlive = birthRange.Any(x => x == liveNeighbors);
        }
    }


    int GetLiveNeighborCount(int x, int y, int z)
    {
        int liveNeighbors = 0;
        Vector3Int subCubePos = new Vector3Int(x, y, z);

        foreach (Vector3Int offset in neighborOffsets)
        {
            Vector3Int neighborPos = subCubePos + offset;

            // Bounds checking: Ensure neighbor position is within the cube dimensions
            if (neighborPos.x >= 0 && neighborPos.x < cubesPerAxis &&
                neighborPos.y >= 0 && neighborPos.y < cubesPerAxis &&
                neighborPos.z >= 0 && neighborPos.z < cubesPerAxis)
            {
                // If valid neighbor, check if it's in the dictionary and alive
                if (subCubes[neighborPos.x, neighborPos.y, neighborPos.z].isAlive)
                {
                    liveNeighbors++;
                }
            } 
            // else: neighbor is outside of the cube boundaries - we do nothing
        }

        return liveNeighbors;
    }
    public delegate void ApplyRulesDelegate(int x, int y, int z);
}
