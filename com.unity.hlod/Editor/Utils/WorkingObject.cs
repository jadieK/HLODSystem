﻿using System;
using Unity.Collections;
using UnityEngine;

namespace Unity.HLODSystem.Utils
{
    public static class MeshRendererExtension
    {
        public static WorkingObject ToWorkingObject(this MeshRenderer renderer, Allocator allocator)
        {
            WorkingObject obj = new WorkingObject(allocator);
            obj.FromRenderer(renderer);
            return obj;
        }
    }

    
    
    public class WorkingObject : IDisposable
    {
        private WorkingMesh m_mesh;
        private DisposableList<WorkingMaterial> m_materials;
        private Matrix4x4 m_localToWorld;

        private Allocator m_allocator;

        public string Name { set; get; }
        public WorkingMesh Mesh
        {
            get { return m_mesh; }
        }

        public DisposableList<WorkingMaterial> Materials
        {
            get { return m_materials; }
        }

        public Matrix4x4 LocalToWorld
        {
            get { return m_localToWorld; }
        }

        public WorkingObject(Allocator allocator)
        {
            m_allocator = allocator;
            m_mesh = null;
            m_materials = new DisposableList<WorkingMaterial>();
            m_localToWorld = Matrix4x4.identity;
        }

        public void FromRenderer(MeshRenderer renderer)
        {
            //clean old data
            Dispose();
            
            MeshFilter filter = renderer.GetComponent<MeshFilter>();
            if (filter != null)
            {
                m_mesh = filter.sharedMesh.ToWorkingMesh(m_allocator);
            }

            foreach (var mat in renderer.sharedMaterials)
            {
                m_materials.Add(mat.ToWorkingMaterial(m_allocator));
            }

            m_localToWorld = renderer.localToWorldMatrix;
        }

        public void SetMesh(WorkingMesh mesh)
        {
            if (m_mesh == mesh)
                return;
            
            if (m_mesh != null)
            {
                m_mesh.Dispose();
                m_mesh = null;
            }

            m_mesh = mesh;
        }

        public void Dispose()
        {
            m_mesh?.Dispose();
            m_materials?.Dispose();
        }
    }

}