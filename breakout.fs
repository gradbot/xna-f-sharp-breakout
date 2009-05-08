#light

open System
open System.Collections.Generic

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics

open FarseerGames
open FarseerGames.FarseerPhysics;
open FarseerGames.FarseerPhysics.Dynamics
open FarseerGames.FarseerPhysics.Factories
open FarseerGames.FarseerPhysics.Collisions
open Microsoft.Xna.Framework.Audio

open Resource
open Shape
open Player
open Primative
open Object
open Board

type Breakout(resource) =
    let player = new Player(resource)
    let width = 10
    let height = 22
    let mutable points = 0
    let mutable stage = 1
    let mutable board = Board(resource, width, height)
    let mutable ball = Object(resource.Models.["cube"], Circle, Vector2(0.0f, -20.0f), Vector2(0.3f), 0.0f, Color.Blue, Solid, CollisionCategory.Cat3, ~~~CollisionCategory.Cat1)
    let mutable stateDead = true
    let mutable hit = false

    let PhysicsSimulator = new PhysicsSimulator(new Vector2(0.0f, -0.0025f))
    let floor = Object(resource.Models.["cube"], Square, Vector2(), Vector2(19.0f, 2.0f), 0.0f, Color.Green, Static, CollisionCategory.Cat1, ~~~CollisionCategory.Cat3)

    let mutable collisionCount = 0;
    let mutable collisionNormal = Vector2();

    do 
        board.addToSimulation(PhysicsSimulator)
        floor.addToSimulation(PhysicsSimulator)
        ball.addToSimulation(PhysicsSimulator)
        player.getPaddle().addToSimulation(PhysicsSimulator)
        
        ball.Body.IgnoreGravity <- true
        ball.Body.Mass <- 10.0f
        ball.Body.MomentOfInertia <- ball.Body.Mass
        ball.Body.ApplyImpulse(Vector2(0.1f, 0.1f))
        ball.Geometry.OnCollision <- Geom.CollisionEventHandler(fun g1 g2 contactList ->
            //let l = contactList |> Seq.min_by (fun c -> Vector2.Distance(ball.OldPosition(), c.Position))
            
            let normal = contactList.[0].Normal
            let velocity = ball.Body.LinearVelocity
            
            if (g2.CollisionCategories &&& CollisionCategory.Cat2 = CollisionCategory.Cat2) then
                contactList |> Seq.iter (fun l ->
                    collisionNormal <- collisionNormal + l.Normal
                    collisionCount <- collisionCount + 1)
                board.remove(g2)
                g2.Dispose()
                hit <- true
            else
                let impulse = -2.0f * normal * Vector2.Dot(normal, velocity)
                ball.Body.ApplyImpulse(impulse * ball.Body.Mass * ball.Body.Mass)

                
            true)


    member this.Initialize() =
        stateDead <- false
        let rnd = Random(1)
        let rnd32() = float32 (rnd.NextDouble())
        let rnd(min, max) = float32 (rnd.NextDouble() * (max - min) + min)
        let rndColor() = Color(rnd32(), rnd32(), rnd32())
        for i in {1 .. 180} do
            let obj = 
                Object(
                    resource.Models.["cube"],
                    Square, 
                    Vector2(rnd(-17.0, 17.0), (float32 i) * 0.4f + 3.0f), 
                    Vector2(rnd(1.0, 1.5), rnd(1.5, 4.0)), 
                    rnd32() * MathHelper.PiOver2, 
                    rndColor(),
                    Breakable,
                    CollisionCategory.Cat2,
                    ~~~CollisionCategory.Cat5)
            obj.addToSimulation(PhysicsSimulator)
            if not(obj.intersect(board.getPieces())) then
                board.add(obj)
            else
                obj.removeFromSimulation(PhysicsSimulator)
                
        
    member this.Draw(gd, gameTime) =
        //board.draw(gd, resource.Effects.["colorfill"])
//        floor.draw(gd, effects.["colorfill"])
        player.getPaddle().draw(gd, resource.Effects.["colorfill"])

        board.draw()
        
        if not(stateDead) then ball.draw(gd, resource.Effects.["colorfill"])
        
        let spriteBatch = resource.SpriteBatch.["hud"]
        spriteBatch.Begin();
        let str = sprintf "Level: %d" stage
        spriteBatch.DrawString(resource.Fonts.["arial"], str, new Vector2(10.0f, 0.0f), Color.White);
        
        let str = sprintf "Points: %d" points
        spriteBatch.DrawString(resource.Fonts.["arial"], str, new Vector2(100.0f, 0.0f), Color.White);
        spriteBatch.End();

    member this.Update(gameTime) =
        if hit then
            let s = resource.Sounds.["button-22"].Play()
            hit <- false

        if (collisionCount > 0) then
            let normal = Vector2.Normalize(collisionNormal)
            let impulse = -2.0f * normal * Vector2.Dot(normal, ball.Body.LinearVelocity)
            ball.Body.ApplyImpulse(impulse * ball.Body.Mass * ball.Body.Mass)
            collisionNormal <- Vector2()
            collisionCount <- 0

        let velocityMax = 0.5f
        let velocity = ball.Body.LinearVelocity

        // this keeps the ball from having a horizontal velocity    
        if (velocity.Y <> 0.0f) then
            let ratio = abs(velocity.X) / abs(velocity.Y)
            if (ratio > 1.3f) then
                ball.Body.ApplyImpulse(Vector2(velocity.Y, velocity.X) * 0.5f * ball.Body.Mass * ball.Body.Mass)

        // this keeps the balls speed relativly constant 
        ball.Body.ApplyImpulse(velocity * (velocityMax - velocity.Length()) * 0.95f * ball.Body.Mass * ball.Body.Mass)
            
        board.update()
        PhysicsSimulator.Update(1.0f)
        player.updateInput()

        if player.restart() then
            board <- Board(resource, width, height)

    member this.Dead() =
        stateDead
        
        
        
        
        
        