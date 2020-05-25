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

  private int generateKernel;
  private int xformKernel;
  private int colorKernel;
  private int shapeKernel;

  private MaterialPropertyBlock propertyBlock;
  private Bounds bounds;

  public int triangles = 12;
  public int count = 2;
  public float radius = 1.0f;
  public int sides = 8;
  public int loops = 3;

  private Mesh _mesh;

  void Start()
  {
    _mesh = new Mesh();
    GetComponent<MeshFilter>().mesh = _mesh;

    indirectArgs = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);
    positionBuffer = new ComputeBuffer(triangles * 3, 4 * sizeof(float));
    colorBuffer = new ComputeBuffer(count, 4 * sizeof(float));
    normalBuffer = new ComputeBuffer(triangles * 3, 4 * sizeof(float));
    indexBuffer = new ComputeBuffer(triangles * 3, sizeof(int));
    xformBuffer = new ComputeBuffer(count, 16 * sizeof(float));

    shapePositionBuffer = new ComputeBuffer(sides * loops + 1, 3 * sizeof(float));
    shapeIndexBuffer = new ComputeBuffer((sides * loops * 6) - (3 * sides), sizeof(uint));

    generateKernel = computeShader.FindKernel("GenerateOutput");
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
    computeShader.SetInt("TotalLoops", loops);
    computeShader.SetInt("TotalSides", sides);
    computeShader.SetInt("InstanceCount", count);
    computeShader.SetInt("Triangles", triangles);

    bounds = new Bounds(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(20.0f, 20.0f, 20.0f));
  }

  void Update()
  {
    computeShader.SetFloat("ShapeRadius", radius);
    computeShader.Dispatch(shapeKernel, loops, sides, 1);
    computeShader.Dispatch(generateKernel, triangles, 1, 1);
    computeShader.Dispatch(xformKernel, count, 1, 1);
    computeShader.Dispatch(colorKernel, triangles, 1, 1);
    Graphics.DrawProceduralIndirect(proceduralMaterial, bounds, MeshTopology.Triangles, indirectArgs, 0, null, null, UnityEngine.Rendering.ShadowCastingMode.On, true);

    int[] cid = new int[shapeIndexBuffer.count];
    shapeIndexBuffer.GetData(cid);
    Vector3[] pid = new Vector3[shapePositionBuffer.count];
    shapePositionBuffer.GetData(pid);
    _mesh.SetVertices(pid);
    _mesh.SetIndices(cid, MeshTopology.Triangles, 0);
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
    shapeIndexBuffer.Dispose();
  }
}
