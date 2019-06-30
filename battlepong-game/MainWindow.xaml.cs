using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Media;
using System.Diagnostics;
using battlepong_game.Models;
using battlepong_game.Utilities;
using SharpGL;
using NAudio;
using NAudio.Wave;

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
        private Mesh player1Paddle = new Mesh() {
            Position = new Vector3(-50, 0, commonZ),
            Scale = new Vector3(1.0f, 6, 1),
            Mass = 1.0f,
            Color = new Vector4(1.0f, 0.0f, 0.0f)
        };

        private Mesh player2Paddle = new Mesh() {
            Position = new Vector3(50, 0, commonZ),
            Scale = new Vector3(1.0f, 6, 1),
            Mass = 1.0f,
            Color = new Vector4(0.0f, 0.0f, 1.0f)
        };

        private Mesh ball = new Mesh() {
            Position = new Vector3(0, 0, commonZ),
            Scale = new Vector3(1, 1, 1),
            Color = new Vector4(1.0f, 1.0f, 1.0f)
        };
        #endregion

        #region Static Meshes
        private Mesh UpperBoundary = new Mesh() {
            Position = new Vector3(0, 30, commonZ),
            Scale = new Vector3(70, 0.5f, 1),
            Color = new Vector4(1.0f, 1.0f, 1.0f)
        };
        private Mesh LowerBoundary = new Mesh() {
            Position = new Vector3(0, -30, commonZ),
            Scale = new Vector3(70, 0.5f, 1),
            Color = new Vector4(1.0f, 1.0f, 1.0f)
        };
        private Mesh Player1Line = new Mesh() {
            Color = new Vector4(0.5f, 0, 0)
        };
        private Mesh Player2Line = new Mesh() {
            Color = new Vector4(0, 0, 0.5f)
        };
        private Mesh CenterLine = new Mesh() {
            Color = new Vector4(0.5f, 0.5f, 0.5f)
        };
        private Mesh StartMenu = new Mesh() {
            Position = new Vector3(0, 0, commonZ + 10.0f),
            Scale = new Vector3(40, 20, 0),
            Color = new Vector4(0.1f, 0.1f, 0.1f, 0.9f)
        };
        private Mesh OptionMenu = new Mesh() {
            Position = new Vector3(0, 0, commonZ + 10.0f),
            Scale = new Vector3(40, 20, 0),
            Color = new Vector4(0.2f, 0.2f, 0.2f, 0.7f)
        };

        #endregion

        #region Variables
        //System Related
        private Vector3 mouseVector = new Vector3();
        private Vector3 mousePos = new Vector3();

        //Controls
        private Key playerOneUp = Key.W;
        private Key playerOneDown = Key.S;
        private Key playerTwoUp = Key.Up;
        private Key playerTwoDown = Key.Down;
        private Key playerOneQuitKey = Key.Escape;
        private Key playerTwoQuitKey = Key.End;
        private Key pauseKey = Key.P; //opens option menu
        private Key testKey = Key.T;
        private Key startKey = Key.Enter;
        private Key player2Enable = Key.RightShift;

        //Stats
        private float player1MaxSpeed = 4.0f;
        private Vector3 player1Accel = new Vector3(0, 1.0f, 0);
        private float player2MaxSpeed = 4.0f;
        private Vector3 player2Accel = new Vector3(0, 1.0f, 0);

        //GameSettings
        //private float maxHorizontalBorder = 50.0f;
        private float maxVerticalBorder = 30.0f;
        private bool isSoundEnabled = true;

        //Sound Files 
        SoundPlayer startSound = new SoundPlayer(Properties.Resources.Power);
        SoundPlayer paddleHitSound = new SoundPlayer(Properties.Resources.Mouse);
        SoundPlayer wallHitSound = new SoundPlayer(Properties.Resources.Corked);
        SoundPlayer scoredSound = new SoundPlayer(Properties.Resources.Nursery);
        SoundPlayer pauseSound = new SoundPlayer(Properties.Resources.Charmer);
        IWavePlayer waveOutDevice = new WaveOut();   
        Mp3FileReader odysseyMus = new Mp3FileReader("Resources/odyssey-by-kevin-macleod.mp3");

        //Game Related Variables
        private bool isStartMusPlayed = false;
        private bool isGameStarted = false;
        private bool isBallPlayed = false;
        private bool isScoredAlready = false;
        private bool isOptionMenuOpen = false;
        private bool isTestToggled = false;
        private bool isResetOngoing = false;
        private float angleRandom;
        private int resetCounter = 30;
        private int player1Score = 0;
        private int player2Score = 0;
        private float ballTime = 0;
        private float scoringArea = 10.0f;
        private bool didPlayer1Scored = false;
        private List<Mesh> ballTrails = new List<Mesh>();

        //AI CODE
        private bool isPlayer2AI = true;
        private bool willReturnToCenter = true;
        //private bool willPredictTrajectory = false;
        private int visionDistance = 55;

        //String Variables
        private float cons = 0.375f; //Monospace Constant to be able to get the length of string
        private string[] menuText = {"C# BATTLE PONG", "Enter to Start", "Right Shift to toggle Player 2", "Player 2 Enabled", "Player 2 Disabled" };
        private float[] menuSize = { 50.0f, 30.0f, 20.0f, 15.0f, 15.0f };
        private string[] optionText = { "Options", "Test1", "Test2", "Test3", "Test4", "Test5", "Test6" };
        private float[] optionSize = { 40.0f, 20.0f, 20.0f, 20.0f, 20.0f, 20.0f, 20.0f };

        //FPS
        private long lastFrame = GameUtils.NanoTime();
        public float FPS = 0;
        public float avgFPS = 0;
        public float lowFPS = 900000;
        public float highFPS = 0;
        #endregion

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args) {
            Title = "C# Battle Pong";

            OpenGL gl = args.OpenGL;
            gl.Viewport(0, 0, (int)Width, (int)Height);

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.LoadIdentity();
            gl.Translate(0.0f, .0f, -150.0f);
            gl.LookAt(0, 2.0f, 0, 0, 2.0f, -150.0f, 0, 1, 0);

            UpperBoundary.DrawCube(gl);
            LowerBoundary.DrawCube(gl);
            CenterLine.DrawDottedLine(gl, new Vector3(0, LowerBoundary.Position.y, commonZ), new Vector3(0, UpperBoundary.Position.y, commonZ));
            Player1Line.DrawDottedLine(gl, new Vector3(player1Paddle.Position.x, LowerBoundary.Position.y, commonZ), new Vector3(player1Paddle.Position.x, UpperBoundary.Position.y, commonZ));
            Player2Line.DrawDottedLine(gl, new Vector3(player2Paddle.Position.x, LowerBoundary.Position.y, commonZ), new Vector3(player2Paddle.Position.x, UpperBoundary.Position.y, commonZ));

            player1Paddle.DrawCube(gl);
            player2Paddle.DrawCube(gl);

            ball.DrawCube(gl);


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

            #region Main
            //Start 
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
                    if (((ballTime % 120) == 0) && (ball.Velocity.GetLength() < 3)) {
                        ball.IncreaseSpeed(0.1f);
                    }
                    angleRandom = (float)Randomizer.Generate(-5.0f, 5.0f);
                }
            }
            #endregion

            #region Ball Aesthetics
            //Draw ball trails
            if (isBallPlayed) {
                Mesh ballTrail = new Mesh() {
                    Position = ball.Position - ball.Velocity,
                    Scale = new Vector3(0.8f, 0.8f, 0),
                    //Color = new Vector3(0.8f, 0.8f, 0.8f)
                };
                ballTrails.Add(ballTrail);
                foreach (var trail in ballTrails) {
                    trail.Color = ball.Color - new Vector4(0.2f, 0.2f, 0.2f);
                    trail.DrawCube(gl);
                    //Reduce size
                    if (trail.Scale.x > 0)  {
                        if (!isOptionMenuOpen) {
                            trail.Scale.x = trail.Scale.x - 0.06f;
                            trail.Scale.y = trail.Scale.y - 0.06f;
                        }
                    }
                    else {
                        trail.Scale.x = 0.8f;
                        trail.Scale.y = 0.8f;
                    }
                    //Use it back again
                    if (trail.Scale.x < 0) {
                        trail.Position = ball.Position;
                    }
                }
            }
            #endregion

            #region Reset Code
            if (isResetOngoing) {
                resetCounter--;
            }

            if (resetCounter <= 0) {
                ball.Velocity *= 0;
                isBallPlayed = false;
                isResetOngoing = false;
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

                //Toggle Player 2
                if (Keyboard.IsKeyToggled(player2Enable)) {
                    isPlayer2AI = false;
                }
                else {
                    isPlayer2AI = true;
                }
            }
            else {
                //Option Menu
                if (Keyboard.IsKeyToggled(pauseKey)) {
                    isOptionMenuOpen = true;
                    ball.enabledUpdate = false;
                    waveOutDevice.Volume = 0.5f;
                }
                else {
                    if (isBallPlayed) {
                        isOptionMenuOpen = false;
                        ball.enabledUpdate = true;
                        waveOutDevice.Volume = 1.0f;
                    }
                }
            }

            //Quit Game
            if (Keyboard.IsKeyDown(playerOneQuitKey) || Keyboard.IsKeyDown(playerTwoQuitKey)) {
                Environment.Exit(0);
            }

            //Test Toggle
            if (Keyboard.IsKeyToggled(testKey)) {
                isTestToggled = true;           
            }
            else {
                isTestToggled = false;
            }

            //Smooth out paddle movement, will come up a good fix later
            if ((!Keyboard.IsKeyDown(playerOneUp)) || (!Keyboard.IsKeyDown(playerOneDown))) {
                player1Paddle.Velocity *= 0;
            }

            //Player 1 Controls
            if (Keyboard.IsKeyDown(playerOneUp) &&
                (player1Paddle.Position.y + player1Paddle.Scale.y <= UpperBoundary.Position.y - (UpperBoundary.Scale.y * 4)) &&
                !isOptionMenuOpen && isGameStarted) {
                if (player1Paddle.Velocity.y < player1MaxSpeed) {
                    player1Paddle.ApplyForce(player1Accel);
                }
            } 
            if (Keyboard.IsKeyDown(playerOneDown) &&
                (player1Paddle.Position.y - player1Paddle.Scale.y >= LowerBoundary.Position.y + (LowerBoundary.Scale.y * 4)) &&
                !isOptionMenuOpen && isGameStarted) {
                if (player1Paddle.Velocity.y > -player1MaxSpeed) {
                    player1Paddle.ApplyForce(player1Accel * -1);
                }
            }

            //Smooth out paddle movement, will come up a good fix later
            if ((!Keyboard.IsKeyDown(playerTwoUp)) || (!Keyboard.IsKeyDown(playerTwoDown))) {
                player2Paddle.Velocity *= 0;
            }

            //Check if AI is not enabled, if so allow Player 2 controls
            if (!isPlayer2AI) {
                if (Keyboard.IsKeyDown(playerTwoUp) &&
                    (player2Paddle.Position.y + player2Paddle.Scale.y <= UpperBoundary.Position.y - (UpperBoundary.Scale.y * 4)) &&
                    !isOptionMenuOpen && isGameStarted) {
                    player2Paddle.ApplyForce(player2Accel);
                }
                if (Keyboard.IsKeyDown(playerTwoDown) &&
                    (player2Paddle.Position.y - player2Paddle.Scale.y >= LowerBoundary.Position.y + (LowerBoundary.Scale.y * 4)) &&
                    !isOptionMenuOpen && isGameStarted) {
                    player2Paddle.ApplyForce(player2Accel * -1);
                }
            }
            else {
                //AI Code
                //Above paddle
                if ((ball.Position.y > player2Paddle.Position.y) &&
                    (ball.Position.x > player2Paddle.Position.x - visionDistance) &&
                    (player2Paddle.Position.y + player2Paddle.Scale.y <= UpperBoundary.Position.y - (UpperBoundary.Scale.y * 4)) &&
                    !isOptionMenuOpen) {
                    if (player2Paddle.Velocity.y < player2MaxSpeed) {
                        player2Paddle.Velocity *= 0;
                        player2Paddle.ApplyForce(player2Accel);
                    }
                }
                //Below paddle
                if ((ball.Position.y < player2Paddle.Position.y) &&
                    (ball.Position.x > player2Paddle.Position.x - visionDistance) &&
                    (player2Paddle.Position.y - player2Paddle.Scale.y >= LowerBoundary.Position.y + (LowerBoundary.Scale.y * 4)) &&
                    !isOptionMenuOpen) {
                    if (player2Paddle.Velocity.y > -player2MaxSpeed) {
                        player2Paddle.Velocity *= 0;
                        player2Paddle.ApplyForce(player2Accel * -1);
                    }
                }

                //AI will return Paddle to center if enabled
                if (willReturnToCenter && (ball.Position.x < player2Paddle.Position.x - visionDistance)) {
                    if (player2Paddle.Position.y == 0) {
                        player2Paddle.Velocity *= 0;
                    }
                    else if (player2Paddle.Position.y > 0) {
                        player2Paddle.ApplyForce(player2Accel * -1);
                    }
                    else {
                        player2Paddle.ApplyForce(player2Accel);
                    }
                }
            }
            #endregion

            #region Scoring
            //Reached behind player 1
            if ((ball.Position.x < player1Paddle.Position.x - scoringArea) &&
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
            if ((ball.Position.x > player2Paddle.Position.x + scoringArea) &&
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

            #region Paddle Wall Collision and Ball Angle
            //Ball reached top
            if (ball.Position.y + ball.Scale.x > maxVerticalBorder) {
                wallHitSound.Play();
                ball.Velocity.y = -ball.Velocity.y;
                ball.Position.y = ball.Position.y - 1.0f;
            }
            //Ball reached bottom
            if (ball.Position.y - ball.Scale.x < maxVerticalBorder * -1) {
                wallHitSound.Play();
                ball.Velocity.y = -ball.Velocity.y;
                ball.Position.y = ball.Position.y + 1.0f;
            }

            //Collision for Player 1 Paddle
            if ((ball.Position.x - ball.Scale.x <= player1Paddle.Position.x + player1Paddle.Scale.x) &&
                (ball.Position.y + ball.Scale.x >= player1Paddle.Position.y - player1Paddle.Scale.y) &&
                (ball.Position.y - ball.Scale.x <= player1Paddle.Position.y + player1Paddle.Scale.y) &&
                //Prevent ball from colliding behind the paddle
                (ball.Position.x > player1Paddle.Position.x)) {
                if (isSoundEnabled) {
                    paddleHitSound.Play();
                }
                //Allowable angle is (0 - 89) && (269 - 360)
                //Ball hitting the exact center is reflected back straight plus random
                if (ball.Position.y == player1Paddle.Position.y) {
                    ball.ChangeAngle(0 + angleRandom);
                }              
                else if (ball.Position.y > player1Paddle.Position.y) {
                    //Max Deviation from center to eachtip is 60 degrees
                    ball.ChangeAngle(10 * (ball.Position.y - player1Paddle.Position.y) + angleRandom);
                }
                else {
                    ball.ChangeAngle(360 + (10 * (ball.Position.y - player1Paddle.Position.y)) + angleRandom);
                }
                ball.Color = player1Paddle.Color + new Vector4(0.2f, 0.2f, 0.2f);
                //Console.WriteLine("Ball hit paddle 1 " + (ball.Position.y - player1Paddle.Position.y) + " from center");
            }

            //Collision for Player 2 Paddle
            if ((ball.Position.x + ball.Scale.x >= player2Paddle.Position.x - player2Paddle.Scale.x) &&
                (ball.Position.y + ball.Scale.x >= player2Paddle.Position.y - player2Paddle.Scale.y) &&
                (ball.Position.y - ball.Scale.x <= player2Paddle.Position.y + player2Paddle.Scale.y) &&
                //Prevent ball from colliding behind the paddle
                (ball.Position.x < player2Paddle.Position.x)) {
                if (isSoundEnabled) {
                    paddleHitSound.Play();
                }

                //Allowable angle is (91 - 269)
                if (ball.Position.y == player2Paddle.Position.y) {
                    ball.ChangeAngle(180 + angleRandom);
                }
                else if (ball.Position.y > player2Paddle.Position.y) {
                    ball.ChangeAngle(180 - (10 * (ball.Position.y - player2Paddle.Position.y)) + angleRandom);
                }
                else {
                    ball.ChangeAngle(180 - (10 * (ball.Position.y - player2Paddle.Position.y)) + angleRandom);
                }
                ball.Color = player2Paddle.Color + new Vector4(0.2f, 0.2f, 0.2f);
                //Console.WriteLine("Ball hit paddle 2 " + (ball.Position.y - player2Paddle.Position.y) + " from center");
            }
            #endregion

            #region Text and Menus
            //Game Screen
            gl.DrawText((int)Width / 2 - 65, (int)Height / 2 - 340, 1.0f, 0.3f, 0.3f, "Courier", 60, "" + player1Score);
            gl.DrawText((int)Width / 2, (int)Height / 2 - 340, 0.3f, 0.3f, 1.0f, "Courier", 60, "" + player2Score);
            gl.DrawText(50, (int)Height / 2 + 230, 1.0f, 0.3f, 0.3f, "Courier", 20, "Player 1");

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
            if (isOptionMenuOpen)
            {
                gl.LoadIdentity();
                gl.Translate(0.0f, .0f, -150.0f);
                OptionMenu.DrawSquare(gl);
                gl.DrawText((int)Width / 2 - ((int)(optionSize[0] * cons) * optionText[0].Length), (int)Height / 2 + 90, 1.0f, 1.0f, 1.0f, "Courier New", optionSize[0], optionText[0]);

                for (int i = 1; i < optionText.GetLength(0); i++) {
                    gl.DrawText((int)Width / 2 - 200, (int)Height / 2 + 30 - (30 * i), 1.0f, 1.0f, 1.0f, "Courier New", optionSize[i], "" + optionText[i]);
                }
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
        }

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
