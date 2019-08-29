using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;

namespace battlepong_game.Models {
    public partial class Effects : Mesh {

        public List<Mesh> ballTrails = new List<Mesh>();
        public List<Mesh> ballExplosions = new List<Mesh>();

        public void addTrails(OpenGL gl, Mesh ball, bool isOptionMenuOpen) {
            Mesh ballTrail = new Mesh() {
                Position = ball.Position - ball.Velocity - new Vector3(0, 0, 0.5f),
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
        }

        public void addExplosion(OpenGL gl, Mesh ball, bool isOptionMenuOpen) {
            Mesh ballExplosion = new Mesh() {
                Position = ball.Position - ball.Velocity - new Vector3(0, 0, 0.1f),
                Radius = 1.0f,
                Color = new Vector4(0.8f, 0.8f, 0.8f)
            };

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
