using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalProperties : MonoBehaviour
{
    float flashPoint = 10; // Flash Point: Temperature required to ignite using external source
    public float temperature = 0;

    void Start()
    {
        
    }

    void Update()
    {
        if (temperature > flashPoint / 3 && temperature < 2 * flashPoint / 3)
        {
            Glow();
        }
        if (temperature >= flashPoint)
        {
            Ignite();
        }
    }

    void Glow()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.yellow;
    }

    void Ignite()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.red;
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "PhysicalObject")
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
    }
}