uniform StructuredBuffer<int> IndexBuffer : register(t0);
uniform StructuredBuffer<float4> PositionBuffer : register(t1);
uniform StructuredBuffer<float2> UVBuffer : register(t2);
uniform StructuredBuffer<int> FacevaryingIndexBuffer : register(t3);

struct Attributes
{
	uint vertexID : SV_VertexID;
};

PackedVaryingsType FacevaryingVertex(Attributes input, uint vertexID : SV_VertexID)
{
    float3 vertex = PositionBuffer[IndexBuffer[vertexID]].xyz;
    float2 uv = UVBuffer[vertexID]; 

	AttributesMesh am;
	am.positionOS = vertex.xyz;
#ifdef ATTRIBUTES_NEED_NORMAL
    am.normalOS = float3(0.0f, 0.0f, 1.0f);
#endif
#ifdef ATTRIBUTES_NEED_TANGENT
    am.tangentOS = 0;
#endif
#ifdef ATTRIBUTES_NEED_TEXCOORD0
	am.uv0 = uv;
#endif
#ifdef ATTRIBUTES_NEED_TEXCOORD1
    am.uv1 = 0;
#endif
#ifdef ATTRIBUTES_NEED_TEXCOORD2
    am.uv2 = 0;
#endif
#ifdef ATTRIBUTES_NEED_TEXCOORD3
    am.uv3 = 0;
#endif
#ifdef ATTRIBUTES_NEED_COLOR
	am.color = 1;
#endif

    VaryingsType varyingsType;
    varyingsType.vmesh = VertMesh(am);
    return PackVaryingsType(varyingsType);
}