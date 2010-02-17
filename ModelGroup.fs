namespace Breakout

open System
open System.Collections.Generic

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics

type DirectionalLight =
    {
        mutable DiffuseColor : Vector3;
        mutable SpecularColor : Vector3;
        mutable Direction : Vector3;
        mutable Enabled : bool;
    }
    with
    
    static member create(l : BasicDirectionalLight) =
        {
            DiffuseColor = l.DiffuseColor;
            SpecularColor = l.SpecularColor;
            Direction = l.Direction;
            Enabled = l.Enabled;
        }

    member l.applyTo(b : BasicDirectionalLight) =
        b.DiffuseColor <- l.DiffuseColor
        b.SpecularColor <- l.SpecularColor
        b.Direction <- l.Direction
        b.Enabled <- l.Enabled

type EffectState =
    {
        mutable DiffuseColor : Vector3;
        mutable DirectionalLight0 : DirectionalLight;
        mutable DirectionalLight1 : DirectionalLight;
        mutable DirectionalLight2 : DirectionalLight;
    }
    with                 
                 
    static member create(b : BasicEffect) =
        {
            DiffuseColor = b.DiffuseColor; 
            DirectionalLight0 = DirectionalLight.create(b.DirectionalLight0);
            DirectionalLight1 = DirectionalLight.create(b.DirectionalLight1);
            DirectionalLight2 = DirectionalLight.create(b.DirectionalLight2);
        }
    
    member e.applyTo(b : BasicEffect) =
        b.DiffuseColor <- e.DiffuseColor
        e.DirectionalLight0.applyTo(b.DirectionalLight0)
        e.DirectionalLight1.applyTo(b.DirectionalLight1)
        e.DirectionalLight2.applyTo(b.DirectionalLight2)


    static member Default =
        {
            DiffuseColor = Vector3(1.0f, 1.0f, 1.0f); 
            DirectionalLight0 = {DiffuseColor = Vector3(0.0f, 1.0f, 1.0f); SpecularColor = Vector3(1.0f); Direction = Vector3(-0.5f, -0.5f, -0.5f); Enabled = true};
            DirectionalLight1 = {DiffuseColor = Vector3(0.0f, 0.5f, 0.5f); SpecularColor = Vector3(1.0f); Direction = Vector3(0.5f, -0.5f, 0.5f); Enabled = true};
            DirectionalLight2 = {DiffuseColor = Vector3(); SpecularColor = Vector3(); Direction = Vector3(); Enabled = false};
        }

type ModelState = {
        mutable Model : Model;
        mutable Position : Vector3;
        mutable Scale : Vector3;
        mutable Rotation : float32;
        mutable EffectState : EffectState;
    }
    with
    
    static member view(position, rotation) =
        Matrix.CreateLookAt(
            position + new Vector3(0.0f, 0.0f, 80.0f),
            position,
            Vector3(sin(rotation), cos(rotation), 0.0f))

    static member projection =
        Matrix.CreatePerspectiveFieldOfView(
            MathHelper.Pi / 4.0f, 3.0f / 4.0f,
            0.01f,
            1000.0f)
    
    member this.draw(color : Color, alpha) =
        for mesh in this.Model.Meshes do
            for effect in mesh.Effects do
                let e = effect :?> BasicEffect
                e.World <- Matrix.Identity
                e.View <- Matrix.CreateScale(this.Scale * 0.25f) * Matrix.CreateRotationZ(this.Rotation) * ModelState.view(this.Position, 0.0f)
                e.Projection <- ModelState.projection
                e.LightingEnabled <- true
                this.EffectState.applyTo(e)
                
                e.Alpha <- alpha
                e.DirectionalLight0.DiffuseColor <- color.ToVector3()
                e.DirectionalLight1.DiffuseColor <- color.ToVector3()
                
                // since we rotate the camera we need to rotate the lights back so they appear stationary
                let rot = Matrix.CreateRotationZ(-this.Rotation)
                e.DirectionalLight0.Direction <- Vector3.Transform(e.DirectionalLight0.Direction, rot)
                e.DirectionalLight1.Direction <- Vector3.Transform(e.DirectionalLight1.Direction, rot)
                
            mesh.Draw();
