using System;
using SharpGL;
using battlepong_game.Utilities;

namespace battlepong_game.Models {

    //Draw Functions
    public partial class Mesh : Movable {

        public bool enabledUpdate = true;
        public Vector3 Scale = new Vector3(0.5f, 0.5f, 0.5f);
        public Vector4 Color = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
        public float Radius = 0.5f;
        public bool enabledCollision = false;

        //public bool EnabledUpdate { get; set; }

        public Mesh() {
            Position = new Vector3();
            Velocity = new Vector3();
            Acceleration = new Vector3();
            Rotation = 0;
        }

        public Mesh(Vector3 initPos) {
            Position = initPos;
            Velocity = new Vector3();
            Acceleration = new Vector3();
            Rotation = 0;
        }

        public Mesh(float x, float y, float z, int r) {
            Position = new Vector3();
            Velocity = new Vector3();
            Acceleration = new Vector3();
            Position.x = x;
            Position.y = y;
            Position.z = z;
            Rotation = r;
        }

        public void DrawSquare(OpenGL gl, float lineWidth = 1.0f) {
            gl.LineWidth(lineWidth);
            gl.Color(Color.x, Color.y, Color.z, Color.a);
            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
            gl.Vertex(Position.x - Scale.x, Position.y - Scale.y, Position.z);
            gl.Vertex(Position.x - Scale.x, Position.y + Scale.y, Position.z);
            gl.Vertex(Position.x + Scale.x, Position.y + Scale.y, Position.z);
            gl.Vertex(Position.x - Scale.x, Position.y - Scale.y, Position.z);
            gl.Vertex(Position.x + Scale.x, Position.y - Scale.y, Position.z);
            gl.Vertex(Position.x + Scale.x, Position.y + Scale.y, Position.z);
            gl.End();

            if (enabledUpdate) {
                UpdateMotion();
            }
        }

        public void DrawTriangle(OpenGL gl, float lineWidth = 1.0f) {
            gl.LineWidth(lineWidth);
            gl.Color(Color.x, Color.y, Color.z, Color.a);
            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
            gl.Vertex(Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z);
            gl.Vertex(Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z);
            gl.Vertex(Position.x, this.Position.y + this.Scale.y, this.Position.z);
            gl.End();
        }

        public void DrawPyramid(OpenGL gl, Vector3 NewPosiition) {


            //POLYGON
            gl.Color(Color.x, Color.y, Color.z);
            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);

            //gl.Begin(OpenGL.GL_LINE_STRIP);
            //FRONT FACE
            gl.Vertex(NewPosiition.x, NewPosiition.y + Scale.y, NewPosiition.z);
            gl.Vertex(NewPosiition.x - (Scale.x / 2), NewPosiition.y, NewPosiition.z + (Scale.z / 2));
            gl.Vertex(NewPosiition.x + (Scale.x / 2), NewPosiition.y, NewPosiition.z + (Scale.z / 2));
            //LEFT FACE
            gl.Vertex(NewPosiition.x, NewPosiition.y + Scale.y, NewPosiition.z);
            gl.Vertex(NewPosiition.x - (Scale.x / 2), NewPosiition.y, NewPosiition.z - (Scale.z / 2));
            //BOTTOM FACE ????
            gl.Vertex(NewPosiition.x - (Scale.x / 2), NewPosiition.y, NewPosiition.z + (Scale.z / 2));
            gl.Vertex(NewPosiition.x + (Scale.x / 2), NewPosiition.y, NewPosiition.z - (Scale.z / 2));
            gl.Vertex(NewPosiition.x + (Scale.x / 2), NewPosiition.y, NewPosiition.z + (Scale.z / 2));

            gl.Vertex(NewPosiition.x, NewPosiition.y + Scale.y, NewPosiition.z);
            gl.Vertex(NewPosiition.x + (Scale.x / 2), NewPosiition.y, NewPosiition.z - (Scale.z / 2));
            gl.Vertex(NewPosiition.x - (Scale.x / 2), NewPosiition.y, NewPosiition.z - (Scale.z / 2));

            gl.End();

            if (enabledUpdate) {
                UpdateMotion();
            }
        }

        public void DrawCube(OpenGL gl) {
            //POLYGON
            gl.Color(Color.x, Color.y, Color.z);
            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
            //Front face
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);

