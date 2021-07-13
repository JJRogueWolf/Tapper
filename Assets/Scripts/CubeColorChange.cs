using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeColorChange : MonoBehaviour
{
    public Animator shakeAnimator;
    
    private int colorIndex = 0;
    public int health = 3;

    public Texture fullLife;
    public Texture emptyLife;

    public RawImage[] healthRawImage;

    public int score = 0;
    public Text scoreText;

    public PlatformManager platformManager;

    public float cubeSize = 0.1f;
    public int cubesInRow = 5;

    public Material cubeMaterial;

    float cubesPivotDistance;
    Vector3 cubesPivot;

    public float explosionForce = 0.1f;
    public float explosionRadius = 0.1f;

    Renderer cubeRenderer;
    Color[] colors = { new Color(1, 1, 1, 1), new Color(0, 0, 1, 1), new Color(1, 0, 0, 1), new Color(0, 1, 0, 1)};

    // Start is called before the first frame update
    void Start()
    {
        cubeRenderer = GetComponent<Renderer>();
        //setMaterialColor();
        cubeRenderer.material.SetColor("_Color", colors[getNextIndex()]);

        //calculate pivot distance
        cubesPivotDistance = cubeSize * cubesInRow / 2;
        //use this value to create pivot vector)
        cubesPivot = new Vector3(cubesPivotDistance, cubesPivotDistance, cubesPivotDistance);
    }

    private void Update()
    {
        scoreText.text = score.ToString();
    }

    private void changeCubeMaterial()
    {
        int index = getNextIndex();
        cubeRenderer.material.SetColor("_Color", colors[index]);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != "CheckAreaCollider" && collision.gameObject.name != "Cube")
        {
            if (!collision.gameObject.GetComponent<Renderer>().material.color.Equals(cubeRenderer.material.color))
            {
                health -= 1;
                healthRawImage[health].texture = emptyLife;
                return;
            }
            score += 1;
            shakeAnimator.SetTrigger("shake");
            changeCubeMaterial();
            Destroy(platformManager.platformsCreated[0]);
            platformManager.platformsCreated.RemoveAt(0);
        }
    }

    private void OnMouseDown()
    {
        if (platformManager.platformsCreated[0] != null)
        {
            if (platformManager.platformsCreated[0].GetComponent<Renderer>().material.color.Equals(cubeRenderer.material.color))
            {
                health -= 1;
                healthRawImage[health].texture = emptyLife;
            }
        }
        DestroyPlatform();
    }

    private int getNextIndex()
    {
        if(colorIndex >= colors.Length-1)
        {
            colorIndex = 0;
        } else
        {
            colorIndex += 1;
        }
        return Random.Range(0, colors.Length);
    }

    private void DestroyPlatform()
    {
        GameObject objectTemp = platformManager.platformsCreated[0];
        platformManager.platformsCreated.RemoveAt(0);
        if (objectTemp != null)
        {
            Transform objectTransform = objectTemp.transform;
            Destroy(objectTemp);

            //loop 3 times to create 5x5x5 pieces in x,y,z coordinates
            for (int x = 0; x < cubesInRow; x++)
            {
                for (int y = 0; y < cubesInRow; y++)
                {
                    for (int z = 0; z < cubesInRow; z++)
                    {
                        createPiece(objectTransform, x, y, z);
                    }
                }
            }

            //get explosion position
            Vector3 explosionPos = objectTransform.position;
            //get colliders in that position and radius
            Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
            //add explosion force to all colliders in that overlap sphere
            foreach (Collider hit in colliders)
            {
                //get rigidbody from collider object
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    //add explosion force to this body with given parameters
                    rb.AddExplosionForce(explosionForce, objectTransform.position, explosionRadius);
                }
            }
        }
    }

    void createPiece(Transform objectTransform, int x, int y, int z)
    {

        //create piece
        GameObject piece;
        piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
        piece.transform.name = "Cube";
        piece.GetComponent<Renderer>().material = cubeMaterial;
        piece.GetComponent<Renderer>().material.color = objectTransform.GetComponent<Renderer>().material.color;

        //set piece position and scale
        piece.transform.position = objectTransform.position + new Vector3(cubeSize * x, cubeSize * y, cubeSize * z) - cubesPivot;
        piece.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

        //add rigidbody and set mass
        piece.AddComponent<Rigidbody>();
        piece.GetComponent<Rigidbody>().mass = cubeSize;

        StartCoroutine(DeleteSmallCubes(piece));
    }

    IEnumerator DeleteSmallCubes(GameObject piece)
    {
        yield return new WaitForSeconds(1);
        Destroy(piece);
    }
}
