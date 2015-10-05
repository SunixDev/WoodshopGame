using UnityEngine;
using System.Collections;

public class WoodLeftover : MonoBehaviour 
{
    public float VanishingSpeed = 2.0f;

    private Material objMaterial;
    private float fullTransparency = 0.0f;

    public void BeginDisappearing()
    {
        if (GetComponent<Rigidbody>() != null)
        {
            Destroy(GetComponent<Rigidbody>());
        }

        if (GetComponent<Collider>() != null)
        {
            Destroy(GetComponent<Collider>());
        }

        objMaterial = GetComponent<Renderer>().material;
        StartCoroutine(Disappear());
    }

    IEnumerator Disappear()
    {
        while (objMaterial.color.a > fullTransparency + 0.05f)
        {
            float alpha = Mathf.Lerp(objMaterial.color.a, fullTransparency, VanishingSpeed * Time.deltaTime);
            objMaterial.color = new Color(objMaterial.color.r, objMaterial.color.g, objMaterial.color.b, alpha);
            yield return null;
        }
        Destroy(gameObject);
    }
}
