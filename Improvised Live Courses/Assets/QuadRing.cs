using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class QuadRing : MonoBehaviour
{
    [SerializeField, Range(0.01f, 2f)] float radiusInner;
    [SerializeField, Range(0.01f, 2f)] float thickness;
    [SerializeField, Range(3, 64)] int angularSegmentsCount;

    Mesh mesh;

    float RadiusOuter => radiusInner + thickness;
    int VertexCount => angularSegmentsCount * 2;

    private void OnDrawGizmosSelected()
    {
        // Gizmos.DrawWireSphere(transform.position, radiusInner);
        // Gizmos.DrawWireSphere(transform.position, RadiusOuter);
        GizmosFs.DrawWireCircle(transform.position, transform.rotation, radiusInner, angularSegmentsCount);
        GizmosFs.DrawWireCircle(transform.position, transform.rotation, RadiusOuter, angularSegmentsCount);
    }

    private void Awake()
    {
        mesh = new Mesh();
        mesh.name = "QuadRing";
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    private void Update() => GenerateMesh();

    private void GenerateMesh()
    {
        mesh.Clear();

        int vCount = VertexCount;
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < angularSegmentsCount; i++)
        {
            float t = i / (float)angularSegmentsCount;
            float angRad = t * MathFs.TWOPI;
            Vector2 dir = MathFs.GetUnitVectorByAngle(angRad);

            Vector3 zOffset = Vector3.forward * Mathf.Cos(angRad * 4f);

            vertices.Add((Vector3)(dir * RadiusOuter) + zOffset);
            vertices.Add((Vector3)(dir * radiusInner) + zOffset);
            normals.Add(Vector3.forward);
            normals.Add(Vector3.forward);
        }

        List<int> triangleIndices = new List<int>();
        for (int i = 0; i < angularSegmentsCount; i++)
        {
            int indexRoot = 2 * i;
            int indexInnerRoot = indexRoot + 1;
            int indexOuterNext = (indexRoot + 2) % vCount;
            int indexInnerNext = (indexRoot + 3) % vCount;

            triangleIndices.Add(indexRoot);
            triangleIndices.Add(indexOuterNext);
            triangleIndices.Add(indexInnerNext);

            triangleIndices.Add(indexRoot);
            triangleIndices.Add(indexInnerNext);
            triangleIndices.Add(indexInnerRoot);
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangleIndices, 0);
        // mesh.RecalculateNormals();
        mesh.SetNormals(normals);
    }
}