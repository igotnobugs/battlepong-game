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
            //Console.WriteLine((mousePos.x)+ " " + (mousePos.y));
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

        #region Movable
        private Mesh player1Paddle = new Mesh()
        {
            Position = new Vector3(-50, 0, commonZ),
            Scale = new Vector3(1.0f, 5, 1),
            Mass = 0.5f,
            Color = new Vector3(1.0f, 0.0f, 0.0f)
        };

        private Mesh player2Paddle = new Mesh()
        {
            Position = new Vector3(50, 0, commonZ),
            Scale = new Vector3(1.0f, 5, 1),
            Mass = 0.5f,
            Color = new Vector3(0.0f, 0.0f, 1.0f)
        };

        private Mesh ball = new Mesh()
        {
            Position = new Vector3(0, 0, commonZ),
            Scale = new Vector3(1, 1, 1),
            Color = new Vector3(1.0f, 1.0f, 1.0f)
        };

        #endregion

        #region Static
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

        public Vector3 mouseVector = new Vector3();
        public Vector3 mousePos = new Vector3();

        //Controls
        public Key player1Up = Key.W;
        public Key player1Down = Key.S;
        public Key player2Up = Key.Up;
        public Key player2Down = Key.Down;

        //Stats
        public float player1Speed = 4.0f;
        public float player2Speed = 4.0f;

        //Game States
        private bool playBall = false;
        private bool alreadyScored = false;
        private int resetCounter = 30;
        private int player1Score = 0;
        private int player2Score = 0;
        private float time = 0;
        private float scoringArea = 10.0f;

        //AI CODE
        private bool isPlayer2AI = true;
        private int visionDistance = 55;

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
                ball.Position = new Vector3(0, 0, commonZ);
                ball.ApplyForce(new Vector3(2, 0, 0));
                playBall = true;
                alreadyScored = false;
                resetCounter = 30;
            }
            else
            {
                time++;
                //Speed up every 120 frames but stop when velocity is 3.

                if (((time % 120) == 0) && (ball.Velocity.GetLength() < 3))
                {
                    ball.IncreaseSpeed(0.1f);
                }
            }

            if (resetCounter <= 0)
            {
                playBall = false;
            }

            #region Controls
            if ((Keyboard.IsKeyDown(player1Up)) &&
                (player1Paddle.Position.y + player1Paddle.Scale.y <= UpperBoundary.Position.y - (UpperBoundary.Scale.y * 4)))
            {
                player1Paddle.Position.y += player1Speed;
            }
            if ((Keyboard.IsKeyDown(player1Down)) &&
                (player1Paddle.Position.y - player1Paddle.Scale.y >= LowerBoundary.Position.y + (LowerBoundary.Scale.y * 4)))
            {
                player1Paddle.Position.y -= player1Speed;
            }
            //Check if AI is enabled
            if (!isPlayer2AI)
            {
                if ((Keyboard.IsKeyDown(player2Up)) &&
                    (player2Paddle.Position.y + player2Paddle.Scale.y <= UpperBoundary.Position.y - (UpperBoundary.Scale.y * 4)))
                {
                    player2Paddle.Position.y += player2Speed;
                }
                if ((Keyboard.IsKeyDown(player2Down)) &&
                    (player2Paddle.Position.y - player2Paddle.Scale.y >= LowerBoundary.Position.y + (LowerBoundary.Scale.y * 4)))
                {
                    player2Paddle.Position.y -= player2Speed;
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
                    player2Paddle.Position.y += player2Speed;
                }
                //Below paddle
                if ((ball.Position.y < player2Paddle.Position.y) &&
                    (ball.Position.x < player2Paddle.Position.x + visionDistance) &&
                    (player2Paddle.Position.y - player2Paddle.Scale.y >= LowerBoundary.Position.y + (LowerBoundary.Scale.y * 4)))
                {
                    player2Paddle.Position.y -= player2Speed;
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
                }
            }
            #endregion

            #region Text
            gl.DrawText(((int)Width / 2) - 40, 35, 1.0f, 0, 0, "Calibri", 30, "" + player1Score);
            gl.DrawText((int)Width / 2 - 10, 35, 0, 1.0f, 0, "Calibri", 30, ":");
            gl.DrawText((int)Width / 2 + 20, 35, 0, 0, 1.0f, "Calibri", 30, "" + player2Score);
            gl.DrawText(10, 20, 1.0f, 0, 0, "Calibri", 10, "" + ball.Velocity.GetLength());
            gl.DrawText(10, 30, 1.0f, 0, 0, "Calibri", 10, "" + resetCounter);
            #endregion

            #region Collision
            if (ball.Position.y + ball.Scale.x > UpperBoundary.Position.y - (UpperBoundary.Scale.y * 4))
            {
                ball.Velocity.y = -ball.Velocity.y;
            }

            if (ball.Position.y - ball.Scale.x < LowerBoundary.Position.y + (LowerBoundary.Scale.y * 4))
            {
                ball.Velocity.y = -ball.Velocity.y;
            }

            //Collision for Player 1 Paddle
            if ((ball.Position.x - ball.Scale.x <= player1Paddle.Position.x + player1Paddle.Scale.x) &&
                (ball.Position.y + ball.Scale.x >= player1Paddle.Position.y - player1Paddle.Scale.y) &&
                (ball.Position.y - ball.Scale.x <= player1Paddle.Position.y + player1Paddle.Scale.y) &&
                //Prevent ball from colliding behind the paddle
                (ball.Position.x > player1Paddle.Position.x))
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"Sound\Power.wav");
                player.Play();

                if (ball.Position.y > player1Paddle.Position.y)
                {
                    ball.ChangeAngle(50);
                }
                else
                {
                    ball.ChangeAngle(310);
                }
            }

            //Collision for Player 2 Paddle
            if ((ball.Position.x + ball.Scale.x >= player2Paddle.Position.x - player2Paddle.Scale.x) &&
                (ball.Position.y + ball.Scale.x >= player2Paddle.Position.y - player2Paddle.Scale.y) &&
                (ball.Position.y - ball.Scale.x <= player2Paddle.Position.y + player2Paddle.Scale.y) &&
                //Prevent ball from colliding behind the paddle
                (ball.Position.x < player2Paddle.Position.x))
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"Sound\Power.wav");
                player.Play();

                if (ball.Position.y > player2Paddle.Position.y)
                {
                    ball.ChangeAngle(130);
                }
                else
                {
                    ball.ChangeAngle(230);
                }
            }
            #endregion
        }
    }
}
