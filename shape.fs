#light

open Microsoft.Xna.Framework;
open Microsoft.Xna.Framework.Graphics;
open FarseerGames.FarseerPhysics.Collisions

open Primative

type ShapeType =
    | Square
    | Arc
    | Circle
    
type ShapeFill =
    | Outline
    | Fill

type Shape(shapeType, scale :Vector2, color) =
    let rotate(angle, v:Vector2) =
        Vector2(
            cos(angle) * v.X - sin(angle) * v.Y, 
            sin(angle) * v.X + cos(angle) * v.Y)

    let points(rotation) = 
        match shapeType with
        | Square -> Shape.square()
        | Arc    -> Shape.arc()
        | Circle -> Shape.circle()
        |> Array.map (fun v -> rotate(rotation, v * scale))

    static member arc() =
        let s = 1.0f// / sqrt(2.0f)
        let n = 10
        [|
            Vector2( s, 0.0f);
            Vector2( s,   -s);
            Vector2(-s,   -s);
            Vector2(-s, 0.0f);
        |]
        |> Array.append (Array.init (n+1) (fun i ->
            let x = s * ((float32 i) / (float32 n) * 2.0f - 1.0f)
            let y = s * ((float32 i) / (float32 n) * 2.0f - 1.0f)
            Vector2(x, 1.0f - y * y * y * y)))

    static member square() =
        let s = 1.0f// / sqrt(2.0f)
        [|
            Vector2(-s, -s);
            Vector2(-s,  s);
            Vector2( s,  s);
            Vector2( s, -s)
        |]

    static member circle() =
        let s = 1.0f// / sqrt(2.0f)
        let n = 20
        Array.init (n+1) (fun i ->
            let a = (float32 i) / (float32 n) * 2.0f * 3.14159f
            Vector2(sin(a) * s, cos(a) * s))
            
    member this.draw(gd, effect, position : Vector2, rotation : float32, shapeFill) =
        let verticies =
            let points = points(rotation)
            points
            |> Seq.map (fun p -> 
                VertexPositionColor(
                    Vector3(
                        p.X + position.X, 
                        p.Y + position.Y, 
                        0.0f), 
                    color))
            |> Seq.append [
                VertexPositionColor(
                    Vector3(
                        points.[points.Length - 1].X + position.X, 
                        points.[points.Length - 1].Y + position.Y, 
                        0.0f), 
                    color)]
            |> Seq.to_array
            
        let primitiveType = 
            match shapeType, shapeFill with
            | Square, Fill    -> PrimitiveType.TriangleStrip
            |      _, Fill    -> PrimitiveType.TriangleFan
            |      _, Outline -> PrimitiveType.LineStrip

        Primative(primitiveType, verticies).draw(gd, effect)
        
    member this.geometry(body) =
        let AABBFactor = (min scale.X scale.Y) * 0.1f
        let vertices = Vertices(points(0.0f))
        vertices.SubDivideEdges(2.0f);
        new Geom(body, vertices, Vector2(), 0.0f, AABBFactor)
        