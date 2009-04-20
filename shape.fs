#light

open Microsoft.Xna.Framework;
open Microsoft.Xna.Framework.Graphics;

open Primative

type ShapeType =
    | ShapeType of int32
    static member Triangle = 1
    static member Square = 1
    static member Circle = 1
    static member Wave = 1

type Shape(position : Vector3, ?_scale : Vector3, ?_color : Color, ?_rotation : float32) =
    let scale = defaultArg _scale (Vector3(1.0f, 1.0f, 1.0f))
    let color = defaultArg _color Color.Red
    let rotation = defaultArg _rotation 0.0f
    let sinf a = float32 (sin a)
    let cosf a = float32 (cos a)
    let rotate(v:Vector3) = Vector3(cosf(rotation) * v.X - sinf(rotation) * v.Y, sinf(rotation) * v.X + cosf(rotation) * v.Y, v.Z)
    let points = Array.map (fun v -> rotate(v * scale) + position) (Shape.square())
        
    member this.getPostion = position
    member this.getScale = scale
    member this.getPoints() =
        points
        
    static member square() =
        let x = 1.0f// / sqrt(2.0f)
        [|Vector3(-x, -x, 0.0f);
        Vector3(-x,  x, 0.0f);
        Vector3( x,  x, 0.0f);
        Vector3( x, -x, 0.0f);
        Vector3(-x, -x, 0.0f)|]
            
    member this.verticies() =
        points
        |> Array.map (fun p -> VertexPositionColor(p, color))
        
    member this.draw(gd, effect) =
        Primative(PrimitiveType.LineStrip, this.verticies()).draw(gd, effect)
            