            //Right face
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z - this.Scale.z);

            //Back face
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z - this.Scale.z);
            //Left face

            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            gl.End();

            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
            //Top face      
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z + this.Scale.z);
            //gl.Color(0, 0, 0);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y + this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y + this.Scale.y, this.Position.z - this.Scale.z);
            gl.End();

            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
            //Bottom face
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z + this.Scale.z);
            gl.Vertex(this.Position.x - this.Scale.x, this.Position.y - this.Scale.y, this.Position.z - this.Scale.z);
            gl.Vertex(this.Position.x + this.Scale.x, this.Position.y - this.Scale.y, this.Position.z - this.Scale.z);
            gl.End();

            if (enabledUpdate) {
                UpdateMotion();
            }
        }

        public void DrawCircle(OpenGL gl, float lineWidth = 1.0f, int Resolution = 50) {
             
            gl.LineWidth(lineWidth);
            gl.Color(Color.x, Color.y, Color.z, Color.a);
            Resolution = (int)GameUtils.Constrain(Resolution, 10, 100);

            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int ii = 0; ii < Resolution; ii++) {
                double angle = 2.0f * Math.PI * ii / Resolution;
                double x = Radius * Math.Cos(angle);
                double y = Radius * Math.Sin(angle);
                gl.Vertex(x + Position.x, y + Position.y, Position.z);
            }
            gl.End();
            UpdateMotion();
        }

        public void DrawLine(OpenGL gl, Mesh origin, Vector3 target, float MultScale = 1.0f, float lineWidth = 1.0f) {
            gl.LineWidth(lineWidth);
            gl.Color(Color.x, Color.y, Color.z);
            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Vertex(origin.Position.x, origin.Position.y, origin.Position.z);
            gl.Vertex((origin.Position.x + target.x) * MultScale, (origin.Position.y + target.y) * MultScale, origin.Position.z);
            gl.End();
            UpdateMotion();
        }

        public void DrawSimpleLine(OpenGL gl, Vector3 origin, Vector3 target, float lineWidth = 1.0f) {
            gl.LineWidth(lineWidth);
            gl.Color(Color.x, Color.y, Color.z, Color.a);
            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Vertex(origin.x, origin.y, origin.z);
            gl.Vertex(target.x, target.y, target.z);
            gl.End();
        }

        public void DrawBasketBall(OpenGL gl, float lineWidth = 1.0f, int Resolution = 50) {
            gl.LineWidth(lineWidth);
            //gl.Color(Color.x, Color.y, Color.z);
            Resolution = (int)GameUtils.Constrain(Resolution, 10, 100);

            //Main Circle
            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int ii = 0; ii < Resolution; ii++) {
                double angle = 2.0f * Math.PI * ii / Resolution;
                double x = Radius * Math.Cos(angle);
                double y = Radius * Math.Sin(angle);
                gl.Vertex(x + Position.x, y + Position.y, Position.z);
            }
            gl.End();

            //Circle Center
            gl.PushMatrix();
            gl.Translate(Position.x, Position.y, Position.z);
            gl.Rotate(0, Rotation, 0);
            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int ii = 0; ii < Resolution; ii++) {
                double angle = 2.0f * Math.PI * ii / Resolution;
                double x = Radius * Math.Sin(angle);
                double z = Radius * Math.Cos(angle);

                gl.Vertex(x, 0, z);
            }
            gl.End();
            gl.PopMatrix();

            //Circle perpendicular to center
            gl.PushMatrix();
            gl.Translate(Position.x, Position.y, Position.z);
            gl.Rotate(0, Rotation - 90, 90);
            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int ii = 0; ii < Resolution; ii++) {
                double angle = 2.0f * Math.PI * ii / Resolution;
                double x = Radius * Math.Sin(angle);
                double z = Radius * Math.Cos(angle);

                gl.Vertex(x, 0, z);
            }
            gl.End();
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(Position.x, Position.y, Position.z);
            gl.Rotate(0, Rotation - 45, 90);
            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int ii = 0; ii < Resolution; ii++) {
                double angle = 2.0f * Math.PI * ii / Resolution;
                double x = Radius * Math.Sin(angle);
                double z = Radius * Math.Cos(angle);

                gl.Vertex(x, 0, z);
            }
            gl.End();
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(Position.x, Position.y, Position.z);
            gl.Rotate(0, Rotation, 90);
            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int ii = 0; ii < Resolution; ii++) {
                double angle = 2.0f * Math.PI * ii / Resolution;
                double x = Radius * Math.Sin(angle);
                double z = Radius * Math.Cos(angle);

                gl.Vertex(x, 0, z);
            }
            gl.End();
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(Position.x, Position.y, Position.z);
            gl.Rotate(0, Rotation + 45, 90);
            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int ii = 0; ii < Resolution; ii++) {
                double angle = 2.0f * Math.PI * ii / Resolution;
                double x = Radius * Math.Sin(angle);
                double z = Radius * Math.Cos(angle);
                gl.Vertex(x, 0, z);
            }
            gl.End();
            gl.PopMatrix();

            //Draw Right Curve
            /*
            gl.Color(0.0f, 0.0f, 0.0f);
            gl.Begin(OpenGL.GL_POLYGON);
            for (int ii = 0; ii < Resolution; ii++) {
                double angle = 2.0f * Math.PI * ii / Resolution;
                double x = Radius * Math.Cos(angle);
                double y = Radius * Math.Sin(angle);
                gl.Vertex(x + this.Position.x, y + this.Position.y, this.Position.z);
            }
            gl.End();
            */
            UpdateMotion();
        }

        private void UpdateMotion() {
            Velocity += Acceleration;
            Position += Velocity;
            Acceleration *= 0;
        }
    }
}
