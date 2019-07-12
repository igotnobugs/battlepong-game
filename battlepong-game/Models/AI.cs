using System;

namespace battlepong_game.Models {

    //Main AI code
    public partial class Mesh : Movable {

        public void AIPaddle(Mesh ball, float visionDistance, bool lookRight, float maxSpeed, float friction, Vector3 acceleration, Mesh UpperBoundary, Mesh LowerBoundary, bool isOptionMenuOpen, bool willReturnToCenter, bool isEnabled) {
            if (((ball.Position.x < Position.x + visionDistance) && lookRight) || ((ball.Position.x > Position.x - visionDistance) && !lookRight)) {
                //Above paddle
                if ((ball.Position.y > Position.y + (Scale.y / 2)) &&
                (Position.y + Scale.y <= UpperBoundary.Position.y - (UpperBoundary.Scale.y)) &&
                !isOptionMenuOpen) {
                    if (Velocity.y < maxSpeed) {
                        ApplyForce(acceleration);
                    }
                    else {
                        Velocity.y = maxSpeed;
                    }
                }
                //Below paddle
                else if ((ball.Position.y < Position.y - (Scale.y / 2)) &&
                    (Position.y - Scale.y >= LowerBoundary.Position.y + (LowerBoundary.Scale.y)) &&
                    !isOptionMenuOpen) {
                    if (Velocity.y > -maxSpeed) {
                        ApplyForce(acceleration * -1);
                    }
                    else {
                        Velocity.y = -maxSpeed;
                    }
                }
                else {
                    ApplyFriction(friction);
                }
            }
            else {
                //AI will return Paddle to center if enabled
                if (willReturnToCenter && isEnabled) {
                    if ((Position.y > -2) && (Position.y < 2)) {
                        ApplyFriction(friction);
                    }
                    else if (Position.y > 0) {
                        ApplyForce(acceleration * -1);
                    }
                    else {
                        ApplyForce(acceleration);
                    }
                }
            }
        }
    }
}
