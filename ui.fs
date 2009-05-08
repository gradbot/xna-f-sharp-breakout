#light

open System
open System.Collections.Generic

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

open Resource
open Breakout

type Ui(gd : GraphicsDevice, resource) =
    let mutable DPad = GamePad.GetState(PlayerIndex.One).DPad
    let mutable DPadOld = DPad
    let mutable buttons = GamePad.GetState(PlayerIndex.One).Buttons
    let mutable buttonsOld = buttons
    let mutable breakout = Unchecked.defaultof<Breakout>
    
    let mutable exitFlag = false;
    let mutable menu = 0;
    let mutable strobeTick = 1;
    let mutable strobeVelocity = 10;

    let mutable stateMenu = false

    member this.Initialize() =
        breakout <- Breakout(resource)
        breakout.Initialize()
        
    member this.Draw(gameTime) =
        let spriteBatch = resource.SpriteBatch.["hud"]
        if stateMenu then
            spriteBatch.Begin();
            
            strobeTick <- strobeTick + strobeVelocity
            if strobeTick > 90 then strobeVelocity <- -strobeVelocity
            if strobeTick < 11 then strobeVelocity <- -strobeVelocity
            let strobe = Color((float32)(strobeTick % 100 + 150) / 255.0f, 0.2f, 0.2f)
            
            let mutable color = Color.Green
            let font = resource.Fonts.["arial"]
            
            if menu = 0 then color <- strobe else color <- Color.Red
            spriteBatch.DrawString(font, "Start Game", new Vector2(50.0f, 50.0f), color);
            
            if menu = 1 then color <- strobe else color <- Color.Red
            spriteBatch.DrawString(font, "Quit Game", new Vector2(50.0f, 80.0f), color);
            
            spriteBatch.End();
        
        breakout.Draw(gd, gameTime)

    member this.Update(gameTime) =
        if stateMenu then
            DPadOld <- DPad
            buttonsOld <- buttons
            DPad <- GamePad.GetState(PlayerIndex.One).DPad
            buttons <- GamePad.GetState(PlayerIndex.One).Buttons
            
            let PressedOnce(button, oldButton) =
                (button = ButtonState.Pressed && oldButton <> ButtonState.Pressed)
            
            if PressedOnce(DPad.Up, DPadOld.Up) then menu <- (menu + 1) % 2
            if PressedOnce(DPad.Down, DPadOld.Down) then menu <- (menu + 1) % 2
            if (buttons.Back = ButtonState.Pressed) then exitFlag <- true
            if (buttons.A = ButtonState.Pressed) then
                if menu = 0 then
                    stateMenu <- false
                    breakout <- Breakout(resource)
                    breakout.Initialize()
                if menu = 1 then
                    exitFlag <- true
        else
            breakout.Update(gameTime)
            if breakout.Dead() then
                stateMenu <- true
                DPad <- GamePad.GetState(PlayerIndex.One).DPad
                buttons <- GamePad.GetState(PlayerIndex.One).Buttons
                buttonsOld <- buttons
                DPadOld <- DPad
                menu <- 0
            
    member this.exit() =
        exitFlag