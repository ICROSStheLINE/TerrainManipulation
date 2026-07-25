using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaObject : MonoBehaviour
{
    Rigidbody rb;
    [HideInInspector] public bool attachedToHand = false;
    [HideInInspector] public Transform handTransform;
    // bool attachedToEgg = false;
    Transform eggTransform;
    Egg eggProps;
    public SpellSlot spellSlotInfo;
    // ^ It might lowkey be bloat to add the manaObject's spellslot info here lol
    int scrollForwardCount = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // This is actually a NECESSARY FUNCTION!
    // C# has a weird quirk where setting a variable (who's datatype is a CLASS) to an object
    // doesn't make it a copy of the object like how setting a string or an int does. It
    // instead sets it to a REFERENCE to that existing object (like how a pointer works).
    // This is why I can do var = GetComponent<Class>() and it would still track the values
    // in that component as they change.
    // Therefore, if we want to keep a snapshot of the spell slot info, we can't just set
    // spellSlotInfo directly to the value of the spellSlot object, cause the next time that
    // slot gets changed to something else in the menu this variable would change too!
    // Therefore, this method was created for the sole purpose of keeping a "snapshot" of
    // the spellslot data.
    public void SetSpellSlotInfo(SpellSlot source)
    {
        if (source == null)
        {
            spellSlotInfo = null;
            return;
        }

        spellSlotInfo = new SpellSlot();
        spellSlotInfo.spellType = source.spellType;
        spellSlotInfo.manaResistancePercent = source.manaResistancePercent;
        spellSlotInfo.manaFlowAmount = source.manaFlowAmount;
        spellSlotInfo.manaFlowType = source.manaFlowType;
    }

    void FixedUpdate()
    {
        if (attachedToHand)
        {
            StickToHand();
        }
    }

    void Update()
    {
        if (attachedToHand)
        {
            if (Input.mouseScrollDelta.y > 0) // scrolled up
            {
                scrollForwardCount++;
                if (scrollForwardCount > 2) {scrollForwardCount = 2;}
            }
            if (Input.mouseScrollDelta.y < 0) // scrolled down
            {
                scrollForwardCount--;
                if (scrollForwardCount < -2) {scrollForwardCount = -2;}
            }
        }
    }

    void StickToHand()
    {
        Vector3 scrollOffset = -handTransform.forward * scrollForwardCount;
        rb.MoveRotation(handTransform.rotation);
        Vector3 targetPosition = handTransform.position + handTransform.up - handTransform.forward + scrollOffset;
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

    public void Release(bool releaseFrozenCluster = true)
    {
        if (releaseFrozenCluster)
        {
            PhysicalProperties physicalProperties = GetComponent<PhysicalProperties>();
            if (physicalProperties && physicalProperties.frozenCluster)
            {
                physicalProperties.frozenCluster.ReleaseFromHand();
            }
        }

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
