#light

open System;

open Microsoft.Xna.Framework;
open Microsoft.Xna.Framework.Input;
open Microsoft.Xna.Framework.Graphics;
open FarseerGames.FarseerPhysics;

open Resource
open Shape
open Object

type Player(resource) =
    let mutable position = Vector2(0.0f, -25.0f)
    let mutable DPad = GamePad.GetState(PlayerIndex.One).DPad
    let mutable DPadOld = DPad
    let mutable buttons = GamePad.GetState(PlayerIndex.One).Buttons
    let mutable buttonsOld = buttons
    let mutable buttonDelay = 0
    let random = Random()
    let obj = Object(resource.Models.["cube"], Arc, position, Vector2(5.0f, 1.0f), 0.0f, Color.Green, Static, CollisionCategory.Cat4)
    
    do
        obj.Geometry.CollisionGroup <- 1
    
    let PressedOnce(button, oldButton) =
        (button = ButtonState.Pressed && oldButton <> ButtonState.Pressed)

    member this.getPosition() =
        position
        
    member this.getPaddle() =
        obj

    member this.updateInput() =
        DPadOld <- DPad
        buttonsOld <- buttons
        DPad <- GamePad.GetState(PlayerIndex.One).DPad
        buttons <- GamePad.GetState(PlayerIndex.One).Buttons
                        
        let sticks = GamePad.GetState(PlayerIndex.One).ThumbSticks
        position <- position + Vector2(sticks.Left.X * 1.0f, 0.0f)
        if position.X < -15.0f then position.X <- -15.0f
        if position.X > 15.0f then position.X <- 15.0f
        
        obj.Body.Position <- position

//        if (DPad.Down = ButtonState.Pressed) then piece.down()
//        if (DPad.Up = ButtonState.Pressed) then piece.up()

//        if DPad.Left = ButtonState.Pressed || DPad.Right = ButtonState.Pressed then
//            buttonDelay <- buttonDelay + 1
//        else
//            buttonDelay <- 0
            
//        if buttonDelay = 1 || (buttonDelay > 5 && buttonDelay % 2 = 1) then
//            if DPad.Left = ButtonState.Pressed then piece.left()
//            if DPad.Right = ButtonState.Pressed then piece.right()
//            
//        if PressedOnce(buttons.A, buttonsOld.A) then piece.rotate(true)
//        if PressedOnce(buttons.B, buttonsOld.B) then piece.rotate(false)
        
    member this.restart() =
        PressedOnce(buttons.Start, buttonsOld.Start)