using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace battlepong_game.Models {
    class Paddle : Mesh {
        //23.5
        public float topLimit;
        public float bottomLimit;
        public float maxSpeed;
        public float visionDistance;
        public float frictionCoefficient;
        public Vector3 paddleAcceleration;

        public Key KeyUp;
        public Key KeyDown;

        public Paddle() {
            topLimit = 10.0f;
            bottomLimit = -10.0f;
            maxSpeed = 1.0f;
            visionDistance = 0;
            frictionCoefficient = 0;
            paddleAcceleration = new Vector3();
        }

        public void EnableControl(bool enable, Mesh ball, bool isAI = false)  {

            if (!enable) {
                return;
            }

            if (!isAI) {
                if (Keyboard.IsKeyDown(KeyUp) &&
                    (Position.y + Scale.y <= topLimit)) {
                    if (Velocity.y < maxSpeed) {
                        ApplyForce(paddleAcceleration);
                    }
                    else {
                        Velocity.y = maxSpeed;
                    }
                }
                else if (Keyboard.IsKeyDown(KeyDown) &&
                    (Position.y - Scale.y >= bottomLimit)) {
                    if (Velocity.y > -maxSpeed) {
                        ApplyForce(paddleAcceleration * -1);
                    }
                    else {
                        Velocity.y = -maxSpeed;
                    }
                }
                else {
                    //Slow down the Paddle to zero
                    ApplyFriction(frictionCoefficient);
                }
            }
            else {
                //AI Code
                AIPaddle(ball, visionDistance, true, maxSpeed, frictionCoefficient, paddleAcceleration, topLimit, bottomLimit, true);
            }

            //Collision Correction
            if (Position.y + Scale.y > topLimit) {
                Position.y = topLimit - Scale.y;
            }
            else if (Position.y - Scale.y < bottomLimit) {
                Position.y = bottomLimit + Scale.y;
            }
        }
    }
}



