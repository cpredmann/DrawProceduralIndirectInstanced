using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DrawFaceVarying : MonoBehaviour
{

    private List<Vector4> m_Points;
    private List<Vector2> m_UVs;
    private List<Int32> m_Indices;
    private List<Int32> m_FaceVaryingIndices;
    private Int32 m_FaceCount;

    private MeshFilter m_MeshFilter;
    private Mesh m_Mesh;

    public Material FacevaryingMaterial;

    private GraphicsBuffer m_VertexIndexBuffer;
    private GraphicsBuffer m_PointsBuffer;
    private GraphicsBuffer m_UVBuffer;
    private GraphicsBuffer m_FacevaryingIndexBuffer;

    private Bounds m_Bounds;
    private static readonly int IndexBuffer = Shader.PropertyToID("IndexBuffer");
    private static readonly int PositionBuffer = Shader.PropertyToID("PositionBuffer");
    private static readonly int UVBuffer = Shader.PropertyToID("UVBuffer");
    private static readonly int FacevaryingIndexBuffer = Shader.PropertyToID("FacevaryingIndexBuffer");

    void Start()
    {
        //m_MeshFilter = GetComponent<MeshFilter>();
        //m_Mesh = new Mesh();
        //m_MeshFilter.sharedMesh = m_Mesh;
        m_Bounds = new Bounds(Vector3.zero, new Vector3(10.0f, 10.0f, 10.0f));

        m_Points = new List<Vector4>
        {
            new Vector4(-0.5f, -0.5f, -0.5f),
            new Vector4(0.5f, -0.5f, -0.5f),
            new Vector4(0.5f, -0.5f, 0.5f),
            new Vector4(-0.5f, -0.5f, 0.5f),
            new Vector4(-0.5f, 0.5f, -0.5f),
            new Vector4(0.5f, 0.5f, -0.5f),
            new Vector4(0.5f, 0.5f, 0.5f),
            new Vector4(-0.5f, 0.5f, 0.5f),
        };
        m_UVs = new List<Vector2>
        {
            new Vector2(0.00f, 0.00f),
            new Vector2(0.00f, 1.00f),
            new Vector2(1.00f, 1.00f),
            new Vector2(0.00f, 1.00f),
            new Vector2(1.00f, 1.00f),
            new Vector2(1.00f, 0.00f),
            new Vector2(0.00f, 1.00f),
            new Vector2(1.00f, 1.00f),
            new Vector2(1.00f, 0.00f),
            new Vector2(0.00f, 1.00f),
            new Vector2(1.00f, 1.00f),
            new Vector2(1.00f, 0.00f),
            new Vector2(0.00f, 1.00f),
            new Vector2(1.00f, 1.00f),
            new Vector2(1.00f, 0.00f),
            new Vector2(0.00f, 1.00f),
            new Vector2(1.00f, 1.00f),
            new Vector2(1.00f, 0.00f),
            new Vector2(1.00f, 0.00f),
            new Vector2(0.00f, 0.00f),
            new Vector2(0.00f, 1.00f),
            new Vector2(1.00f, 0.00f),
            new Vector2(0.00f, 0.00f),
            new Vector2(0.00f, 1.00f),
            new Vector2(1.00f, 0.00f),
            new Vector2(0.00f, 0.00f),
            new Vector2(0.00f, 1.00f),
            new Vector2(1.00f, 0.00f),
            new Vector2(0.00f, 0.00f),
            new Vector2(0.00f, 1.00f),
            new Vector2(1.00f, 0.00f),
            new Vector2(0.00f, 0.00f),
            new Vector2(0.00f, 1.00f),
            new Vector2(1.00f, 1.00f),
            new Vector2(1.00f, 0.00f),
            new Vector2(0.00f, 0.00f),
        };
        m_Indices = new List<Int32>
        {
            1,
            0,
            4,
            2,
            1,
            5,
            3,
            2,
            6,
            0,
            3,
            7,
            2,
            3,
            0,
            5,
            4,
            7,
            7,
            6,
            5,
            0,
            1,
            2,
            7,
            4,
            0,
            6,
            7,
            3,
            5,
            6,
            2,
            4,
            5,
            1
        };
        m_FaceCount = m_Indices.Count / 3;
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
            22,
            23,
            24,
            25,
            26,
            27,
            28,
            29,
            30,
            31,
            32,
            33,
            34,
            35
        };

        m_VertexIndexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, m_Indices.Count, sizeof(Int32));
        m_PointsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, m_Points.Count, 4 * sizeof(float));
        m_UVBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, m_UVs.Count, 2 * sizeof(float));
        m_FacevaryingIndexBuffer =
            new GraphicsBuffer(GraphicsBuffer.Target.Structured, m_FaceVaryingIndices.Count, sizeof(Int32));

        m_VertexIndexBuffer.SetData(m_Indices);
        m_PointsBuffer.SetData(m_Points);
        m_UVBuffer.SetData(m_UVs);
        m_FacevaryingIndexBuffer.SetData(m_FaceVaryingIndices);
        
        FacevaryingMaterial.SetBuffer(IndexBuffer, m_VertexIndexBuffer);
        FacevaryingMaterial.SetBuffer(PositionBuffer, m_PointsBuffer);
        FacevaryingMaterial.SetBuffer(UVBuffer, m_UVBuffer);
        FacevaryingMaterial.SetBuffer(FacevaryingIndexBuffer, m_FacevaryingIndexBuffer);

        //m_Mesh.SetVertices(m_Points);
        //m_Mesh.SetIndices(m_Indices, MeshTopology.Quads, 0);

        RenderPipelineManager.beginCameraRendering += DrawCube;
    }

    //private void Update()
    //{
    //    MaterialPropertyBlock block = new MaterialPropertyBlock();
    //    //block.SetBuffer("IndexBuffer", m_VertexIndexBuffer);
    //    block.SetBuffer("PositionBuffer", m_PointsBuffer);
    //    block.SetBuffer("UVBuffer", m_UVBuffer);
    //    block.SetBuffer("FacevaryingIndexBuffer", m_FacevaryingIndexBuffer);
    //    Graphics.DrawProcedural(FacevaryingMaterial, m_Bounds, MeshTopology.Quads, m_VertexIndexBuffer, m_Indices.Count, 1, properties: block);
    //}

    void DrawCube(ScriptableRenderContext context, Camera camera)
    {
        //MaterialPropertyBlock block = new MaterialPropertyBlock();
        //block.SetBuffer("IndexBuffer", m_VertexIndexBuffer);
        //block.SetBuffer("PositionBuffer", m_PointsBuffer);
        //block.SetBuffer("UVBuffer", m_UVBuffer);
        //block.SetBuffer("FacevaryingIndexBuffer", m_FacevaryingIndexBuffer);
        Graphics.DrawProcedural(FacevaryingMaterial, m_Bounds, MeshTopology.Triangles, m_FacevaryingIndexBuffer, m_Indices.Count, 1);
        //Graphics.DrawProcedural(FacevaryingMaterial, m_Bounds, MeshTopology.Quads, m_Indices.Count, 1, Camera.current);
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= DrawCube;
        m_VertexIndexBuffer?.Dispose();
        m_PointsBuffer?.Dispose();
        m_UVBuffer?.Dispose();
        m_FacevaryingIndexBuffer?.Dispose();
    }
}
