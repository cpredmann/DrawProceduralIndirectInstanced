using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawFaceVarying : MonoBehaviour
{

    private List<Vector3> m_Points;
    private List<Vector2> m_UVs;
    private List<Int32> m_Indices;
    private List<Int32> m_FaceVaryingIndices;

    private MeshFilter m_MeshFilter;
    private Mesh m_Mesh;
    
    void Start()
    {
        m_MeshFilter = GetComponent<MeshFilter>();
        m_Mesh = new Mesh();
        m_MeshFilter.sharedMesh = m_Mesh;
        
        m_Points = new List<Vector3>
        {
            new Vector3( -0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f, -0.5f, 0.5f),
            new Vector3( -0.5f, -0.5f, 0.5f),
            new Vector3( -0.5f, 0.5f, -0.5f),
            new Vector3( 0.5f, 0.5f, -0.5f),
            new Vector3( 0.5f, 0.5f, 0.5f),
            new Vector3( -0.5f, 0.5f, 0.5f),
        };
        m_UVs = new List<Vector2>
        {
            new Vector2( 0.3333330f, 0.6666670f),
            new Vector2( 0.3333330f, 0.9822830f),
            new Vector2( 0.6489500f, 0.9822830f),
            new Vector2( 0.6489500f, 0.6666670f),
            new Vector2( 0.0000000f, 0.6666670f),
            new Vector2( 0.0000000f, 0.9822830f),
            new Vector2( 0.3156160f, 0.9822830f),
            new Vector2( 0.3156160f, 0.6666670f),
            new Vector2( 0.0000000f, 0.3333330f),
            new Vector2( 0.0000000f, 0.6489500f),
            new Vector2( 0.3156160f, 0.6489500f),
            new Vector2( 0.3156160f, 0.3333330f),
            new Vector2( 0.3333330f, 0.3333330f),
            new Vector2( 0.3333330f, 0.6489500f),
            new Vector2( 0.6489500f, 0.6489500f),
            new Vector2( 0.6489500f, 0.3333330f),
            new Vector2( 0.6489500f, 0.3156160f),
            new Vector2( 0.6489500f, 0.0000000f),
            new Vector2( 0.3333330f, 0.0000000f),
            new Vector2( 0.3333330f, 0.3156160f),
            new Vector2( 0.3156160f, 0.3156160f),
            new Vector2( 0.3156160f, 0.0000000f),
            new Vector2( 0.0000000f, 0.0000000f),
            new Vector2( 0.0000000f, 0.3156160f),
        };
        m_Indices = new List<Int32>
        {
            1,
            5,
            4,
            0,
            2,
            6,
            5,
            1,
            3,
            7,
            6,
            2,
            0,
            4,
            7,
            3,
            2,
            1,
            0,
            3,
            5,
            6,
            7,
            4
        };
        m_FaceVaryingIndices = new List<Int32>
        {
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12,
            13,
            14,
            15,
            16,
            17,
            18,
            19,
            20,
            21,
            21,
            23,
        };
        
        m_Mesh.SetVertices(m_Points);
        m_Mesh.SetIndices(m_Indices, MeshTopology.Quads, 0);
    }
    
    
}
