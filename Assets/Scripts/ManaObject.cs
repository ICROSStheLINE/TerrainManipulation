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
    Coroutine eggAttachmentCoroutine;

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
        float orbitSpeed = 150f + eggProps.pressure;

        float t = Time.time;

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

        rb.constraints = RigidbodyConstraints.FreezePosition |
                     RigidbodyConstraints.FreezeRotation;

        rb.useGravity = false;

        StartCoroutine("MoveToEgg");
    }

    IEnumerator MoveToEgg()
    {
        transform.SetParent(eggTransform);
        transform.position = eggTransform.position;
        float randomX = Random.Range(-1, 1);
        float randomY = Random.Range(-1, 1);
        float randomZ = Random.Range(-1, 1);
        Vector3 randomDir = new Vector3(randomX,randomY,randomZ);
        Vector3 startingPosition = eggTransform.position + (randomDir.normalized/2);
        while (transform.position != startingPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, startingPosition, 1 * Time.deltaTime);
            yield return null;
        }
        attachedToEgg = true;
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
