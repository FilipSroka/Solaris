using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGenerator : MonoBehaviour
{
    private int cubeSize = 20;
    private int cubeCount = 5;
    private int padding = 1; // Number of additional cubes at the beginning and end
    private GameObject[,,] cubes;
    private Dictionary<Color, int> neighborCountByColor = new Dictionary<Color, int>();
    private int iteration = 0;
    private GameObject generatedCubesContainer;
    private Vector3 offset;
    private GameObject[,,] nextGeneration;

    void Start()
    {   
        offset = new Vector3(-cubeSize * (cubeCount + 2 * padding) / 2f + cubeSize / 2f,
                                      -cubeSize * (cubeCount + 2 * padding) / 2f + cubeSize / 2f,
                                      -cubeSize * (cubeCount + 2 * padding) / 2f + cubeSize / 2f);
       
        generatedCubesContainer = new GameObject("GeneratedCubesContainer");

        cubes = new GameObject[cubeCount + 2 * padding, cubeCount + 2 * padding, cubeCount + 2 * padding];

        IterateOverCubes(GenerateCube);
        IterateOverCubes(ChangeCubeColors);
        InvokeRepeating("Dummy", 2f, 2f);
    }

    void Dummy() 
    {
        IterateOverCubes(Rules);
    }

    void IterateOverCubes(ApplyRulesDelegate ApplyRules)
    {
        // Create a new array to store the next state of the cubes
        nextGeneration = new GameObject[cubeCount + 2 * padding, cubeCount + 2 * padding, cubeCount + 2 * padding];

        for (int x = padding; x < cubeCount + padding; x++)
        {
            for (int y = padding; y < cubeCount + padding; y++)
            {
                for (int z = padding; z < cubeCount + padding; z++)
                {
                    CountNeighboringCubes(x, y, z);
                    ApplyRules(x, y, z);
                }
            }
        }
        Debug.Log(iteration++-1);

        // Update the cubes array with the next generation
        cubes = nextGeneration;
    }

    void GenerateCube(int x, int y, int z)
    {
        Vector3 position = new Vector3(x * cubeSize, y * cubeSize, z * cubeSize) + offset;
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = generatedCubesContainer.transform;
        cube.transform.position = position;
        cube.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

        nextGeneration[x, y, z] = cube;
    }

    void ChangeCubeColors(int x, int y, int z)
    {
        cubes[x, y, z].GetComponent<Renderer>().material.color = Random.Range(0, 2) == 0 ? Color.red : Color.blue;
        nextGeneration[x, y, z] = cubes[x, y, z];
    }
    
    void Rules(int x, int y, int z) 
    {
        GameObject currentCube = cubes[x, y, z];

        Color cubeColor = currentCube.GetComponent<Renderer>().material.color;

        if (cubeColor == Color.blue) // If the current cell is alive
        {
            // Apply rules for live cells
            if (neighborCountByColor.ContainsKey(Color.blue) && (neighborCountByColor[Color.blue] < 2 || neighborCountByColor[Color.blue] > 3))
            {
                // Cell dies by overpopulation or having dead neighbors
                nextGeneration[x, y, z] = CreateCube(x, y, z, Color.red); // Set the cell as dead
            }
            else
            {
                // No living neighbors, the cell dies
                nextGeneration[x, y, z] = CreateCube(x, y, z, Color.red); // Set the cell as dead
            }
        }
        else // If the current cell is dead
        {
            if (neighborCountByColor.ContainsKey(Color.blue) && neighborCountByColor[Color.blue] == 3)
            {
                // Cell becomes alive by reproduction
                nextGeneration[x, y, z] = CreateCube(x, y, z, Color.blue); // Create a live cell
            }
            // else: Cell remains dead (default assumption)
            else
            {
                nextGeneration[x, y, z] = CreateCube(x, y, z, Color.red);
            }
        }
        nextGeneration[x, y, z].SetActive(true);
        Destroy(cubes[x, y, z]);
    }

    void CountNeighboringCubes(int centerX, int centerY, int centerZ)
    {
        neighborCountByColor.Clear(); // Clear the dictionary before counting neighbors

        for (int x = centerX - padding; x <= centerX + padding; x++)
        {
            for (int y = centerY - padding; y <= centerY + padding; y++)
            {
                for (int z = centerZ - padding; z <= centerZ + padding; z++)
                {
                    // Skip the center cube
                    if (x == centerX && y == centerY && z == centerZ)
                        continue;

                    GameObject neighborCube = cubes[x, y, z];

                    if (neighborCube != null)
                    {
                        Color neighborColor = neighborCube.GetComponent<Renderer>().material.color;

                        // Update the count in the dictionary
                        if (neighborCountByColor.ContainsKey(neighborColor))
                        {
                            neighborCountByColor[neighborColor]++;
                        }
                        else
                        {
                            neighborCountByColor[neighborColor] = 1;
                        }
                    }
                }
            }
        }
    }

    GameObject CreateCube(int x, int y, int z, Color color)
    {
        Vector3 position = cubes[x, y, z].transform.position;
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = generatedCubesContainer.transform;
        cube.transform.position = position;
        cube.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
        cube.GetComponent<Renderer>().material.color = color;
        return cube;
    }

    public delegate void ApplyRulesDelegate(int x, int y, int z);

}
