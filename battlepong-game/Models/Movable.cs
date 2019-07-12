using System;
using battlepong_game.Utilities;

namespace battlepong_game.Models {

    public class Movable {

        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 Acceleration;
        public Vector3 Friction;
        public int Rotation;

        public float Mass = 1;

        public void ApplyForce(Vector3 force) {
            this.Acceleration += (force / Mass);
        }

        public void ApplyGravity(float scalar = 0.1f) {
            this.Acceleration += (new Vector3(0, -scalar * Mass, 0) / Mass);
        }

        public void ApplyFriction(float frictionCoefficient = 0.1f, float normalForce = 1.0f) {
            var frictionMagnitude = frictionCoefficient * normalForce;

            Friction = this.Velocity;
            Friction *= -1;
            Friction.Normalize();
            Friction *= frictionMagnitude;
            this.ApplyForce(Friction);
        }

        public void ChangeAngle(float angle) {
            var speed = this.Velocity.GetLength();
            //this.Velocity.x = speed * (float)Math.Cos(GameUtils.DegreeToRadian(angle));
            //this.Velocity.y = speed * (float)Math.Sin(GameUtils.DegreeToRadian(angle));
            //Vector Velocity changed to Specific Angle Multiplied by Speed
            Velocity = new Vector3((float)Math.Cos(GameUtils.DegreeToRadian(angle)), (float)Math.Sin(GameUtils.DegreeToRadian(angle)), Velocity.z) * speed;
        }

        public void IncreaseSpeed(float speed) {
            var curSpeed = this.Velocity.GetLength();
            Velocity = (Velocity.Normalized() * (curSpeed + speed));
        }
    }
}
