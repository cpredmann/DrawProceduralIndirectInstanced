uniform StructuredBuffer<float4> ColorBuffer;

uniform StructuredBuffer<int> ShapeIndexBuffer;
uniform StructuredBuffer<float3> ShapePointBuffer;
uniform StructuredBuffer<float3> ShapeNormalBuffer;

uniform StructuredBuffer<int> IndexBuffer;
uniform StructuredBuffer<matrix> XformBuffer;

struct Attributes
{
	uint vertexID : SV_VertexID;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

PackedVaryingsType ProceduralVertex(Attributes input, uint instanceID : SV_InstanceID)
{
	float4 vertex = mul(XformBuffer[instanceID], float4(ShapePointBuffer[ShapeIndexBuffer[input.vertexID]], 1.0f));
	float3 normal = normalize(mul(XformBuffer[instanceID], float4(ShapeNormalBuffer[ShapeIndexBuffer[input.vertexID]], 1.0f)).xyz);

	AttributesMesh am;
	am.positionOS = vertex.xyz;
#ifdef ATTRIBUTES_NEED_NORMAL
    am.normalOS = normal;
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
	am.color = ColorBuffer[instanceID];
#endif
    UNITY_TRANSFER_INSTANCE_ID(input, am);

    VaryingsType varyingsType;
    varyingsType.vmesh = VertMesh(am);
    return PackVaryingsType(varyingsType);
}