using System;

namespace battlepong_game.Models {

    public class Vector4 {

        public float x, y, z, a; //vector coordinates

        public Vector4() {
            x = 0; y = 0; z = 0; a = 0;
        }

        public Vector4(float x, float y, float z, float a = 1.0f) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.a = a;
        }

        public static Vector4 operator +(Vector4 left, Vector4 right) {
            return new Vector4(left.x + right.x,
                left.y + right.y,
                left.z + right.z,
                left.a + right.a);
        }

        public static Vector4 operator -(Vector4 left, Vector4 right) {
            return new Vector4(left.x - right.x,
                left.y - right.y,
                left.z - right.z,
                left.a - right.a);
        }

        public override string ToString() {
            return "x: " + x + " y: " + y + " z: " + z + " a: " + a;
        }
    }
}
