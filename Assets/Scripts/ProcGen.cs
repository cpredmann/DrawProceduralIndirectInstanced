using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;

public class ProcGen : MonoBehaviour
{

  private ComputeBuffer indexBuffer;
  private ComputeBuffer pointBuffer;
  public ComputeShader computeShader;
  private int kernel;

  private Mesh _mesh;

  public float radius = 1.0f;

  public Transform _transform;

  public int sides = 8;
  public int loops = 3;

  void Start()
  {
    _mesh = new Mesh();
    GetComponent<MeshFilter>().mesh = _mesh;

    pointBuffer = new ComputeBuffer(sides * loops + 1, 3 * sizeof(float));
    indexBuffer = new ComputeBuffer((sides * loops * 6) - (3 * sides), sizeof(uint));

    kernel = computeShader.FindKernel("IndexKernel");

  }
  private void Update()
  {
    computeShader.SetBuffer(kernel, "ShapeIndexBuffer", indexBuffer);
    computeShader.SetBuffer(kernel, "ShapePointBuffer", pointBuffer);
    computeShader.SetInt("TotalLoops", loops);
    computeShader.SetInt("TotalSides", sides);
    computeShader.Dispatch(kernel, loops, sides, 1);

    int[] cid = new int[indexBuffer.count];
    indexBuffer.GetData(cid);
    Vector3[] pid = new Vector3[pointBuffer.count];
    pointBuffer.GetData(pid);
    _mesh.SetVertices(pid);
    _mesh.SetIndices(cid, MeshTopology.Triangles, 0);

  }
  private void OnDestroy()
  {
    pointBuffer.Dispose();
    indexBuffer.Dispose();
  }
}
