using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeColorChange : MonoBehaviour
{
    public Animator shakeAnimator;
    public GameMainFile gameManager;
    public GameObject pointPrefab;

    private int colorIndex = 0;
    public int health = 3;

    public Texture fullLife;
    public Texture emptyLife;

    public Material fullLifeMaterial;
    public Material emptyLifeMaterial;

    public RawImage[] healthRawImage;

    public int score = 0;
    public Text scoreText;

    public PlatformManager platformManager;

    public float cubeSize = 0.1f;
    public int cubesInRow = 5;

    public Material cubeMaterial;

    float cubesPivotDistance;
    Vector3 cubesPivot;

    public ParticleSystemRenderer skyParticle1;
    public ParticleSystemRenderer skyParticle2;
    public ParticleSystemRenderer skyParticle3;
    public ParticleSystemRenderer skyParticle4;

    public float explosionForce = 0.1f;
    public float explosionRadius = 0.1f;

    [HideInInspector]
    public Animator cubeAnimator;

    public GameObject ripple;

    [HideInInspector]
    public Renderer cubeRenderer;
    Color[] colors = { new Color(1, 1, 1, 1), new Color(0, 0, 1, 1), new Color(1, 0, 0, 1), new Color(0, 1, 0, 1)};

    [Header("Audio")]
    public AudioClip slashAudio;
    public AudioClip hitAudio;
    //public AudioClip shatterAudio;
    public AudioClip pointAudio;

    [HideInInspector]
    public AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        cubeRenderer = GetComponent<Renderer>();
        //setMaterialColor();
        cubeRenderer.material.SetColor("_BaseColor", colors[getNextIndex()]);
        cubeRenderer.material.SetColor("_EmissionColor", colors[getNextIndex()] * 3f);
        cubeAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        //calculate pivot distance
        cubesPivotDistance = cubeSize * cubesInRow / 2;
        //use this value to create pivot vector)
        cubesPivot = new Vector3(cubesPivotDistance, cubesPivotDistance, cubesPivotDistance);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            //if (platformManager.platformsCreated[0] != null)
            //{
            //    Color test9 = platformManager.platformsCreated[0].GetComponent<Renderer>().material.color;
            //    Color test5 = cubeRenderer.material.GetColor("_EmissionColor");
            //    if (platformManager.platformsCreated[0].GetComponent<Renderer>().material.color.Equals(cubeRenderer.material.color))
            //    {
            //        health -= 1;
            //        healthRawImage[health].texture = emptyLife;
            //    }
            //}
            //DestroyPlatform();
            if (!gameManager.RetryScreen.activeSelf)
            {
                cubeAnimator.SetTrigger("isPressed");
                audioSource.clip = slashAudio;
                GameObject rippleObject = Instantiate(ripple, transform);
                rippleObject.transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
                rippleObject.transform.position = new Vector3(transform.position.x, -0.45f, transform.position.z);
                rippleObject.GetComponent<ripple>().cube = this;
                rippleObject.GetComponent<ripple>().platformManager = platformManager;
                audioSource.Play();
            }
        }
    }

    private void changeCubeMaterial()
    {
        int index = getNextIndex();
        cubeRenderer.material.SetColor("_BaseColor", colors[index]);
        cubeRenderer.material.SetColor("_EmissionColor", colors[index] * 3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != "CheckAreaCollider" && collision.gameObject.name != "Cube" && collision.gameObject.tag != "platformEdge" && collision.gameObject.tag != "Player")
        {
            Color test9 = collision.gameObject.GetComponent<Renderer>().material.color * 3;
            Color test5 = cubeRenderer.material.GetColor("_EmissionColor");
            if (test9 != test5)
            {
                audioSource.clip = hitAudio;
                audioSource.Play();
                health -= 1;
                healthRawImage[health].texture = emptyLife;
                healthRawImage[health].material = emptyLifeMaterial;
                Destroy(platformManager.platformsCreated[0]);
                platformManager.platformsCreated.RemoveAt(0);
                return;
            }
            audioSource.clip = pointAudio;
            audioSource.Play();
            pointGained(gameManager.gameObject.transform, pointPrefab);
            score += 1;
            shakeAnimator.SetTrigger("shake");
            changeCubeMaterial();
            Destroy(platformManager.platformsCreated[0]);
            platformManager.platformsCreated.RemoveAt(0);
        }
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

    public void DestroyPlatform()
    {
        GameObject objectTemp = platformManager.platformsCreated[0];
        platformManager.platformsCreated.RemoveAt(0);
        if (objectTemp != null)
        {
            Material minCubeMat = objectTemp.GetComponent<Renderer>().material;
            Transform objectTransform = objectTemp.transform;
            Destroy(objectTemp);

            //loop 3 times to create 5x5x5 pieces in x,y,z coordinates
            for (int x = 0; x < cubesInRow; x++)
            {
                for (int y = 0; y < cubesInRow; y++)
                {
                    for (int z = 0; z < cubesInRow; z++)
                    {
                        createPiece(objectTransform, minCubeMat, x, y, z);
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

    void createPiece(Transform objectTransform, Material materialTemp, int x, int y, int z)
    {

        //create piece
        GameObject piece;
        piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
        piece.transform.name = "Cube";
        piece.GetComponent<Renderer>().material = materialTemp;
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

    public void pointGained(Transform parent, GameObject prefab)
    {
        GameObject gainImage = Instantiate(prefab);
        gainImage.transform.parent = parent;
        StartCoroutine(DestroypointGained(gainImage));
    }

    IEnumerator DestroypointGained(GameObject destroyObject)
    {
        yield return new WaitForSeconds(0.8f);
        scoreText.text = score.ToString();
        Destroy(destroyObject);
    }
}
