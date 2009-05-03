#light

open System

open Microsoft.Xna.Framework;
open Microsoft.Xna.Framework.Graphics;

open FarseerGames
open FarseerGames.FarseerPhysics;
open FarseerGames.FarseerPhysics.Dynamics
open FarseerGames.FarseerPhysics.Factories
open FarseerGames.FarseerPhysics.Collisions

open Primative
open Shape

type ObjectType = Breakable | Solid | Static

type Object(shapeType, position, scale, rotation : float32, color, objectType, collisionCategory, ?collisionWith) =
    let body = new Body()
    let shape = Shape(shapeType, scale, color)
    let geom = shape.geometry(body)

    let mutable life = 1
    
    do  geom.CollisionCategories <- collisionCategory
        geom.CollidesWith <- defaultArg collisionWith CollisionCategory.All

    member this.draw(gd, effect) =
        shape.draw(gd, effect, body.Position, body.Rotation)
     
    member this.lifeOffset(amount) =
        life <- life + amount
        
    member this.needsDisposing() =
        match objectType with
        | Breakable -> (life <= 0)
        | Solid -> false
        | Static -> false
        
    member this.points() =
        match objectType with
        | Breakable -> 1
        | Solid -> 0
        | Static -> 0
        
    member this.addToSimulation(simulation :PhysicsSimulator) =
        body.Mass <- 1.0f;
        body.MomentOfInertia <- body.Mass;
        body.Position <- position
        body.Rotation <- rotation

        if (objectType = Static) then
            body.IsStatic <- true

        simulation.Add(body)
        simulation.Add(geom)

    member this.removeFromSimulation(simulation :PhysicsSimulator) =
        simulation.Remove(body)
        simulation.Remove(geom)
        
    member this.Geometry =
        geom

    member this.Body =
        body
        
    member this.intersect(objects : Object list) =
        objects |> Seq.exists (fun obj -> 
            AABB.Intersect(geom.AABB, obj.Geometry.AABB))
        
