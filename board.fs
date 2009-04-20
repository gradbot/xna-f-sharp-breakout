#light

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

open Piece
open Shape

type Board(width:int, height:int) =
    let mutable pieces = [
        Piece(0, Vector3(20.0f, 0.0f, 0.0f), Vector3(1.0f, 30.0f, 1.0f), 0.0f, Color.Red);
        Piece(0, Vector3(-20.0f, 0.0f, 0.0f), Vector3(1.0f, 30.0f, 1.0f), 0.0f, Color.Red);
        Piece(0, Vector3(0.0f, 30.0f, 0.0f), Vector3(20.0f, 1.0f, 1.0f), 0.0f, Color.Red);
        Piece(0, Vector3(0.0f, -30.0f, 0.0f), Vector3(20.0f, 1.0f, 1.0f), 0.0f, Color.Red);
        ]
    
    member this.draw(gd, effect) =        
        pieces |> Seq.iter (fun piece -> piece.getShape().draw(gd, effect))
            
    member this.clip(piece : Piece) =
        piece.clip(pieces)
            
//    member this.normal(collisions:(Piece*Vector3*Vector3) list) =
//        let n = float32 collisions.Length
//        collisions
//        |> List.map (fun(piece, collision, normal) -> normal)
//        |> List.fold_left (+) (Vector3(0.0f, 0.0f, 0.f))
//        |> (fun v -> Vector3(v.X / n, v.Y / n, v.Z / n))

    member this.add(piece) =
        pieces <- piece :: pieces