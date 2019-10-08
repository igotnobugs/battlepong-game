using battlepong_game.Models;
using battlepong_game.Utilities;
using NAudio.Wave;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Media;
using System.Windows;
using System.Windows.Input;
using battlepong_game.Settings;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph;

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
            Title = Properties.strings.Title;
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

        //The common z coordinate for most meshes
        private static float commonZ = 0.0f;

        #region Movable Meshes
        private Paddle player1Paddle = new Paddle() {
            Position = new Vector3(-50, 0, commonZ),
            Scale = new Vector3(1.0f, 6, 1),
            Mass = 1.0f,
            Color = new Vector4(1.0f, 0.3f, 0.3f)
        };
        private Paddle player2Paddle = new Paddle() {
            Position = new Vector3(50, 0, commonZ),
            Scale = new Vector3(1.0f, 6, 1),
            Mass = 1.0f,
            Color = new Vector4(0.3f, 0.3f, 1.0f)
        };
        private Ball ball = new Ball() {
            Position = new Vector3(0, 0, commonZ),
            Scale = new Vector3(1, 1, 1),
            Color = new Vector4(1.0f, 1.0f, 1.0f, 0.0f)
        };
        #endregion

        #region Static Meshes
        private Mesh Triangle = new Mesh() {
            //Position = new Vector3(0, 0, commonZ),
            Scale = new Vector3(1, 1, 1),
            Color = new Vector4(0.5f, 1.0f, 0.5f)
        };
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
            Position = new Vector3(0, 0, commonZ),
            Scale = new Vector3(40, 20, 0),
            Color = new Vector4(0.1f, 0.1f, 0.1f, 0.9f)
        };
        private Mesh OptionMenu = new Mesh() {
            Position = new Vector3(0, 0, commonZ),
            Scale = new Vector3(40, 20, 0),
            Color = new Vector4(0.2f, 0.2f, 0.2f, 0.7f)
        };
        private Mesh ResultMenu = new Mesh() {
            Position = new Vector3(0, 0, commonZ),
            Scale = new Vector3(40, 20, 0),
            Color = new Vector4(0.2f, 0.2f, 0.2f, 0.7f)
        };
        private Mesh Line = new Mesh() {
            Color = new Vector4(0.6f, 0.1f, 0.6f, 0.6f)
        };
        private Mesh Moon = new Mesh() {
            Position = new Vector3(0, 5, commonZ),
            Radius = 10,
            Color = new Vector4(0.6f, 0.1f, 0.6f, 0.4f)
        };
        private Mesh Ground = new Mesh() {
            Position = new Vector3(0, -50, commonZ - 40.0f),
            Scale = new Vector3(20, 50, 0),
            Color = new Vector4(0.0f, 0.0f, 0.0f)

        };

        private Mesh Pyramid = new Mesh() {
            Position = new Vector3(-200, -50, commonZ - 200),
            Scale = new Vector3(40, 60, 30),
            Color = new Vector4(0.6f, 0.1f, 0.6f, 1.0f)
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
        private Key player1Enable = Key.LeftShift;
        private Key player2Enable = Key.RightShift;
        private Key restart = Key.R;

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
        public bool isGameOver = false;
        public float angleRandom;
        public int resetCounter = 30;
        public int player1Score = 0;
        public int player2Score = 0;
        public int winningScore = 5;
        public float ballTime = 0;
        public float scoringLine = 10.0f;
        public bool didPlayer1Scored = false;
        public int framesToCount = 120;
        //public float maxBallVelocity = 3.0f;
        public float angleDeviation = 5.0f;
        public Vector4 lighterColor = new Vector4(0.1f, 0.1f, 0.1f);
        public List<Mesh> ballTrails = new List<Mesh>();
        public List<Mesh> ballExplosions = new List<Mesh>();
        public List<Mesh> landLines = new List<Mesh>();

        //AI CODE
        public bool isPlayer1AI = false;
        public bool isPlayer2AI = true;
        public bool willReturnToCenter = true;
        //private bool willPredictTrajectory = false;

        //String Variables
        public float cons = 0.375f; //Monospace Constant to be able to get the length of string
        public string[] menuText = { "C# PONG", "Enter to Start", "Hold Right Shift to toggle Player 2", "Player 2 Enabled", "Player 2 Disabled" };
        public float[] menuSize = { 50.0f, 30.0f, 20.0f, 15.0f, 15.0f };
        public string optionTitle = "Options";
        public string[] optionText = { "Max Ball Speed", "Test2", "Test3", "Test4", "Test5", "Restart Game" };
        public float[] optionSize = { 20.0f, 20.0f, 20.0f, 20.0f, 20.0f, 20.0f };
        public int indexSelected = 1;
        public string[] resultText = { "Game Over", "Player 1 Won", "Player 2 Won", "Press R to Restart" };
        public float[] resultSize = { 30.0f, 50.0f, 50.0f, 20.0f };
        public float move = -1;

        #endregion

        public Controls Control = new Controls();

        #region GL Draw Functions
        private void Render3D(OpenGL gl) {
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

            gl.ShadeModel(OpenGL.GL_SMOOTH);

            gl.ColorMaterial(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);

            gl.Enable(OpenGL.GL_LINE_SMOOTH);

            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            gl.Enable(OpenGL.GL_BLEND);
        }

        public void DrawMovingGridLines(OpenGL gl, Mesh target) {

            gl.Viewport(0, 0, (int)Width, (int)Height);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.LoadIdentity();
            //gl.Translate(0.0f, 0.0f, 0.0f);

            gl.LookAt(target.Position.x / 8, 60.0f, 10.0f, target.Position.x / 8, -50.0f, -250.0f, 0, 1, 0);



            //Center Line
            Line.DrawSimpleLine(gl, new Vector3(-550, 0, -200), new Vector3(550, 0, -200));
            //Bottom Line
            Line.DrawSimpleLine(gl, new Vector3(-250, 0, 0), new Vector3(250, 0, 0));
            //Vertical Lines To Right
            for (int i = 0; i < 25; i++) {
                Line.DrawSimpleLine(gl, new Vector3(20 * i, 0, 0), new Vector3(20 * i, 0, -200));
            }
            //Vertical Lines to Left
            for (int i = 1; i < 25; i++) {
                Line.DrawSimpleLine(gl, new Vector3(-20 * i, -0, 0), new Vector3(-20 * i, 0, -200));
            }
            //Lines Top to Bottom
            for (int i = 0; i < 10; i++) {
                Line.DrawSimpleLine(gl, new Vector3(-550, 0 - (3 * i), -200 + (20 * i)), new Vector3(550, 0 - (3 * i), -200 + (20 * i)));
            }

            //Draw Pyramid
            for (int i = 0; i < 20; i++) {
                Pyramid.DrawPyramid(gl, new Vector3(-300 + (i * (50 + Pyramid.Scale.z)), 0, -200));
            }
        }

        public void DrawArena(OpenGL gl) {
            gl.LoadIdentity();

            Player1Line.DrawSimpleLine(gl, new Vector3(player1Paddle.Position.x, LowerBoundary.Position.y, commonZ), new Vector3(player1Paddle.Position.x, UpperBoundary.Position.y, commonZ), 5);
            Player2Line.DrawSimpleLine(gl, new Vector3(player2Paddle.Position.x, LowerBoundary.Position.y, commonZ), new Vector3(player2Paddle.Position.x, UpperBoundary.Position.y, commonZ), 5);

            UpperBoundary.DrawSquare(gl);
            LowerBoundary.DrawSquare(gl);

            player1Paddle.DrawSquare(gl);
            player2Paddle.DrawSquare(gl);
            ball.DrawSquare(gl);
        }
        #endregion

        public bool initialized = false;

        public void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args) {

            OpenGL gl = args.OpenGL;

            // Clear The Screen And The Depth Buffer
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            Render3D(gl);
            DrawMovingGridLines(gl, ball);
            DrawArena(gl);

            //Initialize Variables of Players
            while (!initialized) {
                player1Paddle.topLimit = UpperBoundary.Position.y - UpperBoundary.Scale.y;
                player1Paddle.bottomLimit = LowerBoundary.Position.y + LowerBoundary.Scale.y;
                player1Paddle.KeyUp = playerOneUp;
                player1Paddle.KeyDown = playerOneDown;
                player2Paddle.topLimit = UpperBoundary.Position.y - UpperBoundary.Scale.y;
                player2Paddle.bottomLimit = LowerBoundary.Position.y + LowerBoundary.Scale.y;
                player2Paddle.KeyUp = playerTwoUp;
                player2Paddle.KeyDown = playerTwoDown;

                initialized = true;
            }

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
            if (isGameStarted && !isOptionMenuOpen && !isGameOver) {
                //Ball is not moving
                if (!isBallPlayed) {
                    ball.Reset();
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

                    player1Paddle.Rotation = new Vector4(45, 0, 0, 1);
                    //Ball is moving
                    ballTime++;
                    //Speed up every framesToCount
                    if (((ballTime % framesToCount) == 0)) {
                        ball.IncreaseSpeed(0.1f);
                    }
                    angleRandom = (float)Randomizer.Generate(-angleDeviation, angleDeviation);
                    //Ball Aesthetics                 
                    ball.AddTrails(gl, isOptionMenuOpen); //Add a trail to the ball                   
                    ball.AddExplosion(gl, maxVerticalBorder, UpperBoundary, LowerBoundary, isOptionMenuOpen); //A shockwave when ball hit the boundaries

                    //Scoring
                    if ((ball.Position.x < player1Paddle.Position.x - scoringLine) && (resetCounter > 0)) {
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
                    if ((ball.Position.x > player2Paddle.Position.x + scoringLine) && (resetCounter > 0)) {
                        isResetOngoing = true;
                        if (!isScoredAlready) {
                            scoredSound.Play();
                            player1Score++;
                            isScoredAlready = true;
                            didPlayer1Scored = true;
                        }
                    }
                }
            }

            //Winning check
            if ((player1Score >= winningScore) || (player2Score >= winningScore)) {
                isGameOver = true;
                ball.enabledUpdate = false;
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
                player1Paddle.enabledUpdate = Keyboard.IsKeyToggled(pauseKey) ? false : true;
                player2Paddle.enabledUpdate = Keyboard.IsKeyToggled(pauseKey) ? false : true;
                waveOutDevice.Volume = Keyboard.IsKeyToggled(pauseKey) ? 0.5f : 1.0f;
            }
            //Game Over and Restart
            if (isGameOver && Keyboard.IsKeyDown(restart)) {
                isGameOver = false;
                player1Score = 0;
                player2Score = 0;
            }

            //Quit Game
            if (Keyboard.IsKeyDown(playerOneQuitKey) || Keyboard.IsKeyDown(playerTwoQuitKey)) {
                Environment.Exit(0);
            }

            //Test Toggle
            isTestToggled = (Keyboard.IsKeyToggled(testKey)) ? true : false;

            //Enable Player Paddle Controls if optionmenu is not open and game hasn't started
            player1Paddle.EnableControl(((!isOptionMenuOpen) && (isGameStarted)), ball, isPlayer1AI);
            player2Paddle.EnableControl(((!isOptionMenuOpen) && (isGameStarted)), ball, isPlayer2AI);

            #endregion

            #region Ball Collision
            //Ball collided with top or bottom plus safety checks
            if (ball.HasCollidedWith(UpperBoundary) || (ball.HasCollidedWith(LowerBoundary))) {
                wallHitSound.Play();
                ball.Position.y = ball.HasCollidedWith(UpperBoundary) ? ball.Position.y - ball.Scale.y : ball.Position.y + ball.Scale.y;
                ball.Velocity.y = -ball.Velocity.y;
            }

            //Ball collision for Player 1
            if (ball.HasCollidedWith(player1Paddle)) { 
                if (isSoundEnabled) {
                    paddleHitSound.Play();
                }
                //Allowable angle is (0 - 89)
                ball.ChangeAngle(0 + (10 * (ball.Position.y - player1Paddle.Position.y)) + angleRandom);
                ball.Color = player1Paddle.Color + lighterColor;
            }

            //Ball Collision for Player 2 Paddle
            if (ball.HasCollidedWith(player2Paddle)) {
                if (isSoundEnabled) {
                    paddleHitSound.Play();
                }
                //Allowabled angle is(91 - 269)
                ball.ChangeAngle(180 - (10 * (ball.Position.y - player2Paddle.Position.y)) + angleRandom);
                ball.Color = player2Paddle.Color + lighterColor;
            }
            #endregion

            #region Text and Menus
            //Game Screen
            gl.DrawText((int)Width / 2 - 65, (int)Height / 2 - 340, 1.0f, 0.3f, 0.3f, "Courier", 60, "" + player1Score);
            gl.DrawText((int)Width / 2, (int)Height / 2 - 340, 0.3f, 0.3f, 1.0f, "Courier", 60, "" + player2Score);

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

            //Results Menu
            if (isGameOver) {
                gl.LoadIdentity();
                gl.Translate(0.0f, .0f, -150.0f);
                ResultMenu.DrawSquare(gl);
                gl.DrawText((int)Width / 2 - ((int)(resultSize[0] * cons) * resultText[0].Length), (int)Height / 2 + 60, 0.5f, 1.0f, 0.5f, "Courier New", resultSize[0], resultText[0]);
                if (player1Score > player2Score) {
                    gl.DrawText((int)Width / 2 - ((int)(resultSize[1] * cons) * resultText[1].Length), (int)Height / 2 - 30, 1.0f, 0.3f, 0.3f, "Courier New", resultSize[1], resultText[1]);
                } else {
                    gl.DrawText((int)Width / 2 - ((int)(resultSize[2] * cons) * resultText[2].Length), (int)Height / 2 - 30, 0.3f, 0.3f, 1.0f, "Courier New", resultSize[2], resultText[2]);
                }
                gl.DrawText((int)Width / 2 - ((int)(resultSize[3] * cons) * resultText[3].Length), (int)Height / 2 - 140, 1.0f, 1.0f, 1.0f, "Courier New", resultSize[3], resultText[3]);

            }

            //Debug
            if (isTestToggled) {
                gl.DrawText(10, 50, 1.0f, 1.0f, 0, "Calibri", 10, "Angle Randomizer: " + angleRandom);
                gl.DrawText(10, 40, 1.0f, 1.0f, 0, "Calibri", 10, "Ball Position: " + ball.Position.x + ", " + ball.Position.y);
                gl.DrawText(10, 30, 1.0f, 1.0f, 0, "Calibri", 10, "Ball Vector Direction: " + ball.Velocity.x + ", " + ball.Velocity.y);
                gl.DrawText(10, 20, 1.0f, 0.0f, 0, "Calibri", 10, "Reset in: " + resetCounter);
                gl.DrawText(10, 10, 1.0f, 0.0f, 0, "Calibri", 10, "Speed: " + ball.Velocity.GetLength());
                gl.DrawText(10, 60, 1.0f, 1.0f, 0, "Calibri", 10, "Paddle 1 Pos: " + player1Paddle.Position);
            }
            #endregion
        }
        //End

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args) {
            OpenGL gl = args.OpenGL;

            gl.Enable(OpenGL.GL_DEPTH_TEST);

            float[] global_ambient = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
            float[] light0pos = new float[] { 0.0f, 5.0f, 10.0f, 1.0f };
            float[] light0ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light0diffuse = new float[] { 0.3f, 0.3f, 0.3f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

            float[] lmodel_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, lmodel_ambient);

            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);

            gl.ShadeModel(OpenGL.GL_SMOOTH);
        }

        private void OpenGLControl_Resized(object sender, OpenGLEventArgs args) {
            //  TODO: Set the projection matrix here.

            //  Get the OpenGL object.
            OpenGL gl = args.OpenGL;

            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();

            //  Create a perspective transformation.
            gl.Perspective(90.0f, (double)Width / (double)Height, 0.01, 500.0);

            //  Use the 'look at' helper function to position and aim the camera.
            gl.LookAt(0, 1.0f, 45.0f, 0, 1.0f, -250.0f, 0, 1, 0);

            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }
    }
}
