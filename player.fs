#light

open System;

open Microsoft.Xna.Framework;
open Microsoft.Xna.Framework.Input;

open Piece

type Player() =
    let mutable position = Vector3(0.0f, 0.0f, 0.0f)
    let mutable DPad = GamePad.GetState(PlayerIndex.One).DPad
    let mutable DPadOld = DPad
    let mutable buttons = GamePad.GetState(PlayerIndex.One).Buttons
    let mutable buttonsOld = buttons
    let mutable buttonDelay = 0
    let random = Random()
    
    let PressedOnce(button, oldButton) =
        (button = ButtonState.Pressed && oldButton <> ButtonState.Pressed)

    member this.getPosition() = position

    member this.updateInput(piece: Piece) =
        DPadOld <- DPad
        buttonsOld <- buttons
        DPad <- GamePad.GetState(PlayerIndex.One).DPad
        buttons <- GamePad.GetState(PlayerIndex.One).Buttons
                        
//        if (DPad.Down = ButtonState.Pressed) then piece.down()
//        if (DPad.Up = ButtonState.Pressed) then piece.up()

        if DPad.Left = ButtonState.Pressed || DPad.Right = ButtonState.Pressed then
            buttonDelay <- buttonDelay + 1
        else
            buttonDelay <- 0
            
//        if buttonDelay = 1 || (buttonDelay > 5 && buttonDelay % 2 = 1) then
//            if DPad.Left = ButtonState.Pressed then piece.left()
//            if DPad.Right = ButtonState.Pressed then piece.right()
//            
//        if PressedOnce(buttons.A, buttonsOld.A) then piece.rotate(true)
//        if PressedOnce(buttons.B, buttonsOld.B) then piece.rotate(false)
        
    member this.restart() =
        PressedOnce(buttons.Start, buttonsOld.Start)