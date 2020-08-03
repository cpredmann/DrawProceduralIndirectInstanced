using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ProceduralIndirect : MonoBehaviour
{

    public Material material;
    private Bounds bounds;
    private ComputeBuffer indexBuffer;
    private ComputeBuffer vertexBuffer;
    private CommandBuffer commandBuffer;
    
    // Start is called before the first frame update
    void Start()
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
         indexBuffer = new ComputeBuffer(6, sizeof(int));
         vertexBuffer = new ComputeBuffer(4, 4 * sizeof(float));
         indexBuffer.SetData(indices);
         vertexBuffer.SetData(vertices);
         material.SetBuffer("IndexBuffer", indexBuffer);
         material.SetBuffer("VertexBuffer", vertexBuffer);
         BuildCommandBuffer();
    }

    void BuildCommandBuffer()
    {
        commandBuffer = new CommandBuffer() {name = "DrawProc"};
        commandBuffer.DrawProcedural(Matrix4x4.identity, material, -1, MeshTopology.Triangles, 6, 1, null);
    }


    // Update is called once per frame
    void Update()
    {
        Graphics.DrawProcedural(material, bounds, MeshTopology.Triangles, 6); 
        //Graphics.ExecuteCommandBuffer(commandBuffer); 
    }

    private void OnDestroy()
    {
        indexBuffer?.Dispose();
        vertexBuffer?.Dispose();
    }
}
