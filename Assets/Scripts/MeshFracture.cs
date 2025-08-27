using UnityEngine;

public class MeshFracture : MonoBehaviour
{
    public float explosionForce = 300f;
    public float explosionRadius = 2f;
    public float pieceLifetime = 5f;

    public void Fracture()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] verts = mesh.vertices;
        int[] tris = mesh.triangles;

        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3[] pieceVerts = new Vector3[3];
            int[] pieceTris = { 0, 1, 2 };

            for (int j = 0; j < 3; j++)
                pieceVerts[j] = verts[tris[i + j]];

            Mesh pieceMesh = new Mesh();
            pieceMesh.vertices = pieceVerts;
            pieceMesh.triangles = pieceTris;
            pieceMesh.RecalculateNormals();

            GameObject piece = new GameObject("Piece");
            piece.transform.position = transform.position;
            piece.transform.rotation = transform.rotation;

            piece.AddComponent<MeshFilter>().mesh = pieceMesh;
            piece.AddComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;

            var col = piece.AddComponent<MeshCollider>();
            col.convex = true;

            var rb = piece.AddComponent<Rigidbody>();
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            Destroy(piece, pieceLifetime);
        }

        Destroy(gameObject);
    }
}