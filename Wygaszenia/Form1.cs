using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Xna.Framework;



namespace Wygaszenia
{
    public partial class Wygaszenia : Form
    {
        Window windowReal;
        Window windowRec;


        public static List<object3d> CellAtoms = new List<object3d>();//atoms expanded by the symmetry elements
        public static List<object3d> AllAtoms = new List<object3d>();//atoms expanded by the symmetry elements
        public static List<object3d> UnitCellFrame = new List<object3d>();//atoms expanded by the symmetry elements
        public static List<object3d> RecCellFrame = new List<object3d>();//atoms expanded by the symmetry elements
        public static List<object3d> rec_points = new List<object3d>();//atoms expanded by the symmetry elements


        float[,] AtomPos = new float[,] { { 0.1f, 0.05f, 0.05f }, { 0.05f, 0.1f, 0.05f }, { 0.05f, 0.05f, 0.1f } }; //Position of atom in irreducible part of the unit cell



        public Wygaszenia()
        {
            InitializeComponent();

            windowReal = new Window(panel1);
            windowReal.Update();
            windowReal.Render();

            windowRec = new Window(panel6);
            windowRec.Update();
            windowRec.Render();
        }



        //Size correction for all
        public void SizeCorrection()
        {
            if (windowReal != null)
                windowReal.SizeChange();
            windowReal.Render();

            if (windowRec != null)
                windowRec.SizeChange();
            windowRec.Render();

        }

        private void Wygaszenia_FormClosing(object sender, FormClosingEventArgs e)
        {
            windowReal.service.ResetingDevice();
            windowRec.service.ResetingDevice();

            Flags.Col_real = windowReal.GetBackground();
            Flags.Col_rec = windowReal.GetBackground();
            Flags.Save();
        }

        private void Wygaszenia_Shown(object sender, EventArgs e)
        {
            SizeCorrection();
            windowReal.Render();
            windowRec.Render();
        }

        private void Wygaszenia_Load(object sender, EventArgs e)
        {
            Flags.Load();
            windowRec.mCamera.zoom = 100f; windowRec.mCamera.OneOverZoom = 1.0f / windowRec.mCamera.zoom;

            windowReal.OnLoadWindow();
            windowRec.OnLoadWindow();


            windowReal.Background(Flags.Col_real);
            windowRec.Background(Flags.Col_real);

            SizeCorrection();

            //Fill the comboBox
            for (int i = 0; i < SymmetryElements.SymmetryOperations.Count; i++)
            {
                string[] value = SymmetryElements.SymmetryOperations[i];
                comboBox1.Items.Add(value[2]);
                SetCell(1);
            }
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            windowReal.OnLoadWindow();
            windowReal.perspective(false);

            windowRec.OnLoadWindow();
            windowRec.perspective(false);

            SizeCorrection();
            comboBox1.SelectedIndex = 1;
            trackBar1.Value = (int)(10 * Flags.ballSize);
            trackBar2.Value = (int)(10 * Flags.ballSizeRec);

            numericUpDown1.Value = Cell.x_max;
            numericUpDown2.Value = Cell.y_max;
            numericUpDown3.Value = Cell.z_max;

            numericUpDown9.Value = Reciprocal.h_min;
            numericUpDown8.Value = Reciprocal.k_min;
            numericUpDown7.Value = Reciprocal.l_min;
            numericUpDown12.Value = Reciprocal.h_max;
            numericUpDown11.Value = Reciprocal.k_max;
            numericUpDown10.Value = Reciprocal.l_max;

            numericUpDown13.Value = Flags.AtomsToCalculate;
            Prepare();
        }


        private void Wygaszenia_SizeChanged(object sender, EventArgs e)
        {
            SizeCorrection();
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            SizeCorrection();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Prepare();
        }

