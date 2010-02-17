namespace Breakout

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type Button =
    |X    |Y
    |A    |B
    |Start
    |Back
    |LeftShoulder
    |RightShoulder
    |LeftStick
    |RightStick
    |BigButton

type DPad =
    |Up
    |Down
    |Right
    |Left

type UserInput(player) =
    let mutable dPad = GamePad.GetState(player).DPad
    let mutable dPadOld = dPad
    
    let mutable buttons = GamePad.GetState(player).Buttons
    let mutable buttonsOld = buttons
    
    let mutable keys = Keyboard.GetState(player)
    let mutable keysOld = keys
    
    member this.Update() =
        dPadOld <- dPad
        buttonsOld <- buttons
        keysOld <- keys
        
        dPad <- GamePad.GetState(PlayerIndex.One).DPad
        buttons <- GamePad.GetState(PlayerIndex.One).Buttons
        keys <- Keyboard.GetState(PlayerIndex.One)

    member this.KeyPressed(key) =
        keysOld.IsKeyDown(key) && keys.IsKeyUp(key)
        
    member this.ButtonPressed(button) =
        match button with
        | X -> (buttons.X = ButtonState.Pressed && buttonsOld.X <> ButtonState.Pressed)
        | Y -> (buttons.Y = ButtonState.Pressed && buttonsOld.Y <> ButtonState.Pressed)
        | A -> (buttons.A = ButtonState.Pressed && buttonsOld.A <> ButtonState.Pressed)
        | B -> (buttons.B = ButtonState.Pressed && buttonsOld.B <> ButtonState.Pressed)
        | Start -> (buttons.Start = ButtonState.Pressed && buttonsOld.Start <> ButtonState.Pressed)
        | Back  -> (buttons.Back = ButtonState.Pressed && buttonsOld.B <> ButtonState.Pressed)
        | LeftShoulder  -> (buttons.LeftShoulder = ButtonState.Pressed && buttonsOld.LeftShoulder <> ButtonState.Pressed)
        | RightShoulder -> (buttons.RightShoulder = ButtonState.Pressed && buttonsOld.RightShoulder <> ButtonState.Pressed)
        | LeftStick  -> (buttons.LeftStick = ButtonState.Pressed && buttonsOld.LeftStick <> ButtonState.Pressed)
        | RightStick -> (buttons.RightStick = ButtonState.Pressed && buttonsOld.RightStick <> ButtonState.Pressed)
        | BigButton  -> (buttons.BigButton = ButtonState.Pressed && buttonsOld.BigButton <> ButtonState.Pressed)


    member this.DpadPressed(pad) =
        match pad with
        | Up    -> (dPad.Up = ButtonState.Pressed && dPadOld.Up <> ButtonState.Pressed)
        | Down  -> (dPad.Down = ButtonState.Pressed && dPadOld.Down <> ButtonState.Pressed)
        | Left  -> (dPad.Left = ButtonState.Pressed && dPadOld.Left <> ButtonState.Pressed)
        | Right -> (dPad.Right = ButtonState.Pressed && dPadOld.Right <> ButtonState.Pressed)
