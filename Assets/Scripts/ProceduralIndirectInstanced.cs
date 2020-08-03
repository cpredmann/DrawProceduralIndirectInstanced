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
    private ComputeBuffer xformBuffer;

    private ComputeBuffer shapeIndexBuffer;
    private ComputeBuffer shapePositionBuffer;
    private ComputeBuffer shapeNormalBuffer;

    private int xformKernel;
    private int colorKernel;
    private int shapeKernel;

    private MaterialPropertyBlock propertyBlock;
    private Bounds bounds;

    private int count = 2;
    public float radius = 1.0f;
    public float parabolic = 1.0f;
    public int sides = 8;
    public int loops = 3;

    public MeshFilter meshfilter;
    private List<Vector3> inputPositions;
    private List<Vector3> inputNormals;
    private List<Vector4> inputColors;

    private CommandBuffer commandBuffer;
    
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


        shapePositionBuffer = new ComputeBuffer(sides * loops + 1, 3 * sizeof(float));
        shapeNormalBuffer = new ComputeBuffer(sides * loops + 1, 3 * sizeof(float));
        shapeIndexBuffer = new ComputeBuffer((sides * loops * 6) - (3 * sides), sizeof(int));

        xformKernel = computeShader.FindKernel("GenerateXforms");
        colorKernel = computeShader.FindKernel("GenerateColors");
        shapeKernel = computeShader.FindKernel("GenerateShape");

        Shader.SetGlobalBuffer("PositionBuffer", positionBuffer);
        Shader.SetGlobalBuffer("ColorBuffer", colorBuffer);
        Shader.SetGlobalBuffer("NormalBuffer", normalBuffer);
        Shader.SetGlobalBuffer("XformBuffer", xformBuffer);
        Shader.SetGlobalBuffer("IndirectArguments", indirectArgs);
        Shader.SetGlobalBuffer("ShapeIndexBuffer", shapeIndexBuffer);
        Shader.SetGlobalBuffer("ShapePointBuffer", shapePositionBuffer);
        Shader.SetGlobalBuffer("ShapeNormalBuffer", shapeNormalBuffer);
        computeShader.SetInt("TotalLoops", loops);
        computeShader.SetInt("TotalSides", sides);
        computeShader.SetInt("InstanceCount", count);
        computeShader.SetFloat("ShapeRadius", radius);
        computeShader.SetFloat("Flattening", parabolic);

        bounds = new Bounds(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(200.0f, 200.0f, 200.0f));
        computeShader.Dispatch(shapeKernel, loops, sides, 1);
        
        BuildCommandBuffer();
    }

    void BuildCommandBuffer()
    {
        commandBuffer = new CommandBuffer {name = "DrawProc"};
        commandBuffer.BeginSample("Compute");
        commandBuffer.DispatchCompute(computeShader, xformKernel, count, 1, 1);
        commandBuffer.DispatchCompute(computeShader, colorKernel, count, 1, 1);
        commandBuffer.EndSample("Compute");
        //commandBuffer.BeginSample("Draw");
        //commandBuffer.DrawProceduralIndirect(Matrix4x4.identity, proceduralMaterial, -1, MeshTopology.Triangles, indirectArgs, 0);
        //commandBuffer.EndSample("Draw");
        
    }

    void Update()
    {
        Graphics.ExecuteCommandBuffer(commandBuffer);
        //computeShader.Dispatch(xformKernel, count, 1, 1);
        //computeShader.Dispatch(colorKernel, count, 1, 1);
        Graphics.DrawProceduralIndirect(proceduralMaterial, bounds, MeshTopology.Triangles, indirectArgs, 0, null, null, UnityEngine.Rendering.ShadowCastingMode.On, true);
    }

    private void OnDestroy()
    {
        indirectArgs?.Dispose();
        positionBuffer?.Dispose();
        colorBuffer?.Dispose();
        normalBuffer?.Dispose();
        xformBuffer?.Dispose();
        shapePositionBuffer?.Dispose();
        shapeNormalBuffer?.Dispose();
        shapeIndexBuffer?.Dispose();
    }
}
