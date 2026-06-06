using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaObject : MonoBehaviour
{
    Rigidbody rb;
    bool attachedToHand = false;
    Transform handTransform;
    // bool attachedToEgg = false;
    Transform eggTransform;
    Egg eggProps;
    public SpellSlot spellSlotInfo;
    // ^ It might lowkey be bloat to add the manaObject's spellslot info here lol

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (attachedToHand)
        {
            StickToHand();
        }
    }

    void StickToHand()
    {
        rb.MoveRotation(handTransform.rotation);
        Vector3 targetPosition = handTransform.position + handTransform.up - handTransform.forward;
        Vector3 toTarget = targetPosition - rb.position;
        float springStrength = 100f;
        float dampingStrength = 8f;
        rb.AddForce(toTarget * springStrength, ForceMode.Acceleration);
        rb.AddForce(-rb.linearVelocity * dampingStrength, ForceMode.Acceleration);
    }

    public void AttachToEgg(Transform egg)
    {
        eggTransform = egg;
        eggProps = eggTransform.GetComponent<Egg>();
        eggProps.contents.Add(transform);
        // attachedToEgg = true;
    }

    public void AttachToHand(Transform hand)
    {
        handTransform = hand;
        attachedToHand = true;

        rb.useGravity = false;
    }

    public void Release()
    {
        attachedToHand = false;
        // attachedToEgg = false;

        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;
    }

    public void Propel(Vector3 direction, float strength)
    {
        Release();
        rb.AddForce(direction * strength, ForceMode.VelocityChange);
    }
}
