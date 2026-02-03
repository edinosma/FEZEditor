using FezEditor.Structure;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MultiMeshDataType = FezEditor.Services.IRenderingService.MultiMeshDataType;

namespace FezEditor.Services;

public partial class RenderingService
{
    private class MultiMeshData
    {
        public Rid Mesh;
        public int InstanceCount;
        public int VisibleInstances = -1; // -1 = all
        public MultiMeshDataType DataType;
        public int Stride; // Vector4s per instance: 1 for Vector4, 4 for Matrix
        public Matrix[] MatrixData = Array.Empty<Matrix>();
        public Vector4[] Vector4Data = Array.Empty<Vector4>();
        public bool Dirty = true;

        // GPU buffers for hardware instancing.
        public VertexBuffer? TemplateVertexBuffer;
        public IndexBuffer? TemplateIndexBuffer;
        public DynamicVertexBuffer? InstanceBuffer;
        public VertexDeclaration? InstanceDeclaration;
        public int TemplateVertexCount;
        public int TemplatePrimitiveCount;
        public PrimitiveType TemplatePrimitiveType;
    }
    
    private readonly Dictionary<Rid, MultiMeshData> _multiMeshes = new();
    
    public Rid MultiMeshCreate()
    {
        var rid = AllocateRid(typeof(MultiMeshData));
        _multiMeshes[rid] = new MultiMeshData();
        return rid;
    }

    public void MultiMeshSetMesh(Rid multiMesh, Rid mesh)
    {
        GetResource(_multiMeshes, multiMesh).Mesh = mesh;
    }

    public Rid MultiMeshGetMesh(Rid multiMesh)
    {
        return GetResource(_multiMeshes, multiMesh).Mesh;
    }

    public void MultiMeshAllocate(Rid multiMesh, int instances, MultiMeshDataType dataType)
    {
        var data = GetResource(_multiMeshes, multiMesh);
        data.InstanceBuffer?.Dispose();
        data.InstanceDeclaration?.Dispose();
        data.TemplateVertexBuffer = null;
        data.TemplateIndexBuffer = null;
        data.InstanceBuffer = null;
        data.InstanceDeclaration = null;
        data.InstanceCount = instances;
        data.DataType = dataType;
        data.VisibleInstances = instances;
        data.Dirty = true;

        // Determine stride and allocate typed arrays.
        if (dataType == MultiMeshDataType.Matrix)
        {
            data.Stride = 4;
            data.MatrixData = new Matrix[instances];
            data.Vector4Data = Array.Empty<Vector4>();
        }
        else
        {
            data.Stride = 1;
            data.Vector4Data = new Vector4[instances];
            data.MatrixData = Array.Empty<Matrix>();
        }

        // Build instance vertex declaration.
        // Layout: InstanceIndex (TEXCOORD1) + Data0..DataN (TEXCOORD2..TEXCOORD5)
        var elements = new VertexElement[1 + data.Stride];
        var offset = 0;

        // InstanceIndex: float -> TEXCOORD1
        elements[0] = new VertexElement(offset, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1);
        offset += sizeof(float);

        // Data0..DataN: Vector4 -> TEXCOORD2..TEXCOORD5
        for (var i = 0; i < data.Stride; i++)
        {
            elements[1 + i] = new VertexElement(offset, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2 + i);
            offset += 16; // sizeof(Vector4)
        }

        // Allocate instance buffer.
        data.InstanceDeclaration = new VertexDeclaration(offset, elements);
        data.InstanceBuffer = new DynamicVertexBuffer(_device, data.InstanceDeclaration, instances, BufferUsage.WriteOnly);

        // Build template GPU buffers from the first surface of the referenced mesh.
        if (TryGetResource(_meshes, data.Mesh, out var mesh) && mesh!.Surfaces.Count != 0)
        {
            var surface = mesh.Surfaces[0];
            data.TemplatePrimitiveType = surface.PrimitiveType;
            data.TemplatePrimitiveCount = surface.PrimitiveCount;
            data.TemplateVertexCount = surface.VertexCount;
            data.TemplateVertexBuffer = surface.VertexBuffer;
            data.TemplateIndexBuffer = surface.IndexBuffer;
        }
    }

    public int MultiMeshGetInstanceCount(Rid multiMesh)
    {
        return GetResource(_multiMeshes, multiMesh).InstanceCount;
    }

    public void MultiMeshSetVisibleInstances(Rid multiMesh, int visible)
    {
        GetResource(_multiMeshes, multiMesh).VisibleInstances = visible;
    }

    public int MultiMeshGetVisibleInstances(Rid multiMesh)
    {
        var data = GetResource(_multiMeshes, multiMesh);
        return data.VisibleInstances < 0 ? data.InstanceCount : data.VisibleInstances;
    }

    public void MultiMeshSetInstanceMatrix(Rid multiMesh, int index, Matrix data)
    {
        var mm = GetResource(_multiMeshes, multiMesh);
        ValidateMultiMeshIndex(mm, index);
        if (mm.DataType != MultiMeshDataType.Matrix)
        {
            throw new InvalidOperationException(
                "MultiMesh was allocated with Vector4 data type, use MultiMeshSetInstanceVector4");
        }

        mm.MatrixData[index] = data;
        mm.Dirty = true;
    }

