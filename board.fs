#light

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open FarseerGames.FarseerPhysics;
open FarseerGames.FarseerPhysics.Collisions

open Resource
open Object
open Shape

type Board(resource, width:int, height:int) =
    let mutable pieces = [
        Object(resource.Models.["cube"], Square, Vector2( 20.0f,  0.0f), Vector2(1.0f , 30.0f), 0.0f, Color.Red, Static, CollisionCategory.Cat4, CollisionCategory.All, Outline);
        Object(resource.Models.["cube"], Square, Vector2(-20.0f,  0.0f), Vector2(1.0f , 30.0f), 0.0f, Color.Red, Static, CollisionCategory.Cat4, CollisionCategory.All, Outline);
        Object(resource.Models.["cube"], Square, Vector2(  0.0f, 30.0f), Vector2(20.0f,  1.0f), 0.0f, Color.Red, Static, CollisionCategory.Cat5, ~~~CollisionCategory.Cat2, Outline);
        Object(resource.Models.["cube"], Square, Vector2(  0.0f,-30.0f), Vector2(20.0f,  1.0f), 0.0f, Color.Red, Static, CollisionCategory.Cat4, CollisionCategory.All, Outline);
        ]

    member this.draw(gd, effect) =
        pieces |> Seq.iter (fun obj -> obj.draw(gd, effect))

    member this.draw() =
        pieces |> Seq.iter (fun obj -> obj.draw())
            
    member this.getPieces() =
        pieces

    member this.add(piece) =
        pieces <- piece :: pieces
        
    member this.update() =
        pieces <- pieces |> List.filter (fun p -> not(p.needsDisposing()))
    
    member this.addToSimulation(simulation :PhysicsSimulator) =
        pieces
        |> List.iter (fun obj -> obj.addToSimulation(simulation))
        
    member this.remove(geom) =
        let piece = pieces |> List.find (fun obj -> obj.Geometry = geom)
        piece.startFade()
        