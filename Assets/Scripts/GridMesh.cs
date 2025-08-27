using UnityEngine;

public class GridMesh : MonoBehaviour
{
    public int xCount = 10;
    public int zCount = 10;
    public float size = 1f;

    void Start()
    {
        Mesh mesh = new Mesh();
        Vector3[] verts = new Vector3[(xCount + 1) * (zCount + 1)];
        int[] tris = new int[xCount * zCount * 6];

        // 버텍스 생성
        for (int z = 0; z <= zCount; z++)
        {
            for (int x = 0; x <= xCount; x++)
            {
                verts[z * (xCount + 1) + x] = new Vector3(x * size, 0, z * size);
            }
        }

        // 삼각형 인덱스 생성
        int t = 0;
        for (int z = 0; z < zCount; z++)
        {
            for (int x = 0; x < xCount; x++)
            {
                int i = z * (xCount + 1) + x;
                tris[t++] = i;
                tris[t++] = i + xCount + 1;
                tris[t++] = i + 1;
                tris[t++] = i + 1;
                tris[t++] = i + xCount + 1;
                tris[t++] = i + xCount + 2;
            }
        }

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}