        private void Prepare()
        {
            Cell.a = float.Parse(textBox1.Text);
            Cell.b = float.Parse(textBox2.Text);
            Cell.c = float.Parse(textBox3.Text);
            Cell.alp = float.Parse(textBox4.Text);
            Cell.bet = float.Parse(textBox5.Text);
            Cell.gam = float.Parse(textBox6.Text);



            if (Cell.system == "Monoclinic")
                Cell.alp = Cell.gam = 90;

            if (Cell.system == "Orthorhombic")
                Cell.alp = Cell.bet = Cell.gam = 90;

            if (Cell.system == "Tetragonal")
            {
                Cell.alp = Cell.bet = Cell.gam = 90;
                Cell.b = Cell.a;
            }

            if (Cell.system == "Trigonal" || Cell.system == "Hexagonal")
            {
                Cell.alp = Cell.bet = 90; Cell.gam = 120;
                Cell.b = Cell.a;
            }

            if (Cell.system == "Cubic")
            {
                Cell.alp = Cell.bet = Cell.gam = 90;
                Cell.c = Cell.b = Cell.a;
            }


            if (Cell.a <= 0) Cell.a = 1;
            if (Cell.b <= 0) Cell.b = 1;
            if (Cell.c <= 0) Cell.c = 1;
            if (Cell.alp <= 0) Cell.alp = 1;
            if (Cell.bet <= 0) Cell.bet = 1;
            if (Cell.gam <= 0) Cell.gam = 1;

            //Convert deg to rad
            Cell.alp = MathHelper.ToRadians(Cell.alp);
            Cell.bet = MathHelper.ToRadians(Cell.bet);
            Cell.gam = MathHelper.ToRadians(Cell.gam);
            Cell.Init();

            PrepareUnitCell();
            CalculateAtomsInCell();
            windowReal.Render();
            windowRec.Render();

        }


        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            windowReal.mCamera.MouseDown(e);
            windowReal.Update();
            windowReal.Render();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            windowReal.mCamera.MouseMove(e, windowReal.panelViewport);
            windowReal.Update();
            windowReal.Render();
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            windowReal.mCamera.MouseUp(e);
            windowReal.Update();
            windowReal.Render();
        }

        private void panel1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            windowReal.mCamera.scroll(e);
            windowReal.Update();
            windowReal.Render();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = comboBox1.SelectedIndex;
            SymmetryElements.group_number = comboBox1.SelectedIndex;
            SetCell(comboBox1.SelectedIndex);
            SymmetryElements.CreateTransformations(SymmetryElements.group_number);

            Prepare();

