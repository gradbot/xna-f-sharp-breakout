open System
open System.Collections.Generic

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open Microsoft.Xna.Framework.Audio

open Breakout

type MyGame() as this =
    inherit Game()
    
    let mutable graphics = Unchecked.defaultof<GraphicsDeviceManager>
    let mutable content = Unchecked.defaultof<ContentManager>
    let mutable audioEngine = Unchecked.defaultof<AudioEngine>
    let resource =
        {
            Effects = new Dictionary<string, Effect>();
            Sounds = new Dictionary<string, SoundEffect>();
            Models = new Dictionary<string, Model>();
//            BoneTransforms = Array.init 1 (fun i -> Matrix());
            Fonts = new Dictionary<string, SpriteFont>();
            SpriteBatch = new Dictionary<string, SpriteBatch>();
        }
    let mutable ui = Unchecked.defaultof<Ui>

    do  graphics <- new GraphicsDeviceManager(this)
        graphics.PreferredBackBufferWidth <- 600
        graphics.PreferredBackBufferHeight <- 800
        graphics.PreferMultiSampling <- true
        graphics.SynchronizeWithVerticalRetrace <- true
        this.Window.Title <- "Breakout Clone - Remaking the Classics"
        content <- new ContentManager(this.Services)

    override this.LoadContent() =
        let gd = graphics.GraphicsDevice
        resource.Effects.["colorfill"]  <- content.Load("colorfill")
        resource.Sounds.["button-22"] <- content.Load("button-22")
        resource.Fonts.["arial"] <- content.Load<SpriteFont>("arial");
        resource.Models.["cube"] <- content.Load("cube")
        resource.SpriteBatch.["hud"] <- new SpriteBatch(gd)
        
        
    override this.Initialize() =
        base.Initialize()
        ui <- new Ui(graphics.GraphicsDevice, resource)
        ui.Initialize()
        
    override this.Draw(gameTime) =
        let gd = graphics.GraphicsDevice
        gd.Clear(new Color(0.0f, 0.0f, 0.0f))
        gd.VertexDeclaration <- new VertexDeclaration(gd, VertexPositionColor.VertexElements)

        let WorldViewProj = ModelState.view(Vector3(0.0f, 0.0f, 0.0f), 0.0f) * ModelState.projection
        resource.Effects.["colorfill"].Parameters.Item("WorldViewProj").SetValue(WorldViewProj)

        ui.Draw(gameTime)

    override this.Update(gameTime) =
        ui.Update(gameTime)
        if ui.exit() then
            this.Exit()            
    
let main() =
    let game = new MyGame()
    game.Run()

[<STAThread>]
do main()

