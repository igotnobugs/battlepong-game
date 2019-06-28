using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using battlepong_game.Utilities;
using battlepong_game.Models;
using SharpGL;

namespace battlepong_game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(delegate (object sender, RoutedEventArgs args)
            {
                Top = 0;
                Left = 50;
            });
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);
            mousePos.x = (float)position.X - (float)Width / 2.0f;
            mousePos.y = -((float)position.Y - (float)Height / 2.0f);
            //Console.WriteLine((mousePos.x) + " " + (mousePos.y));
        }

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {

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

            gl.Enable(OpenGL.GL_LINE_SMOOTH);
            gl.ShadeModel(OpenGL.GL_SMOOTH);
        }
        public static float commonZ = 50.0f;

        #region Movable Meshes
        private Mesh player1Paddle = new Mesh()
        {
            Position = new Vector3(-50, 0, commonZ),
            Scale = new Vector3(1.0f, 6, 1),
            Mass = 1.0f,
            Color = new Vector3(1.0f, 0.0f, 0.0f)
        };

        private Mesh player2Paddle = new Mesh()
        {
            Position = new Vector3(50, 0, commonZ),
            Scale = new Vector3(1.0f, 6, 1),
            Mass = 1.0f,
            Color = new Vector3(0.0f, 0.0f, 1.0f)
        };

        private Mesh ball = new Mesh()
        {
            Position = new Vector3(0, 0, commonZ),
            Scale = new Vector3(1, 1, 1),
            Color = new Vector3(1.0f, 1.0f, 1.0f)
        };
        #endregion

        #region Static Meshes
        private Mesh UpperBoundary = new Mesh()
        {
            Position = new Vector3(0, 30, commonZ),
            Scale = new Vector3(60, 0.5f, 1),
            Color = new Vector3(1.0f, 1.0f, 1.0f)
        };
        private Mesh LowerBoundary = new Mesh()
        {
            Position = new Vector3(0, -30, commonZ),
            Scale = new Vector3(60, 0.5f, 1),
            Color = new Vector3(1.0f, 1.0f, 1.0f)
        };
        private Mesh Player1Line = new Mesh()
        {
            Color = new Vector3(0.5f, 0, 0)
        };
        private Mesh Player2Line = new Mesh()
        {
            Color = new Vector3(0, 0, 0.5f)
        };
        private Mesh CenterLine = new Mesh()
        {
            Color = new Vector3(0.5f, 0.5f, 0.5f)
        };

        #endregion

        #region Variables
        public Vector3 mouseVector = new Vector3();
        public Vector3 mousePos = new Vector3();

        //Controls
        public Key player1Up = Key.W;
        public Key player1Down = Key.S;
        public Key player2Up = Key.Up;
        public Key player2Down = Key.Down;
        public Key player1quitKey = Key.Escape;
        public Key player2Quit = Key.End;

        //GameSettings
        //private float maxHorizontalBorder = 50.0f;
        private float maxVerticalBorder = 30.0f;
        //private double angleRandom = 10.0f;
        private bool isSoundEnabled = false;
        System.Media.SoundPlayer paddleHitSound = new System.Media.SoundPlayer(@"Sound\Power.wav");


        //Stats
        public float player1MaxSpeed = 4.0f;
        public Vector3 player1Accel = new Vector3(0, 1.0f, 0);
        public float player2MaxSpeed = 4.0f;
        public Vector3 player2Accel = new Vector3(0, 1.0f, 0);
 
        //Game States
        private bool playBall = false;
        private bool alreadyScored = false;
        private int resetCounter = 30;
        private int player1Score = 0;
        private int player2Score = 0;
        private float time = 0;
        private float scoringArea = 10.0f;
        private bool player1Scored = false;
        private List<Mesh> ballTrails = new List<Mesh>();

        //AI CODE
        private bool isPlayer2AI = true;
        private int visionDistance = 55;
        #endregion

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            Title = "battlepong-game";

            OpenGL gl = args.OpenGL;
            gl.Viewport(0, 0, (int)Width, (int)Height);

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.LoadIdentity();
            gl.Translate(0.0f, 0.0f, -150.0f);

            UpperBoundary.DrawCube(gl);
            LowerBoundary.DrawCube(gl);
            CenterLine.DrawDottedLine(gl, new Vector3(0, LowerBoundary.Position.y, commonZ), new Vector3(0, UpperBoundary.Position.y, commonZ));
            Player1Line.DrawDottedLine(gl, new Vector3(player1Paddle.Position.x, LowerBoundary.Position.y, commonZ), new Vector3(player1Paddle.Position.x, UpperBoundary.Position.y, commonZ));
            Player2Line.DrawDottedLine(gl, new Vector3(player2Paddle.Position.x, LowerBoundary.Position.y, commonZ), new Vector3(player2Paddle.Position.x, UpperBoundary.Position.y, commonZ));

            player1Paddle.DrawCube(gl);
            player2Paddle.DrawCube(gl);

            ball.DrawCube(gl);

            //Start
            if (!playBall)
            {
                ball.Color = new Vector3(1.0f, 1.0f, 1.0f);
                ball.Position = new Vector3(0, 0, commonZ);
                if (player1Scored)
                {
                    //if Player 1 scored Ball to player2
                    ball.ApplyForce(new Vector3(1.4f, 1.4f, 0));
                } else
                {
                    ball.ApplyForce(new Vector3(-1.4f, 1.4f, 0));
                }
                player1Paddle.Position.y *= 0;
                player2Paddle.Position.y *= 0;
                playBall = true;
                alreadyScored = false;
                resetCounter = 30;
            }
            else
            {
                time++; 
                //Speed up every 120 frames but stop when velocity of ball is 3.
                if (((time % 120) == 0) && (ball.Velocity.GetLength() < 3))
                {
                    ball.IncreaseSpeed(0.1f);
                } 
            }

            //Draw ball trails
            if (playBall)
            {
                Mesh ballTrail = new Mesh()
                {
                    Position = ball.Position - ball.Velocity,
                    Scale = new Vector3(0.8f, 0.8f, 0),
                    //Color = new Vector3(0.8f, 0.8f, 0.8f)
                };
                ballTrails.Add(ballTrail);
                foreach (var trail in ballTrails)
                {

                    trail.Color = ball.Color - new Vector3(0.2f, 0.2f, 0.2f);
                    trail.DrawCube(gl);

                    //Reduce size
                    if (trail.Scale.x > 0)
                    {
                        trail.Scale.x = trail.Scale.x - 0.06f;
                        trail.Scale.y = trail.Scale.y - 0.06f;
                    } else
                    {
                        trail.Scale.x = 0.8f;
                        trail.Scale.y = 0.8f;
                    }

                    //Use it back again
                    if (trail.Scale.x < 0)
                    {
                        trail.Position = ball.Position;
                    }
                }
            }
            if (resetCounter <= 0)
            {
                playBall = false;
            }

            #region Controls
            //Quit Game
            if (Keyboard.IsKeyDown(player2Quit))
            {
                Environment.Exit(0);
            }

            //Smooth out paddle movement, will come up a good fix later
            if ((!Keyboard.IsKeyDown(player1Up)) || (!Keyboard.IsKeyDown(player1Down)))
            {
                player1Paddle.Velocity *= 0;
            }

            //Player 1 Controls maxVerticalBorder
            if ((Keyboard.IsKeyDown(player1Up)) &&
                (player1Paddle.Position.y + player1Paddle.Scale.y <= UpperBoundary.Position.y - (UpperBoundary.Scale.y * 4)))
            {
                if (player1Paddle.Velocity.y < player1MaxSpeed)
                {
                    player1Paddle.ApplyForce(player1Accel);
                }
            } 
            if ((Keyboard.IsKeyDown(player1Down)) &&
                (player1Paddle.Position.y - player1Paddle.Scale.y >= LowerBoundary.Position.y + (LowerBoundary.Scale.y * 4)))
            {
                if (player1Paddle.Velocity.y > -player1MaxSpeed)
                {
                    player1Paddle.ApplyForce(player1Accel * -1);
                }
            }

            //Smooth out paddle movement, will come up a good fix later
            if ((!Keyboard.IsKeyDown(player2Up)) || (!Keyboard.IsKeyDown(player2Down)))
            {
                player2Paddle.Velocity *= 0;
            }

            //Check if AI is not enabled, if so allow Player 2 controls
            if (!isPlayer2AI)
            {
                if ((Keyboard.IsKeyDown(player2Up)) &&
                    (player2Paddle.Position.y + player2Paddle.Scale.y <= UpperBoundary.Position.y - (UpperBoundary.Scale.y * 4)))
                {
                    player2Paddle.ApplyForce(player2Accel);
                }
                if ((Keyboard.IsKeyDown(player2Down)) &&
                    (player2Paddle.Position.y - player2Paddle.Scale.y >= LowerBoundary.Position.y + (LowerBoundary.Scale.y * 4)))
                {
                    player2Paddle.ApplyForce(player2Accel * -1);

                }
            }
            else
            {
                //AI Code
                //Above paddle
                if ((ball.Position.y > player2Paddle.Position.y) &&
                    (ball.Position.x > player2Paddle.Position.x - visionDistance) &&
                    (player2Paddle.Position.y + player2Paddle.Scale.y <= UpperBoundary.Position.y - (UpperBoundary.Scale.y * 4)))
                {
                    if (player2Paddle.Velocity.y < player2MaxSpeed)
                    {
                        player2Paddle.Velocity *= 0;
                        player2Paddle.ApplyForce(player2Accel);
                    }
                }
                //Below paddle
                if ((ball.Position.y < player2Paddle.Position.y) &&
                    (ball.Position.x < player2Paddle.Position.x + visionDistance) &&
                    (player2Paddle.Position.y - player2Paddle.Scale.y >= LowerBoundary.Position.y + (LowerBoundary.Scale.y * 4)))
                {
                    if (player2Paddle.Velocity.y > -player2MaxSpeed)
                    {
                        player2Paddle.Velocity *= 0;
                        player2Paddle.ApplyForce(player2Accel * -1);
                    }
                }
            }
            #endregion

            #region Scoring
            //Reached behind player 1
            if ((ball.Position.x < player1Paddle.Position.x - scoringArea) &&
                (resetCounter > 0))
            {
                //Stop the ball
                ball.Velocity *= 0;
                //Start reset counter
                resetCounter--;
                //Score once
                if (!alreadyScored)
                {
                    player2Score++;
                    alreadyScored = true;
                    player1Scored = false;
                }

            }
            //Reached behind player 2
            if ((ball.Position.x > player2Paddle.Position.x + scoringArea) &&
                (resetCounter > 0))
            {
                ball.Velocity *= 0;
                resetCounter--;
                if (!alreadyScored)
                {
                    player1Score++;
                    alreadyScored = true;
                    player1Scored = true;
                }
            }
            #endregion

            #region Text
            gl.DrawText((int)Width / 2 - 60, (int)Height / 2 + 160, 1.0f, 0.5f, 0.5f, "Calibri", 60, "" + player1Score);
            gl.DrawText((int)Width / 2 + 10, (int)Height / 2 + 160, 0.5f, 0.5f, 1.0f, "Calibri", 60, "" + player2Score);
            gl.DrawText(10, 20, 1.0f, 0, 0, "Calibri", 10, "Speed: " + ball.Velocity.GetLength());
            gl.DrawText(10, 30, 1.0f, 0, 0, "Calibri", 10, "Reset in: " + resetCounter);
            //Debug
            gl.DrawText(10, 50, 1.0f, 1.0f, 0, "Calibri", 20, "Ball Vector Direction:" + ball.Velocity.x + ", " + ball.Velocity.y);
            gl.DrawText(10, 70, 1.0f, 1.0f, 0, "Calibri", 10, "Ball Position: " + ball.Position.x + ", " + ball.Position.y);
            #endregion

            #region Collision
            //Ball reached top
            if (ball.Position.y + ball.Scale.x > maxVerticalBorder)
            {          
                ball.Velocity.y = -ball.Velocity.y;
                ball.Position.y = ball.Position.y - 1.0f;
            }
            //Ball reached bottom
            if (ball.Position.y - ball.Scale.x < maxVerticalBorder * -1)
            {
                ball.Velocity.y = -ball.Velocity.y;
                ball.Position.y = ball.Position.y + 1.0f;
            }

            //Collision for Player 1 Paddle
            if ((ball.Position.x - ball.Scale.x <= player1Paddle.Position.x + player1Paddle.Scale.x) &&
                (ball.Position.y + ball.Scale.x >= player1Paddle.Position.y - player1Paddle.Scale.y) &&
                (ball.Position.y - ball.Scale.x <= player1Paddle.Position.y + player1Paddle.Scale.y) &&
                //Prevent ball from colliding behind the paddle
                (ball.Position.x > player1Paddle.Position.x))
            {
                if (isSoundEnabled)
                {
                    paddleHitSound.Play();
                }
                //Allowable angle is (0 - 89) && (269 - 360)
                //Ball hitting the exact center is reflected back straight
                if (ball.Position.y == player1Paddle.Position.y)
                {
                    ball.ChangeAngle(0);
                }              
                else if (ball.Position.y > player1Paddle.Position.y)
                {
                    //Max Deviation from center to eachtip is 60 degrees
                    ball.ChangeAngle(10 * (ball.Position.y - player1Paddle.Position.y));
                } else
                {
                    ball.ChangeAngle(360 + (10 * (ball.Position.y - player1Paddle.Position.y)));
                }
                ball.Color = player1Paddle.Color + new Vector3(0.2f, 0.2f, 0.2f);
                Console.WriteLine("Ball hit paddle 1 " + (ball.Position.y - player1Paddle.Position.y) + " from center");
            }

            //Collision for Player 2 Paddle
            if ((ball.Position.x + ball.Scale.x >= player2Paddle.Position.x - player2Paddle.Scale.x) &&
                (ball.Position.y + ball.Scale.x >= player2Paddle.Position.y - player2Paddle.Scale.y) &&
                (ball.Position.y - ball.Scale.x <= player2Paddle.Position.y + player2Paddle.Scale.y) &&
                //Prevent ball from colliding behind the paddle
                (ball.Position.x < player2Paddle.Position.x))
            {
                if (isSoundEnabled)
                {
                    paddleHitSound.Play();
                }

                //Allowable angle is (91 - 269)
                if (ball.Position.y == player2Paddle.Position.y)
                {
                    ball.ChangeAngle(180);
                }
                else if (ball.Position.y > player2Paddle.Position.y)
                {
                    ball.ChangeAngle(180 - (10 * (ball.Position.y - player2Paddle.Position.y)));
                }
                else
                {
                    ball.ChangeAngle(180 - (10 * (ball.Position.y - player2Paddle.Position.y)));
                }

                ball.Color = player2Paddle.Color + new Vector3(0.2f, 0.2f, 0.2f);
                Console.WriteLine("Ball hit paddle 2 " + (ball.Position.y - player2Paddle.Position.y) + " from center");
            }
            #endregion
        }
    }
}
