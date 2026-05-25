using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    public float pressure = 0;
    public float maxPressure = 100;

    public float pressureLeakRate = 1f;

    public List<Transform> contents = new List<Transform>();
    Material material;

    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        GeneratePressure();
        LeakPressure();
        VisualizePressure();

        if (pressure >= maxPressure)
        {
            Explode();
        }
    }

    void VisualizePressure()
    {
        material.SetFloat("_CrackStrength", (pressure/maxPressure) - 0.05f);
    }

    void LeakPressure()
    {
        pressure -= pressureLeakRate * Time.deltaTime;

        if (pressure < 0)
            pressure = 0;
    }

    void GeneratePressure()
    {
        foreach (Transform obj in contents)
        {
            if (obj == null)
            { continue; }
            
            PhysicalProperties objPhysProps = obj.GetComponent<PhysicalProperties>();
            if (objPhysProps == null)
            { continue; }

            if (objPhysProps.isIgnited)
            {
                pressure += objPhysProps.pressureGenerationRate * Time.deltaTime;
            }
        }
    }

    void Explode()
    {
        Debug.Log("BOOM");

        foreach (Transform obj in contents)
        {
            if (obj == null)
            { continue; }
            
            ManaObject manaObj = obj.GetComponent<ManaObject>();
            if (manaObj == null)
                { continue; }

            manaObj.Release();

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null)
                { continue; }

            Vector3 dir =
                (obj.position -
                transform.position).normalized;

            rb.AddForce(
                dir * pressure * 1f,
                ForceMode.Impulse
            );
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.tag == "World")
        {
            // Explode();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.root.tag == "World")
        {
            // Explode();
        }
    }
}
