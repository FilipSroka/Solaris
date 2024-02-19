using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGenerator : MonoBehaviour
{
    public int cubeSize = 20;
    public int cubeCount = 5;
    public int padding = 2; // Number of additional cubes at the beginning and end
    public GameObject[,,] cubes;

    void Start()
    {
        cubes = new GameObject[cubeCount + 2 * padding, cubeCount + 2 * padding, cubeCount + 2 * padding];
        GenerateCube();
        InvokeRepeating("ChangeCubeColors", 2f, 2f); // Change colors every 2 seconds (adjust as needed)
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
                    // Check if the current cube is within the padding range
                    if (x < padding || x >= cubeCount + padding ||
                        y < padding || y >= cubeCount + padding ||
                        z < padding || z >= cubeCount + padding)
                    {
                        cubes[x, y, z] = null; // Set cubes in padding range to null
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
                    if (cube != null)
                    {
                        cube.GetComponent<Renderer>().material.color = Random.Range(0, 2) == 0 ? Color.red : Color.blue;
                    }
                }
            }
        }
    }
}