    public void MultiMeshSetInstanceVector4(Rid multiMesh, int index, Vector4 data)
    {
        var mm = GetResource(_multiMeshes, multiMesh);
        ValidateMultiMeshIndex(mm, index);
        if (mm.DataType != MultiMeshDataType.Vector4)
        {
            throw new InvalidOperationException(
                "MultiMesh was allocated with Matrix data type, use MultiMeshSetInstanceMatrix");
        }

        mm.Vector4Data[index] = data;
        mm.Dirty = true;
    }
    
    private void DrawMultiMesh(RenderTargetData rt, WorldData world, Rid multiMeshRid, InstanceMatrices matrices)
    {
        if (!TryGetResource(_multiMeshes, multiMeshRid, out var mm))
        {
            return;
        }

        if (mm!.TemplateVertexBuffer == null || mm.TemplateIndexBuffer == null ||
            mm.InstanceBuffer == null || mm.InstanceDeclaration == null)
        {
            return;
        }

        var visible = mm.VisibleInstances < 0 ? mm.InstanceCount : mm.VisibleInstances;
        if (visible <= 0 || mm.TemplatePrimitiveCount <= 0)
        {
            return;
        }

        // Upload dirty instance data to GPU.
        if (mm.Dirty)
        {
            UploadInstanceData(mm, visible);
            mm.Dirty = false;
        }

        // Resolve material from first surface.
        MaterialData? mat = null;
        if (TryGetResource(_meshes, mm.Mesh, out var mesh))
        {
            var firstSurface = mesh!.Surfaces.FirstOrDefault();
            TryGetResource(_materials, firstSurface?.Material ?? Rid.Invalid, out mat);
        }
        
        if (mat == null)
        {
            return;
        }
       
        ApplyMaterialState(mat);
        if (mat.Effect is BasicEffect)
        {
            UpdateBasicEffect(mat, matrices);
        }
        else
        {
            UpdateBaseEffect(rt, world, mat, matrices);
        }

        // Bind template geometry + instance buffer.
        _device.SetVertexBuffers(
            new VertexBufferBinding(mm.TemplateVertexBuffer, 0, 0),
            new VertexBufferBinding(mm.InstanceBuffer, 0, 1)
        );
        _device.Indices = mm.TemplateIndexBuffer;

        // Draw instanced.
        foreach (var pass in mat.Effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _device.DrawInstancedPrimitives(
                primitiveType: mm.TemplatePrimitiveType,
                baseVertex: 0,
                minVertexIndex: 0,
                numVertices: mm.TemplateVertexCount,
                startIndex: 0,
                primitiveCount: mm.TemplatePrimitiveCount,
                instanceCount: visible
            );
        }

        RestoreDefaultState();
    }

    private void InvalidateMultiMesh(Rid mesh)
    {
        foreach (var mm in _multiMeshes.Values)
        {
            if (mm.Mesh == mesh)
            {
                mm.Mesh = Rid.Invalid;
                mm.TemplateVertexBuffer = null;
                mm.TemplateIndexBuffer = null;
                mm.TemplateVertexCount = 0;
                mm.TemplatePrimitiveCount = 0;
            }
        }
    }
        
    /// <summary>
    /// Pack per-instance data into a flat float array and upload to the DynamicVertexBuffer.
    /// <para/>Layout per instance: [InstanceIndex (float)] + [Data0..DataN (Vector4s)]
    /// <para/>This maps to the shader's VS_INPUT: InstanceIndex (TEXCOORD1), Data0-3 (TEXCOORD2-5).
    /// </summary>
    private static void UploadInstanceData(MultiMeshData mm, int visible)
    {
        if (mm.InstanceBuffer == null)
        {
            return;
        }

        var floatsPerInstance = 1 + mm.Stride * 4; // 1 float index + N * 4 floats
        var buffer = new float[visible * floatsPerInstance];

        for (var i = 0; i < visible; i++)
        {
            var offset = i * floatsPerInstance;

            // InstanceIndex
            buffer[offset] = i;

            // Custom data
            if (mm.DataType == MultiMeshDataType.Matrix)
            {
                var m = mm.MatrixData[i];
                buffer[offset + 1] = m.M11; buffer[offset + 2] = m.M12;
                buffer[offset + 3] = m.M13; buffer[offset + 4] = m.M14;
                buffer[offset + 5] = m.M21; buffer[offset + 6] = m.M22;
                buffer[offset + 7] = m.M23; buffer[offset + 8] = m.M24;
                buffer[offset + 9] = m.M31; buffer[offset + 10] = m.M32;
                buffer[offset + 11] = m.M33; buffer[offset + 12] = m.M34;
                buffer[offset + 13] = m.M41; buffer[offset + 14] = m.M42;
                buffer[offset + 15] = m.M43; buffer[offset + 16] = m.M44;
            }
            else
            {
                var v = mm.Vector4Data[i];
                buffer[offset + 1] = v.X; buffer[offset + 2] = v.Y;
                buffer[offset + 3] = v.Z; buffer[offset + 4] = v.W;
            }
        }

        mm.InstanceBuffer.SetData(buffer, 0, buffer.Length, SetDataOptions.Discard);
    }
    
    private static void ValidateMultiMeshIndex(MultiMeshData mm, int index)
    {
        if (index < 0 || index >= mm.InstanceCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index),
                $"MultiMesh instance index {index} out of range [0, {mm.InstanceCount})");
        }
    }
}