using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;

namespace battlepong_game.Models {
    public partial class Ball : Mesh {

        public bool isBallPlayed = false;
        public Vector4 defaultColor;

        private List<Mesh> ballExplosions = new List<Mesh>();
        private List<Mesh> ballTrails = new List<Mesh>();
        private float ballTime = 0;

        public Ball() {
            MaxVelocity = 30.0f;
            defaultColor = new Vector4(1.0f, 1.0f, 1.0f);
        }

        public void Play() {
            ballTime++;
        }

        public void SpeedUp(float when) {
            if (((ballTime % when) == 0)) {
                this.IncreaseSpeed(0.1f);
            }
        }

        public void Reset() {
            Color = defaultColor;
            ballTime = 0;
        }

        public void AddTrails(OpenGL gl, bool isOptionMenuOpen) {
            Mesh ballTrail = new Mesh() {
                Position = Position - Velocity - new Vector3(0, 0, 0.5f),
                Scale = new Vector3(0.8f, 0.8f, 0),
            };
            ballTrails.Add(ballTrail);
            foreach (var trail in ballTrails) {
                trail.Color = Color - new Vector4(0.2f, 0.2f, 0.2f, 0.0f);
                //Reduce size
                if (trail.Scale.x > 0) {
                    trail.DrawSquare(gl);
                    if (!isOptionMenuOpen) {
                        trail.Scale -= 0.06f;
                    }
                }
            }
        }

        public void AddExplosion(OpenGL gl, float maxVerticalBorder, Mesh UpperBoundary, Mesh LowerBoundary, bool isOptionMenuOpen) {
            Mesh ballExplosion = new Mesh() {
                Position = Position - Velocity - new Vector3(0, 0, 0.1f),
                Radius = 1.0f,
                Color = new Vector4(0.8f, 0.8f, 0.8f)
            };

            //Ball hitting top or bottom
            if (HasCollidedWith(UpperBoundary) || (HasCollidedWith(LowerBoundary))) {
                ballExplosions.Add(ballExplosion);
            }

            foreach (var explosions in ballExplosions) {
                //Reduce size and opacity
                if (explosions.Radius < 6.0f) {
                    explosions.DrawCircle(gl, 5.0f);
                    if (!isOptionMenuOpen) {
                        explosions.Radius += (6.0f - explosions.Radius) / 2;
                        explosions.Color.a -= 0.2f;
                    }
                }
            }
        }

    }
}
