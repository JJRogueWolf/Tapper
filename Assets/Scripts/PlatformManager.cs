using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public Transform platformPrefab;
    public float spawnSpeed = 0.1f;
    private Vector3 spwanVector;

    public CubeColorChange cubeColor;

    public List<GameObject> platformsCreated = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        spwanVector = new Vector3(0, 0, 24);
    }

    private void Update()
    {
        if (cubeColor.health > 0)
        {
            if (platformsCreated.Count < 20)
            {
                GameObject instantiatedObject = Instantiate(platformPrefab, spwanVector, Quaternion.identity).gameObject;
                platformsCreated.Add(instantiatedObject);
                spwanVector.z += 16;
            }
        }
    }
}
