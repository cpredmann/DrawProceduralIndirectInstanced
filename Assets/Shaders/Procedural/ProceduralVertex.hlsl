uniform StructuredBuffer<int> IndexBuffer;
uniform StructuredBuffer<float4> VertexBuffer;

struct Attributes
{
	uint vertexID : SV_VertexID;
};

PackedVaryingsType ProceduralVertex(Attributes input, uint vertexID : SV_VertexID)
{
    float4 vertex = VertexBuffer[vertexID];	

	AttributesMesh am;
	am.positionOS = vertex.xyz;
#ifdef ATTRIBUTES_NEED_NORMAL
    am.normalOS = 0;
#endif
#ifdef ATTRIBUTES_NEED_TANGENT
    am.tangentOS = 0;
#endif
#ifdef ATTRIBUTES_NEED_TEXCOORD0
	am.uv0 = 0;
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
	am.color = 0;
#endif

    VaryingsType varyingsType;
    varyingsType.vmesh = VertMesh(am);
    return PackVaryingsType(varyingsType);
}