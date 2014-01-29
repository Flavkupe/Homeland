using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nuclex.Input;
using Nuclex.UserInterface;
using Nuclex.UserInterface.Visuals.Flat;
using TacticsGame.Managers;
using TacticsGame.Scene;

namespace TacticsGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GuiManager guiManager;
        InputManager input;

        public MainGame()
        {
            this.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.guiManager = new GuiManager(this.Services);

            this.IsMouseVisible = true;
            //this.graphics.IsFullScreen = true;

            GameStateManager.Instance.GameReference = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            InitializeInput();                
           
            base.Initialize();

            // Make sure the first scene gets initialized after everything else.                        
        }

        private static void InitializeFirstScene()
        {
            //ZoneCombatScene firstScene = new ZoneCombatScene();
            
            // TODO: put this somewhere else?
            GameStateManager.Instance.World = WorldGenerationManager.Instance.GenerateNewWorld();
            ZoneManagementScene firstScene = new ZoneManagementScene();

            GameStateManager.Instance.PushScene(firstScene);
        }

        public GraphicsDeviceManager Graphics { get { return this.graphics; } }

        private void InitializeInput()
        {
            this.input = new InputManager(Services, Window.Handle);
            Components.Add(this.input);
        }

        private void InitializeGUI()
        {
            Viewport viewport = GraphicsDevice.Viewport;
            Screen mainScreen = new Screen(viewport.Width, viewport.Height);
            this.guiManager.Screen = mainScreen;
            
            mainScreen.Desktop.Bounds = new UniRectangle(
              new UniScalar(0.0f, 0.0f), new UniScalar(0.0f, 0.0f), // x and y
              new UniScalar(1.0f, 0.0f), new UniScalar(1.0f, 0.0f) // width and height
            );
           
            this.guiManager.Initialize();
            GameStateManager.Instance.InitCameraView();

            InterfaceManager.Instance.Gui = this.guiManager;

            InterfaceManager.Instance.CreateDialogsAndPanes();

            FlatGuiVisualizer visualizer = Nuclex.UserInterface.Visuals.Flat.FlatGuiVisualizer.FromResource(Services, UIResources.ResourceManager, "FlavSkin_skin");                        
            this.guiManager.Visualizer = visualizer;
            InterfaceManager.Instance.AddAdditionalControls();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);            
            TextureManager.Instance.SpriteBatch = spriteBatch;
            TextureManager.Instance.LoadAllFonts();
            this.LoadResources();            

            InitializeGUI();

            InitializeFirstScene();               
        }

        private void LoadResources()
        {
            GameResourceManager.Instance.LoadAllResources();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            GameStateManager.Instance.CurrentScene.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            // TODO: Add your drawing code here

            GameStateManager.Instance.CurrentScene.Draw(gameTime);

            spriteBatch.End();

            GameStateManager.Instance.CurrentScene.DrawUI(gameTime);

            base.Draw(gameTime);            
        }
    }
}
