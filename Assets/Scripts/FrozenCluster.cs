using System.Collections.Generic;
using UnityEngine;

public class FrozenCluster : MonoBehaviour
{
    public List<PhysicalProperties> members = new List<PhysicalProperties>();
    public Rigidbody clusterRigidbody;
    public BoxCollider boxCollider;
    public ManaObject manaObject;
    public PhysicalProperties physicalProperties;
    private Transform visualMesh;
    private Material visualMaterial;

    void Awake()
    {
        transform.tag = "PhysicalObject";
        EnsureRequiredComponents();
    }

    public void AddMember(PhysicalProperties obj)
    {
        AddMember(obj, true);
    }

    public void RemoveMember(PhysicalProperties obj)
    {
        if (!obj || !members.Remove(obj))
        {
            return;
        }

        obj.frozenCluster = null;
        obj.transform.SetParent(null, true);
        SetMemberCollidersEnabled(obj, true);

        Rigidbody childRigidbody = obj.GetComponent<Rigidbody>();
        if (childRigidbody)
        {
            childRigidbody.isKinematic = false;
        }

        RecalculateCenter();
    }

    public void MergeWith(FrozenCluster other)
    {
        if (!other || other == this)
        {
            return;
        }

        if (other.manaObject && other.manaObject.attachedToHand)
        {
            AttachToHand(other.manaObject.handTransform);
        }

        physicalProperties.ApplyMeanStatsWith(other.physicalProperties);
        physicalProperties.frozenCluster = this;
        physicalProperties.isFrozen = true;

        for (int i = other.members.Count - 1; i >= 0; i--)
        {
            PhysicalProperties member = other.members[i];
            other.members.RemoveAt(i);
            AddMember(member, false, false);
        }

        RecalculateCenter();
        Destroy(other.gameObject);
    }

    public void AttachToHand(Transform hand)
    {
        if (!hand)
        {
            return;
        }

        manaObject.AttachToHand(hand);
    }

    public void ReleaseFromHand()
    {
        manaObject.Release(false);
    }

    public void RecalculateCenter()
    {
        if (members.Count == 0)
        {
            return;
        }

        Vector3 center = Vector3.zero;
        int validMemberCount = 0;

        for (int i = members.Count - 1; i >= 0; i--)
        {
            PhysicalProperties member = members[i];
            if (!member)
            {
                members.RemoveAt(i);
                continue;
            }

            center += member.transform.position;
            validMemberCount++;
        }

        if (validMemberCount == 0)
        {
            return;
        }

        center /= validMemberCount;

        Vector3[] worldPositions = new Vector3[members.Count];
        Quaternion[] worldRotations = new Quaternion[members.Count];

        for (int i = 0; i < members.Count; i++)
        {
            Transform memberTransform = members[i].transform;
            worldPositions[i] = memberTransform.position;
            worldRotations[i] = memberTransform.rotation;
        }

        transform.position = center;

        for (int i = 0; i < members.Count; i++)
        {
            Transform memberTransform = members[i].transform;
            memberTransform.SetPositionAndRotation(worldPositions[i], worldRotations[i]);
        }

        RecalculateBoxColliderBounds();
    }

    void AddMember(PhysicalProperties obj, bool recalculateCenter)
    {
        AddMember(obj, recalculateCenter, true);
    }

    void AddMember(PhysicalProperties obj, bool recalculateCenter, bool applyPhysicalProperties)
    {
        if (!obj || members.Contains(obj))
        {
            return;
        }

        if (obj.frozenCluster && obj.frozenCluster != this)
        {
            obj.frozenCluster.members.Remove(obj);
        }

        if (applyPhysicalProperties)
        {
            ApplyPhysicalPropertiesForNewMember(obj);
        }

        members.Add(obj);
        obj.frozenCluster = this;
        obj.transform.SetParent(transform, true);
        obj.transform.rotation = Quaternion.Euler(Vector3.zero);

        ManaObject memberManaObject = obj.GetComponent<ManaObject>();
        if (memberManaObject)
        {
            if (memberManaObject.attachedToHand)
            {
                AttachToHand(memberManaObject.handTransform);
            }

            memberManaObject.Release(false);
        }

        Rigidbody childRigidbody = obj.GetComponent<Rigidbody>();
        if (childRigidbody)
        {
            childRigidbody.isKinematic = true;
        }

        SetMemberCollidersEnabled(obj, false);

        if (recalculateCenter)
        {
            RecalculateCenter();
        }
    }

    void CreateVisualMesh()
    {
        if (visualMesh != null)
        {
            return;
        }

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "FrozenClusterVisual";
        cube.transform.SetParent(transform, false);

        Destroy(cube.GetComponent<Collider>());

        visualMesh = cube.transform;

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");

        visualMaterial = new Material(shader);
        visualMaterial.color = new Color(0.6f, 0.85f, 1f, 0.25f);

        // Standard shader transparency
        if (shader.name == "Standard")
        {
            visualMaterial.SetFloat("_Mode", 3);
            visualMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            visualMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            visualMaterial.SetInt("_ZWrite", 0);
            visualMaterial.DisableKeyword("_ALPHATEST_ON");
            visualMaterial.EnableKeyword("_ALPHABLEND_ON");
            visualMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            visualMaterial.renderQueue = 3000;
        }

        cube.GetComponent<MeshRenderer>().material = visualMaterial;
    }

