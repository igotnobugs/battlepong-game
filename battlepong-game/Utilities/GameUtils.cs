using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using battlepong_game.Models;

namespace battlepong_game.Utilities
{
    public class GameUtils : Mesh
    {
        public static float Constrain(float val, float min, float max)
        {
            if (val <= min) return min;
            else if (val >= max) return max;
            else return val;
        }
        public static float GetDistanceBetween(Mesh firstObject, Mesh secondObject)
        {
            var x = secondObject.Position.x - firstObject.Position.x;
            var y = secondObject.Position.y - firstObject.Position.y;
            var distance = (float)Math.Sqrt((x * x) + (y * y));
            return distance;
        }

        public static double GetAngleBetween(Mesh firstObject, Mesh secondObject)
        {
            var x = secondObject.Position.x - firstObject.Position.x;
            var y = secondObject.Position.y - firstObject.Position.y;
            var angle = Math.Tan(y / x);
            return angle;
        }
        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
        public static double RadianToDegree(double angle)
        {
            return Math.PI / angle * 180.0;
        }
    }
}
