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

    private int iteration = 1;


    // Cube grid properties
    private int cubesPerAxis = 70;
    public int subCubeSize = 1;
    private int limit = 15;

    // Efficiency
    private Dictionary<Vector3Int, SubCube> subCubeDict;
    private SubCube[,,] subCubes;

    // Neighbor Counting Optimization
    private Vector3Int[] neighborOffsets = GenerateNeighborOffsets();

    // Initial state
    private bool[,,] initialState;

    void Start()
    {
        initialState = new bool[cubesPerAxis, cubesPerAxis, cubesPerAxis];
        subCubeDict = new Dictionary<Vector3Int, SubCube>();
        subCubes = new SubCube[cubesPerAxis, cubesPerAxis, cubesPerAxis];

        IterateOverSubCubes(GenerateInitialState);
        IterateOverSubCubes(CreateSubCubeRule);
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
        while (iteration <= limit)
        {
            IterateOverSubCubes(ThreeDCellularAutomataRules);
            Debug.Log(iteration++);
            yield return new WaitForSeconds(updateInterval);
        }
        IterateOverSubCubes(EnableRendering);
    }

    void EnableRendering(int x, int y, int z)
    {
        if (subCubes[x, y, z].isAlive && GetLiveNeighborCountNoDiagonal(x,y,z)!=6)
            subCubes[x, y, z].setMeshRenderer(true);
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


int GetLiveNeighborCountNoDiagonal(int x, int y, int z) 
{
    int count = 0;

    // Check neighbors in the up, down, left, right, front, and back directions
    int[] dx = { -1, 1, 0, 0, 0, 0 };
    int[] dy = { 0, 0, -1, 1, 0, 0 };
    int[] dz = { 0, 0, 0, 0, -1, 1 };

    for (int direction = 0; direction < 6; direction++) 
    {
        int newX = x + dx[direction];
        int newY = y + dy[direction];
        int newZ = z + dz[direction];

        // Ensure that the neighboring cell is within the valid bounds
        if (newX >= 0 && newX < cubesPerAxis && newY >= 0 && newY < cubesPerAxis && newZ >= 0 && newZ < cubesPerAxis) 
        {
            // Increment the count if the neighboring cell is alive
            // Adjust the condition based on your specific requirements
            if (subCubes[newX, newY, newZ].isAlive)
                count++;
        }
    }

    Debug.Log(count);
    return count;
}

    // Setting up

    void GenerateInitialState(int x, int y, int z)
    {
        initialState[x, y, z] = true;
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
