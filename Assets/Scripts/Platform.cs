using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    Renderer plateformRenderer;
    public Renderer glowMatT;
    public Renderer glowMatB;
    public Renderer glowMatR;
    public Renderer glowMatL;
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
        plateformRenderer.material.SetColor("_BaseColor", colors[colorId]);
        plateformRenderer.material.SetColor("_EmissionColor", colors[colorId] * 1.5f);
        glowMatT.material.SetColor("_BaseColor", colors[colorId]);
        glowMatT.material.SetColor("_EmissionColor", colors[colorId] * 0.5f);
        glowMatR.material.SetColor("_BaseColor", colors[colorId]);
        glowMatR.material.SetColor("_EmissionColor", colors[colorId] * 0.5f);
        glowMatB.material.SetColor("_BaseColor", colors[colorId]);
        glowMatB.material.SetColor("_EmissionColor", colors[colorId] * 0.5f);
        glowMatL.material.SetColor("_BaseColor", colors[colorId]);
        glowMatL.material.SetColor("_EmissionColor", colors[colorId] * 0.5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != "CheckAreaCollider")
        {
            Destroy(gameObject);
        }
    }
}
