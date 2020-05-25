using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ProceduralIndirectInstanced : MonoBehaviour
{

    public ComputeShader computeShader;
    public Material proceduralMaterial;
    private ComputeBuffer indirectArgs;
    private ComputeBuffer positionBuffer;
    private ComputeBuffer colorBuffer;
    private ComputeBuffer normalBuffer;
    private ComputeBuffer indexBuffer;
    private ComputeBuffer xformBuffer;

    private ComputeBuffer shapeIndexBuffer;
    private ComputeBuffer shapePositionBuffer;
    private ComputeBuffer shapeNormalBuffer;

    private int xformKernel;
    private int colorKernel;
    private int shapeKernel;

    private MaterialPropertyBlock propertyBlock;
    private Bounds bounds;

    public int triangles = 12;
    private int count = 2;
    public float radius = 1.0f;
    public int sides = 8;
    public int loops = 3;

    public MeshFilter meshfilter;
    private List<Vector3> inputPositions;
    private List<Vector3> inputNormals;
    private List<Vector4> inputColors;

    void Start()
    {
        inputPositions = new List<Vector3>();
        inputNormals = new List<Vector3>();
        inputColors = new List<Vector4>();
        var mesh = meshfilter.sharedMesh;
        mesh.GetVertices(inputPositions);
        mesh.GetNormals(inputNormals);
        mesh.GetUVs(0, inputColors);
        count = inputNormals.Count;

        indirectArgs = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);

        positionBuffer = new ComputeBuffer(count, 3 * sizeof(float));
        positionBuffer.SetData(inputPositions);
        colorBuffer = new ComputeBuffer(count, 4 * sizeof(float));
        colorBuffer.SetData(inputColors);
        normalBuffer = new ComputeBuffer(count, 3 * sizeof(float));
        normalBuffer.SetData(inputNormals);

        xformBuffer = new ComputeBuffer(count, 16 * sizeof(float));

        indexBuffer = new ComputeBuffer(triangles * 3, sizeof(int));

        shapePositionBuffer = new ComputeBuffer(sides * loops + 1, 3 * sizeof(float));
        shapeNormalBuffer = new ComputeBuffer(sides * loops + 1, 3 * sizeof(float));
        shapeIndexBuffer = new ComputeBuffer((sides * loops * 6) - (3 * sides), sizeof(int));

        xformKernel = computeShader.FindKernel("GenerateXforms");
        colorKernel = computeShader.FindKernel("GenerateColors");
        shapeKernel = computeShader.FindKernel("GenerateShape");

        Shader.SetGlobalBuffer("PositionBuffer", positionBuffer);
        Shader.SetGlobalBuffer("ColorBuffer", colorBuffer);
        Shader.SetGlobalBuffer("NormalBuffer", normalBuffer);
        Shader.SetGlobalBuffer("IndexBuffer", indexBuffer);
        Shader.SetGlobalBuffer("XformBuffer", xformBuffer);
        Shader.SetGlobalBuffer("IndirectArguments", indirectArgs);
        Shader.SetGlobalBuffer("ShapeIndexBuffer", shapeIndexBuffer);
        Shader.SetGlobalBuffer("ShapePointBuffer", shapePositionBuffer);
        Shader.SetGlobalBuffer("ShapeNormalBuffer", shapeNormalBuffer);
        computeShader.SetInt("TotalLoops", loops);
        computeShader.SetInt("TotalSides", sides);
        computeShader.SetInt("InstanceCount", count);
        computeShader.SetInt("Triangles", triangles);

        bounds = new Bounds(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(200.0f, 200.0f, 200.0f));
        computeShader.SetFloat("ShapeRadius", radius);
        computeShader.Dispatch(shapeKernel, loops, sides, 1);
    }

    void Update()
    {
        computeShader.Dispatch(xformKernel, count, 1, 1);
        computeShader.Dispatch(colorKernel, triangles, 1, 1);
        Graphics.DrawProceduralIndirect(proceduralMaterial, bounds, MeshTopology.Triangles, indirectArgs, 0, null, null, UnityEngine.Rendering.ShadowCastingMode.TwoSided, true);
    }

    private void OnDestroy()
    {
        indirectArgs.Dispose();
        positionBuffer.Dispose();
        colorBuffer.Dispose();
        normalBuffer.Dispose();
        indexBuffer.Dispose();
        xformBuffer.Dispose();
        shapePositionBuffer.Dispose();
        shapeNormalBuffer.Dispose();
        shapeIndexBuffer.Dispose();
    }
}
