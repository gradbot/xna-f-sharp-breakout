namespace Breakout

open Microsoft.Xna.Framework;
open Microsoft.Xna.Framework.Graphics;

type Primative(primitiveType : PrimitiveType, verticies : VertexPositionColor[]) =
    member this.verticies = verticies
    
    member this.draw(gd : GraphicsDevice, effect : Effect) =
        let length = 
            match primitiveType with
            | PrimitiveType.LineList -> verticies.Length / 2
            | PrimitiveType.LineStrip -> verticies.Length - 1
            | PrimitiveType.TriangleFan -> verticies.Length - 2
            | PrimitiveType.TriangleList -> verticies.Length / 3
            | PrimitiveType.TriangleStrip -> verticies.Length - 2
            | _ -> verticies.Length
    
        effect.Begin()
        for pass in effect.CurrentTechnique.Passes do
            pass.Begin();
            gd.DrawUserPrimitives(primitiveType, verticies, 0, length);
            pass.End();
        effect.End()
