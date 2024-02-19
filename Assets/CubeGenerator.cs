using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGenerator : MonoBehaviour
{
    public int cubeSize = 20;
    public int cubeCount = 5;
    public int padding = 2; // Number of additional cubes at the beginning and end
    public GameObject[,,] cubes;
    public Dictionary<Color, int> neighborCountByColor = new Dictionary<Color, int>();

    public int iteration = 0;
    void Start()
    {
        cubes = new GameObject[cubeCount + 2 * padding, cubeCount + 2 * padding, cubeCount + 2 * padding];
        GenerateCube();
        //InvokeRepeating("ChangeCubeColors", 2f, 2f); // Change colors every 2 seconds (adjust as needed)
        ChangeCubeColors();
        InvokeRepeating("ApplyGameOfLifeRules", 2f, 2f);
        
    }

    void GenerateCube()
    {
        Vector3 offset = new Vector3(-cubeSize * (cubeCount + 2 * padding) / 2f + cubeSize / 2f,
                                      -cubeSize * (cubeCount + 2 * padding) / 2f + cubeSize / 2f,
                                      -cubeSize * (cubeCount + 2 * padding) / 2f + cubeSize / 2f);

        for (int x = 0; x < cubeCount + 2 * padding; x++)
        {
            for (int y = 0; y < cubeCount + 2 * padding; y++)
            {
                for (int z = 0; z < cubeCount + 2 * padding; z++)
                {
                    if (x < padding || x >= cubeCount + padding ||
                        y < padding || y >= cubeCount + padding ||
                        z < padding || z >= cubeCount + padding)
                    {
                        cubes[x, y, z] = null;
                    }
                    else
                    {
                        Vector3 position = new Vector3(x * cubeSize, y * cubeSize, z * cubeSize) + offset;
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = position;
                        cube.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
                        cubes[x, y, z] = cube;
                    }
                }
            }
        }
    }

    void ChangeCubeColors()
    {
        for (int x = padding; x < cubeCount + padding; x++)
        {
            for (int y = padding; y < cubeCount + padding; y++)
            {
                for (int z = padding; z < cubeCount + padding; z++)
                {
                    GameObject cube = cubes[x, y, z];
                    cube.GetComponent<Renderer>().material.color = Random.Range(0, 2) == 0 ? Color.red : Color.blue;
                }
            }
        }
    }

    void CountAllNeighboringCubes()
    {
        for (int x = padding; x < cubeCount + padding; x++)
        {
            for (int y = padding; y < cubeCount + padding; y++)
            {
                for (int z = padding; z < cubeCount + padding; z++)
                {
                    // Apply CountNeighboringCubes function to each cube
                    CountNeighboringCubes(x, y, z);   
                }
            }
        }
    }

    void CountNeighboringCubes(int centerX, int centerY, int centerZ)
    {
        neighborCountByColor.Clear(); // Clear the dictionary before counting neighbors

        for (int x = centerX - 1; x <= centerX + 1; x++)
        {
            for (int y = centerY - 1; y <= centerY + 1; y++)
            {
                for (int z = centerZ - 1; z <= centerZ + 1; z++)
                {
                    // Skip the center cube
                    if (x == centerX && y == centerY && z == centerZ)
                        continue;


                    GameObject neighborCube = cubes[x, y, z];

                    // Ignore null cubes
                    if (neighborCube != null)
                    {
                        Color neighborColor = neighborCube.GetComponent<Renderer>().material.color;

                        // Update the count in the dictionary
                        if (neighborCountByColor.ContainsKey(neighborColor))
                            neighborCountByColor[neighborColor]++;
                        else
                            neighborCountByColor[neighborColor] = 1;
                    }
                    
                }
            }
        }

        // // Print the counts for testing (you can modify as needed)
        // foreach (var kvp in neighborCountByColor)
        // {
        //     Debug.Log($"Color: {kvp.Key}, Count: {kvp.Value}");
        // }
    }

    void ApplyGameOfLifeRules()
    {
        // Create a new array to store the next state of the cubes
        GameObject[,,] nextGeneration = new GameObject[cubeCount + 2 * padding, cubeCount + 2 * padding, cubeCount + 2 * padding];

        for (int x = padding; x < cubeCount + padding; x++)
        {
            for (int y = padding; y < cubeCount + padding; y++)
            {
                for (int z = padding; z < cubeCount + padding; z++)
                {
                    GameObject currentCube = cubes[x, y, z];
                    CountNeighboringCubes(x, y, z); // No need to assign to a variable

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
                    cubes[x, y, z].SetActive(false);
                    neighborCountByColor.Clear();
                }
            }
        }
        Debug.Log(iteration++);
        // Update the cubes array with the next generation
        cubes = nextGeneration;
    }

    GameObject CreateCube(int x, int y, int z, Color color)
    {
        Vector3 position = cubes[x, y, z].transform.position;
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
        cube.GetComponent<Renderer>().material.color = color;
        return cube;
    }

}
