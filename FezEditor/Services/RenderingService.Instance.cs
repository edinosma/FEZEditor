using FezEditor.Structure;
using Microsoft.Xna.Framework;

using InstanceType = FezEditor.Services.IRenderingService.InstanceType;

namespace FezEditor.Services;

public partial class RenderingService
{
    private class InstanceData
    {
        public Rid Parent;
        public readonly List<Rid> Children = new();
        public bool Visible = true;
        public Vector3 Position = Vector3.Zero;
        public Quaternion Rotation = Quaternion.Identity;
        public Vector3 Scale = Vector3.One;
        public InstanceType Type = InstanceType.None;
        public Rid Internal;

        public bool LocalDirty = true;
        public bool WorldDirty = true;
        public Matrix WorldMatrix = Matrix.Identity;
        private Matrix _localMatrix = Matrix.Identity;

        public Matrix GetLocalMatrix()
        {
            if (!LocalDirty)
            {
                return _localMatrix;
            }

            _localMatrix = Matrix.CreateScale(Scale)
                           * Matrix.CreateFromQuaternion(Rotation)
                           * Matrix.CreateTranslation(Position);
            LocalDirty = false;
            return _localMatrix;
        }
    }
    
    private readonly Dictionary<Rid, InstanceData> _instances = new();

    private readonly Stack<InstanceData> _transformChain = new();
    
    public Rid InstanceCreate(Rid parent)
    {
        var rid = AllocateRid(typeof(InstanceData));
        _instances[rid] = new InstanceData();
        InstanceSetParent(rid, parent);
        return rid;
    }

    public void InstanceSetParent(Rid instance, Rid parent)
    {
        // Check for cycles: walk up from parent, if we hit instance it's a cycle.
        var current = parent;
        while (TryGetResource(_instances, current, out var check))
        {
            if (current == instance)
            {
                throw new ArgumentException("Cannot set parent: would create a cycle.");
            }
            current = check!.Parent;
        }

        var data = GetResource(_instances, instance);

        // Remove from old parent's children.
        if (TryGetResource(_instances, data.Parent, out var oldParent))
        {
            oldParent!.Children.Remove(instance);
            data.Parent = Rid.Invalid;
        }

        // Add to new parent's children.
        if (TryGetResource(_instances, parent, out var newParent))
        {
            data.Parent = parent;
            newParent!.Children.Add(instance);
        }
    }

    public void InstanceSetVisibility(Rid instance, bool visible)
    {
        GetResource(_instances, instance).Visible = visible;
    }

    public void InstanceSetPosition(Rid instance, Vector3 position)
    {
        var data = GetResource(_instances, instance);
        data.Position = position;
        data.LocalDirty = true;
        MarkWorldMatrixDirty(instance);
    }

    public void InstanceSetRotation(Rid instance, Quaternion rotation)
    {
        var data = GetResource(_instances, instance);
        data.Rotation = rotation;
        data.LocalDirty = true;
        MarkWorldMatrixDirty(instance);
    }

    public void InstanceSetScale(Rid instance, Vector3 scale)
    {
        var data = GetResource(_instances, instance);
        data.Scale = scale;
        data.LocalDirty = true;
        MarkWorldMatrixDirty(instance);
    }

    public void InstanceSetMesh(Rid instance, Rid mesh)
    {
        var data = GetResource(_instances, instance);
        data.Type = InstanceType.Mesh;
        data.Internal = mesh;
    }

    public void InstanceSetMultiMesh(Rid instance, Rid multimesh)
    {
        var data = GetResource(_instances, instance);
        data.Type = InstanceType.MultiMesh;
        data.Internal = multimesh;
    }

    public InstanceType InstanceGetType(Rid instance)
    {
        return GetResource(_instances, instance).Type;
    }

    public Rid InstanceGetWorld(Rid instance)
    {
        var current = instance;
        while (TryGetResource(_instances, current, out var data) && data!.Parent.IsValid)
        {
            current = data.Parent;
        }

        foreach (var (rid, world) in _worlds)
        {
            if (world.Root == current)
            {
                return rid;
            }
        }

        return Rid.Invalid;
    }

    public Rid InstanceGetParent(Rid instance)
    {
        return GetResource(_instances, instance).Parent;
    }

    public Vector3 InstanceGetPosition(Rid instance)
    {
        return GetResource(_instances, instance).Position;
    }

    public Quaternion InstanceGetRotation(Rid instance)
    {
        return GetResource(_instances, instance).Rotation;
    }

    public Vector3 InstanceGetScale(Rid instance)
    {
        return GetResource(_instances, instance).Scale;
    }

    public bool InstanceIsVisible(Rid instance)
    {
        return GetResource(_instances, instance).Visible;
    }

    public Matrix InstanceGetWorldMatrix(Rid instance)
    {
        var data = GetResource(_instances, instance);
        return ComputeWorldMatrix(data);
    }
    
    private void MarkWorldMatrixDirty(Rid instanceRid)
    {
        var stack = new Stack<Rid>();
        stack.Push(instanceRid);

        while (stack.Count > 0)
        {
            var rid = stack.Pop();
            var instance = GetResource(_instances, rid);
            instance.WorldDirty = true;
            foreach (var childRid in instance.Children)
            {
                stack.Push(childRid);
            }
        }
    }
    
    private Matrix ComputeWorldMatrix(InstanceData instance)
    {
        if (!instance.WorldDirty)
        {
            return instance.WorldMatrix;
        }
        
        var current = instance;
        while (current is { WorldDirty: true })
        {
            _transformChain.Push(current);
            TryGetResource(_instances, current.Parent, out current);
        }
        
        var parentWorld = current?.WorldMatrix ?? Matrix.Identity;
        while (_transformChain.Count > 0)
        {
            var instanceData = _transformChain.Pop();
            instanceData.WorldMatrix = instanceData.GetLocalMatrix() * parentWorld;
            instanceData.WorldDirty = false;
            parentWorld = instanceData.WorldMatrix;
        }

        return instance.WorldMatrix;
    }

    private void DisposeInstanceTree(Rid instance)
    {
        // Unlink from parent before removing the tree.
        if (TryGetResource(_instances, instance, out var root) && TryGetResource(_instances, root!.Parent, out var parent))
        {
            parent!.Children.Remove(instance);
        }

        var stack = new Stack<Rid>();
        stack.Push(instance);

        while (stack.Count > 0)
        {
            var rid = stack.Pop();
            if (_instances.Remove(rid, out var data))
            {
                foreach (var childRid in data.Children)
                {
                    stack.Push(childRid);
                }
            }
        }
    }
}