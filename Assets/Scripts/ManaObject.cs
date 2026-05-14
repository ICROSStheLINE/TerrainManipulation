using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaObject : MonoBehaviour
{
    Rigidbody rb;

    bool attachedToHand = false;
    Transform handTransform;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (attachedToHand)
        {
            transform.position = handTransform.position + Vector3.up;
            transform.rotation = handTransform.rotation;
        }
    }

    public void AttachToHand(Transform hand)
    {
        handTransform = hand;
        attachedToHand = true;

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void Release()
    {
        attachedToHand = false;

        rb.isKinematic = false;
        rb.useGravity = true;
    }
}
