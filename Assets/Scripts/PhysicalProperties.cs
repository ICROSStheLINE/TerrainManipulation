using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalProperties : MonoBehaviour
{
    float flashPoint = 10; // Flash Point: Temperature required to ignite using external source
    public float temperature = 0;
    public bool isIgnited;
    public float pressureGenerationRate = 2f;
    Material material;
    SphereCollider sphereCollider;

    void Start()
    {
        material = GetComponent<Renderer>().material;
        sphereCollider = GetComponent<SphereCollider>();
    }

    void Update()
    {
        if (temperature > flashPoint / 3 && temperature < 2 * flashPoint / 3)
        {
            Glow();
        }
        if (temperature >= flashPoint && !isIgnited)
        {
            Ignite();
        }
    }

    void Glow()
    {
        material.SetColor("_Color", Color.yellow);
    }

    void Ignite()
    {
        isIgnited = true;
        material.SetColor("_Color", Color.red);
        StartCoroutine("Disintegrate");
    }

    IEnumerator Disintegrate()
    {
        float dissolveDuration = 5;
        float hitboxDissolveDuration = dissolveDuration - (dissolveDuration / 5);
        float dissolveStrength;
        float elapsedTime = 0;

        while ( elapsedTime < dissolveDuration )
        {
            elapsedTime += Time.deltaTime;

            dissolveStrength = Mathf.Lerp(-0.05f, 1f, elapsedTime / dissolveDuration);
            sphereCollider.radius = Mathf.Lerp(0.5f, 0.0f, elapsedTime / hitboxDissolveDuration);
            sphereCollider.center = new Vector3(0,Mathf.Lerp(0f, -0.5f, elapsedTime / hitboxDissolveDuration),0);
            material.SetFloat("_DissolveStrength", dissolveStrength);

            yield return null;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "PhysicalSpell")
        {
            if (collision.gameObject.transform.GetComponent<PhysicalProperties>().temperature >= flashPoint)
            {
                temperature += 0.2f;
            }
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Spark")
        {
            temperature += 10;
        }
        if (collision.gameObject.tag == "PhysicalSpell")
        {
            if (collision.gameObject.transform.GetComponent<PhysicalProperties>().temperature >= flashPoint)
            {
                temperature += 0.2f;
            }
        }
    }
}