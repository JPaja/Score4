using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Score4.UI.Primitives;

/// <summary>
/// Custom vertex type for vertices that have just a
/// position and a normal, without any texture coordinates.
/// 
/// This struct is borrowed from the Primitives3D sample.
/// </summary>
public readonly struct VertexPositionNormal : IVertexType
{
    public readonly Vector3 Position;
    public readonly Vector3 Normal;


    /// <summary>
    /// Constructor.
    /// </summary>
    public VertexPositionNormal(Vector3 position, Vector3 normal)
    {
        Position = position;
        Normal = normal;
    }

    /// <summary>
    /// A VertexDeclaration object, which contains information about the vertex
    /// elements contained within this struct.
    /// </summary>
    public static readonly VertexDeclaration VertexDeclaration = new(
        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
        new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
    );

    VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;
}