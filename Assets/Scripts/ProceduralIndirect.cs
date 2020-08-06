﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class ProceduralIndirect : MonoBehaviour
{

    public Material material;
    private Bounds bounds;
    //private ComputeBuffer indexBuffer;
    //private ComputeBuffer vertexBuffer;
    private GraphicsBuffer indexBuffer;
    private GraphicsBuffer vertexBuffer;
    
    private CommandBuffer commandBuffer;
    private Camera main;
    
    // Start is called before the first frame update
    void OnEnable()
    {
         bounds = new Bounds(Vector3.zero, new Vector3(10.0f, 10.0f, 10.0f));
         int[] indices = new[] {0, 1, 2, 1, 3, 2};
         Vector4[] vertices = new[]
         {
             new Vector4(0.0f, 0.0f, 0.0f, 0.0f), 
             new Vector4(0.0f, 1.0f, 0.0f, 0.0f), 
             new Vector4(0.0f, 0.0f, 1.0f, 0.0f), 
             new Vector4(0.0f, 1.0f, 1.0f, 0.0f), 
         };
         
         //indexBuffer = new ComputeBuffer(6, sizeof(int));
         //vertexBuffer = new ComputeBuffer(4, 4 * sizeof(float));
         indexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Index, 6, sizeof(int));
         vertexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 4, 4 * sizeof(float));
         
         indexBuffer.SetData(indices);
         vertexBuffer.SetData(vertices);
         //material.SetBuffer("IndexBuffer", indexBuffer);
         //material.SetBuffer("VertexBuffer", vertexBuffer);
         //BuildCommandBuffer();
         //var pipelineAsset = GraphicsSettings.renderPipelineAsset;
         RenderPipelineManager.beginCameraRendering += DrawProc;
    }

    void BuildCommandBuffer()
    {
        commandBuffer = new CommandBuffer() {name = "DrawProc"};
        commandBuffer.DrawProcedural(indexBuffer, Matrix4x4.identity, material, -1, MeshTopology.Triangles, 6, 1);
    }

    void DrawProc(ScriptableRenderContext context, Camera camera)
    {
        if (camera.cameraType != CameraType.Reflection)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            block.SetBuffer("VertexBuffer", vertexBuffer);
            Graphics.DrawProcedural(material, bounds, MeshTopology.Triangles, indexBuffer, 6, 1, properties: block);
        }
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= DrawProc;
        indexBuffer?.Dispose();
        vertexBuffer?.Dispose();
        commandBuffer?.Dispose();
    }
}
