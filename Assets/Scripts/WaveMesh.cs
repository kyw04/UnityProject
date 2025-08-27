using UnityEngine;

public class WaveMesh : MonoBehaviour
{
    public float amplitude = 0.5f;
    public float frequency = 1f;
    private Vector3[] baseVerts;
    private Mesh mesh;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        baseVerts = mesh.vertices;
    }

    void Update()
    {
        Vector3[] verts = new Vector3[baseVerts.Length];
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] = baseVerts[i];
            verts[i].y += Mathf.Sin(Time.time * frequency + verts[i].x) * amplitude;
        }
        mesh.vertices = verts;
        mesh.RecalculateNormals();
    }
}