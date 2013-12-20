using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #region User Defined Variables
        //------------------------------------------
        // Added for use with fonts
        //------------------------------------------
        SpriteFont fontToUse;

        //--------------------------------------------------
        // Added for use with playing Audio via Media player
        //--------------------------------------------------
        private Song bkgMusic;
        private String songInfo;

        Vector2 mPosition = new Vector2(0, 0);
        Texture2D space;
        //--------------------------------------------------
        //Set the sound effects to use
        //--------------------------------------------------
        private SoundEffectInstance playerSoundInstance;
        private Song playerSound;
        private SoundEffect explosionSound;
        private SoundEffect firingSound;
        private SoundEffect cherrySound;

        // Set the 3D model to draw.
        private Model mdlPlayer;
        private Matrix[] mdlPlayerTransforms;
        private float playerScale;

        private ResetLives objcherry;

        // The aspect ratio determines how to scale 3d to 2d projection.
        private float aspectRatio;

        // Set the position of the model in world space, and set the rotation.
        private Vector3 mdlPosition = new Vector3(0.0f,0.0f,40.0f);
        private float mdlRotation = 3.15f;        
        public Vector3 mdlVelocity = Vector3.Zero;
       
        // create an array of enemy daleks
        private Model mdlEnemy;
        private Matrix[] mdlEnemyTransforms;
        private Daleks[] enemyList = new Daleks[GameConstants.NumDaleks];

        // A float to hold the scale value for the enemy models
        private float enemyScale;

        //Reset lives variable
        private Model mdlCherry;
        private Matrix[] mdResetLiveTransforms;
        private ResetLives[] cherryList = new ResetLives[GameConstants.NumLives];
        private float livesScale;
        private Vector3 cherryPosition = new Vector3(0.0f,0.0f,30.0f);        

        // create an array of laser bullets
        private Model mdlRocket;
        private Matrix[] mdlRocketTransforms;
        private Laser laser;
        private float laserRotation;
        private Random random = new Random();
        private KeyboardState lastState;
        private GamePadState oldSate;
       
        //Variable to hold the hitCount
        private int hitCount;

        //Textures to hold start and end screens
        public Texture2D gameOver;
        public Texture2D start;

        //audio variables
        public static float MasterVolume { get; set; }
        
        // Set the position of the camera in world space, for our view matrix.
        private Vector3 cameraPosition = new Vector3(0.0f, 100.0f, 100.0f);
        private Vector3 cameraPosition2 = new Vector3(0.0f, 100.0f, 100.0f);
        
        // View and projection matrix variables
        private Matrix viewMatrix;
        private Matrix projectionMatrix;
        private Matrix ViewMatrix2;
        private Matrix ProjectionMatrix2;

        //The rotation matrix
        Matrix RotationMatrix;
        //Sets the timer
        private float timer = 0.0f;
        // variable for score
        private int score;    
        // sets lives to 5
        private int lives = 5;

        // Booleans for switching between the different cameras and for the music to be turned on/off
        private bool cams = true;
        private bool MusicOn = true;
        
        // An enum to hold different Gamestates to switch between the start screen, the game and end screen.
        public enum GameState
        {
            start,
            game,
            end
        }

        //set the current Gamestate to the start screen
        GameState currentGameState = GameState.start;

        private void InitializeTransform()
        {
            

        }

        //Create the view and projection matrices for the first camera
        private void Camera1()
        {
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

            viewMatrix = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45), aspectRatio, 1.0f, 350.0f);
        }

        //Create the 2nd camera
        private void Camera2()
        {
            //Set the camera position
            cameraPosition2 = new Vector3(0, 10, 35);
            RotationMatrix = Matrix.CreateRotationY(mdlPosition.Y);

            //Transforms the camera position  
            Vector3 transformedReference = Vector3.Transform(cameraPosition2, RotationMatrix);

            cameraPosition2 = transformedReference + mdlPosition;            

            ViewMatrix2 = Matrix.CreateLookAt(cameraPosition2, mdlPosition, Vector3.Up);

            Viewport viewPort = graphics.GraphicsDevice.Viewport;
            float aspectRatio = (float)viewPort.Width / (float)viewPort.Height;
            ProjectionMatrix2 = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45), aspectRatio, 1.0f, 350.0f);
        }

        //Method for moving the models
        private void MoveModel()
        {
            //Retrieve the state of the keyboard
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);


            // Create some velocity if the right trigger is down.
            Vector3 mdlVelocityAdd = Vector3.Zero;
            Vector3 modelVelocityLeftRight = Vector3.Zero;
            modelVelocityLeftRight.X = 1.0f;

            // Find out what direction we should be thrusting, using rotation.
            mdlVelocityAdd.X = -(float)Math.Sin(mdlRotation);
            mdlVelocityAdd.Z = -(float)Math.Cos(mdlRotation);

            //If the start screen is active and the user presses the enter key then load the game.
            if (currentGameState == GameState.start)
            {
                if (keyboardState.IsKeyDown(Keys.Enter) && lastState.IsKeyUp(Keys.Enter))
                {
                    currentGameState = GameState.game;
                }
                
                    //If the game itself is active then run the game.
                else if (currentGameState == GameState.game)
                {
                    Game1 game = new Game1();
                    game.Run();
                }

            }

            //If the left key is down move left
            if (keyboardState.IsKeyDown(Keys.Left) || (gamePadState.DPad.Left == ButtonState.Pressed)
)
            {
                
                modelVelocityLeftRight *= -0.1f;
                mdlVelocity += modelVelocityLeftRight;

            }

            //If the right key is down move right
            if (keyboardState.IsKeyDown(Keys.Right) || (gamePadState.DPad.Right == ButtonState.Pressed))
            {
                
                modelVelocityLeftRight *= 0.1f;
                mdlVelocity += modelVelocityLeftRight;
            }
                       
            //If the L key is pressed switch to the first camera by setting the camera bool to true
            if (keyboardState.IsKeyDown(Keys.L))
            {
                cams = true;
            }
            //If the K key is pressed switch to the second camera by setting the camera bool to false
            if (keyboardState.IsKeyDown(Keys.K))
            {
                cams = false;
            }
            //Toggle the background music and the sound effects off by pressing the A key
            if (keyboardState.IsKeyDown(Keys.A) && lastState.IsKeyUp(Keys.A))
            {              
                    MediaPlayer.Pause();
                    SoundEffect.MasterVolume = 0.0f; 
            }
            //Resume playing the background music and the sound effects by pressing S
            if (keyboardState.IsKeyDown(Keys.S) && lastState.IsKeyUp(Keys.S))
            {
                    MediaPlayer.Resume();
                    SoundEffect.MasterVolume = 1.0f;
            }

            if (currentGameState == GameState.start)
            {
                if (keyboardState.IsKeyDown(Keys.Enter) && lastState.IsKeyUp(Keys.Enter))
                {
                    
                }

                else if (currentGameState == GameState.game)
                {
 
                }
 
            }



            
            //are we shooting?
            if (keyboardState.IsKeyDown(Keys.Space) && lastState.IsKeyUp(Keys.Space)||(gamePadState.Buttons.A == ButtonState.Pressed))
            {
                //add another bullet.  Find an inactive bullet slot and use it
                //if all bullets slots are used, ignore the user input
                //for (int i = 0; i < GameConstants.NumLasers; i++)
                //{
                laserRotation = mdlRotation;
                    
                        Matrix tardisTransform = Matrix.CreateRotationY(laserRotation);
                        laser.direction = tardisTransform.Forward;
                        laser.speed = GameConstants.LaserSpeedAdjustment;
                        laser.position = mdlPosition + laser.direction;
                        laser.isActive = true;
                        firingSound.Play();
                             
                    

                  //}
            }
            lastState = keyboardState;

        }

        //Resets the enemy Daleks
        private void ResetEnemies()
        {
            float xStart;
            float zStart;
            for (int i = 0; i < GameConstants.NumDaleks; i++)
            {
                if (random.Next(2) == 0)
                {
                    xStart = (float)-GameConstants.PlayfieldSizeX;
                }
                else
                {
                    xStart = (float)GameConstants.PlayfieldSizeX;
                }
                zStart = (float)random.NextDouble() * GameConstants.PlayfieldSizeZ;
                enemyList[i].position = new Vector3(xStart, 0.0f, 200);
                double angle = random.NextDouble() * 2 * Math.PI;
                enemyList[i].direction.X = -(float)Math.Sin(angle);
                enemyList[i].direction.Z = (float)Math.Cos(angle);
                enemyList[i].speed = GameConstants.DalekMinSpeed +
                   (float)random.NextDouble() * GameConstants.DalekMaxSpeed;
                enemyList[i].isActive = true;
            }

            //Resets the number of life objects spawning in the game
            for (int i = 0; i < GameConstants.NumLives; i++)
            {
                if (random.Next(2) == 0)
                {
                    xStart = (float)-GameConstants.PlayfieldSizeX;
                }
                else
                {
                    xStart = (float)GameConstants.PlayfieldSizeX;
                }
                zStart = (float)random.NextDouble() * GameConstants.PlayfieldSizeZ;
                cherryList[i].position = new Vector3(xStart, 0.0f, zStart);
                double angle = random.NextDouble() * 2 * Math.PI;
                cherryList[i].direction.X = -(float)Math.Sin(angle);
                cherryList[i].direction.Z = (float)Math.Cos(angle);
                cherryList[i].speed = GameConstants.lifeMinSpeed +
                   (float)random.NextDouble() * GameConstants.lifeMaxSpeed;
                cherryList[i].isActive = true;
            }

        }

        private Matrix[] SetupEffectTransformDefaults(Model myModel)
        {
            Matrix[] absoluteTransforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(absoluteTransforms);

            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //Changes the camera boolean to show the different camera
                    effect.EnableDefaultLighting();
                    if (cams == true)
                    {
                        effect.Projection = projectionMatrix;
                        effect.View = viewMatrix;
                    }

                    // Changes the boolean to view the second camera
                    if (cams == false)
                    {
                        effect.Projection = ProjectionMatrix2;
                        effect.View = ViewMatrix2;
                    }
                    
                }
            }
            return absoluteTransforms;
        }

        public void DrawModel(Model model, Matrix modelTransform, Matrix[] absoluteBoneTransforms)
        {
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in model.Meshes)
            {
                //This is where the mesh orientation is set
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //If tardis/spaceship
                    if (model == mdlPlayer)
                    {
                        effect.World = Matrix.CreateScale(playerScale) * absoluteBoneTransforms[mesh.ParentBone.Index] * modelTransform;
                    }
                    else
                    //every other model
                    {
                        effect.World = Matrix.CreateScale(enemyScale) * Matrix.CreateRotationZ(250.0f) * absoluteBoneTransforms[mesh.ParentBone.Index] * modelTransform;
                    }

                    //Changes the camera boolean to show the different camera
                    if (cams == true)
                    {
                        effect.Projection = projectionMatrix;
                        effect.View = viewMatrix;
                    }

                        //Changes the camera boolean to show the second camera
                    else if(cams == false) 
                    {
                        effect.Projection = ProjectionMatrix2;
                        effect.View = ViewMatrix2;
                    }
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }

        //Method for writing text to the screen 
        private void writeText(string msg, Vector2 msgPos, Color msgColour)
        {
            //Begin the spriteBatch
            spriteBatch.Begin();
            string output = msg;
            // Find the center of the string
            Vector2 FontOrigin = fontToUse.MeasureString(output) / 2;
            Vector2 FontPos = msgPos;
            // Draw the string
            spriteBatch.DrawString(fontToUse, output, FontPos, msgColour);
            //End the spritebatch
            spriteBatch.End();
        }

        #endregion

        //Loads the game
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// 

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            Window.Title = "Space Invaders";
            hitCount = 0;
            InitializeTransform();
            Camera1();
            Camera2();
            ResetEnemies();

            
            
            base.Initialize();

            
           
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //load background
            space = this.Content.Load<Texture2D>("Texture\\spaceBackground");
            gameOver = Content.Load<Texture2D>("Texture\\End");
            start = Content.Load <Texture2D>("Texture\\Start");
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            //-------------------------------------------------------------
            // added to load font
            //-------------------------------------------------------------
            fontToUse = Content.Load<SpriteFont>(".\\Fonts\\DrWho");
            //-------------------------------------------------------------
            // added to load Song
            //-------------------------------------------------------------
           // bkgMusic = Content.Load<Song>(".\\Audio\\gravity");
            // if (MusicOn == true)
           // {
               // MediaPlayer.Play(bkgMusic);
            //}
            
           // songInfo = "Song: " + bkgMusic.Name + " Song Duration: " + bkgMusic.Duration.Minutes + ":" + bkgMusic.Duration.Seconds;
            //-------------------------------------------------------------
            // added to load Model
            //-------------------------------------------------------------
            mdlPlayer = Content.Load<Model>(".\\Models\\eurofighter fbx");
            playerScale = 0.5f;
            mdlPlayerTransforms = SetupEffectTransformDefaults(mdlPlayer);
            mdlCherry = Content.Load<Model>(".\\Models\\cherry");
            livesScale = 0.5f;
            mdResetLiveTransforms = SetupEffectTransformDefaults(mdlCherry);

            mdlEnemy = Content.Load<Model>(".\\Models\\SpaceInvader");
            enemyScale = 18.0f;
            mdlEnemyTransforms = SetupEffectTransformDefaults(mdlEnemy);
            mdlRocket = Content.Load<Model>(".\\Models\\Missile");
            mdlRocketTransforms = SetupEffectTransformDefaults(mdlRocket);
            //-------------------------------------------------------------
            // added to load SoundFX's
            //-------------------------------------------------------------
            playerSound = Content.Load<Song>("Audio\\gravity");
            explosionSound = Content.Load<SoundEffect>("Audio\\explosion");
            firingSound = Content.Load<SoundEffect>("Audio\\laser");
            cherrySound = Content.Load<SoundEffect>("Audio\\health");

           
            MediaPlayer.Play(playerSound);

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
            MoveModel();
            Camera1();
            Camera2();
            // Add velocity to the current position.
            mdlPosition += mdlVelocity;

            // Bleed off velocity over time.
            mdlVelocity *= 0.95f;

         
            //If players position goes outwith the specified boundaries set the players velocity to 0
            if (mdlPosition.X > 50 || mdlPosition.X < -50)
            {
                mdlVelocity = new Vector3(0,0,0);
                
            }
            
            //Set the daleks position so that they don't spawn behind the players position
             for (int i = 0; i < GameConstants.NumDaleks; i++)
             {
                 if (enemyList[i].position.Z > 50)
                 {
                     enemyList[i].direction.Z = -enemyList[i].direction.Z;
                 }
                     
             }

            //Updates the enemy list as the game progresses
            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < GameConstants.NumDaleks; i++)
            {
                enemyList[i].Update(timeDelta);
            }
            //Updates the laser array list as the game progresses
            for (int i = 0; i < GameConstants.NumLasers; i++)
            {
                if (laser.isActive)
                {
                    laser.Update(timeDelta);
                }
            }
            //Updates the lives array list as the game progresses
            for (int j = 0; j < GameConstants.NumLives; j++)
            {
                cherryList[j].Update(timeDelta);
            }

            
            //Bounding sphere for the player model
            BoundingSphere TardisSphere =
              new BoundingSphere(mdlPosition,
                       mdlPlayer.Meshes[0].BoundingSphere.Radius *
                             GameConstants.ShipBoundingSphereScale);

            

            //Check for collisions
            for (int i = 0; i < enemyList.Length; i++)
            {
                //Creates a bounding sphere for the enemy models
                if (enemyList[i].isActive)
                {
                    BoundingSphere dalekSphereA =
                      new BoundingSphere(enemyList[i].position, mdlEnemy.Meshes[0].BoundingSphere.Radius *
                                     GameConstants.DalekBoundingSphereScale);

                    //Creates a bounding sphere for the laser model 
                        if (laser.isActive)
                        {
                            BoundingSphere laserSphere = new BoundingSphere(
                              laser.position, mdlRocket.Meshes[0].BoundingSphere.Radius *
                                     GameConstants.LaserBoundingSphereScale);

                            //Checks if the enemy model intersects the laser model
                            if (dalekSphereA.Intersects(laserSphere))
                            {
                                //Plays a sound, takes the laser and enemy models that collided off the screen, increases the hitcount and adds 50 to the score
                                explosionSound.Play();
                                enemyList[i].isActive = false;
                                laser.isActive = false;
                                hitCount++;
                                score = score + 50;

                                //If the hitcount is greater than the original amount of enemies reset the amount of enemies and the hitcount                                
                                if (hitCount >= 23)
                                {
                                    ResetEnemies();
                                    hitCount = 0;
                                }

                                
                                
                                

                                break; //no need to check other bullets
                            }
                        }
                        if (dalekSphereA.Intersects(TardisSphere)) //Check collision between Dalek and Tardis
                        {
                            //Play a sound on collision
                            explosionSound.Play();
                            //Changes the direction
                            enemyList[i].direction *= -1.0f;
                            //Removes a life
                            lives--;
                            //Change the enemy direction upon collision to go away from the player
                            mdlVelocity = -mdlVelocity;
                            //laserList[k].isActive = false;
                            break; //no need to check other bullets
                        }
                        

                   
                        }

                
                
            }


            for (int j = 0; j < cherryList.Length; j++)
            {
                if (cherryList[j].isActive)
                {
                    //Creates a bounding sphere for the health model
                    BoundingSphere CherrySphere =
                      new BoundingSphere(cherryList[j].position, mdlCherry.Meshes[0].BoundingSphere.Radius *
                                     GameConstants.cherryBoundingSphereScale);

                        if (laser.isActive)
                        {
                            //Creates a bounding sphere for the laser model
                            BoundingSphere laserSphere = new BoundingSphere(
                              laser.position, mdlRocket.Meshes[0].BoundingSphere.Radius *
                                     GameConstants.LaserBoundingSphereScale);

                            //If the player shoots a health model
                            if (CherrySphere.Intersects(laserSphere))
                            {
                                //Plays the health sound effect
                                cherrySound.Play();
                                //Removes the laser and health models that have collided together
                                cherryList[j].isActive = false;
                                laser.isActive = false;
                                //Increments the hitcount
                                hitCount++;
                                //Sets lives back to 5 and adds 150 to the score
                                lives= 5;
                                score = score + 150;                                
                                break; //no need to check other bullets
                            }
                        }

                        if (CherrySphere.Intersects(TardisSphere)) //Check collision between Dalek and Tardis
                        {
                            //Plays the health sound effect 
                            cherrySound.Play();
                            //Removes the health model from the screen
                            cherryList[j].isActive = false;
                            enemyList[j].direction *= -1.0f;
                            //Sets lives back to 5 and adds 150 to the score
                            lives= 5;
                            score = score + 150;                   
                            break; //no need to check other bullets
                        }

                    
                        

                   
                        }
                
            }
           

            base.Update(gameTime);
            timer=timeDelta++;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //If the current GameState is the start screen load the start screen
            if (currentGameState == GameState.start)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(space, Vector2.Zero, Color.White);
                spriteBatch.End();            
            }

            GraphicsDevice.Clear(Color.CornflowerBlue);

            //If the players lives are greater than 0 draw the game
            if (lives > 0)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(start, mPosition, Color.White);                              
                spriteBatch.End();
                

                // Set the array list i to 0 then check if it's smaller than NumLasers defined in the game constants and increment it by 1
                for (int i = 0; i < GameConstants.NumLasers; i++)
                {
                    //If the laser is active draw it on the screen
                    if (laser.isActive)
                    {
                        Matrix laserTransform = Matrix.CreateScale(GameConstants.LaserScalar) * Matrix.CreateTranslation(laser.position);
                        DrawModel(mdlRocket, laserTransform, mdlRocketTransforms);
                    }
                }


                Matrix modelTransform = Matrix.CreateRotationY(mdlRotation) * Matrix.CreateTranslation(mdlPosition);
                DrawModel(mdlPlayer, modelTransform, mdlPlayerTransforms);

                //Display text on the screen. This gives the text to be shown, its position and the colour
                writeText("Space Invaders", new Vector2(50, 10), Color.Yellow);
                writeText("Score: " + score, new Vector2(50, 50), Color.Yellow);
                writeText("Lives: " + lives, new Vector2(50, 90), Color.Yellow);

                // Set the array list i to 0 then check if it's smaller than NumDaleks defined in the game constants and increment it by 1
                for (int i = 0; i < GameConstants.NumDaleks; i++)
                {
                    if (enemyList[i].isActive)
                    {
                        Matrix dalekTransform = Matrix.CreateScale(GameConstants.DalekScalar) * Matrix.CreateTranslation(enemyList[i].position);
                        DrawModel(mdlEnemy, dalekTransform, mdlEnemyTransforms);
                    }
                }

                // Set the array list j to 0 then check if it's smaller than NumLives defined in the game constants and increment it by 1
                for (int j = 0; j < GameConstants.NumLives; j++)
                {
                    if (cherryList[j].isActive)
                    {
                        Matrix lifeTransform = Matrix.CreateScale(GameConstants.lifeScalar) * Matrix.CreateTranslation(cherryList[j].position);
                        DrawModel(mdlCherry, lifeTransform, mdResetLiveTransforms);
                    }
                }

            }

            //If the players lives are less than 0 load the end screen and display the final score 
            if (lives<0)
            {
                //Begin the spriteBatch
                spriteBatch.Begin();
                //Draw the end screen
                spriteBatch.Draw(gameOver,Vector2.Zero,Color.White);
                //Display the final score in the bottom left of the screen in yellow writing
                spriteBatch.DrawString(fontToUse, "" + score, new Vector2(280, 370), Color.Yellow);
                //End the spriteBatch
                spriteBatch.End();

                //Turn of the background music and sound effects
                MediaPlayer.Stop();
                SoundEffect.MasterVolume = 0.0f;

                
            }
            

            //writeText(songInfo, new Vector2(50, 125), Color.AntiqueWhite);
            
            base.Draw(gameTime);
        }
    }
}
