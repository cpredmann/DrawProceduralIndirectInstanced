using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadAttributes : MonoBehaviour
{

    List<Vector3> positions;
    List<Vector3> normals;
    List<Vector3> colors;
    void Start()
    {
        positions = new List<Vector3>();
        normals = new List<Vector3>();
        colors = new List<Vector3>();

        var mesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.GetVertices(positions);
        mesh.GetUVs(0, colors);
        mesh.GetNormals(normals);
    }

    void Update()
    {
        
    }
}
