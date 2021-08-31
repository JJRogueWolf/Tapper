using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformManager : MonoBehaviour
{
    public Transform platformPrefab;
    public float spawnSpeed = 0.1f;
    private Vector3 spwanVector;

    public CubeColorChange cubeColor;

    public Image background;
    public Image backgroundSec;

    [HideInInspector]
    public List<GameObject> platformsCreated = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        spwanVector = new Vector3(0, 0, 72);
    }

    private void Update()
    {
        if (cubeColor.health > 0)
        {
            if (platformsCreated.Count < 20)
            {
                GameObject instantiatedObject = Instantiate(platformPrefab, spwanVector, Quaternion.identity).gameObject;
                platformsCreated.Add(instantiatedObject);
                spwanVector.z += 48;
            }
            cubeColor.skyParticle1.material.SetColor("_TintColor", platformsCreated[0].GetComponent<Renderer>().material.color);
            cubeColor.skyParticle2.material.SetColor("_TintColor", platformsCreated[0].GetComponent<Renderer>().material.color);
            cubeColor.skyParticle3.material.SetColor("_TintColor", platformsCreated[0].GetComponent<Renderer>().material.color);
            cubeColor.skyParticle4.material.SetColor("_TintColor", platformsCreated[0].GetComponent<Renderer>().material.color);
            background.color = platformsCreated[0].GetComponent<Renderer>().material.color;
            backgroundSec.color = platformsCreated[0].GetComponent<Renderer>().material.color;
        }
    }
}
