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
    private int generateKernel;
    private int xformKernel;
    private int colorKernel;

    private MaterialPropertyBlock propertyBlock;
    private Bounds bounds;

    Vector4[] positionData;
    Vector4[] colorData;
    Vector4[] normalData;
    int[] indexData;
    int[] indirectData;

    public int triangles = 12;
    public int count = 2;

    void Start()
    {
        indirectArgs = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);
        positionBuffer = new ComputeBuffer(triangles * 3, 4 * sizeof(float));
        colorBuffer = new ComputeBuffer(count, 4 * sizeof(float));
        normalBuffer = new ComputeBuffer(triangles * 3, 4 * sizeof(float));
        indexBuffer = new ComputeBuffer(triangles * 3, sizeof(int));
        xformBuffer = new ComputeBuffer(count, 16 * sizeof(float));
        generateKernel = computeShader.FindKernel("GenerateOutput");
        xformKernel = computeShader.FindKernel("GenerateXforms");
        colorKernel = computeShader.FindKernel("GenerateColors");
        Shader.SetGlobalBuffer("PositionBuffer", positionBuffer);
        Shader.SetGlobalBuffer("ColorBuffer", colorBuffer);
        Shader.SetGlobalBuffer("NormalBuffer", normalBuffer);
        Shader.SetGlobalBuffer("IndexBuffer", indexBuffer);
        Shader.SetGlobalBuffer("XformBuffer", xformBuffer);
        Shader.SetGlobalInt("InstanceCount", count);
        positionData = new Vector4[positionBuffer.count];
        colorData = new Vector4[colorBuffer.count];
        normalData = new Vector4[normalBuffer.count];
        indexData = new int[indexBuffer.count];
        indirectData = new int[4]
        {
            triangles * 3, count, 0, 0
        };
        indirectArgs.SetData(indirectData);
        bounds = new Bounds(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(20.0f, 20.0f, 20.0f));
    }

    void Update()
    {
        computeShader.Dispatch(generateKernel, triangles, 1, 1);
        computeShader.Dispatch(xformKernel, count, 1, 1);
        computeShader.Dispatch(colorKernel, triangles, 1, 1);
        Graphics.DrawProceduralIndirect(proceduralMaterial, bounds, MeshTopology.Triangles, indirectArgs, 0, null, null, UnityEngine.Rendering.ShadowCastingMode.On, true);
    }

    private void OnDestroy()
    {
        indirectArgs.Dispose();
        positionBuffer.Dispose();
        colorBuffer.Dispose();
        normalBuffer.Dispose();
        indexBuffer.Dispose();
        xformBuffer.Dispose();
    }
}
