using FezEditor.Structure;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FezEditor.Services;

public interface IRenderingService : IDisposable
{
    enum InstanceType
    {
        None,
        Mesh,
        MultiMesh
    }

    enum BlendMode
    {
        Opaque,
        AlphaBlend,
        Additive,
        Multiply,
        Multiply2X,
        Screen,
        Maximum,
        Minimum,
        Subtract,
        Premultiplied
    }

    enum CullMode
    {
        None,
        Front,
        Back
    }

    enum FogType
    {
        None,
        Exponential2 = 2
    }

    enum MultiMeshDataType
    {
        Vector4,
        Matrix
    }
    
    class MeshSurface
    {
        public required Vector3[] Vertices;
        public required int[] Indices;
        public Vector3[]? Normals;
        public Color[]? Colors;
        public Vector2[]? TexCoords;
    }

    #region Public

    void Draw(GameTime gameTime);

    void FreeRid(Rid rid);

    #endregion

    #region RenderTarget

    Rid RenderTargetCreate();

    Rid RenderTargetGetBackbuffer();

    Texture2D? RenderTargetGetTexture(Rid rt);

    void RenderTargetSetWorld(Rid rt, Rid world);

    void RenderTargetSetSize(Rid rt, int width, int height);

    void RenderTargetSetClearColor(Rid rt, Color color);

    #endregion

    #region World

    Rid WorldCreate();

    Rid WorldGetRoot(Rid world);

    void WorldSetCamera(Rid world, Rid camera);

    void WorldSetAmbientLight(Rid world, Vector3 color);

    void WorldSetDiffuseLight(Rid world, Vector3 color);

    void WorldSetFogType(Rid world, FogType type);

    void WorldSetFogColor(Rid world, Color color);

    void WorldSetFogDensity(Rid world, float density);

    #endregion

    #region Camera

    Rid CameraCreate();

    void CameraSetView(Rid camera, Matrix view);

    void CameraSetProjection(Rid camera, Matrix projection);

    Matrix CameraGetView(Rid camera);

    Matrix CameraGetProjection(Rid camera);

    #endregion

    #region Instance

    Rid InstanceCreate(Rid parent);

    void InstanceSetParent(Rid instance, Rid parent);

    void InstanceSetVisibility(Rid instance, bool visible);

    void InstanceSetPosition(Rid instance, Vector3 position);

    void InstanceSetRotation(Rid instance, Quaternion rotation);

    void InstanceSetScale(Rid instance, Vector3 scale);

    void InstanceSetMesh(Rid instance, Rid mesh);

    void InstanceSetMultiMesh(Rid instance, Rid multimesh);

    InstanceType InstanceGetType(Rid instance);

    Rid InstanceGetWorld(Rid instance);

    Rid InstanceGetParent(Rid instance);

    Vector3 InstanceGetPosition(Rid instance);
    
    Quaternion InstanceGetRotation(Rid instance);
    
    Vector3 InstanceGetScale(Rid instance);

    bool InstanceIsVisible(Rid instance);

    Matrix InstanceGetWorldMatrix(Rid instance);

    #endregion

    #region Mesh

    Rid MeshCreate();

    void MeshClear(Rid mesh);

    void MeshAddSurface(Rid mesh, PrimitiveType primitive, MeshSurface surface, Rid? material = null);

    void MeshUpdateSurface(Rid mesh, int surfaceIdx, MeshSurface surface);

    int MeshGetSurfaceCount(Rid mesh);

    void MeshRemoveSurface(Rid mesh, int surfaceIdx);

    PrimitiveType MeshGetSurfacePrimitiveType(Rid mesh, int surfaceIdx);

    void MeshSurfaceSetMaterial(Rid mesh, int surfaceIdx, Rid material);

    Rid MeshSurfaceGetMaterial(Rid mesh, int surfaceIdx);

    #endregion

    #region Material

    Rid MaterialCreate(Effect effect);

    void MaterialAssignBaseTexture(Rid material, Texture2D texture);

    void MaterialSetDiffuse(Rid material, Vector3 color);

    void MaterialSetOpacity(Rid material, float opacity);

    Vector3 MaterialGetDiffuse(Rid material);

    float MaterialGetOpacity(Rid material);

    void MaterialSetBlendMode(Rid material, BlendMode mode);

    void MaterialSetCullMode(Rid material, CullMode mode);

    void MaterialSetDepthWrite(Rid material, bool enabled);

    void MaterialSetDepthTest(Rid material, CompareFunction func);

    void MaterialSetColorWriteChannels(Rid material, ColorWriteChannels channels);

    void MaterialSetSamplerState(Rid material, SamplerState state);

    #endregion

    #region Shader

    T MaterialShaderGetParam<T>(Rid material, string name, int count = 0);
    
    void MaterialShaderSetParam<T>(Rid material, string name, T value);
    
    #endregion
    
    #region MultiMesh

    Rid MultiMeshCreate();

    void MultiMeshSetMesh(Rid multiMesh, Rid mesh);

    Rid MultiMeshGetMesh(Rid multiMesh);

    void MultiMeshAllocate(Rid multiMesh, int instances, MultiMeshDataType dataType);

    int MultiMeshGetInstanceCount(Rid multiMesh);

    void MultiMeshSetVisibleInstances(Rid multiMesh, int visible);

    int MultiMeshGetVisibleInstances(Rid multiMesh);

    void MultiMeshSetInstanceMatrix(Rid multiMesh, int index, Matrix data);

    void MultiMeshSetInstanceVector4(Rid multiMesh, int index, Vector4 data);

    #endregion
}