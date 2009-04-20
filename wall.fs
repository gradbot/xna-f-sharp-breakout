#light

open System;
open System.Collections.Generic;

open Microsoft.Xna.Framework;
open Microsoft.Xna.Framework.Audio;
open Microsoft.Xna.Framework.Content;
open Microsoft.Xna.Framework.Graphics;
open Microsoft.Xna.Framework.Input;
open Microsoft.Xna.Framework.Storage;

type Shape =
    val points : Vector3[]
    
    new (points) =
        { points = points }
    
    member this.scale(scale) =
        Shape(Array.map (fun v -> v * scale) this.points)

    member this.translate(offset) =
        Shape(Array.map (fun v -> v + offset) this.points)
        
    static member triangle() =
        let x = sqrt(3.0f) * 0.5f
        Shape([|Vector3(  -x, -0.5f, 0.0f);
                Vector3(   x, -0.5f, 0.0f);
                Vector3(0.0f,  1.0f, 0.0f);
                Vector3(  -x, -0.5f, 0.0f)|])

    static member square() =
        let x = 1.0f / sqrt(2.0f)
        Shape([|Vector3(-x, -x, 0.0f);
                Vector3(-x,  x, 0.0f);
                Vector3( x,  x, 0.0f);
                Vector3( x, -x, 0.0f);
                Vector3(-x, -x, 0.0f)|])
                
    static member wave() =
        let size = 20
        let rnd = new Random(1)
        let rnd32(x) = x * (float32)(rnd.NextDouble() * 2.0 - 1.0)
        
        let landPoint(i, max) = Vector3(i-max*0.5f, rnd32(2.0f), 0.0f)
        
        let mutable points : Vector3[] = Array.init size (fun i -> 
            Vector3(
                ((float32)i - (float32)size * 0.5f) * 0.25f, 
                0.0f, 
                0.0f))
                    
        let mutable offset = 0.0f;
        let mutable velocity = 0.0f;
        
        for i = 0 to size - 1 do
            velocity <- velocity + rnd32(0.25f)
            offset <- offset + velocity
            points.[i] <- points.[i] + Vector3(0.0f, offset, 0.0f)
            
        Shape(points)


type Wall =
    val points : Vector3[]
    
    new (points) =
        { points = points; }
    
    static member New(points) =
        Wall(points)
    
    static member create(seed, size) =
        let rnd = new Random(seed)
        let rnd32(x) = x * (float32)(rnd.NextDouble() * 2.0 - 1.0)


        [Shape.triangle().scale(4.0f); Shape.square().scale(8.0f)]
            |> List.map (fun shape -> shape.points)
            |> Array.concat
            |> Wall.New
                    
    member this.verticies() =
        let size = this.points.Length
        let mutable verticies : VertexPositionColor[] = Array.init (size * 6) (fun _ -> VertexPositionColor())
        
        for i = 0 to size - 2 do
            let p1 = this.points.[i]
            let p2 = this.points.[i+1]
            let offset = Vector3(0.125f, 0.125f, 0.0f);
            
            verticies.[i*6+0] <- VertexPositionColor(p1 + offset, Color.Red)
            verticies.[i*6+1] <- VertexPositionColor(p2 + offset, Color.Red)
            verticies.[i*6+2] <- VertexPositionColor(p1 - offset, Color.Red)
            verticies.[i*6+3] <- VertexPositionColor(p1 - offset, Color.Blue)
            verticies.[i*6+4] <- VertexPositionColor(p2 + offset, Color.Blue)
            verticies.[i*6+5] <- VertexPositionColor(p2 - offset, Color.Blue)

        verticies
        
    member this.count() = 
        this.points.Length * 2 - 2