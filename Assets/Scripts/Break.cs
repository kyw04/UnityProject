using System.Collections.Generic;
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

        List<Vector3> newVerts = new List<Vector3>();
        List<int> newTris = new List<int>();

        for (int i = 0; i < oldTris.Length; i += 3)
        {
            int t0 = oldTris[i];
            int t1 = oldTris[i + 1];
            int t2 = oldTris[i + 2];

            Vector3 v0 = oldVerts[t0];
            Vector3 v1 = oldVerts[t1];
            Vector3 v2 = oldVerts[t2];

            Vector3 m0 = (v0 + v1) * 0.5f;
            Vector3 m1 = (v1 + v2) * 0.5f;
            Vector3 m2 = (v2 + v0) * 0.5f;

            int m0Index = newVerts.Count; newVerts.Add(m0);
            int m1Index = newVerts.Count; newVerts.Add(m1);
            int m2Index = newVerts.Count; newVerts.Add(m2);

            newTris.Add(t0); newTris.Add(m0Index); newTris.Add(m2Index);
            newTris.Add(m0Index); newTris.Add(t1); newTris.Add(m1Index);
            newTris.Add(m2Index); newTris.Add(m1Index); newTris.Add(t2);
            newTris.Add(m0Index); newTris.Add(m1Index); newTris.Add(m2Index);
        }
        
        mesh.vertices = newVerts.ToArray();
        mesh.triangles = newTris.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
