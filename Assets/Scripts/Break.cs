using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Break : MonoBehaviour
{
    private Mesh mesh;

    private void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    public void Subdivision()
    {
        Vector3[] oldVerts = mesh.vertices;
        int[] oldTris = mesh.triangles;
        
        foreach (var vert in oldVerts)
        {
            Debug.Log($"vert {vert}");
        }

        foreach (var tris in oldTris)
        {
            Debug.Log($"tris {tris}");
        }
    }
}
