#light

open System

open Microsoft.Xna.Framework;
open Microsoft.Xna.Framework.Graphics;

open Primative
open Shape

type Piece(num: int, ?pos, ?sca, ?rot, ?col) =
    let mutable position = defaultArg pos (Vector3(0.1f, 1.0f, 0.0f))
    let mutable scale = defaultArg sca (Vector3(1.0f, 1.0f, 1.0f))
    let mutable color = defaultArg col Color.Red
    let mutable positionOld = position
    let mutable velocity = Vector3(10.82f, 0.1f, 0.0f)
    let mutable rotation = defaultArg rot 0.0f

    let mutable trail = [VertexPositionColor(position, Color.Blue);VertexPositionColor(position, Color.Blue)]
    let recorderOn = true
    let random = Random(1)
        
    let refPiece = ref (Unchecked.defaultof<Piece>)
        
    let record() =
        if (recorderOn) then
            let color = Color(float32(random.NextDouble()), float32(random.NextDouble()), float32(random.NextDouble()))
//            trail <- VertexPositionColor(position, color) :: trail
            ()

    member this.recordNormal(collision, normal) =
        if (recorderOn) then
            let color = Color(float32(random.NextDouble()), float32(random.NextDouble()), float32(random.NextDouble()))
            trail <- VertexPositionColor(collision, color) :: trail
//            trail <- VertexPositionColor(collision + normal, color) :: trail
//            trail <- VertexPositionColor(collision, color) :: trail
    
    member this.updatePosition() =
        positionOld <- position
        position <- position + velocity
        record()

    member this.getShape() =
        Shape(position, scale, color, rotation)
        
    member this.draw(gd, effect) =
        this.getShape().draw(gd, effect)
        
        if (recorderOn) then
            Primative(PrimitiveType.LineStrip, trail |> List.to_array).draw(gd, effect)

    member this.MovedSegment() =
        (positionOld, position)
        
    member this.Segments() =
        let points = this.getShape().getPoints()
        Array.init (points.Length - 1) (fun n -> (points.[n], points.[n + 1]))

    member this.clip(pieces : Piece list) =
        let normal(a:Vector3, b:Vector3) =
            Vector3.Normalize(Vector3(a.Y - b.Y, b.X - a.X, 0.0f))
            
        let intersection (a:Vector3, b:Vector3, c:Vector3, d:Vector3) =
            let di = (b.X-a.X)*(d.Y-c.Y)-(b.Y-a.Y)*(d.X-c.X)  
 
            if (di = 0.0f) then
                None
            else
                let q = (a.Y-c.Y)*(d.X-c.X)-(a.X-c.X)*(d.Y-c.Y) 
                let r = q / di
                let p = (a.Y-c.Y)*(b.X-a.X)-(a.X-c.X)*(b.Y-a.Y)
                let s = p / di
 
                if r < 0.0f or r > 1.0f or s < 0.0f or s > 1.0f then
                    None
                else
                    let collision = Vector3(a.X + r * (b.X - a.X), a.Y + r * (b.Y - a.Y), 0.0f)
                    if collision <> a && collision <> b then
                        Some(collision, normal(c, d))
                    else
                        None
                
        let collisions = 
            pieces
            |> List.map (fun piece ->
                piece.Segments()
                    |> Seq.choose (fun (a, b) -> intersection(positionOld, position, a, b))
                    |> Seq.filter (fun (collision, normal) -> !refPiece <> piece)
                    |> Seq.map (fun (collision, normal) -> (piece, collision, normal)))
            |> Seq.concat
            |> Seq.to_list
            
        if (collisions.Length <> 0) then
            let (piece, collision, normal) = collisions |> List.min_by (fun (_, collision, _) -> Vector3.Distance(positionOld, collision))
            refPiece := piece
            (collision, normal) |> Some
        else
            None
            
        
     
    member this.reflection(collision, normal:Vector3) =
        velocity <- velocity - 2.0f * normal * Vector3.Dot(normal, velocity)
        position <- collision
        //record()
        positionOld <- position
        position <- position + velocity

