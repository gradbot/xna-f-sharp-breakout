#light

open System
open System.Collections.Generic

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

open Ui

type MyGame() as this =
    inherit Game()
    
    let mutable graphics = Unchecked.defaultof<GraphicsDeviceManager>
    let mutable content = Unchecked.defaultof<ContentManager>
    let mutable font = Unchecked.defaultof<SpriteFont>
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let effects = new Dictionary<string, Effect>()
    
    let mutable ui = Unchecked.defaultof<Ui>

    do  graphics <- new GraphicsDeviceManager(this)
        graphics.PreferredBackBufferWidth <- 600
        graphics.PreferredBackBufferHeight <- 800
        graphics.SynchronizeWithVerticalRetrace <- true
        this.Window.Title <- "Breakout Clone - Remaking the Classics"
        content <- new ContentManager(this.Services)
        ui <- new Ui()

    override this.LoadContent() =
        effects.["colorfill"]  <- content.Load("colorfill")
        spriteBatch <- new SpriteBatch(graphics.GraphicsDevice)
        //font <- content.Load<SpriteFont>("arial");
    
    override this.Initialize() =
        base.Initialize()
        ui.Initialize()
        
    override this.Draw(gameTime) =
        let view(camera) =
            Matrix.CreateLookAt(
                camera + new Vector3(0.0f, 0.0f, 80.0f),
                camera,
                new Vector3(0.0f, 1.0f, 0.0f))

        let projection() =
            Matrix.CreatePerspectiveFieldOfView(
                MathHelper.Pi / 4.0f, 3.0f / 4.0f,
                0.01f,
                1000.0f)
                
        let gd = graphics.GraphicsDevice
        
        gd.Clear(new Color(0.0f, 0.0f, 0.0f))
        gd.RenderState.CullMode <- CullMode.None
        gd.VertexDeclaration <- new VertexDeclaration(gd, VertexPositionColor.VertexElements)

        let WorldViewProj = view(Vector3(0.0f, 0.0f, 0.0f)) * projection()
        effects.["colorfill"].Parameters.Item("WorldViewProj").SetValue(WorldViewProj)

        ui.Draw(gd, spriteBatch, effects, font, gameTime)

    override this.Update(gameTime) =
        ui.Update(gameTime)
        if ui.exit() then
            this.Exit()
            
    
let main() =
    let game = new MyGame()
    game.Run()

[<STAThread>]
do main()


