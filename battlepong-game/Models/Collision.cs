using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace battlepong_game.Models {

    //Collision Functions
    public partial class Mesh : Movable {

        public float TopCollision() {
            return Position.y + Scale.y;
        }

        public float BottomCollision() {
            return Position.y - Scale.y;
        }

        public float RightCollision() {
            return Position.x + Scale.x;
        }

        public float LeftCollision() {
            return Position.x - Scale.x;
        }

        public bool HasCollidedWith(Mesh target) {
            bool xHasNotCollided =
                Position.x - Scale.x + (Velocity.x / 2) > target.Position.x + target.Scale.x ||
                Position.x + Scale.x + (Velocity.x / 2) < target.Position.x - target.Scale.x;

            bool yHasNotCollided =
                Position.y - Scale.y + (Velocity.y / 2) > target.Position.y + target.Scale.y ||
                Position.y + Scale.y + (Velocity.y / 2) < target.Position.y - target.Scale.y;

            bool zHasNotCollided =
                Position.z - Scale.z > target.Position.z + target.Scale.z ||
                Position.z + Scale.z < target.Position.z - target.Scale.z;

            return !(xHasNotCollided || yHasNotCollided || zHasNotCollided);
        }
    }
}
