using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
// [RequireComponent(typeof(MeshCollider))]
public class Ball : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    // MeshCollider meshCollider;

    [SerializeField] Material mat;
    [SerializeField] float radius = 1f;

    [Header("Merge Settings")]
    [SerializeField] float mergeStrength = 0.8f;

    [HideInInspector] public Mesh mesh;
    [HideInInspector] public List<Vector3> vertices;
    [HideInInspector] public List<int> triangles;
    public List<Vector2> UVs;

    private List<Vector3> baseVertices;
    private readonly List<Ball> nearbyBalls = new();

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = mat;
        // meshCollider = GetComponent<MeshCollider>();

        vertices = new List<Vector3>();
        triangles = new List<int>();
        UVs = new List<Vector2>();
        baseVertices = new List<Vector3>();

        mesh = new Mesh();

        ClearData();
        GenerateMesh();
        UploadMesh();
    }

    void Update()
    {
        DeformTowardsNearbyBalls();

        mesh.SetVertices(vertices);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public void ClearData()
    {
        vertices.Clear();
        triangles.Clear();
        UVs.Clear();
        mesh.Clear();
    }

    public void GenerateMesh()
    {
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        vertices.AddRange(new Vector3[]
        {
            new Vector3(-1,  t,  0),
            new Vector3( 1,  t,  0),
            new Vector3(-1, -t,  0),
            new Vector3( 1, -t,  0),

            new Vector3( 0, -1,  t),
            new Vector3( 0,  1,  t),
            new Vector3( 0, -1, -t),
            new Vector3( 0,  1, -t),

            new Vector3( t,  0, -1),
            new Vector3( t,  0,  1),
            new Vector3(-t,  0, -1),
            new Vector3(-t,  0,  1)
        });

        // Project vertices onto sphere
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = vertices[i].normalized * radius;
        }

        triangles.AddRange(new int[]
        {
            0,11,5,
            0,5,1,
            0,1,7,
            0,7,10,
            0,10,11,

            1,5,9,
            5,11,4,
            11,10,2,
            10,7,6,
            7,1,8,

            3,9,4,
            3,4,2,
            3,2,6,
            3,6,8,
            3,8,9,

            4,9,5,
            2,4,11,
            6,2,10,
            8,6,7,
            9,8,1
        });

        baseVertices.Clear();

        for (int i = 0; i < vertices.Count; i++)
        {
            baseVertices.Add(vertices[i]);
        }
    }

    void DeformTowardsNearbyBalls()
    {
        // Reset to original sphere shape
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = baseVertices[i];
        }

        foreach (Ball other in nearbyBalls)
        {
            if (other == null)
                continue;

            Vector3 midpoint =
                (transform.position + other.transform.position) * 0.5f;

            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3 worldVertex =
                    transform.TransformPoint(vertices[i]);

                Vector3 normal =
                    (worldVertex - transform.position).normalized;

                Vector3 dirToOther =
                    (other.transform.position - worldVertex).normalized;

                float dot =
                    Vector3.Dot(normal, dirToOther);

                if (dot <= 0f)
                    continue;

                // Much stronger falloff
                float influence = Mathf.Pow(dot, 0.25f);

                // Pull hard toward midpoint
                worldVertex = Vector3.Lerp(
                    worldVertex,
                    midpoint,
                    influence * mergeStrength);

                vertices[i] =
                    transform.InverseTransformPoint(worldVertex);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();

        if (ball == null || ball == this)
            return;

        if (!nearbyBalls.Contains(ball))
            nearbyBalls.Add(ball);
    }

    private void OnTriggerExit(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();

        if (ball == null)
            return;

        nearbyBalls.Remove(ball);
    }

    public void UploadMesh()
    {
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
        // meshCollider.sharedMesh = mesh;
    }
}