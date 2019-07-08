using battlepong_game.Models;
using battlepong_game.Utilities;
using NAudio.Wave;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Input;

namespace battlepong_game {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
            Loaded += new RoutedEventHandler(delegate (object sender, RoutedEventArgs args) {
                //Load directly to the center
                Top = (SystemParameters.VirtualScreenHeight / 2) - (Height / 2);
                Left = (SystemParameters.VirtualScreenWidth / 2) - (Width / 2);
            });

            //Load Sounds
            startSound.Load();
            paddleHitSound.Load();
            wallHitSound.Load();
            scoredSound.Load();
            pauseSound.Load();
        }

        private void OnMouseMove(object sender, MouseEventArgs e) {
            var position = e.GetPosition(this);
            mousePos.x = (float)position.X - (float)Width / 2.0f;
            mousePos.y = -((float)position.Y - (float)Height / 2.0f);
            //Console.WriteLine((mousePos.x) + " " + (mousePos.y));
        }

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args) {
            OpenGL gl = args.OpenGL;
            //Set Background Color
            //gl.ClearColor(0.7f, 0.7f, 0.9f, 0.0f);

            gl.Enable(OpenGL.GL_DEPTH_TEST);
            float[] global_ambient = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
            float[] light0pos = new float[] { 1.0f, 1.0f, 1.0f, 0.0f };
            float[] light0ambient = new float[] { 0.0f, 0.0f, 0.0f, 1.0f };
            float[] light0diffuse = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] light0specular = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] lmodel_ambient = new float[] { 1.2f, 1.2f, 1.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, lmodel_ambient);
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);

            gl.ColorMaterial(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);

            gl.ShadeModel(OpenGL.GL_SMOOTH);
            gl.Enable(OpenGL.GL_LINE_SMOOTH);

            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            gl.Enable(OpenGL.GL_BLEND);
        }

        //The common z coordinate for most meshes
        public static float commonZ = 50.0f;

        #region Movable Meshes
        public Mesh player1Paddle = new Mesh() {
            Position = new Vector3(-50, 0, commonZ),
            Scale = new Vector3(1.0f, 6, 1),
            Mass = 1.0f,
            Color = new Vector4(1.0f, 0.0f, 0.0f)
        };
        public Mesh player2Paddle = new Mesh() {
            Position = new Vector3(50, 0, commonZ),
            Scale = new Vector3(1.0f, 6, 1),
            Mass = 1.0f,
            Color = new Vector4(0.0f, 0.0f, 1.0f)
        };
        public Mesh ball = new Mesh() {
            Position = new Vector3(0, 0, commonZ),
            Scale = new Vector3(1, 1, 1),
            Color = new Vector4(1.0f, 1.0f, 1.0f, 0.0f)
        };
        #endregion

        #region Static Meshes
        public Mesh Triangle = new Mesh() {
            //Position = new Vector3(0, 0, commonZ),
            Scale = new Vector3(1, 1, 1),
            Color = new Vector4(0.5f, 1.0f, 0.5f)
        };
        public Mesh UpperBoundary = new Mesh() {
            Position = new Vector3(0, 30, commonZ),
            Scale = new Vector3(70, 0.5f, 1),
            Color = new Vector4(1.0f, 1.0f, 1.0f)
        };
        public Mesh LowerBoundary = new Mesh() {
            Position = new Vector3(0, -30, commonZ),
            Scale = new Vector3(70, 0.5f, 1),
            Color = new Vector4(1.0f, 1.0f, 1.0f)
        };
        public Mesh Player1Line = new Mesh() {
            Color = new Vector4(0.5f, 0, 0)
        };
        public Mesh Player2Line = new Mesh() {
            Color = new Vector4(0, 0, 0.5f)
        };
        public Mesh CenterLine = new Mesh() {
            Color = new Vector4(0.5f, 0.5f, 0.5f)
        };
        public Mesh StartMenu = new Mesh() {
            Position = new Vector3(0, 0, commonZ + 10.0f),
            Scale = new Vector3(40, 20, 0),
            Color = new Vector4(0.1f, 0.1f, 0.1f, 0.9f)
        };
        public Mesh OptionMenu = new Mesh() {
            Position = new Vector3(0, 0, commonZ + 10.0f),
            Scale = new Vector3(40, 20, 0),
            Color = new Vector4(0.2f, 0.2f, 0.2f, 0.7f)
        };

        #endregion

        #region Variables
        //System Related
        public Vector3 mouseVector = new Vector3();
        public Vector3 mousePos = new Vector3();

        //Controls
        public Key playerOneUp = Key.W;
        public Key playerOneDown = Key.S;
        public Key playerTwoUp = Key.Up;
        public Key playerTwoDown = Key.Down;
        public Key playerOneQuitKey = Key.Escape;
        public Key playerTwoQuitKey = Key.End;
        public Key pauseKey = Key.P; //opens option menu
        public Key testKey = Key.T;
        public Key startKey = Key.Enter;
        public Key player1Enable = Key.LeftShift;
        public Key player2Enable = Key.RightShift;

        //Stats
        public float player1MaxSpeed = 3.0f;
        public Vector3 player1Accel = new Vector3(0, 0.5f, 0);
        public float player2MaxSpeed = 3.0f;
        public Vector3 player2Accel = new Vector3(0, 0.5f, 0);
        public float paddleFriction = 0.5f;

        //GameSettings
        //private float maxHorizontalBorder = 50.0f;
        public float maxVerticalBorder = 30.0f;
        public bool isSoundEnabled = true;

        //Sound Files 
        public SoundPlayer startSound = new SoundPlayer(Properties.Resources.Power);
        public SoundPlayer paddleHitSound = new SoundPlayer(Properties.Resources.Mouse);
        public SoundPlayer wallHitSound = new SoundPlayer(Properties.Resources.Corked);
        public SoundPlayer scoredSound = new SoundPlayer(Properties.Resources.Nursery);
        public SoundPlayer pauseSound = new SoundPlayer(Properties.Resources.Charmer);
        public IWavePlayer waveOutDevice = new WaveOut();
        public Mp3FileReader odysseyMus = new Mp3FileReader("Resources/odyssey-by-kevin-macleod.mp3");

        //Game Related Variables
        public bool isStartMusPlayed = false;
        public bool isGameStarted = false;
        public bool isBallPlayed = false;
        public bool isScoredAlready = false;
        public bool isOptionMenuOpen = false;
        public bool isTestToggled = false;
        public bool isResetOngoing = false;
        public bool isRestarting = false;
        public float angleRandom;
        public int resetCounter = 30;
        public int player1Score = 0;
        public int player2Score = 0;
        public float ballTime = 0;
        public float scoringLine = 10.0f;
        public bool didPlayer1Scored = false;
        public int framesToCount = 120;
        public float maxBallVelocity = 3.0f;
        public float angleDeviation = 5.0f;
        public Vector4 lighterColor = new Vector4(0.1f, 0.1f, 0.1f);
        public List<Mesh> ballTrails = new List<Mesh>();
        public List<Mesh> ballExplosions = new List<Mesh>();

        //AI CODE
        public bool isPlayer1AI = false;
        public bool isPlayer2AI = true;
        public bool willReturnToCenter = true;
        //private bool willPredictTrajectory = false;
        public int visionDistance = 50;

        //String Variables
        public float cons = 0.375f; //Monospace Constant to be able to get the length of string
        public string[] menuText = { "C# PONG", "Enter to Start", "Hold Right Shift to toggle Player 2", "Player 2 Enabled", "Player 2 Disabled" };
        public float[] menuSize = { 50.0f, 30.0f, 20.0f, 15.0f, 15.0f };
        public string optionTitle = "Options";
        public string[] optionText = { "Max Ball Speed", "Test2", "Test3", "Test4", "Test5", "Restart Game" };
        public float[] optionSize = { 20.0f, 20.0f, 20.0f, 20.0f, 20.0f, 20.0f };
        public int indexSelected = 1;

        //FPS
        public long lastFrame = GameUtils.NanoTime();
        public float FPS = 0;
        public float avgFPS = 0;
        public float lowFPS = 900000;
        public float highFPS = 0;
        #endregion

        public void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args) {
            Title = menuText[0];

            OpenGL gl = args.OpenGL;
            gl.Viewport(0, 0, (int)Width, (int)Height);

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.LoadIdentity();
            gl.Translate(0.0f, .0f, -150.0f);
            gl.LookAt(0, 2.0f, -1.0f, 0, 2.0f, -150.0f, 0, 1, 0);


            UpperBoundary.DrawSquare(gl);
            LowerBoundary.DrawSquare(gl);
            CenterLine.DrawDottedLine(gl, new Vector3(0, LowerBoundary.Position.y, commonZ), new Vector3(0, UpperBoundary.Position.y, commonZ));
            Player1Line.DrawDottedLine(gl, new Vector3(player1Paddle.Position.x, LowerBoundary.Position.y, commonZ), new Vector3(player1Paddle.Position.x, UpperBoundary.Position.y, commonZ));
            Player2Line.DrawDottedLine(gl, new Vector3(player2Paddle.Position.x, LowerBoundary.Position.y, commonZ), new Vector3(player2Paddle.Position.x, UpperBoundary.Position.y, commonZ));

            player1Paddle.DrawSquare(gl);
            player2Paddle.DrawSquare(gl);

            ball.DrawSquare(gl);

            //Start main music
            if (!isStartMusPlayed) {
                waveOutDevice.Init(odysseyMus);
                waveOutDevice.Play();
                isStartMusPlayed = true;
            }
            else {
                //waveOutDevice.Stop();
                //audioFileReader.Dispose();
                //waveOutDevice.Dispose();
            }

            //Start 
            #region Main
            if (isGameStarted && !isOptionMenuOpen) {
                if (!isBallPlayed) {
                    ball.Color = new Vector4(1.0f, 1.0f, 1.0f);
                    ball.Position = new Vector3(0, 0, commonZ);

                    if (didPlayer1Scored) {
                        //1f Player 1 scored then ball is targeted to player2, vice-versa
                        ball.ApplyForce(new Vector3(1.4f, 1.4f, 0));
                    }
                    else {
                        ball.ApplyForce(new Vector3(-1.4f, 1.4f, 0));
                    }


                    player1Paddle.Position.y *= 0;
                    player2Paddle.Position.y *= 0;
                    isBallPlayed = true;
                    isScoredAlready = false;
                    resetCounter = 30;
                }
                else {
                    ballTime++;
                    //Speed up every 120 frames but stop when velocity of ball is 3.
                    if (((ballTime % framesToCount) == 0) && (ball.Velocity.GetLength() < maxBallVelocity)) {
                        ball.IncreaseSpeed(0.1f);
                    }
                    angleRandom = (float)Randomizer.Generate(-angleDeviation, angleDeviation);
                }
            }
            #endregion

            #region Ball Aesthetics
            //Draw ball trails
            if (isBallPlayed) {
                Mesh ballTrail = new Mesh() {
                    Position = ball.Position - ball.Velocity - new Vector3(0,0,0.5f),
                    Scale = new Vector3(0.8f, 0.8f, 0),
                };
                ballTrails.Add(ballTrail);
                foreach (var trail in ballTrails) {
                    trail.Color = ball.Color - new Vector4(0.2f, 0.2f, 0.2f, 0.0f);
                    //Reduce size
                    if (trail.Scale.x > 0) {
                        trail.DrawSquare(gl);
                        if (!isOptionMenuOpen) {
                            trail.Scale = trail.Scale - 0.06f;
                        }
                    }
                }

                //A shockwave when ball hit the boundaries
                Mesh ballExplosion = new Mesh() {
                    Position = ball.Position - ball.Velocity,
                    Radius = 1.0f,
                    Color = new Vector4(0.8f, 0.8f, 0.8f)
                };
                //Ball hitting top or bottom
                if ((ball.Position.y + ball.Scale.x > maxVerticalBorder - UpperBoundary.Scale.y) ||
                    (ball.Position.y - ball.Scale.x < maxVerticalBorder * -1 + UpperBoundary.Scale.y)) {
                    ballExplosions.Add(ballExplosion);
                }
                
                foreach (var explosions in ballExplosions) {              
                    //Reduce size and opacity
                    if (explosions.Radius < 6.0f) {
                        explosions.DrawCircle(gl);
                        if (!isOptionMenuOpen) {
                            explosions.Radius += (6.0f - explosions.Radius) / 2 ;
                            explosions.Color.a -= 0.2f;
                        }
                    }
                }
                
            }
            #endregion

            #region Reset Code
            if (isResetOngoing) {
                resetCounter--;
                if (resetCounter <= 0) {
                    ball.Velocity *= 0;
                    isBallPlayed = false;
                    isResetOngoing = false;
                }
            }
            #endregion

            #region Restart Code
            if (isRestarting) {

            }
            #endregion

            #region Controls, Movement and AI
            //Before Game starts controls
            if (!isGameStarted) {
                //Start
                if (Keyboard.IsKeyDown(startKey)) {
                    startSound.Play();
                    isGameStarted = true;
                }
                //Toggle Player''s AI. Only Player 2 starts as AI
                //If Key is down make it false else true;
                isPlayer1AI = (Keyboard.IsKeyDown(player1Enable)) ? true : false;
                isPlayer2AI = (Keyboard.IsKeyDown(player2Enable)) ? false : true;
            }
            else {
                //Option Menu. Open the Menu, Stop ball from moving, reduce music volume
                isOptionMenuOpen = Keyboard.IsKeyToggled(pauseKey) ? true : false;
                ball.enabledUpdate = Keyboard.IsKeyToggled(pauseKey) ? false : true;
                waveOutDevice.Volume = Keyboard.IsKeyToggled(pauseKey) ? 0.5f : 1.0f;
            }

            //Quit Game
            if (Keyboard.IsKeyDown(playerOneQuitKey) || Keyboard.IsKeyDown(playerTwoQuitKey)) {
                Environment.Exit(0);
            }

            //Test Toggle
            isTestToggled = (Keyboard.IsKeyToggled(testKey)) ? true : false;

            //Player 1 Controls           
            if (!isPlayer1AI && !isOptionMenuOpen && isGameStarted) {
                if (Keyboard.IsKeyDown(playerOneUp) &&
                    (player1Paddle.Position.y + player1Paddle.Scale.y <= UpperBoundary.Position.y - (UpperBoundary.Scale.y))) {
                    if (player1Paddle.Velocity.y < player1MaxSpeed) {
                        player1Paddle.ApplyForce(player1Accel);
                    }
                    else {
                        player1Paddle.Velocity.y = player1MaxSpeed;
                    }
                }
                else if (Keyboard.IsKeyDown(playerOneDown) &&
                    (player1Paddle.Position.y - player1Paddle.Scale.y >= LowerBoundary.Position.y + (LowerBoundary.Scale.y))) {
                    if (player1Paddle.Velocity.y > -player1MaxSpeed) {
                        player1Paddle.ApplyForce(player1Accel * -1);
                    }
                    else {
                        player1Paddle.Velocity.y = -player1MaxSpeed;
                    }
                }
                else {
                    //Slow down the Paddle to zero
                    player1Paddle.ApplyFriction(paddleFriction);
                }
            }
            else {
                //AI Code
                //If ball is within Vision distance from the
                if (ball.Position.x < player1Paddle.Position.x + visionDistance) {
                    //Above paddle
                    if ((ball.Position.y > player1Paddle.Position.y) &&
                    (player1Paddle.Position.y + player1Paddle.Scale.y <= UpperBoundary.Position.y - (UpperBoundary.Scale.y)) &&
                    !isOptionMenuOpen) {
                        if (player1Paddle.Velocity.y < player1MaxSpeed) {
                            player1Paddle.ApplyForce(player1Accel);
                        }
                        else {
                            player1Paddle.Velocity.y = player1MaxSpeed;
                        }
                    }
                    //Below paddle
                    else if ((ball.Position.y < player1Paddle.Position.y) &&
                        (player1Paddle.Position.y - player1Paddle.Scale.y >= LowerBoundary.Position.y + (LowerBoundary.Scale.y)) &&
                        !isOptionMenuOpen) {
                        if (player1Paddle.Velocity.y > -player1MaxSpeed) {
                            player1Paddle.ApplyForce(player1Accel * -1);
                        }
                        else {
                            player1Paddle.Velocity.y = -player1MaxSpeed;
                        }
                    }
                    else {
                        player1Paddle.ApplyFriction(paddleFriction);
                    }
                }
                else {
                    //AI will return Paddle to center if enabled
                    if (willReturnToCenter) {
                        if ((player1Paddle.Position.y > -2) && (player1Paddle.Position.y < 2)) {
                            player2Paddle.ApplyFriction(0.5f);
                        }
                        else if (player1Paddle.Position.y > 0) {
                            player1Paddle.ApplyForce(player1Accel * -1);
                        }
                        else {
                            player1Paddle.ApplyForce(player1Accel);
                        }
                    }
                }
            }

            //Check if AI is not enabled, if so allow Player 2 controls
            if (!isPlayer2AI) {
                if (Keyboard.IsKeyDown(playerTwoUp) &&
                    (player2Paddle.Position.y + player2Paddle.Scale.y <= UpperBoundary.Position.y - (UpperBoundary.Scale.y)) &&
                    !isOptionMenuOpen && isGameStarted) {
                    if (player2Paddle.Velocity.y < player2MaxSpeed) {
                        player2Paddle.ApplyForce(player2Accel);
                    }
                    else {
                        player2Paddle.Velocity.y = player2MaxSpeed;
                    }                  
                }
                else if (Keyboard.IsKeyDown(playerTwoDown) &&
                    (player2Paddle.Position.y - player2Paddle.Scale.y >= LowerBoundary.Position.y + (LowerBoundary.Scale.y)) &&
                    !isOptionMenuOpen && isGameStarted) {              
                    if (player2Paddle.Velocity.y < player2MaxSpeed) {
                        player2Paddle.ApplyForce(player2Accel * -1);
                    }
                    else {
                        player2Paddle.Velocity.y = -player2MaxSpeed;
                    }
                }
                else {
                    player2Paddle.ApplyFriction(paddleFriction);
                }
            }
            else {
                //AI Code
                //If ball is within Vision distance
                if (ball.Position.x > player2Paddle.Position.x - visionDistance) {
                    //Above paddle
                    if ((ball.Position.y > player2Paddle.Position.y) &&
                    (player2Paddle.Position.y + player2Paddle.Scale.y <= UpperBoundary.Position.y - (UpperBoundary.Scale.y)) &&
                    !isOptionMenuOpen) {
                        if (player2Paddle.Velocity.y < player2MaxSpeed) {
                            player2Paddle.ApplyForce(player2Accel);
                        }
                        else {
                            player2Paddle.Velocity.y = player2MaxSpeed;
                        }
                    }
                    //Below paddle
                    else if ((ball.Position.y < player2Paddle.Position.y) &&
                        (player2Paddle.Position.y - player2Paddle.Scale.y >= LowerBoundary.Position.y + (LowerBoundary.Scale.y)) &&
                        !isOptionMenuOpen) {
                        if (player2Paddle.Velocity.y > -player2MaxSpeed) {
                            player2Paddle.ApplyForce(player2Accel * -1);
                        }
                        else {
                            player2Paddle.Velocity.y = -player2MaxSpeed;
                        }
                    }
                    else {
                        player2Paddle.ApplyFriction(paddleFriction);
                    }
                }
                else {
                    //AI will return Paddle to center if enabled
                    if (willReturnToCenter) {
                        if ((player2Paddle.Position.y > -2) && (player2Paddle.Position.y <  2)) {
                            player2Paddle.ApplyFriction(paddleFriction);
                        }
                        else if (player2Paddle.Position.y > 0) {
                            player2Paddle.ApplyForce(player2Accel * -1/2);
                        }
                        else {
                            player2Paddle.ApplyForce(player2Accel / 2);
                        }
                    }
                }
            }
            #endregion

            #region Scoring
            //Reached behind player 1
            if ((ball.Position.x < player1Paddle.Position.x - scoringLine) &&
                (resetCounter > 0)) {
                //Start reset counter
                isResetOngoing = true;
                //Score once
                if (!isScoredAlready) {
                    scoredSound.Play();
                    player2Score++;
                    isScoredAlready = true;
                    didPlayer1Scored = false;
                }
            }
            //Reached behind player 2
            if ((ball.Position.x > player2Paddle.Position.x + scoringLine) &&
                (resetCounter > 0)) {
                isResetOngoing = true;
                if (!isScoredAlready) {
                    scoredSound.Play();
                    player1Score++;
                    isScoredAlready = true;
                    didPlayer1Scored = true;
                }
            }
            #endregion

            #region Text and Menus
            //Game Screen
            gl.DrawText((int)Width / 2 - 65, (int)Height / 2 - 340, 1.0f, 0.3f, 0.3f, "Courier", 60, "" + player1Score);
            gl.DrawText((int)Width / 2, (int)Height / 2 - 340, 0.3f, 0.3f, 1.0f, "Courier", 60, "" + player2Score);
            //gl.DrawText(50, (int)Height / 2 + 230, 1.0f, 0.3f, 0.3f, "Courier", 20, "Player 1");

            if (isPlayer1AI) {
                gl.DrawText(50, (int)Height / 2 + 230, 1.0f, 0.3f, 0.3f, "Courier", 20, "AI");
            }
            else {
                gl.DrawText(50, (int)Height / 2 + 230, 1.0f, 0.3f, 0.3f, "Courier", 20, "Player 1");
            }

            if (isPlayer2AI) {
                gl.DrawText((int)Width - 100, (int)Height / 2 + 230, 0.3f, 0.3f, 1.0f, "Courier", 20, "AI");
            }
            else {
                gl.DrawText((int)Width - 200, (int)Height / 2 + 230, 0.3f, 0.3f, 1.0f, "Courier", 20, "Player 2");
            }

            //Start Screen
            if (!isGameStarted) {
                gl.LoadIdentity();
                gl.Translate(0.0f, .0f, -150.0f);
                StartMenu.DrawSquare(gl);
                gl.DrawText((int)Width / 2 - ((int)(menuSize[0] * cons) * menuText[0].Length), (int)Height / 2 + 60, 0.5f, 1.0f, 0.5f, "Courier New", menuSize[0], menuText[0]);
                gl.DrawText((int)Width / 2 - ((int)(menuSize[1] * cons) * menuText[1].Length), (int)Height / 2 - 30, 1.0f, 1.0f, 1.0f, "Courier New", menuSize[1], menuText[1]);
                gl.DrawText((int)Width / 2 - ((int)(menuSize[2] * cons) * menuText[2].Length), (int)Height / 2 - 140, 1.0f, 1.0f, 1.0f, "Courier New", menuSize[2], menuText[2]);
                if (!isPlayer2AI) {
                    //Player 2 Enabled
                    gl.DrawText((int)Width / 2 - ((int)(menuSize[3] * cons) * menuText[3].Length), (int)Height / 2 - 160, 0.3f, 0.3f, 1.0f, "Courier New", menuSize[3], menuText[3]);
                }
                else {
                    gl.DrawText((int)Width / 2 - ((int)(menuSize[4] * cons) * menuText[4].Length), (int)Height / 2 - 160, 1.0f, 0.3f, 0.3f, "Courier New", menuSize[4], menuText[4]);
                }
            }

            //Option Menu
            if (isOptionMenuOpen) {
                gl.LoadIdentity();
                gl.Translate(0.0f, .0f, -150.0f);
                OptionMenu.DrawSquare(gl);
                Triangle.DrawTriangle(gl);
                //Triangle.Position = new Vector3(-25.0f, 0, commonZ + 12.0f);
                gl.DrawText((int)Width / 2 - ((int)(40.0f * cons) * optionTitle.Length), (int)Height / 2 + 90, 1.0f, 1.0f, 1.0f, "Courier New", 40.0f, optionTitle);

                for (int i = 0; i < optionText.GetLength(0); i++) {
                    gl.DrawText((int)Width / 2 - 200, (int)Height / 2 + 30 - (30 * i), 1.0f, 1.0f, 1.0f, "Courier New", optionSize[i], "" + optionText[i]);
                }
                //Moving down the menu cursor
                if (Keyboard.IsKeyDown(playerOneDown)) {                  
                    indexSelected = (indexSelected < optionText.GetLength(0)) ? indexSelected + 1 : 1;
                }
                //Moving up the menu cursor
                if (Keyboard.IsKeyDown(playerOneUp)) {
                    indexSelected = (indexSelected > 1) ? indexSelected - 1 : optionText.GetLength(0);
                }

                Triangle.Position = new Vector3(-25.0f, 9 - (3.3f * indexSelected), commonZ + 12.0f);
            }

            //Debug
            if (isTestToggled) {
                    LogFrame();
                    gl.DrawText(10, (int)Height - 90, 1.0f, 0.0f, 0, "Calibri", 10, "FPS: " + Math.Truncate(FPS) + " | Low: " + Math.Truncate(lowFPS) + " | High: " + Math.Truncate(highFPS) + " | Avg: " + Math.Truncate(avgFPS));
                    gl.DrawText(10, 50, 1.0f, 1.0f, 0, "Calibri", 10, "Angle Randomizer: " + angleRandom);
                    gl.DrawText(10, 40, 1.0f, 1.0f, 0, "Calibri", 10, "Ball Position: " + ball.Position.x + ", " + ball.Position.y);
                    gl.DrawText(10, 30, 1.0f, 1.0f, 0, "Calibri", 10, "Ball Vector Direction: " + ball.Velocity.x + ", " + ball.Velocity.y);
                    gl.DrawText(10, 20, 1.0f, 0.0f, 0, "Calibri", 10, "Reset in: " + resetCounter);
                    gl.DrawText(10, 10, 1.0f, 0.0f, 0, "Calibri", 10, "Speed: " + ball.Velocity.GetLength());
            }

            #endregion

            #region Paddle Wall Collision and Ball Angle
            //Allowable angle is (0 - 89) && (269 - 360) || (91 - 269)
            //Max Deviation from center to eachtip is 60 degrees
            //Ball hitting the exact center is reflected back straight plus random

            //Paddle 1 reached top then bottom
            if (player1Paddle.TopCollision() > UpperBoundary.BottomCollision()) {
                player1Paddle.Position.y = UpperBoundary.BottomCollision() - player1Paddle.Scale.y;
            } else if (player1Paddle.BottomCollision() < LowerBoundary.TopCollision()) {
                player1Paddle.Position.y = LowerBoundary.TopCollision() + player1Paddle.Scale.y;
            }

            //Paddle 2 reached top then bottom
            if (player2Paddle.TopCollision() > UpperBoundary.BottomCollision()) {
                player2Paddle.Position.y = UpperBoundary.BottomCollision() - player2Paddle.Scale.y;
            } else if (player2Paddle.BottomCollision() < LowerBoundary.TopCollision()) {
                player2Paddle.Position.y = LowerBoundary.TopCollision() + player2Paddle.Scale.y;
            }

            //Ball collided with top or bottom plus safety checks
            if (ball.TopCollision() > UpperBoundary.BottomCollision() || ball.BottomCollision() < LowerBoundary.TopCollision()) { 
                //Play Wall hit sound
                wallHitSound.Play();
                //Check if collision is with the upper boundary or the one below 
                ball.Position.y = ball.TopCollision() > UpperBoundary.BottomCollision() ? ball.Position.y - 1.0f : ball.Position.y + 1.0f;
                //Change ball direction
                ball.Velocity.y = -ball.Velocity.y;

            }
            //Ball collision for Player 1
            if (ball.HasCollidedWith(player1Paddle)) { 
                if (isSoundEnabled) {
                    paddleHitSound.Play();
                }                
                if (ball.Position.y == player1Paddle.Position.y) {
                    ball.ChangeAngle(0 + angleRandom);
                }              
                else if (ball.Position.y > player1Paddle.Position.y) {
                    
                    ball.ChangeAngle(10 * (ball.Position.y - player1Paddle.Position.y) + angleRandom);
                }
                else {
                    ball.ChangeAngle(360 + (10 * (ball.Position.y - player1Paddle.Position.y)) + angleRandom);
                }
                ball.Color = player1Paddle.Color + lighterColor;
            }

            //Ball Collision for Player 2 Paddle
            if (ball.HasCollidedWith(player2Paddle)) {
                if (isSoundEnabled) {
                    paddleHitSound.Play();
                }
                if (ball.Position.y == player2Paddle.Position.y) {
                    ball.ChangeAngle(180 + angleRandom);
                }
                else if (ball.Position.y > player2Paddle.Position.y) {
                    ball.ChangeAngle(180 - (10 * (ball.Position.y - player2Paddle.Position.y)) + angleRandom);
                }
                else {
                    ball.ChangeAngle(180 - (10 * (ball.Position.y - player2Paddle.Position.y)) + angleRandom);
                }
                ball.Color = player2Paddle.Color + lighterColor;
            }
            #endregion 
        }
        //End

        
        public void LogFrame() {
            long time = (GameUtils.NanoTime() - lastFrame);
            FPS = 1 / (time / 1000000000.0f);
            lastFrame = GameUtils.NanoTime();

            if (isGameStarted) {
                if (FPS > highFPS) {
                    highFPS = FPS;
                }
                if ((FPS < lowFPS) && isGameStarted) {
                    lowFPS = FPS;
                }
                avgFPS = (highFPS + lowFPS) / 2;
                if (ballTime % 120 == 0) {
                    highFPS = 0;
                    lowFPS = 900000;
                }
            }
        }
        

    }
}
