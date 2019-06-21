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
            Scale = new Vector3(1, 5, 1),
            Mass = 0.5f,
            Color = new Vector3(200, 0, 0)
        };

        private Mesh player2Paddle = new Mesh()
        {
            Position = new Vector3(50, 0, commonZ),
            Scale = new Vector3(1, 5, 1),
            Mass = 0.5f,
            Color = new Vector3(0, 0, 200)
        };

        private Mesh ball = new Mesh()
        {
            Position = new Vector3(0, 0, commonZ),
            Radius = 1,
            Color = new Vector3(200, 200, 200)
        };

        #endregion

        #region Static
        private Mesh UpperBoundary = new Mesh()
        {
            Position = new Vector3(0, 30, commonZ),
            Scale = new Vector3(60, 0.5f, 1),
            Color = new Vector3(200, 200, 200)
        };
        private Mesh LowerBoundary = new Mesh()
        {
            Position = new Vector3(0, -30, commonZ),
            Scale = new Vector3(60, 0.5f, 1),
            Color = new Vector3(200, 200, 200)
        };
        private Mesh CenterLine = new Mesh()
        {
            Color = new Vector3(100, 100, 100)
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
        private float ballSpeedModifier = 1.0f;
        private float time = 0;

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
            
            player1Paddle.DrawCube(gl);
            player2Paddle.DrawCube(gl);

            ball.DrawCircle(gl);
            
            //Start
            if (!playBall)
            {
                ball.Position = new Vector3(0, 0, commonZ);
                ball.ApplyForce(new Vector3(2, 0, 0));
                playBall = true;              
                alreadyScored = false;
                ballSpeedModifier = 1.0f;
                resetCounter = 30;
            } else
            {
                time++;
                //Speed up every 10 frames
                if ((time % 10) == 0)
                {
                    ballSpeedModifier = ballSpeedModifier + 0.01f;
                    ball.Velocity.x *= ballSpeedModifier;
                }
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
            } else
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

            //Reached behind player 1
            if ((ball.Position.x < player1Paddle.Position.x - 5.0f) && 
                (resetCounter > 0))
            {
                ball.Velocity *= 0;
                resetCounter--;
                if (!alreadyScored)
                {
                    player2Score++;
                    alreadyScored = true;
                }
                
            }
            //Reached behind player 2
            if ((ball.Position.x > player2Paddle.Position.x + 5.0f) && 
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
            gl.DrawText(((int)Width / 2) - 40, 35, 1.0f, 0, 0, "Calibri", 30, "" + player1Score);
            gl.DrawText((int)Width / 2 - 10, 35, 0, 1.0f, 0, "Calibri", 30, ":");
            gl.DrawText((int)Width / 2 + 20, 35, 0, 0, 1.0f, "Calibri", 30, "" + player2Score);

            if (resetCounter <= 0)
            {
                playBall = false;
            }



            if (ball.Position.y >= UpperBoundary.Position.y - (UpperBoundary.Scale.y * 4))
            {
                ball.Velocity.y = -ball.Velocity.y;
            }
            if (ball.Position.y <= LowerBoundary.Position.y + (LowerBoundary.Scale.y * 4))
            {
                ball.Velocity.y = -ball.Velocity.y;
            }

            //Collision for Player 1 Paddle
            if (ball.Position.x < -40)
            {
                if ((ball.Position.x - ball.Radius - (ball.Velocity.y / 2) <= player1Paddle.Position.x + player1Paddle.Scale.x) &&
                    (ball.Position.y + ball.Radius >= player1Paddle.Position.y - player1Paddle.Scale.y) &&
                    (ball.Position.y - ball.Radius <= player1Paddle.Position.y + player1Paddle.Scale.y))
                {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"Sound\Power.wav");
                    player.Play();

                    if (ball.Position.y > player1Paddle.Position.y)
                    {
                        ball.Velocity.y = 1;
                    }
                    else
                    {
                        ball.Velocity.y = -1;
                    }

                    ball.Velocity.x = -ball.Velocity.x;
                }
            }

            //Collision for Player 2 Paddle
            if (ball.Position.x > 40)
            {
                if ((ball.Position.x + ball.Radius + (ball.Velocity.y / 2) >= player2Paddle.Position.x - player2Paddle.Scale.x) &&
                    (ball.Position.y + ball.Radius >= player2Paddle.Position.y - player2Paddle.Scale.y) &&
                    (ball.Position.y - ball.Radius <= player2Paddle.Position.y + player2Paddle.Scale.y))
                {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"Sound\Power.wav");
                    player.Play();

                    if (ball.Position.y > player2Paddle.Position.y)
                    {
                        ball.Velocity.y = 1;
                    }
                    else
                    {
                        ball.Velocity.y = -1;
                    }
                    
                    ball.Velocity.x = -ball.Velocity.x;
                }
            }
        }
    }
}
