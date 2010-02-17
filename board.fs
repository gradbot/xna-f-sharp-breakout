namespace Breakout

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open FarseerGames.FarseerPhysics;
open FarseerGames.FarseerPhysics.Collisions

type Board(resource, width:int, height:int) =
    let mutable pieces = [
        Entity(resource.Models.["cube"], Square, Vector2( 20.0f,  0.0f), Vector2(1.0f , 30.0f), 0.0f, Color.Red, Static, CollisionCategory.Cat4, CollisionCategory.All, Outline);
        Entity(resource.Models.["cube"], Square, Vector2(-20.0f,  0.0f), Vector2(1.0f , 30.0f), 0.0f, Color.Red, Static, CollisionCategory.Cat4, CollisionCategory.All, Outline);
        Entity(resource.Models.["cube"], Square, Vector2(  0.0f, 30.0f), Vector2(20.0f,  1.0f), 0.0f, Color.Red, Static, CollisionCategory.Cat5, ~~~CollisionCategory.Cat2, Outline);
        Entity(resource.Models.["cube"], Square, Vector2(  0.0f,-30.0f), Vector2(20.0f,  1.0f), 0.0f, Color.Red, Static, CollisionCategory.Cat4, CollisionCategory.All, Outline);
        ]

    member this.draw(gd, effect) =
        pieces |> Seq.iter (fun ent -> ent.draw(gd, effect))

    member this.draw() =
        pieces |> Seq.iter (fun ent -> ent.draw())
            
    member this.getPieces() =
        pieces

    member this.Length() =
        pieces.Length

    member this.add(piece) =
        pieces <- piece :: pieces
        
    member this.update() =
        pieces <- pieces |> List.filter (fun p -> not(p.needsDisposing()))
    
    member this.addToSimulation(simulation :PhysicsSimulator) =
        pieces
        |> List.iter (fun ent -> ent.addToSimulation(simulation))
        
    member this.remove(geom) =
        let piece = pieces |> List.find (fun ent -> ent.Geometry = geom)
        piece.startFade()
        