            CalculateAsymetricCell();
            CalculateAtomsInCell();
            windowReal.Update();
            windowReal.Render();
            windowRec.Render();
        }


        private void CalculateAsymetricCell()
        {
            //do the hard work calculate all atoms in unit cell
            int count = SymmetryElements.Vector.Count; //number of symmetry transformations
            float[] vector = new float[3];
            int[] matrix = new int[9];

            CellAtoms.Clear();
            for (int j = 0; j < Flags.AtomsToCalculate; j++)//3 basic atoms
            {
                for (int i = 0; i < count; i++)
                {
                    matrix = SymmetryElements.Matrix[i];
                    vector = SymmetryElements.Vector[i];

                    object3d atom_class = new object3d();
                    atom_class.position.X = AtomPos[j, 0] * matrix[0] + AtomPos[j, 1] * matrix[1] + AtomPos[j, 2] * matrix[2] + vector[0];
                    atom_class.position.Y = AtomPos[j, 0] * matrix[3] + AtomPos[j, 1] * matrix[4] + AtomPos[j, 2] * matrix[5] + vector[1];
                    atom_class.position.Z = AtomPos[j, 0] * matrix[6] + AtomPos[j, 1] * matrix[7] + AtomPos[j, 2] * matrix[8] + vector[2];

                    //Colors
                    if (j == 0)
                        atom_class.color = new Vector3(.9f, 0, 0);
                    if (j == 1)
                        atom_class.color = new Vector3(0, .9f, 0);
                    if (j == 2)
                        atom_class.color = new Vector3(0, 0, .9f);

                    CellAtoms.Add(atom_class);
                }


            }
        }

        private void CalculateAtomsInCell()
        {
            AllAtoms.Clear();
            //Calculate all atoms
            for (int i = Cell.x_min; i < Cell.x_max; i++)//repeat x
            {
                for (int j = Cell.y_min; j < Cell.y_max; j++)//repeat y
                {
                    for (int k = Cell.z_min; k < Cell.z_max; k++)//repeat z
                    {
                        for (int l = 0; l < CellAtoms.Count; l++)
                        {
                            object3d atom_class = new object3d();

                            atom_class.position.X = (CellAtoms[l].position.X + i) * Cell.a * Cell.Sin_alp + (CellAtoms[l].position.Y + j) * Cell.b * Cell.Cos_gam + (CellAtoms[l].position.Z + k) * Cell.c * Cell.Cos_bet * Cell.Sin_alp;
                            atom_class.position.Y = (CellAtoms[l].position.Y + j) * Cell.b * Cell.Sin_gam + (CellAtoms[l].position.Z + k) * Cell.c * Cell.Cos_alp;
                            atom_class.position.Z = (CellAtoms[l].position.Z + k) * Cell.c * Cell.Sin_bet * Cell.Sin_alp;

                            atom_class.color = CellAtoms[l].color;
                            AllAtoms.Add(atom_class);

                        }
                    }
                }
            }
            //calculate cell
            PrepareUnitCell();
            CalculateReciprocalSpace();

        }


        void CalculateReciprocalSpace()
        {
            //If we know the vectors of real space lets calculate the reciprocal one and the points
            Reciprocal.rlat_comp();
            label20.Text = "a*=" + Reciprocal.a_ + "  b*=" + Reciprocal.b_ + "  c*=" + Reciprocal.c_ + "  alp*=" + MathHelper.ToDegrees(Reciprocal.alp_) + "  bet*=" + MathHelper.ToDegrees(Reciprocal.bet_) + "  gam*=" + MathHelper.ToDegrees(Reciprocal.gam_);
            rec_points.Clear();

            for (int h = Reciprocal.h_min; h < Reciprocal.h_max; h++)//repeat h
            {
                for (int k = Reciprocal.k_min; k < Reciprocal.k_max; k++)//repeat y
                {
                    for (int l = Reciprocal.l_min; l < Reciprocal.l_max; l++)//repeat z
                    {
                        object3d rcp = new object3d();
                        rcp.position = Reciprocal.transform(h, k, l);

                        if (!Flags.Log_scale)
                            rcp.F = Reciprocal.CalculateF(h, k, l, CellAtoms);
                        else
                            rcp.F = (float)Math.Log(Reciprocal.CalculateF(h, k, l, CellAtoms));
                        if (rcp.F < 0) rcp.F = 0;
                        if (Flags.only_visible)
                            if (rcp.F > 0.0000001f) rcp.F = 1;

                        rcp.h = h;
                        rcp.k = k;
                        rcp.l = l;
                        rec_points.Add(rcp);
                    }
                }
            }

            PrepareRecCell();
        }

        private void PrepareUnitCell()
        {
            UnitCellFrame.Clear();

            //Fractional units
            object3d atom_class = new object3d();
            atom_class.position = new Vector3(0, 0, 0);
            UnitCellFrame.Add(atom_class);
            atom_class = new object3d();
            atom_class.position = new Vector3(1, 0, 0);
            UnitCellFrame.Add(atom_class);
            atom_class = new object3d();
            atom_class.position = new Vector3(1, 1, 0);
            UnitCellFrame.Add(atom_class);
            atom_class = new object3d();
            atom_class.position = new Vector3(0, 1, 0);
            UnitCellFrame.Add(atom_class);
            atom_class = new object3d();
            atom_class.position = new Vector3(0, 0, 1);
            UnitCellFrame.Add(atom_class);
            atom_class = new object3d();
            atom_class.position = new Vector3(1, 0, 1);
            UnitCellFrame.Add(atom_class);
            atom_class = new object3d();
            atom_class.position = new Vector3(1, 1, 1);
            UnitCellFrame.Add(atom_class);
            atom_class = new object3d();
            atom_class.position = new Vector3(0, 1, 1);
            UnitCellFrame.Add(atom_class);

            Cell.Init();
            float V = Cell.Volume();

            label21.Text = "V=" + V;
            float a = Cell.a;
            float b = Cell.b;
            float c = Cell.c;

            for (int i = 0; i < UnitCellFrame.Count; i++)
            {
                float cx = UnitCellFrame[i].position.X;
                float cy = UnitCellFrame[i].position.Y;
                float cz = UnitCellFrame[i].position.Z;

                UnitCellFrame[i].position.X = cx * a * Cell.Sin_alp + cy * b * Cell.Cos_gam + cz * c * Cell.Cos_bet * Cell.Sin_alp;
                UnitCellFrame[i].position.Y = cy * b * Cell.Sin_gam + cz * c * Cell.Cos_alp;
                UnitCellFrame[i].position.Z = cz * c * Cell.Sin_bet * Cell.Sin_alp;
            }

        }


        private void PrepareRecCell()
        {
            RecCellFrame.Clear();

            //Fractional units
            object3d atom_class = new object3d();
            atom_class.position = new Vector3(0, 0, 0);
            RecCellFrame.Add(atom_class);
            atom_class = new object3d();
            atom_class.position = new Vector3(1, 0, 0);
            RecCellFrame.Add(atom_class);
            atom_class = new object3d();
            atom_class.position = new Vector3(1, 1, 0);
            RecCellFrame.Add(atom_class);
            atom_class = new object3d();
            atom_class.position = new Vector3(0, 1, 0);
            RecCellFrame.Add(atom_class);
            atom_class = new object3d();
            atom_class.position = new Vector3(0, 0, 1);
            RecCellFrame.Add(atom_class);
            atom_class = new object3d();
            atom_class.position = new Vector3(1, 0, 1);
            RecCellFrame.Add(atom_class);
            atom_class = new object3d();
            atom_class.position = new Vector3(1, 1, 1);
            RecCellFrame.Add(atom_class);
            atom_class = new object3d();
            atom_class.position = new Vector3(0, 1, 1);
            RecCellFrame.Add(atom_class);


            for (int i = 0; i < RecCellFrame.Count; i++)
            {
                RecCellFrame[i].position = Reciprocal.transform(RecCellFrame[i].position.X, RecCellFrame[i].position.Y, RecCellFrame[i].position.Z);
            }
        }


        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Flags.ballSize = 0.1f * trackBar1.Value;
            windowReal.Render();
        }


        void SetCell(int index)
        {
            //chec the system
            Cell.system = SymmetryElements.RetriveSystem(index);
            label2.Text = Cell.system;

            if (Cell.system == "Triclinic")
            {
                textBox1.Text = "1"; textBox2.Text = "1"; textBox3.Text = "1";
                textBox4.Text = "50"; textBox5.Text = "70"; textBox6.Text = "110";
                textBox1.Enabled = true; textBox2.Enabled = true; textBox3.Enabled = true;
                textBox4.Enabled = true; textBox5.Enabled = true; textBox6.Enabled = true;
            }


            if (Cell.system == "Monoclinic")
            {
                textBox1.Text = "1"; textBox2.Text = "2"; textBox3.Text = "3";
                textBox4.Text = "90"; textBox5.Text = "110"; textBox6.Text = "90";
                textBox1.Enabled = true; textBox2.Enabled = true; textBox3.Enabled = true;
                textBox4.Enabled = false; textBox5.Enabled = true; textBox6.Enabled = false;
            }

            if (Cell.system == "Orthorhombic")
            {
                textBox1.Text = "1"; textBox2.Text = "2"; textBox3.Text = "3";
                textBox4.Text = "90"; textBox5.Text = "90"; textBox6.Text = "90";
                textBox1.Enabled = true; textBox2.Enabled = true; textBox3.Enabled = true;
                textBox4.Enabled = false; textBox5.Enabled = false; textBox6.Enabled = false;
            }

            if (Cell.system == "Tetragonal")
            {
                textBox1.Text = "1"; textBox2.Text = "1"; textBox3.Text = "2";
                textBox4.Text = "90"; textBox5.Text = "90"; textBox6.Text = "90";
                textBox1.Enabled = true; textBox2.Enabled = false; textBox3.Enabled = true;
                textBox4.Enabled = false; textBox5.Enabled = false; textBox6.Enabled = false;
            }

            if (Cell.system == "Trigonal")
            {
                textBox1.Text = "1"; textBox2.Text = "1"; textBox3.Text = "2";
                textBox4.Text = "90"; textBox5.Text = "90"; textBox6.Text = "120";
                textBox1.Enabled = true; textBox2.Enabled = false; textBox3.Enabled = true;
                textBox4.Enabled = false; textBox5.Enabled = false; textBox6.Enabled = false;
            }

            if (Cell.system == "Hexagonal")
            {
                textBox1.Text = "1"; textBox2.Text = "1"; textBox3.Text = "2";
                textBox4.Text = "90"; textBox5.Text = "90"; textBox6.Text = "120";
                textBox1.Enabled = true; textBox2.Enabled = false; textBox3.Enabled = true;
                textBox4.Enabled = false; textBox5.Enabled = false; textBox6.Enabled = false;
            }

            if (Cell.system == "Cubic")
            {
                textBox1.Text = "1"; textBox2.Text = "1"; textBox3.Text = "1";
                textBox4.Text = "90"; textBox5.Text = "90"; textBox6.Text = "90";
                textBox1.Enabled = true; textBox2.Enabled = false; textBox3.Enabled = false;
                textBox4.Enabled = false; textBox5.Enabled = false; textBox6.Enabled = false;
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            windowReal.perspective(checkBox1.Checked);
            windowReal.Render();
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Flags.MenuGraphisc)
            {
                button2.ImageIndex = 1;
                Flags.MenuGraphisc = false;
                panel2.Visible = false;
            }
            else
            {
                button2.ImageIndex = 0;
                Flags.MenuGraphisc = true;
                panel2.Visible = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Flags.MenuCell)
            {
                button3.ImageIndex = 1;
                Flags.MenuCell = false;
                panel3.Visible = false;
            }
            else
            {
                button3.ImageIndex = 0;
                Flags.MenuCell = true;
                panel3.Visible = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (Flags.MenuGroup)
            {
                button4.ImageIndex = 1;
                Flags.MenuGroup = false;
                panel4.Visible = false;
            }
            else
            {
                button4.ImageIndex = 0;
                Flags.MenuGroup = true;
                panel4.Visible = true;
            }
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Flags.MenuNrCells)
            {
                button5.ImageIndex = 1;
                Flags.MenuNrCells = false;
                panel7.Visible = false;
            }
            else
            {
                button5.ImageIndex = 0;
                Flags.MenuNrCells = true;
                panel7.Visible = true;
            }
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            Cell.x_min = (int)numericUpDown6.Value;
            PrepareUnitCell();

            CalculateAtomsInCell();
            windowReal.Render();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Cell.x_max = (int)numericUpDown1.Value;
            PrepareUnitCell();

            CalculateAtomsInCell();
            windowReal.Render();
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            Cell.y_min = (int)numericUpDown5.Value;
            PrepareUnitCell();

            CalculateAtomsInCell();
            windowReal.Render();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Cell.y_max = (int)numericUpDown2.Value;
            PrepareUnitCell();

            CalculateAtomsInCell();
            windowReal.Render();
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            Cell.z_min = (int)numericUpDown4.Value;
            PrepareUnitCell();

            CalculateAtomsInCell();
            windowReal.Render();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            Cell.z_max = (int)numericUpDown3.Value;
            PrepareUnitCell();

            CalculateAtomsInCell();
            windowReal.Render();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            windowReal.Background(colorDialog1.Color);// Color.FromArgb(255, 255, 0);
            windowReal.Render();
        }

        private void panel6_MouseDown(object sender, MouseEventArgs e)
        {
            windowRec.mCamera.MouseDown(e);
            windowRec.Update();
            windowRec.Render();
        }

        private void panel6_MouseUp(object sender, MouseEventArgs e)
        {
            windowRec.mCamera.MouseUp(e);
            windowRec.Update();
            windowRec.Render();
        }

        private void panel6_MouseMove(object sender, MouseEventArgs e)
        {
            windowRec.mCamera.MouseMove(e, windowRec.panelViewport);
            windowRec.Update();
            windowRec.Render();
        }

        private void panel6_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            windowRec.mCamera.scroll(e);
            windowRec.Update();
            windowRec.Render();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

            if (Flags.MenuSettingsRec)
            {
                button7.ImageIndex = 1;
                Flags.MenuSettingsRec = false;
                panel8.Visible = false;
            }
            else
            {
                button7.ImageIndex = 0;
                Flags.MenuSettingsRec = true;
                panel8.Visible = true;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {

            if (Flags.MenuNrCellsRec)
            {
                button9.ImageIndex = 1;
                Flags.MenuNrCellsRec = false;
                panel9.Visible = false;
            }
            else
            {
                button9.ImageIndex = 0;
                Flags.MenuNrCellsRec = true;
                panel9.Visible = true;
            }
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            Reciprocal.h_min = (int)numericUpDown9.Value;
            CalculateReciprocalSpace();
            windowRec.Render();
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            Reciprocal.k_min = (int)numericUpDown8.Value;
            CalculateReciprocalSpace();
            windowRec.Render();
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            Reciprocal.l_min = (int)numericUpDown7.Value;
            CalculateReciprocalSpace();
            windowRec.Render();
        }

        private void numericUpDown12_ValueChanged(object sender, EventArgs e)
        {
            Reciprocal.h_max = (int)numericUpDown12.Value;
            CalculateReciprocalSpace();
            windowRec.Render();
        }

        private void numericUpDown11_ValueChanged(object sender, EventArgs e)
        {
            Reciprocal.k_max = (int)numericUpDown11.Value;
            CalculateReciprocalSpace();
            windowRec.Render();
        }

        private void numericUpDown10_ValueChanged(object sender, EventArgs e)
        {
            Reciprocal.l_max = (int)numericUpDown10.Value;
            CalculateReciprocalSpace();
            windowRec.Render();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            Flags.ballSizeRec = 0.1f * trackBar2.Value;
            windowRec.Render();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            windowRec.Background(colorDialog1.Color);// Color.FromArgb(255, 255, 0);
            windowRec.Render();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            windowRec.perspective(checkBox2.Checked);
            windowRec.Render();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Flags.Log_scale = checkBox3.Checked;
            CalculateReciprocalSpace();
            windowRec.Render();

        }

        private void numericUpDown13_ValueChanged(object sender, EventArgs e)
        {
            Flags.AtomsToCalculate = (int)numericUpDown13.Value;
            CalculateAsymetricCell();
            Prepare();
            windowRec.Render();
            windowReal.Render();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Flags.only_visible = checkBox4.Checked;
            CalculateReciprocalSpace();
            windowRec.Render();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (Flags.MenuAtoms)
            {
                button10.ImageIndex = 1;
                Flags.MenuAtoms = false;
                panel10.Visible = false;
            }
            else
            {
                button10.ImageIndex = 0;
                Flags.MenuAtoms = true;
                panel10.Visible = true;
            }
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            windowReal.Background(colorDialog1.Color);// Color.FromArgb(255, 255, 0);
            windowReal.Render();
        }

        private void numericUpDown13_ValueChanged_1(object sender, EventArgs e)
        {
            Flags.AtomsToCalculate = (int)numericUpDown13.Value;
            CalculateAsymetricCell();
            Prepare();
            windowReal.Render();
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            windowReal.perspective(checkBox1.Checked);
            windowReal.Render();
        }

        private void trackBar1_Scroll_1(object sender, EventArgs e)
        {
            Flags.ballSize = 0.1f * trackBar1.Value;
            windowReal.Render();
        }

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            windowReal.mCamera.Rotate = Matrix.Identity * Matrix.CreateRotationX(MathHelper.Pi / 2) * Matrix.CreateRotationY(Cell.gam - MathHelper.Pi / 2);
            windowReal.Render();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            windowReal.mCamera.Rotate = Matrix.Identity * Matrix.CreateRotationY(3 * MathHelper.Pi / 2);
            windowReal.Render();
        }
        float d;
        private void button13_Click(object sender, EventArgs e)
        {

            windowReal.mCamera.Rotate = Matrix.Identity * Matrix.CreateRotationY(Cell.bet - MathHelper.Pi / 2) * Matrix.CreateRotationX(-(Cell.alp - MathHelper.Pi / 2));
            windowReal.Render();
        }

        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown14_ValueChanged(object sender, EventArgs e)
        {
            d = (int)numericUpDown14.Value;
            label19.Text = "" + d;
            float Pi = MathHelper.Pi;

            windowRec.mCamera.Rotate = Matrix.Identity * Matrix.CreateRotationX(Reciprocal.alp_ - MathHelper.Pi / 2) * Matrix.CreateRotationY(Reciprocal.bet_ - MathHelper.Pi / 2) * Matrix.CreateRotationX(d * Pi / 18);
            windowRec.Render();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            windowRec.mCamera.Rotate = Matrix.Identity * Matrix.CreateRotationY(3 * MathHelper.Pi / 2);
            windowRec.Render();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            windowRec.mCamera.Rotate = Matrix.Identity * Matrix.CreateRotationX(MathHelper.Pi / 2) * Matrix.CreateRotationY(MathHelper.Pi / 2 + Reciprocal.gam_) * Matrix.CreateRotationX(MathHelper.Pi) * Matrix.CreateRotationZ(MathHelper.Pi);
            windowRec.Render();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            windowRec.mCamera.Rotate = Matrix.Identity * Matrix.CreateRotationX(Reciprocal.alp_ - MathHelper.Pi / 2) * Matrix.CreateRotationY(Reciprocal.bet_ - MathHelper.Pi / 2);
            windowRec.Render();
        }



        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Reciprocal.h_min = (int)numericUpDown9.Value;
            Reciprocal.k_min = (int)numericUpDown8.Value;
            Reciprocal.l_min = (int)numericUpDown7.Value;
            Reciprocal.h_max = (int)numericUpDown12.Value;
            Reciprocal.k_max = (int)numericUpDown11.Value;
            Reciprocal.l_max = (int)numericUpDown10.Value;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            Reciprocal.h_min = 0; Reciprocal.h_max = 1;
            Reciprocal.k_min = -5; Reciprocal.k_max = 5;
            Reciprocal.l_min = -5; Reciprocal.l_max = 5;

            windowRec.mCamera.Rotate = Matrix.Identity * Matrix.CreateRotationY(3 * MathHelper.Pi / 2);
            CalculateReciprocalSpace();
            windowRec.Render();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            Reciprocal.h_min = 1; Reciprocal.h_max = 2;
            Reciprocal.k_min = -5; Reciprocal.k_max = 5;
            Reciprocal.l_min = -5; Reciprocal.l_max = 5;

            windowRec.mCamera.Rotate = Matrix.Identity * Matrix.CreateRotationY(3 * MathHelper.Pi / 2);
            CalculateReciprocalSpace();
            windowRec.Render();
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            Reciprocal.h_min = -5; Reciprocal.h_max = 5;
            Reciprocal.k_min = 0; Reciprocal.k_max = 1;
            Reciprocal.l_min = -5; Reciprocal.l_max = 5;

            windowRec.mCamera.Rotate = Matrix.Identity * Matrix.CreateRotationX(MathHelper.Pi / 2) * Matrix.CreateRotationY(MathHelper.Pi / 2 + Reciprocal.gam_) * Matrix.CreateRotationX(MathHelper.Pi) * Matrix.CreateRotationZ(MathHelper.Pi);
            CalculateReciprocalSpace();
            windowRec.Render();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            Reciprocal.h_min = -5; Reciprocal.h_max = 5;
            Reciprocal.k_min = 1; Reciprocal.k_max = 2;
            Reciprocal.l_min = -5; Reciprocal.l_max = 5;

            windowRec.mCamera.Rotate = Matrix.Identity * Matrix.CreateRotationX(MathHelper.Pi / 2) * Matrix.CreateRotationY(MathHelper.Pi / 2 + Reciprocal.gam_) * Matrix.CreateRotationX(MathHelper.Pi) * Matrix.CreateRotationZ(MathHelper.Pi);
            CalculateReciprocalSpace();
            windowRec.Render();
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            Reciprocal.h_min = -5; Reciprocal.h_max = 5;
            Reciprocal.k_min = -5; Reciprocal.k_max = 5;
            Reciprocal.l_min =  0; Reciprocal.l_max = 1;

            windowRec.mCamera.Rotate = Matrix.Identity * Matrix.CreateRotationX(Reciprocal.alp_ - MathHelper.Pi / 2) * Matrix.CreateRotationY(Reciprocal.bet_ - MathHelper.Pi / 2);
            CalculateReciprocalSpace();
            windowRec.Render();
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            Reciprocal.h_min = -5; Reciprocal.h_max = 5;
            Reciprocal.k_min = -5; Reciprocal.k_max = 5;
            Reciprocal.l_min = 1; Reciprocal.l_max = 2;

            windowRec.mCamera.Rotate = Matrix.Identity * Matrix.CreateRotationX(Reciprocal.alp_ - MathHelper.Pi / 2) * Matrix.CreateRotationY(Reciprocal.bet_ - MathHelper.Pi / 2);
            CalculateReciprocalSpace();
            windowRec.Render();
        }

        private void Wygaszenia_MaximumSizeChanged(object sender, EventArgs e)
        {

        }

        private void Wygaszenia_ClientSizeChanged(object sender, EventArgs e)
        {
            SizeCorrection();
        }
    }


}


