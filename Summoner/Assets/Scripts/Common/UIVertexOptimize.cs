using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIVertexOptimize : BaseMeshEffect
{
    struct Triangle
    {
        public UIVertex v1;
        public UIVertex v2;
        public UIVertex v3;
    }

    class TriangleCompare : IEqualityComparer<Triangle>
    {
        public bool Equals(Triangle x, Triangle y)
        {
            return UIVertexEquals(x.v1, y.v1) && UIVertexEquals(x.v2, y.v2) && UIVertexEquals(x.v3, y.v3);
        }

        public int GetHashCode(Triangle obj)
        {
            return GetUIVertexHashCode(obj.v1)
                   ^ GetUIVertexHashCode(obj.v2)
                   ^ GetUIVertexHashCode(obj.v3);
        }

        int GetUIVertexHashCode(UIVertex vertex)
        {
            return vertex.color.a.GetHashCode()
                ^ vertex.color.b.GetHashCode()
                ^ vertex.color.g.GetHashCode()
                ^ vertex.color.r.GetHashCode()
                   ^ vertex.normal.GetHashCode()
                   ^ vertex.position.GetHashCode()
                   ^ vertex.tangent.GetHashCode()
                   ^ vertex.uv0.GetHashCode()
                   ^ vertex.uv1.GetHashCode();
        }

        bool UIVertexEquals(UIVertex x, UIVertex y)
        {
            return x.color.a == y.color.a
                   && x.color.b == y.color.b
                   && x.color.g == y.color.g
                   && x.color.r == y.color.r
                   && x.normal == y.normal
                   && x.position == y.position
                   && x.tangent == y.tangent
                   && x.uv1 == y.uv1
                   && x.uv0 == y.uv0;
        }
    }

    List<UIVertex> verts = new List<UIVertex>();
    List<Triangle> tris = new List<Triangle>();

    public override void ModifyMesh(VertexHelper vh)
    {
        vh.GetUIVertexStream(verts);
        //Debug.Log(verts.Count);

        OptimizeVert(ref verts);
        //Debug.Log(verts.Count);
        vh.Clear();
        vh.AddUIVertexTriangleStream(verts);
    }

    void OptimizeVert(ref List<UIVertex> vertices)
    {
        if (tris.Capacity < vertices.Count / 3)
        {
            tris.Capacity = vertices.Count;
        }
        for (int i = 0; i <= vertices.Count - 3; i += 3)
        {
            tris.Add(new Triangle() { v1 = vertices[i], v2 = vertices[i + 1], v3 = vertices[i + 2] });
        }
        vertices.Clear();

        vertices.AddRange(tris.Distinct(new TriangleCompare()).SelectMany(t => new[]
        {
            t.v1,
            t.v2,
            t.v3
        }));
        tris.Clear();
    }
}