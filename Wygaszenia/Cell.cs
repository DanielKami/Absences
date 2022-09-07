using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wygaszenia
{
    static class Cell
    {
        public static float a=1;
        public static float b=1;
        public static float c=1;
        public static float alp= MathHelper.ToRadians(90);
        public static float bet = MathHelper.ToRadians(90);
        public static float gam = MathHelper.ToRadians(90);

        public static float Sin_alp;
        public static float Cos_alp;
        public static float Sin_bet;
        public static float Cos_bet;
        public static float Sin_gam;
        public static float Cos_gam;


        public static int x_min=0, x_max=2;
        public static int y_min=0, y_max=2;
        public static int z_min=0, z_max=2;

        public static string system;


        static public void Init()
        {
            Sin_alp = (float)Math.Sin(alp);
            Cos_alp = (float)Math.Cos(alp);
            Sin_bet = (float)Math.Sin(bet);
            Cos_bet = (float)Math.Cos(bet);
            Sin_gam = (float)Math.Sin(gam);
            Cos_gam = (float)Math.Cos(gam);

        }

        static public float Volume()
        {
            float V = a * b * c * (float)Math.Sqrt(1.0 - Cos_alp * Cos_alp - Cos_bet * Cos_bet - Cos_gam * Cos_gam + 2.0 * Cos_alp * Cos_bet * Cos_gam);
            return V;
        }
    }
}
