using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Map;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TacticsGame.Utility;
using TacticsGame.GameObjects;
using TacticsGame.Abilities;
using TacticsGame.Items;
using TacticsGame.Managers;
using TacticsGame.Utility.Classes;

namespace TacticsGame.Managers
{
    public class TextureManager : Singleton<TextureManager>
    {
        private TextureManager() 
            : base()
        {                        
        }

        public void LoadAllFonts() 
        {
            debugFont = LoadFont("Fonts/Debug");
            mainFont = LoadFont("Fonts/MainFont");
        }

        private bool ignoreTextures = false;
        public void SetToIgnoreTextures()
        {
            this.ignoreTextures = true;
        }

        private SpriteBatch spriteBatch;

        private Dictionary<string, Texture2D> texturesByContentName = new Dictionary<string, Texture2D>();

        SpriteFont debugFont;
        SpriteFont mainFont;

        public SpriteFont DebugFont
        {
            get { return debugFont; }
            set { debugFont = value; }
        }

        public TextureInfo GetTextureInfo(string name, ResourceType type)
        {
            return GameResourceManager.Instance.GetResourceByResourceType(name, type).TextureInfo;
        }

        public IconInfo GetIconInfo(Enum name)
        {
            return this.GetIconInfo(name.ToString());
        }

        public IconInfo GetIconInfo(string name)
        {
            return GameResourceManager.Instance.GetIcon(name);
        }

        public IconInfo GetTextureAsIconInfo(string name, ResourceType type)
        {
            TextureInfo info = this.GetTextureInfo(name, type);
            return new IconInfo(info.Texture, 32);
        }

        public Dictionary<string, Texture2D> TexturesByContentName
        {
            get { return this.texturesByContentName; }
        }

        /// <summary>
        /// Loads a 2D texture and caches it for future use. If the Texture2D was already loaded, this returns a reference to that existing
        /// Texture2D, such that all objects will share the same instance of it.
        /// </summary>
        /// <param name="name">Name of the content resource thing.</param>
        /// <returns>The Texture2D object.</returns>
        public Texture2D LoadTexture2D(string name)
        {
            if (this.ignoreTextures || string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (TextureManager.Instance.TexturesByContentName.ContainsKey(name))
            {
                return TextureManager.Instance.TexturesByContentName[name];
            }

            Texture2D texture = GameStateManager.Instance.GameReference.Content.Load<Texture2D>(name);
            TextureManager.Instance.TexturesByContentName.Add(name, texture);
            return texture;
        }

        public SpriteFont LoadFont(string name)
        {
            return GameStateManager.Instance.GameReference.Content.Load<SpriteFont>(name);
        }

        public SpriteBatch SpriteBatch
        {
            get { return this.spriteBatch; }
            set 
            { 
                this.spriteBatch = value;
                this.InitializeDummyTextures();
            }
        }

        public Texture2D DummyRectangleTexture;
        private void InitializeDummyTextures()
        {
            this.DummyRectangleTexture = new Texture2D(GameStateManager.Instance.GameReference.GraphicsDevice, 1, 1);
            this.DummyRectangleTexture.SetData(new Color[] { Color.White });
        }

        public void DrawDebugText(string text, Vector2 position)
        {
            this.spriteBatch.DrawString(this.debugFont, text, position, Color.Red);
        }

        public void DrawText(string text, Vector2 position, Color color, float scale = 1.0f, float rotation = 0.0f)
        {
            this.spriteBatch.DrawString(this.mainFont, text, position, color, rotation, new Vector2(0,0), scale, SpriteEffects.None, 0);
        }

        public Vector2 GetTextSize(string text)
        {
            return this.mainFont.MeasureString(text);
        }

        public void DrawRectangle(Rectangle destination, Color color)
        {
            this.spriteBatch.Draw(this.DummyRectangleTexture, destination, color);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? source, Color color, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects, int layerDepth)
        {
            this.spriteBatch.Draw(texture, position, source, color, rotation, origin, scale, spriteEffects, layerDepth);
        }

        public void Draw(Texture2D texture, Rectangle position, Rectangle source, Color color)
        {
            this.spriteBatch.Draw(texture, position, source, color);
        }       
    }
}
