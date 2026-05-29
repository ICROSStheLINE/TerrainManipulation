using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalProperties : MonoBehaviour
{
    [SerializeField] float flashPoint; // Flash Point: Temperature required to ignite using external source
    public float temperature = 0;
    public bool isIgnited;
    public float pressureGenerationRate = 2f;
    Material material;
    SphereCollider sphereCollider;
    [SerializeField] float heatResistance; // Resistance to EXTERNAL HEAT sources
    [SerializeField] float manaConductivity; // How efficiently it can conduct mana (The compliment to internal heat resistance)
    // NOTE: This is the opposite of mana resistance.
    // To adhere to the magic system, the higher the magic resistance, the more internal heat is generated through a mana current.
    // Therefore, the higher the MANA CONDUCTIVITY, the LESS heat is generated through a mana current.
    // I might need some better nomenclature for ts...

    void Start()
    {
        material = GetComponent<Renderer>().material;
        sphereCollider = GetComponent<SphereCollider>();
    }

    void Update()
    {
        if (temperature > 6 * flashPoint / 7 && temperature < flashPoint)
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
            if (sphereCollider)
            {
                sphereCollider.radius = Mathf.Lerp(0.5f, 0.0f, elapsedTime / hitboxDissolveDuration);
                sphereCollider.center = new Vector3(0,Mathf.Lerp(0f, -0.5f, elapsedTime / hitboxDissolveDuration),0);
            }
            material.SetFloat("_DissolveStrength", dissolveStrength);

            yield return null;
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Spark")
        {
            temperature += 10;
            Destroy(collision.gameObject);
        }
    }

    void OnTriggerStay(Collider collision) // Call when touching something
    {
        if (collision.gameObject.tag == "PhysicalObject") // If touching physical object
        {
            PhysicalProperties collisionPhysicalProps = collision.gameObject.transform.GetComponent<PhysicalProperties>(); // Get reference
            if (collisionPhysicalProps.temperature >= temperature) // If the collided object's temperature is greater than THIS object's temperature...
            {
                float temperatureTransferRate = Mathf.Abs(temperature - collisionPhysicalProps.temperature) / heatResistance; // Multiply difference in temps by conductivity
                temperature += 0.01f * temperatureTransferRate; // Add this much heat to THIS object
                collisionPhysicalProps.temperature -= 0.01f * temperatureTransferRate; // Delete this much heat from the collided object
            }
        }
    }
}