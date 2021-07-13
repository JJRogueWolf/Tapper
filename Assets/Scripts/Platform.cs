using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    Renderer plateformRenderer;
    Color[] colors = { new Color(1, 1, 1, 1), new Color(0, 0, 1, 1), new Color(1, 0, 0, 1), new Color(0, 1, 0, 1) };

    // Start is called before the first frame update
    void Start()
    {
        plateformRenderer = GetComponent<Renderer>();
        setMaterialColor();
    }

    IEnumerator destroyInDelay()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);

    }

    private void setMaterialColor()
    {
        int colorId = Random.Range(0, colors.Length);
        plateformRenderer.material.SetColor("_Color", colors[colorId]);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != "CheckAreaCollider")
        {
            Destroy(gameObject);
        }
    }
}
