namespace Breakout

open System.Collections.Generic

open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Audio

type Resource =
    {
        Effects : Dictionary<string, Effect>;
        Sounds : Dictionary<string, SoundEffect>;
        Models : Dictionary<string, Model>;
//        BoneTransforms : Matrix[];
        Fonts : Dictionary<string, SpriteFont>;
        SpriteBatch : Dictionary<string, SpriteBatch>;
    }