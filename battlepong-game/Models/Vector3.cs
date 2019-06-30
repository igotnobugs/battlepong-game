using System;

namespace battlepong_game.Models
{
    public class Vector3
    {
        public static Vector3 Up = new Vector3(0, 1, 0);
        public static Vector3 Down = new Vector3(0, -1, 0);
        public static Vector3 Left = new Vector3(-1, 0, 0);
        public static Vector3 Right = new Vector3(1, 0, 0);
        public float x, y, z, a; //vector coordinates

        public Vector3() // default constructor, 0,0,0
        {
            x = 0; y = 0; z = 0;
        }

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3 operator +(Vector3 left, Vector3 right)
        {
            return new Vector3(left.x + right.x,
                left.y + right.y,
                left.z + right.z);
        }

        public static Vector3 operator -(Vector3 left, Vector3 right)
        {
            return new Vector3(left.x - right.x,
                left.y - right.y,
                left.z - right.z);
        }

        public static Vector3 operator *(Vector3 left, float scalar)
        {
            return new Vector3(left.x * scalar,
                left.y * scalar,
                left.z * scalar);
        }

        public static Vector3 operator /(Vector3 left, float scalar)
        {
            return new Vector3(left.x / scalar,
                left.y / scalar,
                left.z / scalar);
        }

        public float GetLength()
        {
            return (float)Math.Sqrt((x * x) + (y * y) + (z * z));
        }

        public Vector3 Normalize()
        {
            float length = GetLength();
            if (x != 0) x /= length;
            if (y != 0) y /= length;
            if (z != 0) z /= length;

            return new Vector3(x, y, z);
        }

        public Vector3 Normalized()
        {
            Vector3 nrmlzd = new Vector3(x, y, z);
            float len = GetLength();
            if (x != 0) nrmlzd.x /= len;
            if (y != 0) nrmlzd.y /= len;
            if (z != 0) nrmlzd.z /= len;

            return nrmlzd;
        }

        public void Clamp(Vector3 limit)
        {
            if (this.x >= limit.x) this.x = limit.x;
            if (this.y >= limit.y) this.y = limit.y;
            if (this.z >= limit.z) this.z = limit.z;
        }

        public void ClampMin(Vector3 limit)
        {
            if (this.x <= limit.x) this.x = limit.x;
            if (this.y <= limit.y) this.y = limit.y;
            if (this.z <= limit.z) this.z = limit.z;
        }


        public void ClampMin(float x, float y, float z)
        {
            if (this.x <= x) this.x = x;
            if (this.y <= y) this.y = y;
            if (this.z <= z) this.z = z;
        }

        public void Clamp(float x, float y, float z)
        {
            if (this.x >= x) this.x = x;
            if (this.y >= y) this.y = y;
            if (this.z >= z) this.z = z;
        }

        public override string ToString()
        {
            return "x: " + x + " y: " + y + " z: " + z;
        }
    }
}
