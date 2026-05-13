using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    public float pressure = 0;
    public float maxPressure = 100;

    public float pressureLeakRate = 1f;

    List<PhysicalProperties> contents = new List<PhysicalProperties>();

    void Update()
    {
        GeneratePressure();
        LeakPressure();

        if (pressure >= maxPressure)
        {
            Explode();
        }
    }

    void LeakPressure()
    {
        pressure -= pressureLeakRate * Time.deltaTime;

        if (pressure < 0)
            pressure = 0;
    }

    void GeneratePressure()
    {
        foreach (PhysicalProperties obj in contents)
        {
            if (obj.isIgnited)
            {
                pressure += obj.pressureGenerationRate * Time.deltaTime;
            }
        }
    }

    void Explode()
    {
        Debug.Log("BOOM");

        // Explosion physics here

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        PhysicalProperties p = other.GetComponent<PhysicalProperties>();

        if (p != null)
        {
            contents.Add(p);
        }
    }

    void OnTriggerExit(Collider other)
    {
        PhysicalProperties p = other.GetComponent<PhysicalProperties>();

        if (p != null)
        {
            contents.Remove(p);
        }
    }
}
