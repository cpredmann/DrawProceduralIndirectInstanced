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
  private int pkernel;

  private Mesh _mesh;
  private Vector3[] points;
  private int[] indices;

  public float radius = 1.0f;

  public Transform _transform;

  public int sides = 8;
  public int loops = 3;

  private void GeneratePoints()
  {
    List<Vector3> pts = new List<Vector3>();
    pts.Add(new Vector3(0.0f, 0.0f, 0.0f));
    var AngleIncrement = (float)Math.PI * 2.0f / sides;
    var RadIncrement = 1.0f / loops * radius;
    for (int l = 0; l < loops; l++)
    {
      var LRad = RadIncrement * (l + 1);
      for (int s = 0; s < sides; s++)
      {
        pts.Add(new Vector3(Mathf.Cos(s * AngleIncrement) * LRad, Mathf.Sin(s * AngleIncrement) * LRad, 0.0f));
      }
    }
    points = pts.ToArray();
  }

  void GenerateTris()
  {
    List<int> ind = new List<int>();
    var count = sides * 2;
    for (int l = 0; l < loops; l++)
    {
      for (int sl = 0; sl < sides; sl++)
      {
        int s = sl + sides + 1;
        int hs = s - sides;
        int offset = sides * (l - 1);
        int p0 = s + offset;
        int p1 = p0 + 1;
        int p3 = hs + offset;
        int p2 = p3 + 1;
        if (p1 > (sides * (l + 1)))
        {
          p1 = (s - sides) + offset + 1;
        }
        if (p2 > (sides * l))
        {
          p2 = (hs - sides) + offset + 1;
        }
        if (l == 0)
        {
          ind.Add(0);
          ind.Add(p0);
          ind.Add(p1);
        }
        else
        {
          ind.Add(p0);
          ind.Add(p1);
          ind.Add(p2);

          ind.Add(p2);
          ind.Add(p3);
          ind.Add(p0);
        }
      }
    }
    indices = ind.ToArray();
  }

  void Start()
  {
    _mesh = new Mesh();
    GetComponent<MeshFilter>().mesh = _mesh;

    pointBuffer = new ComputeBuffer(sides * loops + 1, 3 * sizeof(float));
    indexBuffer = new ComputeBuffer((sides * loops * 6) - (3 * sides), sizeof(uint));
    
    kernel = computeShader.FindKernel("IndexKernel");
    pkernel = computeShader.FindKernel("PointsKernel");

  }
  private void Update()
  {
    computeShader.SetBuffer(kernel, "indices", indexBuffer);
    computeShader.SetBuffer(pkernel, "points", pointBuffer);
    computeShader.SetInt("loops", loops);
    computeShader.SetInt("sides", sides);
    computeShader.Dispatch(kernel, loops, sides, 1);
    computeShader.Dispatch(pkernel, loops, sides, 1);

    GeneratePoints();
    for (int i = 0; i < points.Length; ++i)
    {
      Vector3 pt = points[i];
      var zval = Mathf.Pow(pt.x, 2) + Mathf.Pow(pt.y, 2);
      var vdir = (_transform.position - pt).normalized * zval;
      pt -= vdir;
      points[i] = pt;
    }
    GenerateTris();
    _mesh.SetVertices(points);
    _mesh.SetIndices(indices, MeshTopology.Triangles, 0);

  }

  private void LateUpdate()
  {
    uint[] cid = new uint[indexBuffer.count];
    indexBuffer.GetData(cid);
    Vector3[] pid = new Vector3[pointBuffer.count];
    pointBuffer.GetData(pid);
    
  }
}
