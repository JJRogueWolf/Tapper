using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ripple : MonoBehaviour
{
    [HideInInspector]
    public CubeColorChange cube;
    [HideInInspector]
    public PlatformManager platformManager;

    private Renderer cubeRenderer;
    private SpriteRenderer rippleRenderer;

    // Start is called before the first frame update
    void Start()
    {
        cubeRenderer = cube.gameObject.GetComponent<Renderer>();
        rippleRenderer = GetComponent<SpriteRenderer>();
        rippleRenderer.material.color = cube.cubeRenderer.material.GetColor("_EmissionColor");
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localScale.x > 16.5f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != "CheckAreaCollider" && collision.gameObject.name != "Cube" && collision.gameObject.tag != "platformEdge" && collision.gameObject.tag != "Player") {
            //cube.audioSource.clip = cube.shatterAudio;
            //cube.audioSource.Play();
            Color test9 = platformManager.platformsCreated[0].GetComponent<Renderer>().material.color * 3f;
            Color test5 = cubeRenderer.material.GetColor("_EmissionColor");
            //if (platformManager.platformsCreated[0].GetComponent<Renderer>().material.color.Equals(cubeRenderer.material.color))
            if (test9.Equals(test5))
            {
                cube.health -= 1;
                if (cube.health >= 0)
                {
                    cube.healthRawImage[cube.health].texture = cube.emptyLife;
                    cube.healthRawImage[cube.health].material = cube.emptyLifeMaterial;
                }
            }
            cube.DestroyPlatform();
            Destroy(gameObject);
        }
    }
}
