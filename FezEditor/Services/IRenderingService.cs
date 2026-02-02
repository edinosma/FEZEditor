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
        MultiMesh,
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
        Premultiplied,
    }

    enum CullMode
    {
        None,
        Front,
        Back,
    }

    enum CompareFunc
    {
        Never,
        Less,
        Equal,
        LessEqual,
        Greater,
        NotEqual,
        GreaterEqual,
        Always,
    }

    enum FogType
    {
        None,
        Linear,
        Exponential,
        Exponential2,
    }

    [Flags]
    enum ColorWriteMask
    {
        None = 0,
        Red = 1 << 0,
        Green = 1 << 1,
        Blue = 1 << 2,
        Alpha = 1 << 3,
        Rgb = Red | Green | Blue,
        All = Red | Green | Blue | Alpha,
    }
    
    #region Public
    
    void Draw(GameTime gameTime);
    
    void FreeRid(Rid rid);
    
    #endregion
    
    #region Render Target
    
    Rid RenderTargetCreate();
    
    Rid RenderTargetGetBackbuffer();
    
    Texture2D RenderTargetGetTexture(Rid rt);
    
    void RenderTargetSetWorld(Rid rt, Rid world);
    
    void RenderTargetSetSize(Rid rt, int width, int height);
    
    void RenderTargetSetClearColor(Rid rt, Color color);
    
    #endregion
    
    #region World
    
    Rid WorldCreate();
    
    void WorldSetCamera(Rid world, Rid camera);
    
    void WorldSetAmbientLight(Rid world, Vector3 color);
    
    void WorldSetDiffuseLight(Rid world, Vector3 color);
    
    void WorldSetFogType(Rid world, FogType type);
    
    void WorldSetFogColor(Rid world, Color color);
    
    void WorldSetFogDensity(Rid world, float density);
    
    #endregion
    
    #region Camera
    
    Rid CameraCreate();
    
    void CameraSetPosition(Rid camera, Vector3 position);
   
    void CameraSetRotation(Rid camera, Quaternion rotation);
    
    void CameraSetViewMatrix(Rid camera, Matrix view);
    
    void CameraSetProjection(Rid camera, Matrix projection);
    
    Vector3 CameraGetPosition(Rid camera);
    
    Quaternion CameraGetRotation(Rid camera);
    
    Matrix CameraGetViewMatrix(Rid camera);
    
    Matrix CameraGetProjectionMatrix(Rid camera);
    
    #endregion
    
    #region Instance
    
    Rid InstanceCreate();
    
    Rid InstanceCreate(Rid baseInstance, Rid world);

    void InstanceSetWorld(Rid instance, Rid world);
    
    void InstanceSetBase(Rid instance, Rid baseInstance);
    
    void InstanceSetEnabled(Rid instance, bool visible);
    
    void InstanceSetPosition(Rid instance, Vector3 position);
    
    void InstanceSetRotation(Rid instance, Quaternion rotation);
    
    void InstanceSetScale(Rid instance, Vector3 scale);
    
    void InstanceSetMesh(Rid instance, Rid mesh);
    
    void InstanceSetMultiMesh(Rid instance, Rid multimesh);
    
    InstanceType InstanceGetType(Rid instance);
    
    Rid InstanceGetWorld(Rid instance);
    
    Vector3 InstanceGetPosition(Rid instance);
    
    bool InstanceGetEnabled(Rid instance);
    
    #endregion
    
    #region Mesh
    
    Rid MeshCreate();
    
    void MeshClear(Rid mesh);
    
    void MeshAddSurface(Rid mesh, PrimitiveType primitive, MeshSurface surface);
    
    void MeshSetSurface(Rid mesh, int surfaceIdx, MeshSurface surface);
    
    int MeshGetSurfaceCount(Rid mesh);
    
    void MeshRemoveSurface(Rid mesh, int surfaceIdx);
    
    MeshSurface MeshGetSurface(Rid mesh, int surfaceIdx);
    
    PrimitiveType MeshGetSurfacePrimitiveType(Rid mesh, int surfaceIdx);
    
    void MeshSurfaceSetMaterial(Rid mesh, int surfaceIdx, Rid material);
    
    Rid MeshSurfaceGetMaterial(Rid mesh, int surfaceIdx);
    
    #endregion
    
    #region Material
    
    Rid MaterialCreate();
    
    void MaterialSetEffect(Rid material, Effect effect);
    
    Effect MaterialGetEffect(Rid material);
    
    void MaterialSetTexture(Rid material, Texture2D texture);
    
    Texture2D MaterialGetTexture(Rid material);
    
    void MaterialSetDiffuse(Rid material, Vector3 color);
    
    void MaterialSetOpacity(Rid material, float opacity);
    
    Vector3 MaterialGetDiffuse(Rid material);
    
    float MaterialGetOpacity(Rid material);
    
    void MaterialSetBlendMode(Rid material, BlendMode mode);
    
    void MaterialSetCullMode(Rid material, CullMode mode);
    
    void MaterialSetDepthWrite(Rid material, bool enabled);
    
    void MaterialSetDepthTest(Rid material, CompareFunc func);
    
    void MaterialSetColorWriteMask(Rid material, ColorWriteMask mask);
    
    void MaterialSetSamplerState(Rid material, SamplerState state);
    
    #endregion
    
    #region MultiMesh
    
    Rid MultiMeshCreate();
    
    void MultiMeshSetMesh(Rid multiMesh, Rid mesh);
    
    Rid MultiMeshGetMesh(Rid multiMesh);
    
    void MultiMeshAllocate(Rid multiMesh, int instances, Type customDataType);
    
    int MultiMeshGetInstanceCount(Rid multiMesh);
    
    void MultiMeshSetVisibleInstances(Rid multiMesh, int visible);
    
    int MultiMeshGetVisibleInstances(Rid multiMesh);
    
    void MultiMeshSetInstanceCustomData(Rid multiMesh, int index, Matrix data);
    
    void MultiMeshSetInstanceCustomData(Rid multiMesh, int index, Vector4 data);
    
    #endregion
}