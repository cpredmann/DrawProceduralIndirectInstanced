using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProceduralIndirectInstanced : MonoBehaviour
{

  public ComputeShader computeShader;
  public Material proceduralMaterial;
  private ComputeBuffer indirectArgs;
  private ComputeBuffer positionBuffer;
  private ComputeBuffer colorBuffer;
  private ComputeBuffer normalBuffer;
  private ComputeBuffer indexBuffer;
  private int generateKernel;

  private Bounds bounds;

  Vector4[] positionData;
  Vector4[] colorData;
  Vector4[] normalData;
  int[] indexData;

  const int triangles = 12;

  void Start()
  {
    indirectArgs = new ComputeBuffer(4, sizeof(int));
    positionBuffer = new ComputeBuffer(triangles * 3, 4 * sizeof(float));
    colorBuffer = new ComputeBuffer(triangles * 3, 4 * sizeof(float));
    normalBuffer = new ComputeBuffer(triangles * 3, 4 * sizeof(float));
    indexBuffer = new ComputeBuffer(triangles * 3, sizeof(int));
    generateKernel = computeShader.FindKernel("GenerateOutput");
    Shader.SetGlobalBuffer("PositionBuffer", positionBuffer);
    Shader.SetGlobalBuffer("ColorBuffer", colorBuffer);
    Shader.SetGlobalBuffer("NormalBuffer", normalBuffer);
    Shader.SetGlobalBuffer("IndexBuffer", indexBuffer);
    positionData = new Vector4[positionBuffer.count];
    colorData = new Vector4[colorBuffer.count];
    normalData = new Vector4[normalBuffer.count];
    indexData = new int[indexBuffer.count];
    bounds = new Bounds(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(20.0f, 20.0f, 20.0f));
  }

  void Update()
  {
    computeShader.Dispatch(generateKernel, triangles, 1, 1);
    //positionBuffer.GetData(positionData);
    //colorBuffer.GetData(colorData);
    //normalBuffer.GetData(normalData);
    //indexBuffer.GetData(indexData);
    Graphics.DrawProcedural(proceduralMaterial, bounds, MeshTopology.Triangles, triangles * 3, 1, null, null, UnityEngine.Rendering.ShadowCastingMode.On, true);
  }

  private void OnDestroy()
  {
    indirectArgs.Dispose();
    positionBuffer.Dispose();
    colorBuffer.Dispose();
    normalBuffer.Dispose();
    indexBuffer.Dispose();
  }
}