    void EnsureRequiredComponents()
    {
        if (!clusterRigidbody)
        {
            clusterRigidbody = GetComponent<Rigidbody>();
        }

        if (!clusterRigidbody)
        {
            clusterRigidbody = gameObject.AddComponent<Rigidbody>();
        }

        if (!manaObject)
        {
            manaObject = GetComponent<ManaObject>();
        }

        if (!manaObject)
        {
            manaObject = gameObject.AddComponent<ManaObject>();
        }

        if (!physicalProperties)
        {
            physicalProperties = GetComponent<PhysicalProperties>();
        }

        if (!physicalProperties)
        {
            physicalProperties = gameObject.AddComponent<PhysicalProperties>();
        }

        physicalProperties.frozenCluster = this;
        physicalProperties.isFrozen = true;

        if (!boxCollider)
        {
            boxCollider = GetComponent<BoxCollider>();
        }

        if (!boxCollider)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }
        
        CreateVisualMesh();
    }

    void SetMemberCollidersEnabled(PhysicalProperties member, bool enabled)
    {
        Collider[] memberColliders = member.GetComponentsInChildren<Collider>();
        for (int i = 0; i < memberColliders.Length; i++)
        {
            memberColliders[i].enabled = enabled;
        }
    }

    void ApplyPhysicalPropertiesForNewMember(PhysicalProperties member)
    {
        if (members.Count == 0)
        {
            physicalProperties.CopyStatsFrom(member);
        }
        else
        {
            physicalProperties.ApplyMeanStatsWith(member);
        }

        physicalProperties.frozenCluster = this;
        physicalProperties.isFrozen = true;
    }

    void UpdateVisualMesh()
{
    if (visualMesh == null)
    {
        return;
    }

    visualMesh.localPosition = boxCollider.center;
    visualMesh.localRotation = Quaternion.identity;
    visualMesh.localScale = boxCollider.size;
}

    void RecalculateBoxColliderBounds()
    {
        bool hasBounds = false;
        Bounds localBounds = new Bounds();

        for (int i = 0; i < members.Count; i++)
        {
            PhysicalProperties member = members[i];
            if (!member)
            {
                continue;
            }

            ManaObject manaObject = member.GetComponent<ManaObject>();

            // Only include Ball spells in the cluster bounds.
            if (manaObject == null ||
                manaObject.spellSlotInfo == null ||
                manaObject.spellSlotInfo.spellType != SpellSlot.SpellType.Ball)
            {
                continue;
            }

            Renderer[] memberRenderers = member.GetComponentsInChildren<Renderer>();
            for (int j = 0; j < memberRenderers.Length; j++)
            {
                Renderer memberRenderer = memberRenderers[j];
                if (!memberRenderer || !memberRenderer.enabled)
                {
                    continue;
                }

                EncapsulateWorldBounds(memberRenderer.bounds, ref localBounds, ref hasBounds);
            }
        }

        if (!hasBounds)
        {
            boxCollider.center = Vector3.zero;
            boxCollider.size = Vector3.one;
            UpdateVisualMesh();
            return;
        }

        boxCollider.center = localBounds.center;
        boxCollider.size = localBounds.size;
        UpdateVisualMesh();
    }

    void EncapsulateWorldBounds(Bounds worldBounds, ref Bounds localBounds, ref bool hasBounds)
    {
        Vector3 min = worldBounds.min;
        Vector3 max = worldBounds.max;

        EncapsulateLocalPoint(new Vector3(min.x, min.y, min.z), ref localBounds, ref hasBounds);
        EncapsulateLocalPoint(new Vector3(min.x, min.y, max.z), ref localBounds, ref hasBounds);
        EncapsulateLocalPoint(new Vector3(min.x, max.y, min.z), ref localBounds, ref hasBounds);
        EncapsulateLocalPoint(new Vector3(min.x, max.y, max.z), ref localBounds, ref hasBounds);
        EncapsulateLocalPoint(new Vector3(max.x, min.y, min.z), ref localBounds, ref hasBounds);
        EncapsulateLocalPoint(new Vector3(max.x, min.y, max.z), ref localBounds, ref hasBounds);
        EncapsulateLocalPoint(new Vector3(max.x, max.y, min.z), ref localBounds, ref hasBounds);
        EncapsulateLocalPoint(new Vector3(max.x, max.y, max.z), ref localBounds, ref hasBounds);
    }

    void EncapsulateLocalPoint(Vector3 worldPoint, ref Bounds localBounds, ref bool hasBounds)
    {
        Vector3 localPoint = transform.InverseTransformPoint(worldPoint);

        if (!hasBounds)
        {
            localBounds = new Bounds(localPoint, Vector3.zero);
            hasBounds = true;
            return;
        }

        localBounds.Encapsulate(localPoint);
    }
}
