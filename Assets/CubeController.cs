using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

public class CubeController : MonoBehaviour
{
    // Rule parameters
    public int[] survivalRange = { 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26 };
    public int[] birthRange = { 13, 14, 17, 18, 19 };
    public int updateInterval = 0; // Seconds between updates
    private int iteration = 0;

    // Cube grid properties
    public int cubesPerAxis = 50;
    public int subCubeSize = 1;

    // Efficiency
    private Dictionary<Vector3Int, SubCube> subCubeDict;
    private SubCube[,,] subCubes;

    private bool subCubesGenerated = false; 

    // Neighbor Counting Optimization
    private Vector3Int[] neighborOffsets = GenerateNeighborOffsets();

    // Initial state
    private bool[,,] initialState;

    void Start()
    {
        subCubeDict = new Dictionary<Vector3Int, SubCube>();
        subCubes = new SubCube[cubesPerAxis, cubesPerAxis, cubesPerAxis];
        initialState = GenerateInitialState();

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
        subCube.isAlive = initialState[x, y, z]; 
        foreach (Vector3Int offset in neighborOffsets)
        {
            Vector3Int neighborPos = new Vector3Int(x, y, z) + offset;

            // Bounds check: Skip neighbors that would be outside the bounds
            if (neighborPos.x >= 0 && neighborPos.x < cubesPerAxis &&
                neighborPos.y >= 0 && neighborPos.y < cubesPerAxis &&
                neighborPos.z >= 0 && neighborPos.z < cubesPerAxis) 
            {
                subCube.neighborCoordinates.Add(neighborPos);
            }
        }
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
        int liveNeighbors = GetLiveNeighborCount(subCube);

        if (subCube.isAlive)
        {
            subCube.nextIsAlive = survivalRange.Any(x => x == liveNeighbors); 
        }
        else
        {
            subCube.nextIsAlive = birthRange.Any(x => x == liveNeighbors);
        }
    }


    int GetLiveNeighborCount(SubCube subCube) 
    {
        int liveNeighbors = 0;

        foreach (Vector3Int neighborCoord in subCube.neighborCoordinates)
        {
            if (subCubes[neighborCoord.x, neighborCoord.y, neighborCoord.z].isAlive)
            {
                liveNeighbors++;
            }
        }

        return liveNeighbors;
    }

    // Setting up

    private bool[,,] GenerateInitialState()
    {
        bool[,,] initialState = new bool[cubesPerAxis, cubesPerAxis, cubesPerAxis];

        for (int x = 0; x < cubesPerAxis; x++)
        {
            for (int y = 0; y < cubesPerAxis; y++)
            {
                for (int z = 0; z < cubesPerAxis; z++)
                {
                    // all true for now
                    initialState[x, y, z] = true;
                }
            }
        }

        return initialState;
    }

    private static Vector3Int[] GenerateNeighborOffsets()
    {
        List<Vector3Int> offsets = new List<Vector3Int>();

        for (int z = -1; z <= 1; z++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (x == 0 && y == 0 && z == 0) continue;  // Skip the center cell
                    offsets.Add(new Vector3Int(x, y, z));
                }
            }
        }

        return offsets.ToArray();
    }


    public delegate void ApplyRulesDelegate(int x, int y, int z);
}
