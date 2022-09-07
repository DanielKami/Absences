using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wygaszenia
{
    public static class Reciprocal
    {
        static float[,] Matrix_B = new float[3, 3];

        //Reciprocal unit cell parameters
        public static float a_ =1.0f;//1
        public static float b_ =1.0f;//2
        public static float c_ =1.0f;//3
        public static float alp_ = MathHelper.ToRadians(90);
        public static float bet_ = MathHelper.ToRadians(90);
        public static float gam_ = MathHelper.ToRadians(90);

        public static int h_min = -2, h_max = 2;
        public static int k_min = -2, k_max = 2;
        public static int l_min = -2, l_max = 2;



        public static float CalculateF(int h, int k, int l, List<object3d> CellAtoms)
        {
            float F_re = 0, F_im = 0;
            float f_re = 1.0f, f_im = 0.0f;


            for (int i = 0; i < CellAtoms.Count; i++)
            {
                Vector3 pos = CellAtoms[i].position;
                F_re+= f_re * (float)Math.Cos(MathHelper.TwoPi * (h * pos.X + k * pos.Y + l * pos.Z));
                F_im+= f_im * (float)Math.Sin(MathHelper.TwoPi * (h * pos.X + k * pos.Y + l * pos.Z));
            }
            return sqr(F_re);

        }

        /***************************************************************************/
        public static void rlat_comp()
        /***************************************************************************/

        /*
        Compute reciprocal lattice parameters. The convention used is that:
        a[i].b[j] = 2*pi*d[ij].
        */

        {

            float volume;

            float a = Cell.a;//1
            float b = Cell.b;//2
            float c = Cell.c;//3
            float alp = Cell.alp;//4
            float bet = Cell.bet;//5
            float gam = Cell.gam;//6

            /* compute volume of real lattice cell */

            volume = a * b * c * (float)Math.Sqrt(1.0f + 2.0f * (float)Math.Cos(alp) * (float)Math.Cos(bet) * (float)Math.Cos(gam)
                - sqr((float)Math.Cos(alp)) - sqr((float)Math.Cos(bet)) - sqr((float)Math.Cos(gam)));

            /* compute reciprocal lattice parameters */

            a_ = MathHelper.TwoPi * b * c * (float)Math.Sin(alp) / volume;
            b_ = MathHelper.TwoPi * a * c * (float)Math.Sin(bet) / volume;
            c_ = MathHelper.TwoPi * a * b * (float)Math.Sin(gam) / volume;
            alp_ = (float)Math.Acos((Math.Cos(bet) * Math.Cos(gam) - Math.Cos(alp)) / (Math.Sin(bet) * Math.Sin(gam)));
            bet_ = (float)Math.Acos((Math.Cos(alp) * Math.Cos(gam) - Math.Cos(bet)) / (Math.Sin(alp) * Math.Sin(gam)));
            gam_ = (float)Math.Acos((Math.Cos(alp) * Math.Cos(bet) - Math.Cos(gam)) / (Math.Sin(alp) * Math.Sin(bet)));

            /* compute matrix B for reciprocal space*/

            Matrix_B[0,0] = a_;
            Matrix_B[1,0] = 0.0f;
            Matrix_B[2,0] = 0.0f;
            Matrix_B[0,1] = b_ * (float)Math.Cos(gam_);
            Matrix_B[1,1] = b_ * (float)Math.Sin(gam_);
            Matrix_B[2,1] = 0.0f;
            Matrix_B[0,2] = c_ * (float)Math.Cos(bet_);
            Matrix_B[1,2] = -c_ * (float)Math.Sin(bet_) * (float)Math.Cos(alp_);
            Matrix_B[2,2] = MathHelper.TwoPi / c;

        }
 

        /***************************************************************************/
        static float sqr(float value)
        /***************************************************************************/
        {
            return (value * value);
        }


        /***************************************************************************/
         public static Vector3 transform(float x, float y, float z )
        /***************************************************************************/

        /*
        Calculates a = m*b, i.e. the transformation from vector b to vector a
        by the matrix m.
        */
        {
            Vector3 a = new Vector3();

            a.X = Matrix_B[0,0] * x + Matrix_B[0,1] * y + Matrix_B[0,2] * z;
            a.Y = Matrix_B[1,0] * x + Matrix_B[1,1] * y + Matrix_B[1,2] * z;
            a.Z = Matrix_B[2,0] * x + Matrix_B[2,1] * y + Matrix_B[2,2] * z;

            return a;
        }



    }
}
