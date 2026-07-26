using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FrozenCluster : MonoBehaviour
{
    [Header("Members")]
    public List<PhysicalProperties> members = new List<PhysicalProperties>();
    public Rigidbody clusterRigidbody;
    public ManaObject manaObject;
    public PhysicalProperties physicalProperties;

    [Header("Frozen geometry")]
    [Min(0.01f)] public float maximumLinkDistance = 1.25f;
    [Min(0.01f)] public float voxelSize = 0.1f;
    [Min(0.01f)] public float frozenThickness = 0.12f;
    public Material frozenMaterial;
    [Min(1)] public int colliderWarningThreshold = 256;
    [Min(1000)] public int voxelEvaluationLimit = 250000;

    private readonly Dictionary<int, MemberComponentState> memberStates =
        new Dictionary<int, MemberComponentState>();
    private readonly List<GameObject> generatedColliderObjects = new List<GameObject>();

    private Transform visualMesh;
    private MeshFilter visualMeshFilter;
    private MeshRenderer visualMeshRenderer;
    private Mesh generatedMesh;
    private Material generatedMaterial;

    private static readonly Vector3Int[] NeighborDirections =
    {
        Vector3Int.right, Vector3Int.left,
        Vector3Int.up, Vector3Int.down,
        new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1)
    };

    void Awake()
    {
        transform.tag = "PhysicalObject";
        EnsureRequiredComponents();
    }

    void Update()
    {
        if (physicalProperties.temperature <= physicalProperties.freezingPoint || members.Count == 0)
        {
            return;
        }

        PhysicalProperties closestMember = members[0];
        float closestAlignment = Vector3.Dot(
            (closestMember.transform.position - transform.position).normalized,
            physicalProperties.tempChangeDirection);

        for (int i = 1; i < members.Count; i++)
        {
            float alignment = Vector3.Dot(
                (members[i].transform.position - transform.position).normalized,
                physicalProperties.tempChangeDirection);
            if (alignment > closestAlignment)
            {
                closestMember = members[i];
                closestAlignment = alignment;
            }
        }

        RemoveMember(closestMember);
        physicalProperties.temperature = physicalProperties.freezingPoint - 5f;
    }

    void OnDestroy()
    {
        if (generatedMesh)
        {
            Destroy(generatedMesh);
        }

        if (generatedMaterial)
        {
            Destroy(generatedMaterial);
        }
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
        RestoreMemberComponents(obj);

        Rigidbody childRigidbody = obj.GetComponent<Rigidbody>();
        if (childRigidbody)
        {
            childRigidbody.isKinematic = false;
        }

        if (members.Count == 0)
        {
            Destroy(gameObject);
            return;
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
            other.RestoreMemberComponents(member);
            AddMember(member, false, false);
        }

        RecalculateCenter();
        Destroy(other.gameObject);
    }

    public void AttachToHand(Transform hand)
    {
        if (hand)
        {
            manaObject.AttachToHand(hand);
        }
    }

    public void ReleaseFromHand()
    {
        manaObject.Release(false);
    }

    public void RecalculateCenter()
    {
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
            ClearGeneratedGeometry();
            return;
        }

        center /= validMemberCount;
        Vector3[] worldPositions = new Vector3[members.Count];
        Quaternion[] worldRotations = new Quaternion[members.Count];

        for (int i = 0; i < members.Count; i++)
        {
            worldPositions[i] = members[i].transform.position;
            worldRotations[i] = members[i].transform.rotation;
        }

        transform.position = center;

        for (int i = 0; i < members.Count; i++)
        {
            members[i].transform.SetPositionAndRotation(worldPositions[i], worldRotations[i]);
        }

        RebuildFrozenGeometry();
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

        HideMemberComponents(obj);

        if (recalculateCenter)
        {
            RecalculateCenter();
        }
    }

    void EnsureRequiredComponents()
    {
        clusterRigidbody = clusterRigidbody ? clusterRigidbody : GetComponent<Rigidbody>();
        if (!clusterRigidbody)
        {
            clusterRigidbody = gameObject.AddComponent<Rigidbody>();
        }

        manaObject = manaObject ? manaObject : GetComponent<ManaObject>();
        if (!manaObject)
        {
            manaObject = gameObject.AddComponent<ManaObject>();
        }

        physicalProperties = physicalProperties ? physicalProperties : GetComponent<PhysicalProperties>();
        if (!physicalProperties)
        {
            physicalProperties = gameObject.AddComponent<PhysicalProperties>();
        }

        physicalProperties.frozenCluster = this;
        physicalProperties.isFrozen = true;

        // Remove the old aggregate collider when upgrading an existing scene object.
        BoxCollider oldBoxCollider = GetComponent<BoxCollider>();
        if (oldBoxCollider)
        {
            oldBoxCollider.enabled = false;
            Destroy(oldBoxCollider);
        }

        CreateVisualMesh();
    }

    void CreateVisualMesh()
    {
        if (visualMesh)
        {
            return;
        }

        GameObject visualObject = new GameObject("FrozenClusterVisual");
        visualObject.transform.SetParent(transform, false);
        visualMesh = visualObject.transform;
        visualMeshFilter = visualObject.AddComponent<MeshFilter>();
        visualMeshRenderer = visualObject.AddComponent<MeshRenderer>();

        if (frozenMaterial)
        {
            visualMeshRenderer.sharedMaterial = frozenMaterial;
            return;
        }

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (!shader)
        {
            shader = Shader.Find("Standard");
        }

        generatedMaterial = new Material(shader);
        generatedMaterial.color = new Color(0.6f, 0.85f, 1f, 0.65f);
        if (generatedMaterial.HasProperty("_Surface"))
        {
            generatedMaterial.SetFloat("_Surface", 1f);
            generatedMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            generatedMaterial.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            generatedMaterial.SetInt("_ZWrite", 0);
            generatedMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            generatedMaterial.renderQueue = (int)RenderQueue.Transparent;
        }
        visualMeshRenderer.sharedMaterial = generatedMaterial;
    }

    void HideMemberComponents(PhysicalProperties member)
    {
        int memberId = member.GetInstanceID();
        Renderer[] renderers = member.GetComponentsInChildren<Renderer>(true);
        Collider[] colliders = member.GetComponentsInChildren<Collider>(true);

        if (!memberStates.ContainsKey(memberId))
        {
            memberStates.Add(memberId, new MemberComponentState(renderers, colliders));
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = false;
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }
    }

    void RestoreMemberComponents(PhysicalProperties member)
    {
        if (!member)
        {
            return;
        }

        int memberId = member.GetInstanceID();
        if (!memberStates.TryGetValue(memberId, out MemberComponentState state))
        {
            return;
        }

        state.Restore();
        memberStates.Remove(memberId);
    }

    void ApplyPhysicalPropertiesForNewMember(PhysicalProperties member)
    {
        if (members.Count == 0)
        {
            physicalProperties.CopyStatsFrom(member);
            physicalProperties.temperature = physicalProperties.freezingPoint - 1f;
            physicalProperties.heatResistance = 0.2f;
        }
        else
        {
            physicalProperties.ApplyMeanStatsWith(member);
        }

        physicalProperties.frozenCluster = this;
        physicalProperties.isFrozen = true;
    }

    void RebuildFrozenGeometry()
    {
        ClearGeneratedGeometry();

        if (members.Count == 0)
        {
            return;
        }

        float safeVoxelSize = Mathf.Max(0.01f, voxelSize);
        float safeThickness = Mathf.Max(0.01f, frozenThickness);
        List<Vector3> points = new List<Vector3>(members.Count);

        for (int i = 0; i < members.Count; i++)
        {
            if (members[i])
            {
                points.Add(transform.InverseTransformPoint(members[i].transform.position));
            }
        }

        if (points.Count == 0)
        {
            return;
        }

        List<Connection> connections = BuildConnections(points);
        Bounds bounds = new Bounds(points[0], Vector3.zero);
        for (int i = 1; i < points.Count; i++)
        {
            bounds.Encapsulate(points[i]);
        }
        bounds.Expand(safeThickness * 2f + safeVoxelSize);

        Vector3Int minimum = FloorToGrid(bounds.min, safeVoxelSize);
        Vector3Int maximum = CeilToGrid(bounds.max, safeVoxelSize);
        long evaluationCount =
            (long)(maximum.x - minimum.x + 1) *
            (maximum.y - minimum.y + 1) *
            (maximum.z - minimum.z + 1);

        if (evaluationCount > voxelEvaluationLimit)
        {
            Debug.LogError(
                $"FrozenCluster '{name}' needs to evaluate {evaluationCount} cells, exceeding " +
                $"the limit of {voxelEvaluationLimit}. Increase voxelSize or the limit.", this);
            return;
        }

        HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>();
        float occupancyRadius = safeThickness + safeVoxelSize * 0.5f;
        float occupancyRadiusSquared = occupancyRadius * occupancyRadius;

        for (int x = minimum.x; x <= maximum.x; x++)
        {
            for (int y = minimum.y; y <= maximum.y; y++)
            {
                for (int z = minimum.z; z <= maximum.z; z++)
                {
                    Vector3Int cell = new Vector3Int(x, y, z);
                    Vector3 center = CellCenter(cell, safeVoxelSize);
                    if (IsOccupied(center, points, connections, occupancyRadiusSquared))
                    {
                        occupiedCells.Add(cell);
                    }
                }
            }
        }

        BuildVisualMesh(occupiedCells, safeVoxelSize);
        BuildCompoundColliders(occupiedCells, safeVoxelSize);
    }

    List<Connection> BuildConnections(List<Vector3> points)
    {
        List<Connection> connections = new List<Connection>();
        float linkDistanceSquared = maximumLinkDistance * maximumLinkDistance;

        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                if ((points[i] - points[j]).sqrMagnitude <= linkDistanceSquared)
                {
                    connections.Add(new Connection(points[i], points[j]));
                }
            }
        }

        return connections;
    }

    bool IsOccupied(
        Vector3 cellCenter,
        List<Vector3> points,
        List<Connection> connections,
        float occupancyRadiusSquared)
    {
        for (int i = 0; i < points.Count; i++)
        {
            if ((cellCenter - points[i]).sqrMagnitude <= occupancyRadiusSquared)
            {
                return true;
            }
        }

        for (int i = 0; i < connections.Count; i++)
        {
            if (SquaredDistanceToSegment(cellCenter, connections[i].start, connections[i].end)
                <= occupancyRadiusSquared)
            {
                return true;
            }
        }

        return false;
    }

    static float SquaredDistanceToSegment(Vector3 point, Vector3 start, Vector3 end)
    {
        Vector3 segment = end - start;
        float lengthSquared = segment.sqrMagnitude;
        if (lengthSquared <= Mathf.Epsilon)
        {
            return (point - start).sqrMagnitude;
        }

        float amount = Mathf.Clamp01(Vector3.Dot(point - start, segment) / lengthSquared);
        Vector3 closestPoint = start + segment * amount;
        return (point - closestPoint).sqrMagnitude;
    }

    void BuildVisualMesh(HashSet<Vector3Int> occupiedCells, float cellSize)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        foreach (Vector3Int cell in occupiedCells)
        {
            Vector3 center = CellCenter(cell, cellSize);
            for (int face = 0; face < NeighborDirections.Length; face++)
            {
                if (!occupiedCells.Contains(cell + NeighborDirections[face]))
                {
                    AddFace(center, cellSize, face, vertices, triangles, uvs);
                }
            }
        }

        generatedMesh = new Mesh { name = "FrozenClusterVoxelMesh" };
        if (vertices.Count > 65535)
        {
            generatedMesh.indexFormat = IndexFormat.UInt32;
        }

        generatedMesh.SetVertices(vertices);
        generatedMesh.SetTriangles(triangles, 0);
        generatedMesh.SetUVs(0, uvs);
        generatedMesh.RecalculateNormals();
        generatedMesh.RecalculateBounds();
        visualMeshFilter.sharedMesh = generatedMesh;
    }

    static void AddFace(
        Vector3 center,
        float size,
        int face,
        List<Vector3> vertices,
        List<int> triangles,
        List<Vector2> uvs)
    {
        float h = size * 0.5f;
        Vector3[] corners =
        {
            new Vector3(-h, -h, -h), new Vector3(h, -h, -h),
            new Vector3(h, h, -h), new Vector3(-h, h, -h),
            new Vector3(-h, -h, h), new Vector3(h, -h, h),
            new Vector3(h, h, h), new Vector3(-h, h, h)
        };

        int[,] faces =
        {
            { 1, 5, 6, 2 }, // +X
            { 4, 0, 3, 7 }, // -X
            { 3, 2, 6, 7 }, // +Y
            { 0, 4, 5, 1 }, // -Y
            { 5, 4, 7, 6 }, // +Z
            { 0, 1, 2, 3 }  // -Z
        };

        int firstVertex = vertices.Count;
        for (int i = 0; i < 4; i++)
        {
            vertices.Add(center + corners[faces[face, i]]);
        }

        triangles.Add(firstVertex);
        triangles.Add(firstVertex + 2);
        triangles.Add(firstVertex + 1);
        triangles.Add(firstVertex);
        triangles.Add(firstVertex + 3);
        triangles.Add(firstVertex + 2);
        uvs.Add(Vector2.zero);
        uvs.Add(Vector2.right);
        uvs.Add(Vector2.one);
        uvs.Add(Vector2.up);
    }

    void BuildCompoundColliders(HashSet<Vector3Int> occupiedCells, float cellSize)
    {
        HashSet<Vector3Int> remaining = new HashSet<Vector3Int>(occupiedCells);
        int colliderCount = 0;

        while (remaining.Count > 0)
        {
            Vector3Int start = FindLowestCell(remaining);
            Vector3Int size = FindLargestBox(start, remaining);

            for (int x = start.x; x < start.x + size.x; x++)
            {
                for (int y = start.y; y < start.y + size.y; y++)
                {
                    for (int z = start.z; z < start.z + size.z; z++)
                    {
                        remaining.Remove(new Vector3Int(x, y, z));
                    }
                }
            }

            GameObject colliderObject = new GameObject("FrozenVoxelCollider");
            colliderObject.transform.SetParent(transform, false);
            BoxCollider collider = colliderObject.AddComponent<BoxCollider>();
            collider.center = new Vector3(
                (start.x + size.x * 0.5f) * cellSize,
                (start.y + size.y * 0.5f) * cellSize,
                (start.z + size.z * 0.5f) * cellSize);
            collider.size = new Vector3(size.x, size.y, size.z) * cellSize;
            generatedColliderObjects.Add(colliderObject);
            colliderCount++;
        }

        if (colliderCount > colliderWarningThreshold)
        {
            Debug.LogWarning(
                $"FrozenCluster '{name}' generated {colliderCount} compound colliders. " +
                "Consider increasing voxelSize or frozenThickness.", this);
        }
    }

    static Vector3Int FindLowestCell(HashSet<Vector3Int> cells)
    {
        bool hasCell = false;
        Vector3Int lowest = Vector3Int.zero;

        foreach (Vector3Int cell in cells)
        {
            if (!hasCell || cell.x < lowest.x ||
                (cell.x == lowest.x && cell.y < lowest.y) ||
                (cell.x == lowest.x && cell.y == lowest.y && cell.z < lowest.z))
            {
                lowest = cell;
                hasCell = true;
            }
        }

        return lowest;
    }

    static Vector3Int FindLargestBox(Vector3Int start, HashSet<Vector3Int> cells)
    {
        int sizeX = 1;
        while (cells.Contains(new Vector3Int(start.x + sizeX, start.y, start.z)))
        {
            sizeX++;
        }

        int sizeY = 1;
        while (LayerIsOccupied(start, sizeX, sizeY + 1, 1, cells))
        {
            sizeY++;
        }

        int sizeZ = 1;
        while (LayerIsOccupied(start, sizeX, sizeY, sizeZ + 1, cells))
        {
            sizeZ++;
        }

        return new Vector3Int(sizeX, sizeY, sizeZ);
    }

    static bool LayerIsOccupied(
        Vector3Int start,
        int sizeX,
        int sizeY,
        int sizeZ,
        HashSet<Vector3Int> cells)
    {
        for (int x = start.x; x < start.x + sizeX; x++)
        {
            for (int y = start.y; y < start.y + sizeY; y++)
            {
                for (int z = start.z; z < start.z + sizeZ; z++)
                {
                    if (!cells.Contains(new Vector3Int(x, y, z)))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    void ClearGeneratedGeometry()
    {
        if (visualMeshFilter)
        {
            visualMeshFilter.sharedMesh = null;
        }

        if (generatedMesh)
        {
            Destroy(generatedMesh);
            generatedMesh = null;
        }

        for (int i = 0; i < generatedColliderObjects.Count; i++)
        {
            if (generatedColliderObjects[i])
            {
                Collider collider = generatedColliderObjects[i].GetComponent<Collider>();
                if (collider)
                {
                    collider.enabled = false;
                }
                Destroy(generatedColliderObjects[i]);
            }
        }
        generatedColliderObjects.Clear();
    }

    static Vector3 CellCenter(Vector3Int cell, float cellSize)
    {
        return new Vector3(
            (cell.x + 0.5f) * cellSize,
            (cell.y + 0.5f) * cellSize,
            (cell.z + 0.5f) * cellSize);
    }

    static Vector3Int FloorToGrid(Vector3 point, float cellSize)
    {
        return new Vector3Int(
            Mathf.FloorToInt(point.x / cellSize),
            Mathf.FloorToInt(point.y / cellSize),
            Mathf.FloorToInt(point.z / cellSize));
    }

    static Vector3Int CeilToGrid(Vector3 point, float cellSize)
    {
        return new Vector3Int(
            Mathf.CeilToInt(point.x / cellSize),
            Mathf.CeilToInt(point.y / cellSize),
            Mathf.CeilToInt(point.z / cellSize));
    }

    private readonly struct Connection
    {
        public readonly Vector3 start;
        public readonly Vector3 end;

        public Connection(Vector3 start, Vector3 end)
        {
            this.start = start;
            this.end = end;
        }
    }

    private sealed class MemberComponentState
    {
        private readonly Renderer[] renderers;
        private readonly bool[] rendererEnabledStates;
        private readonly Collider[] colliders;
        private readonly bool[] colliderEnabledStates;

        public MemberComponentState(Renderer[] renderers, Collider[] colliders)
        {
            this.renderers = renderers;
            this.colliders = colliders;
            rendererEnabledStates = new bool[renderers.Length];
            colliderEnabledStates = new bool[colliders.Length];

            for (int i = 0; i < renderers.Length; i++)
            {
                rendererEnabledStates[i] = renderers[i].enabled;
            }

            for (int i = 0; i < colliders.Length; i++)
            {
                colliderEnabledStates[i] = colliders[i].enabled;
            }
        }

        public void Restore()
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i])
                {
                    renderers[i].enabled = rendererEnabledStates[i];
                }
            }

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i])
                {
                    colliders[i].enabled = colliderEnabledStates[i];
                }
            }
        }
    }
}
