using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaObject : MonoBehaviour
{
    Rigidbody rb;

    bool attachedToHand = false;
    Transform handTransform;
    bool attachedToEgg = false;
    Transform eggTransform;
    Egg eggProps;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (attachedToHand)
        {
            transform.position = handTransform.position + transform.up;
            transform.rotation = handTransform.rotation;
        }
        if (attachedToEgg)
        {
            SimulateEggContainment();
        }
    }

    void SimulateEggContainment()
    {
        float axisSpeed = 1f + eggProps.pressure;
        float orbitSpeed = 150f + eggProps.pressure;

        float t = Time.time * axisSpeed;

        Vector3 orbitAxis = new Vector3(
            Mathf.Sin(t),
            0f,
            Mathf.Cos(t)
        ).normalized;
        transform.RotateAround(eggTransform.position, orbitAxis, orbitSpeed * Time.deltaTime);
    }

    public void AttachToEgg(Transform egg)
    {
        eggTransform = egg;
        eggProps = eggTransform.GetComponent<Egg>();
        eggProps.contents.Add(transform);
        attachedToEgg = true;

        rb.constraints = RigidbodyConstraints.FreezePosition |
                     RigidbodyConstraints.FreezeRotation;

        rb.useGravity = false;

        transform.SetParent(eggTransform, false);
        transform.position = eggTransform.position + (Vector3.forward * 0.5f);
    }

    public void AttachToHand(Transform hand)
    {
        handTransform = hand;
        attachedToHand = true;

        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
    }

    public void Release()
    {
        transform.SetParent(null);
        attachedToHand = false;
        attachedToEgg = false;

        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;
    }
}
