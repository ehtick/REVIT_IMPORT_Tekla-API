using System;
using System.Windows.Forms;

using Tekla.Structures;

using Tekla.Structures.Model;

using Tekla.Structures.Model.UI;

using Tekla.Structures.Geometry3d;

using Tekla.Structures.Model.Operations;

using TSM = Tekla.Structures.Model;
using TSMUI = Tekla.Structures.Model.UI;
using TSG = Tekla.Structures.Geometry3d;

using System.Collections.Generic;
using System.Collections;
using Tekla.Structures.Solid;

namespace REVIT_IMPORT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Material apply for all item
        private static readonly string projectMaterial = "C32/40";

        private void btn_Create_Click(object sender, EventArgs e)
        {
            try
            {
                Model model = new Model();
                if (model.GetConnectionStatus())
                {
                    Console.WriteLine("Model Connected!");
                }
                else
                {
                    Console.WriteLine("Fail to connect!");
                }

                #region Input

                /* Can detect orientation cua model xem cai nao la top, bot
                 * App sẽ vẽ từ Bottom beam
                 *
                 * Đang kỳ vọng Trường sẽ xuất được điểm Lower start point của bottom beam.
                 *
                 * Chưa xử lý case có Middle Beam do thiếu input
                 *
                 *
                 */

                /*Issue: ko chắc về cao độ dưới của dầm. Thông thường sẽ là -225 so với FFL nhưng sẽ thay đổi
                 * nếu Beam Height !== 205
                 * Hiện tại sẽ xử lý module typical với Beam Height = 205
                 */

                //Bot Beam - default input
                string botBeamName = "TB1";
                double botBeamWidth = 200.0;
                double botBeamHeight = 205.0;
                string botBeamProfile = $"{botBeamHeight}*{botBeamWidth}"; // "205*200"
                Point botBeamSP = new Point(0.0, 2000.0, -20.0);
                Point botBeamEP = new Point(1880.0, 2000.0, -20.0);

                //Bot Beam - handle input
                double botZValue = createZValue(botBeamHeight);
                Point _botBeamSP = new Point(botBeamSP.X, 2000.0, botZValue);  //Tọa độ Y ko dùng theo mặc định nhưng chưa xử lý
                Point _botBeamEP = new Point(botBeamSP.X, 2000.0, botZValue);  //Tọa độ Y ko dùng theo mặc định nhưng chưa xử lý
                Point botBeamSPLower = new Point(0.0, 1900.0, botZValue);
                Point botBeamEPLower = new Point(1880.0, 1900.0, botZValue);

                //Left Beam - default input
                string leftBeamName = "TB3";
                double leftBeamWidth = 200.0;
                double leftBeamHeight = 205.0;
                string leftBeamProfile = $"{leftBeamHeight}*{leftBeamWidth}";
                double leftBeamLength = 3990;
                Point leftBeamSP = new Point(0.0, 2000.0, -20.0);
                Point leftBeamEP = new Point(1880.0, 2000.0, -20.0);

                //Left Beam - handle input
                double leftZValue = createZValue(leftBeamHeight);
                Point _leftBeamSP = new Point(botBeamSPLower.X - leftBeamWidth / 2, botBeamSPLower.Y, leftZValue);
                Point _leftBeamEP = new Point(botBeamSPLower.X - leftBeamWidth / 2, botBeamSPLower.Y + leftBeamLength, leftZValue);

                //Right Beam - default input
                string rightBeamName = "TB4";
                double rightBeamWidth = 200.0;
                double rightBeamheight = 205.0;
                string rightBeamProfile = $"{rightBeamheight}*{rightBeamWidth}";
                double rightBeamLength = 3990;
                Point rightBeamSP = new Point(0.0, 2000.0, -20.0);
                Point rightBeamEP = new Point(1880.0, 2000.0, -20.0);

                //Right Beam - handle input
                double rightZValue = createZValue(rightBeamheight);
                Point _rightBeamSP = new Point(botBeamEPLower.X + rightBeamWidth / 2, botBeamEPLower.Y, rightZValue);
                Point _rightBeamEP = new Point(botBeamEPLower.X + rightBeamWidth / 2, botBeamEPLower.Y + rightBeamLength, rightZValue);

                //Top Beam - default input
                string topBeamName = "TB2";
                double topBeamWidth = 200.0;
                double topBeamheight = 215;
                string topBeamProfile = $"{topBeamheight}*{topBeamWidth}";
                Point topBeamSP = new Point(0.0, 8550.0, -20.0);
                Point topBeamEP = new Point(1880.0, 8550.0, -20.0);

                //Top Beam - handle input
                double topZValue = createZValue(topBeamheight);
                Point _topBeamSP = new Point(botBeamSP.X, _leftBeamEP.Y - topBeamWidth / 2, topZValue);
                Point _topBeamEP = new Point(botBeamEP.X, _leftBeamEP.Y - topBeamWidth / 2, topZValue);

                //Slab

                /* Input la List<List<Point>>: 2 truong hop la List chua nhieu ListPoint. Trong ListPoint chua Point
                 * Ưu tiên xử lý List<Point> trước. Viết method để convert sau
                 *
                 */
                List<Point> slabPointsList1 = new List<Point> { new Point(0.0, 2100, -80), new Point(1880.00, 2100, -80), new Point(1880.00, 3940.00, -80), new Point(0.0, 3940.00, -80) };
                List<Point> slabPointsList2 = new List<Point> { new Point(-200.00, 3940.00, -10.00), new Point(2080.0, 3940.00, -10.0), new Point(2080.0, 5690.0, -10.0), new Point(-200.0, 5690.0, -10.0) };
                Hashtable slab1 = new Hashtable();
                slab1.Add("pointsList", slabPointsList1);
                slab1.Add("thickness", "65");
                Hashtable slab2 = new Hashtable();
                slab2.Add("pointsList", slabPointsList2);
                slab2.Add("thickness", "135");

                //Create List of hashtable. Each hashtable contain a slab's info
                List<Hashtable> slabs = new List<Hashtable>() { slab1, slab2 };

                #endregion Input

                beamCreate(botBeamName, botBeamProfile, projectMaterial, botBeamSP, botBeamEP);
                beamCreate(topBeamName, topBeamProfile, projectMaterial, _topBeamSP, _topBeamEP);
                beamCreate(leftBeamName, leftBeamProfile, projectMaterial, _leftBeamSP, _leftBeamEP);
                beamCreate(rightBeamName, rightBeamProfile, projectMaterial, _rightBeamSP, _rightBeamEP);

                //Create multi slabs
                foreach (Hashtable slab in slabs)
                {
                    string slabThickness = (string)slab["thickness"];
                    List<Point> slabPointsList = (List<Point>)slab["pointsList"];
                    slabCreate(slabThickness, projectMaterial, slabPointsList);
                }
                //Check intersection between slab and Beam
                ModelObjectEnumerator slabEnum = model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.CONTOURPLATE);
                while (slabEnum.MoveNext())
                {
                    ContourPlate slab = slabEnum.Current as ContourPlate;
                    Solid slabSolid = slab.GetSolid();
                    ModelObjectEnumerator beamEnum = model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);
                    while (beamEnum.MoveNext())
                    {
                        BooleanPart cut = new BooleanPart();
                        //Missing class. check sau
                        cut.Father = beamEnum.Current;
                        cut.SetOperativePart(slab);
                        if (!cut.Insert())
                        {
                            MessageBox.Show("Inter failed");
                        }
                        //slab.Delete();
                    }
                }

                model.CommitChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private static void beamCreate(string beamName, string beamProfile, string beamMaterial, TSG.Point beamCreateSP, TSG.Point beamCreateEP)
        {
            Beam beamCreate = new Beam(Beam.BeamTypeEnum.BEAM);
            beamCreate.Name = beamName;
            beamCreate.Profile.ProfileString = beamProfile;
            beamCreate.Material.MaterialString = beamMaterial;
            beamCreate.Class = "6";
            beamCreate.StartPoint = beamCreateSP;
            beamCreate.EndPoint = beamCreateEP;
            beamCreate.Insert();
        }

        private static void slabCreate(string slabThickness, string slabMaterial, List<Point> slabPointsList)
        {
            ContourPlate slab = new ContourPlate();

            foreach (Point slabPoint in slabPointsList)
            {
                slab.AddContourPoint(new ContourPoint(slabPoint, null));
            }
            slab.Name = $"SLAB-{slabThickness}";
            slab.Profile.ProfileString = slabThickness;
            slab.Material.MaterialString = slabMaterial;
            slab.Position.Depth = Position.DepthEnum.BEHIND;
            slab.Class = "11";
            slab.Insert();
        }

        private static double createZValue(double beamHeight)
        {
            switch (beamHeight)
            {
                case 205.0:
                    return -20.0;

                case 215.0:
                    return -10.0;

                //Các case này chưa nghiên cứu kỹ. Hiện đang follow rule của PPVC04.
                // => Cần check lại và bổ sung thêm case
                case 225.0:
                    return -50.0;

                case 255.0:
                    return -20.0;

                default:
                    return -20.0;
            }
        }

        private static List<ContourPlate> getAllSlabs(Model model)
        {
            ModelObjectEnumerator slabEnum = model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.CONTOURPLATE);
            List<ContourPlate> slabsList = new List<ContourPlate>();
            while (slabEnum.MoveNext())
            {
                slabsList.Add(slabEnum.Current as ContourPlate);
            }
            return slabsList;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Model model = new Model();

            Beam beam1 = new Beam(new Point(1000.0, 1000.0, 0.0), new Point(1000.0, 2000.0, 0.0));
            Beam beam2 = new Beam(new Point(500.0, 1500.0, 250.0), new Point(1500.0, 1500.0, 250.0));
            BooleanPart cut = new BooleanPart { Father = beam1 };
            cut.SetOperativePart(beam2);

            Solid solid1 = beam1.GetSolid(Solid.SolidCreationTypeEnum.RAW);
            Solid solid2 = beam1.GetSolid(Solid.SolidCreationTypeEnum.NORMAL);

            ShellEnumerator shells = solid1.GetCutPart(solid2);

            int shellCount = 0;
            List<int> faceCounts = new List<int>();

            while (shells.MoveNext())
            {
                var shell = shells.Current as Shell;
                if (shell != null)
                {
                    FaceEnumerator faces = shell.GetFaceEnumerator();
                    faceCounts.Insert(shellCount, 0);
                    while (faces.MoveNext())
                    {
                        faceCounts[shellCount]++;
                    }
                }
                shellCount++;
            }
        }
    }
}