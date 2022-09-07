using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Wygaszenia
{
    public static class Flags
    {

        public static bool MenuGraphisc = true;
        public static bool MenuCell = true;
        public static bool MenuGroup = true;
        public static bool MenuNrCells = true;
        public static bool MenuNrCellsRec = true;
        public static bool MenuSettingsRec = true;
        public static bool MenuAtoms = true;


        public static int AtomsToCalculate = 1;
        public static float ballSize = 1f;
        public static float ballSizeRec = 1f;
        public static bool Log_scale = false;//Rec space
        public static bool only_visible = false;//Recspace

        public static Color Col_real;//Recspace
        public static Color Col_rec;//Recspace


        public static void Save()
        {
            string data = "MenuGraphisc " + MenuGraphisc + "\n\r" +
                "MenuCell " + MenuCell + "\n\r" +
                "MenuGroup " + MenuGroup + "\n\r" +
                "MenuNrCells " + MenuNrCells + "\n\r" +
                "MenuNrCellsRec " + MenuNrCellsRec + "\n\r" +
                "MenuSettingsRec " + MenuSettingsRec + "\n\r" +
                "MenuAtoms " + MenuAtoms + "\n\r" +
                "AtomsToCalculate " + AtomsToCalculate + "\n\r" +
                "ballSize " + ballSize + "\n\r" +
                "ballSizeRec " + ballSizeRec + "\n\r" +
                "Log_scale " + Log_scale + "\n\r" +
                "only_visible " + only_visible + "\n\r" +
                "Col_real " + Col_real.R + " " + Col_real.G + " " + Col_real.B + "\n\r" +
                "Col_rec " + Col_real.R + " " + Col_real.G + " " + Col_real.B + "\n\r";


            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@".\settings.txt"))
            {
                file.WriteLine(data);
            }
        }

        public static void Load()
        {
            if (!File.Exists(@".\settings.txt"))
                return;

                string[] lines = File.ReadAllLines(@".\settings.txt");

            // Display the file contents by using a foreach loop.
            System.Console.WriteLine("Contents of WriteLines2.txt = ");
            foreach (string line in lines)
            {
                // Use a tab to indent each line of the file.
                // Console.WriteLine("\t" + line);
                var result = Regex.Split(line, " ");
                if (result!=null)
                interpret(result);
            }

        }
        public static void interpret(string[] nst)
        {
            if (nst[0] == "MenuGraphisc")
                if (nst[1] == "True") MenuGraphisc = true;
            else
                    MenuGraphisc = false;

            else if(nst[0] == "MenuCell")
                if (nst[1] == "True") MenuCell = true;
                else
                    MenuCell = false;

            else if(nst[0] == "MenuGroup")
                if (nst[1] == "True") MenuGroup = true;
                else
                    MenuGroup = false;

            else if(nst[0] == "MenuNrCells")
                if (nst[1] == "True") MenuNrCells = true;
                else
                    MenuNrCells = false;

            else if(nst[0] == "MenuNrCellsRec")
                if (nst[1] == "True") MenuNrCellsRec = true;
                else
                    MenuNrCellsRec = false;


            else if(nst[0] == "MenuAtoms")
                if (nst[1] == "True") MenuAtoms = true;
                else
                    MenuAtoms = false;

            else if(nst[0] == "AtomsToCalculate")
                AtomsToCalculate = int.Parse(nst[1]);

            else if(nst[0] == "ballSize")
                ballSize = float.Parse(nst[1]);

            else if(nst[0] == "ballSizeRec")
                ballSizeRec = float.Parse(nst[1], CultureInfo.InvariantCulture);

            else if(nst[0] == "Log_scale")
                if (nst[1] == "True") Log_scale = true;
                else
                    Log_scale = false;

            else if(nst[0] == "only_visible")
                if (nst[1] == "True") only_visible = true;
                else
                    only_visible = false;

            else if(nst[0] == "Col_real")
            {
                Col_real.R = byte.Parse(nst[1]);
                Col_real.G = byte.Parse(nst[2]);
                Col_real.B = byte.Parse(nst[3]);
            }

            else if(nst[0] == "Col_rec")
            {
                Col_rec.R = byte.Parse(nst[1]);
                Col_rec.G = byte.Parse(nst[2]);
                Col_rec.B = byte.Parse(nst[3]);
            }
        }
    }


}
