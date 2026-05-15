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
    }

    public void AttachToEgg(Transform egg)
    {
        eggTransform = egg;
        attachedToEgg = true;

        // Find the dimensions of the interior of the egg.
        // If this object leaves the dimensions or approaches the edge then drag it back in.
        // Or actually, maybe just make the object gravitate towards the centre of the egg?
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
        attachedToHand = false;

        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;
    }
}
