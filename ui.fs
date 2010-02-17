namespace Breakout

open System
open System.Collections.Generic

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type UIState = { 
    mutable gameStart : bool;
    mutable mainMenu : bool;
    mutable exit : bool;
    }

type Ui(gd : GraphicsDevice, resource) =
    let userInput = UserInput(PlayerIndex.One)

    let mutable breakout = Unchecked.defaultof<Breakout>
    
    let mutable menu = 0;
    let mutable strobeTick = 1;
    let mutable strobeVelocity = 10;

    let mutable state = {gameStart = true; mainMenu = false; exit = false;}

    member this.Initialize() =
        breakout <- Breakout(resource)
        breakout.Initialize()
        
    member this.Draw(gameTime) =
        let spriteBatch = resource.SpriteBatch.["hud"]
        if state.mainMenu then
            spriteBatch.Begin();
            
            strobeTick <- strobeTick + strobeVelocity
            if strobeTick > 90 then strobeVelocity <- -strobeVelocity
            if strobeTick < 11 then strobeVelocity <- -strobeVelocity
            let strobe = Color(float32 (strobeTick % 100 + 150) / 255.0f, 0.2f, 0.2f)
            
            let mutable color = Color.Green
            let font = resource.Fonts.["arial"]
            
            if menu = 0 then color <- strobe else color <- Color.Red
            spriteBatch.DrawString(font, "Start Game", new Vector2(50.0f, 50.0f), color);
            
            if menu = 1 then color <- strobe else color <- Color.Red
            spriteBatch.DrawString(font, "Quit Game", new Vector2(50.0f, 80.0f), color);
            
            spriteBatch.End();
        
        breakout.Draw(gd, gameTime)

    member this.Update(gameTime) =
        if state.mainMenu then
            userInput.Update()
            
            if userInput.DpadPressed(DPad.Up) then menu <- (menu + 1) % 2
            if userInput.DpadPressed(DPad.Down) then menu <- (menu + 1) % 2
            if userInput.ButtonPressed(Button.Back) then state.exit <- true

            if userInput.ButtonPressed(Button.A) || userInput.KeyPressed(Keys.Enter) then
                if menu = 0 then
                    state.mainMenu <- false
                    if state.gameStart then
                        state.gameStart <- false
                    else
                        breakout <- Breakout(resource)
                        breakout.Initialize()

                if menu = 1 then
                    state.exit <- true
        else
            breakout.Update(gameTime)
            if breakout.Dead() then
                state.mainMenu <- true
                userInput.Update()
                menu <- 0
            
    member this.exit() =
        state.exit