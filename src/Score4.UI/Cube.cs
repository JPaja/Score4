using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame
{
    class Cube
    {
        public Vector3 shapeSize;
        public Vector3 shapePosition;
        private VertexPositionNormalTexture[] shapeVertices;
        private int shapeTriangles;
        private VertexBuffer shapeBuffer;
        private IndexBuffer indexBuffer;
        public Texture2D shapeTexture;
        VertexPositionColor[] vertices = new VertexPositionColor[8];
        
        
        public Cube(Vector3 size, Vector3 position)
        {
            shapeSize = size;
            shapePosition = position;
        }

        private void BuildShape()
        {
            shapeTriangles = 12;

            shapeVertices = new VertexPositionNormalTexture[36];

            vertices = new VertexPositionColor[8];
            
            Vector3 topLeftFront = shapePosition +
                                   new Vector3(-1.0f, 1.0f, -1.0f) * shapeSize;
            Vector3 bottomLeftFront = shapePosition +
                                      new Vector3(-1.0f, -1.0f, -1.0f) * shapeSize;
            Vector3 topRightFront = shapePosition +
                                    new Vector3(1.0f, 1.0f, -1.0f) * shapeSize;
            Vector3 bottomRightFront = shapePosition +
                                       new Vector3(1.0f, -1.0f, -1.0f) * shapeSize;
            Vector3 topLeftBack = shapePosition +
                                  new Vector3(-1.0f, 1.0f, 1.0f) * shapeSize;
            Vector3 topRightBack = shapePosition +
                                   new Vector3(1.0f, 1.0f, 1.0f) * shapeSize;
            Vector3 bottomLeftBack = shapePosition +
                                     new Vector3(-1.0f, -1.0f, 1.0f) * shapeSize;
            Vector3 bottomRightBack = shapePosition +
                                      new Vector3(1.0f, -1.0f, 1.0f) * shapeSize;

            Vector3 frontNormal = new Vector3(0.0f, 0.0f, 1.0f) * shapeSize;
            Vector3 backNormal = new Vector3(0.0f, 0.0f, -1.0f) * shapeSize;
            Vector3 topNormal = new Vector3(0.0f, 1.0f, 0.0f) * shapeSize;
            Vector3 bottomNormal = new Vector3(0.0f, -1.0f, 0.0f) * shapeSize;
            Vector3 leftNormal = new Vector3(-1.0f, 0.0f, 0.0f) * shapeSize;
            Vector3 rightNormal = new Vector3(1.0f, 0.0f, 0.0f) * shapeSize;

            Vector2 textureTopLeft = new Vector2(0.5f * shapeSize.X, 0.0f * shapeSize.Y);
            Vector2 textureTopRight = new Vector2(0.0f * shapeSize.X, 0.0f * shapeSize.Y);
            Vector2 textureBottomLeft = new Vector2(0.5f * shapeSize.X, 0.5f * shapeSize.Y);
            Vector2 textureBottomRight = new Vector2(0.0f * shapeSize.X, 0.5f * shapeSize.Y);
            
            var Color1 = Color.DarkRed;
            
            vertices[0] = new VertexPositionColor(topLeftFront, Color1);
            vertices[1] = new VertexPositionColor(bottomLeftFront, Color1);
            vertices[2] = new VertexPositionColor(topRightFront, Color1);
            vertices[3] = new VertexPositionColor(bottomRightFront, Color1);
            vertices[4] = new VertexPositionColor(topLeftBack, Color1);
            vertices[5] = new VertexPositionColor(topRightBack, Color1);
            vertices[6] = new VertexPositionColor(bottomLeftBack, Color1);
            vertices[7] = new VertexPositionColor(bottomRightBack, Color1);
       
            
            // Front face.
            shapeVertices[0] = new VertexPositionNormalTexture(
                topLeftFront, frontNormal, textureTopLeft);
            shapeVertices[1] = new VertexPositionNormalTexture(
                bottomLeftFront, frontNormal, textureBottomLeft);
            shapeVertices[2] = new VertexPositionNormalTexture(
                topRightFront, frontNormal, textureTopRight);
            shapeVertices[3] = new VertexPositionNormalTexture(
                bottomLeftFront, frontNormal, textureBottomLeft);
            shapeVertices[4] = new VertexPositionNormalTexture(
                bottomRightFront, frontNormal, textureBottomRight);
            shapeVertices[5] = new VertexPositionNormalTexture(
                topRightFront, frontNormal, textureTopRight);

            // Back face.
            shapeVertices[6] = new VertexPositionNormalTexture(
                topLeftBack, backNormal, textureTopRight);
            shapeVertices[7] = new VertexPositionNormalTexture(
                topRightBack, backNormal, textureTopLeft);
            shapeVertices[8] = new VertexPositionNormalTexture(
                bottomLeftBack, backNormal, textureBottomRight);
            shapeVertices[9] = new VertexPositionNormalTexture(
                bottomLeftBack, backNormal, textureBottomRight);
            shapeVertices[10] = new VertexPositionNormalTexture(
                topRightBack, backNormal, textureTopLeft);
            shapeVertices[11] = new VertexPositionNormalTexture(
                bottomRightBack, backNormal, textureBottomLeft);

            // Top face.
            shapeVertices[12] = new VertexPositionNormalTexture(
                topLeftFront, topNormal, textureBottomLeft);
            shapeVertices[13] = new VertexPositionNormalTexture(
                topRightBack, topNormal, textureTopRight);
            shapeVertices[14] = new VertexPositionNormalTexture(
                topLeftBack, topNormal, textureTopLeft);
            shapeVertices[15] = new VertexPositionNormalTexture(
                topLeftFront, topNormal, textureBottomLeft);
            shapeVertices[16] = new VertexPositionNormalTexture(
                topRightFront, topNormal, textureBottomRight);
            shapeVertices[17] = new VertexPositionNormalTexture(
                topRightBack, topNormal, textureTopRight);

            // Bottom face. 
            shapeVertices[18] = new VertexPositionNormalTexture(
                bottomLeftFront, bottomNormal, textureTopLeft);
            shapeVertices[19] = new VertexPositionNormalTexture(
                bottomLeftBack, bottomNormal, textureBottomLeft);
            shapeVertices[20] = new VertexPositionNormalTexture(
                bottomRightBack, bottomNormal, textureBottomRight);
            shapeVertices[21] = new VertexPositionNormalTexture(
                bottomLeftFront, bottomNormal, textureTopLeft);
            shapeVertices[22] = new VertexPositionNormalTexture(
                bottomRightBack, bottomNormal, textureBottomRight);
            shapeVertices[23] = new VertexPositionNormalTexture(
                bottomRightFront, bottomNormal, textureTopRight);

            // Left face.
            shapeVertices[24] = new VertexPositionNormalTexture(
                topLeftFront, leftNormal, textureTopRight);
            shapeVertices[25] = new VertexPositionNormalTexture(
                bottomLeftBack, leftNormal, textureBottomLeft);
            shapeVertices[26] = new VertexPositionNormalTexture(
                bottomLeftFront, leftNormal, textureBottomRight);
            shapeVertices[27] = new VertexPositionNormalTexture(
                topLeftBack, leftNormal, textureTopLeft);
            shapeVertices[28] = new VertexPositionNormalTexture(
                bottomLeftBack, leftNormal, textureBottomLeft);
            shapeVertices[29] = new VertexPositionNormalTexture(
                topLeftFront, leftNormal, textureTopRight);

            // Right face. 
            shapeVertices[30] = new VertexPositionNormalTexture(
                topRightFront, rightNormal, textureTopLeft);
            shapeVertices[31] = new VertexPositionNormalTexture(
                bottomRightFront, rightNormal, textureBottomLeft);
            shapeVertices[32] = new VertexPositionNormalTexture(
                bottomRightBack, rightNormal, textureBottomRight);
            shapeVertices[33] = new VertexPositionNormalTexture(
                topRightBack, rightNormal, textureTopRight);
            shapeVertices[34] = new VertexPositionNormalTexture(
                topRightFront, rightNormal, textureTopLeft);
            shapeVertices[35] = new VertexPositionNormalTexture(
                bottomRightBack, rightNormal, textureBottomRight);
        }

        public void RenderShape(GraphicsDevice device)
        {
            BuildShape();
            shapeBuffer = new VertexBuffer(device,
                VertexPositionNormalTexture.VertexDeclaration, shapeVertices.Length,
                BufferUsage.WriteOnly);
            
            shapeBuffer.SetData(shapeVertices);

            
            var colorBuffer = new VertexBuffer(device, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
            colorBuffer.SetData(vertices);

            device.SetVertexBuffer(shapeBuffer);
            //device.SetVertexBuffer(indexBuffer2);
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, shapeTriangles);
        }

    }
}
