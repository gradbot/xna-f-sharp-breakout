#light

open System
open System.Collections.Generic

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics

open Shape
open Player
open Primative
open Piece
open Board

type Breakout() =
    let player = new Player()
    let width = 10
    let height = 22
    let mutable points = 0
    let mutable stage = 1
    let mutable board = Board(width, height)
    let mutable piece = Piece(0, Vector3(4.690252f, -4.359645f, 0.0f) + Vector3(0.9596867f, 0.2810721f, 0.0f) * 4.0f)
    let mutable stateDead = true
    
    member this.Initialize() =
        stateDead <- false
        let rnd = Random(1)
        let rnd32() = float32 (rnd.NextDouble() * 2.0 - 1.0)
        let rnd(min, max) = float32 (rnd.NextDouble() * (max - min) + min)
        for i in {1 .. 40} do
            board.add(Piece(0, Vector3(rnd32() * 20.0f, rnd32() * 30.0f, 0.0f), Vector3(rnd(1.0, 5.0), rnd(1.0, 5.0), 1.0f), rnd32() * 3.1415926f, Color.Red))
        
    member this.Draw(gd, (spriteBatch:SpriteBatch), (effects:Dictionary<string, Effect>), (font:SpriteFont), gameTime) =
        board.draw(gd, effects.["colorfill"])
        
        if not(stateDead) then piece.draw(gd, effects.["colorfill"])
        
                
//        spriteBatch.Begin();
//        let str = sprintf "Level: %d" stage
//        spriteBatch.DrawString(font, str, new Vector2(50.0f, 210.0f), Color.White);
//        let str = sprintf "Lines: %d" points
//        spriteBatch.DrawString(font, str, new Vector2(50.0f, 240.0f), Color.White);
//        spriteBatch.End();

    member this.intersect() =
        match board.clip(piece) with
            | Some(collision, normal) ->
                piece.recordNormal(collision, normal) 
                piece.reflection(collision, normal) 
                this.intersect()
            | none -> ()
                
    member this.Update(gameTime) =
        piece.updatePosition()

        this.intersect()                

        if player.restart() then
            board <- Board(width, height)

    member this.Dead() =
        stateDead