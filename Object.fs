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
open ModelGroup

type ObjectType = Breakable | Solid | Static

type Object(model : Model, shapeType, position, scale, rotation : float32, color, objectType, collisionCategory, ?collisionWith, ?shapeFill) =
    let body = new Body()
    let shape = Shape(shapeType, scale, color)
    let geom = shape.geometry(body)
    let modelState =
        {
            Model = model; 
            Position = Vector3(5.0f, 0.0f, 0.0f); 
            Rotation = MathHelper.ToRadians(45.0f);
            EffectState = EffectState.Default;
            Scale = Vector3(scale.X, scale.Y, 0.0f);
        }

    let mutable life = 1
    let mutable fade = false
    let mutable alpha = 1.0f
    let mutable oldPosition = modelState.Position
        
    do  geom.CollisionCategories <- collisionCategory
        geom.CollidesWith <- defaultArg collisionWith CollisionCategory.All

    member this.startFade() =
        fade <- true
        
    member this.OldPosition() =
        Vector2(oldPosition.X, oldPosition.Y)

    member this.draw(gd, effect) =
        shape.draw(gd, effect, body.Position, body.Rotation, defaultArg shapeFill Fill)

    member this.draw() =
        if fade then
            alpha <- alpha * 0.9f
            
        oldPosition <- modelState.Position
        modelState.Position <- Vector3(-body.Position.X, -body.Position.Y, 1.0f)
        modelState.Rotation <- body.Rotation
        
        modelState.draw(color, alpha)
     
    member this.lifeOffset(amount) =
        life <- life + amount
        
    member this.needsDisposing() =
        match objectType with
        | Breakable -> (life <= 0) || (body.Position.Y < -10.0f)
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
        
