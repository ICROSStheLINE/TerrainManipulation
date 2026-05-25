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
    [SerializeField] GameObject explosion;
    World world;

    void Start()
    {
        GameObject worldGameObject = GameObject.FindWithTag("World");
        world = worldGameObject.transform.GetComponent<World>();
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

        DestroyNearbyCubes();
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    void DestroyNearbyCubes()
    {
        int x;
        int y;
        int z;
        int chunkX;
        int chunkY;
        (x,y,z,chunkX,chunkY) = World.ConvertWorldPositionToCubeInChunk(transform.position);
        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                for (int k = -1; k <= 1; k++)
                {
                    world.DrawBlock( x + i, y + j, z + k, chunkX, chunkY, Block.BlockType.Air, true);
                }
            }
        }
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
