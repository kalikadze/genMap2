using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace genMap
{
    /// <summary>
    /// procedural generating of map
    /// source: http://nehe.ceske-hry.cz/cl_gl_generovani_terenu.php
    /// </summary>
    public partial class testGen : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// normalize values to desired interval
        /// </summary>
        /// <param name="val">input value</param>
        /// <param name="min">interval minimum</param>
        /// <param name="max">interval maximum</param>
        /// <returns></returns>
        private Color setToInterval(Color val, Color min, Color max)
        {
            if (val.A < min.A)
                val.A = min.A;
            if (val.R < min.R)
                val.R = min.R;
            if (val.G < min.G)
                val.G = min.G;
            if (val.B < min.B)
                val.B = min.B;
            if (val.A > max.A)
                val.A = max.A;
            if (val.R > max.R)
                val.R = max.R;
            if (val.G > max.G)
                val.G = max.G;
            if (val.B > max.B)
                val.B = max.B;
            return val;
        }

        /// <summary>
        /// generate color gradient between two colors
        /// </summary>
        /// <param name="color1">input color1</param>
        /// <param name="color2">input color2</param>
        /// <returns></returns>
        private Color gradientColor(Color color1, Color color2, bool mixColor, int x1, int y1, int x2, int y2)
        {
            if (!mixColor)
            {
                color1.R = (byte)((color1.R + color2.R) / 2);
                color1.G = (byte)((color1.G + color2.G) / 2);
                color1.B = (byte)((color1.B + color2.B) / 2);
                return color1;
            }
            else
            {
                int m = rand.Next(2);
                int retVal = 0;

                if (m == 0)
                    retVal = (int)(rand.Next(Math.Abs(x2 - x1) + Math.Abs(y2 - y1)) * 0.3);
                else
                    retVal = -(int)(rand.Next(Math.Abs(x2 - x1) + Math.Abs(y2 - y1)) * 0.3);

                color1.R = (byte)((color1.R + color2.R + (byte)retVal) / 2);
                color1.G = (byte)((color1.G + color2.G + (byte)retVal) / 2);
                color1.B = (byte)((color1.B + color2.B + (byte)retVal) / 2);
                //color1.A = (byte)((color1.A + color2.A + (byte)retVal) / 2);
                return color1;
            }
        }

        /// <summary>
        /// return random number based on the size of rectangle area
        /// </summary>
        /// <param name="x1">x coord of 1st rectangle point</param>
        /// <param name="y1">y coord of 1st rectangle point</param>
        /// <param name="x2">x coord of 2nd rectangle point</param>
        /// <param name="y2">y coord of 2nd rectangle point</param>
        /// <returns></returns>

        private void divideMap(Color[] colMap, int width, int height, int x1, int y1, int x2, int y2)
        {
            // procedure put into map 5 points
            //
            // 1 - mid [X1,Y1][X2,Y1], assigned color will be (Cl1+Cl2) div 2
            // 2 - mid [X1,Y2][X2,Y2], assigned color will be (Cl3+Cl4) div 2
            // 3 - mid [X1,Y1][X1,Y2], assigned color will be (Cl1+Cl3) div 2
            // 4 - mid [X2,Y1][X2,Y2], assigned color will be (Cl2+Cl4) div 2
            // 5 - mid of rectangle, assigned color will be (1+2+3+4) div 4 + getRandRect
            //
            // [X1,Y1]       [X2,Y1]
            // Cl1-------1-------Cl2
            //   |       |       |
            //   |       |       |
            //   3-------5-------4
            //   |       |       |
            //   |       |       |
            // Cl3-------2-------Cl4
            // [X1,Y2]       [X2,Y2]
            // then its recursivly call itself to all 4 small rectangles

            Color cl1, cl2, cl3, cl4;   // coord in corners of rectangle
            Color c1, c2, c3, c4;       // colors in middles of sides
            int nx, ny;                 // coords of middle of rectangle

            /*
             * http://stackoverflow.com/questions/4517818/how-do-i-plot-individual-pixels-using-the-xna-apis
             * http://www-cs-students.stanford.edu/~amitp/game-programming/polygon-map-generation/#source
             * http://gamedev.stackexchange.com/questions/25018/apply-vertex-colors-to-xna-spritebatch-sprites
             *
             */

            // read values of colors in corners of rectangle
            cl1 = colMap[y1 * width + x1];
            cl2 = colMap[y1 * width + x2];
            cl3 = colMap[y2 * width + x1];
            cl4 = colMap[y2 * width + x2];

            // if rectangle is smaller then 2x2 function should quit
            if ((Math.Abs(x2 - x1) < 2) && Math.Abs(y2 - y1) < 2)
                return;

            // middle of rectangle
            nx = (int)Math.Round((double)((x1 + x2) / 2));
            ny = (int)Math.Round((double)((y1 + y2) / 2));

            // middles of rectangles sides
            c1 = gradientColor(cl1, cl2, false, 0, 0, 0, 0);
            c2 = gradientColor(cl3, cl4, false, 0, 0, 0, 0);
            c3 = gradientColor(cl1, cl3, false, 0, 0, 0, 0);
            c4 = gradientColor(cl2, cl4, false, 0, 0, 0, 0);

            colMap[y1 * width + nx] = c1;
            colMap[y2 * width + nx] = c2;
            colMap[ny * width + x1] = c3;
            colMap[ny * width + x2] = c4;

            // middle of rectangle (ff) is random number, smaller if rectangle is smaller
            Color minc = new Color(0, 0, 0);
            Color maxc = new Color(255, 255, 255);
            colMap[ny * width + nx] = setToInterval(gradientColor(gradientColor(cl1, cl2, false, 0, 0, 0, 0), gradientColor(cl3, cl4, true, x1, y1, x2, y2), false, 0, 0, 0, 0), minc, maxc);

            //recursion (will ensure to fill whole rectangle)
            divideMap(colMap, width, height, x1, y1, nx, ny);
            divideMap(colMap, width, height, nx, y1, x2, ny);
            divideMap(colMap, width, height, x1, ny, nx, y2);
            divideMap(colMap, width, height, nx, ny, x2, y2);
        }
    }
}