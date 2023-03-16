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

using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using Tekla.Structures.Solid;
using WHExtensionElement;
using System.Runtime.Remoting.Messaging;
using System.Reflection;

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
                 * Chưa xử lý case có Middle Beam do thiếu input
                 *
                 *
                 */
                string moduleArea = cboModuleArea.SelectedItem.ToString();

                #region Beam Input

                string beamData = File.ReadAllText(@"..\..\Data\Beam.json");
                ICollection<WhBeam> whBeamList = JsonConvert.DeserializeObject<ICollection<WhBeam>>(beamData);

                #region Create Center Point of module

                List<TSG.Point> pointList = new List<TSG.Point>();
                foreach (WhBeam beamDetail in whBeamList)
                {
                    Point beamSP = new Point(beamDetail.StartPoint.X, beamDetail.StartPoint.Y, beamDetail.StartPoint.Z);
                    Point beamEP = new Point(beamDetail.EndPoint.X, beamDetail.EndPoint.Y, beamDetail.EndPoint.Z);

                    pointList.Add(beamSP);
                    pointList.Add(beamEP);
                }
                TSG.Point centerPoint = CreateCenterPoint(pointList);

                #endregion Create Center Point of module

                foreach (WhBeam beamDetail in whBeamList)
                {
                    Point beamSP = new Point(beamDetail.StartPoint.X, beamDetail.StartPoint.Y, beamDetail.StartPoint.Z + Convert.ToDouble(beamDetail.h) / 2 - 29500.00);
                    Point beamEP = new Point(beamDetail.EndPoint.X, beamDetail.EndPoint.Y, beamDetail.EndPoint.Z + Convert.ToDouble(beamDetail.h) / 2 - 29500.00);

                    /*
                     * Truong hop can xu ly Z axis
                    double beamTopLevel = createZValue(Convert.ToDouble(beamDetail.h));
                    Point beamSP = new Point(beamDetail.StartPoint.X, beamDetail.StartPoint.Y, beamTopLevel);
                    Point beamEP = new Point(beamDetail.EndPoint.X, beamDetail.EndPoint.Y, beamTopLevel);
                    */

                    Beam beamMain = beamCreate(beamDetail.name, beamDetail.h, beamDetail.b, beamDetail.type, projectMaterial, beamSP, beamEP, moduleArea, centerPoint);
                    Point beamPCStartPoint = new Point(-1927.42, 3252.39, -20);
                    Point beamPCEndPoint = new Point(-1927.42, 4382.39, -20);
                    createBeamPartCut(beamMain, 60.0, 200.0, beamPCStartPoint, beamPCEndPoint);

                    //string beamLength = beamDetail.length;   -wait for Beam's Length
                }

                #endregion Beam Input

                #region Slab Input

                string slabData = File.ReadAllText(@"..\..\Data\MockSlab.json");
                ICollection<WhSlab> whSlabList = JsonConvert.DeserializeObject<ICollection<WhSlab>>(slabData);
                foreach (WhSlab slabDetail in whSlabList)
                {
                    List<TSG.Point> slabPointBoundaryList = new List<Point>();
                    double slabThickness = slabDetail.ThickNess;
                    slabPointBoundaryList.Add(new Point(slabDetail.SketchOut[0].CurvePoint[0].X, slabDetail.SketchOut[0].CurvePoint[0].Y, slabDetail.SketchOut[0].CurvePoint[0].Z - 29500.00));
                    foreach (WhCurve slabSketchOut in slabDetail.SketchOut)
                    {
                        slabPointBoundaryList.Add(new Point(slabSketchOut.CurvePoint[1].X, slabSketchOut.CurvePoint[1].Y, slabSketchOut.CurvePoint[1].Z - 29500.00));
                    }
                    ContourPlate slab = slabCreate(slabThickness, projectMaterial, slabPointBoundaryList);

                    createPartCutWithBeam(model, slab);
                }

                #endregion Slab Input

                #region Wall Input

                //string wallData = File.ReadAllText(@"..\..\Data\MockWall.json");
                Point wallSP = new Point(-1872.42, 6252.39, -225.00);
                Point wallEP = new Point(-1872.42, 8122.39, -225.00);
                Beam wall = createWall("PANEL", "2985", "90", wallSP, wallEP);
                createPartCutWithBeam(model, wall);
                /*
                 * In future I want to just check 1 or 2 beams relate to wall, not loop through all beam
                 */

                #endregion Wall Input

                #region Old Input

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
                Hashtable slab1 = new Hashtable
                {
                    { "pointsList", slabPointsList1 },
                    { "thickness", "65" }
                };
                Hashtable slab2 = new Hashtable
                {
                    { "pointsList", slabPointsList2 },
                    { "thickness", "135" }
                };

                //Create List of hashtable. Each hashtable contain a slab's info
                List<Hashtable> slabs = new List<Hashtable>() { slab1, slab2 };

                #endregion Old Input

                #endregion Input

                model.CommitChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static Beam beamCreate(string beamName, string beamHeight, string beamWidth, string beamMark, string beamMaterial,
            TSG.Point beamSP, TSG.Point beamEP, string moduleArea, Point centerPoint)
        {
            Beam beam = new Beam(Beam.BeamTypeEnum.BEAM);
            beam.Name = beamName;
            beam.Profile.ProfileString = $"{beamHeight}*{beamWidth}";
            beam.Material.MaterialString = beamMaterial;
            beam.Class = "6";
            beam.StartPoint = beamSP;
            beam.EndPoint = beamEP;
            beam.Insert();

            #region check beam's orientation

            double gapX = beamSP.X - beamEP.X;
            double gapY = beamSP.Y - beamEP.Y;
            bool isGapXExist = true;
            bool isGapYExist = true;
            string beamOrientation = null;
            string beamPosition = null;

            if (gapX >= 0.0 && gapX < 1.0)
            {
                isGapXExist = false;
            }

            if (gapY >= 0.0 && gapY < 1.0)
            {
                isGapYExist = false;
            }

            if (isGapXExist)
            {
                beamOrientation = "hor";
            }

            if (isGapYExist)
            {
                beamOrientation = "ver";
            }

            switch (beamOrientation)
            {
                case "hor":
                    if (beamSP.Y > centerPoint.Y)
                    {
                        beamPosition = "top";
                    }
                    else
                    {
                        beamPosition = "bottom";
                    }
                    break;

                case "ver":
                    if (beamSP.X > centerPoint.X)
                    {
                        beamPosition = "right";
                    }
                    else
                    {
                        beamPosition = "left";
                    }
                    break;

                default:
                    break;
            }
            beam.SetUserProperty("comment", beamPosition);

            #endregion check beam's orientation

            beam.SetUserProperty("USER_FIELD_2", beamMark);
            Console.WriteLine("beam is created!");
            return beam;
        }

        private static ContourPlate slabCreate(double slabThickness, string slabMaterial, List<Point> slabPointsList)
        {
            ContourPlate slab = new ContourPlate();

            foreach (Point slabPoint in slabPointsList)
            {
                slab.AddContourPoint(new ContourPoint(slabPoint, null));
            }
            slab.Name = $"SLAB-{slabThickness}";
            slab.Profile.ProfileString = slabThickness.ToString();
            slab.Material.MaterialString = slabMaterial;
            slab.Position.Depth = Position.DepthEnum.BEHIND;
            slab.Class = "11";
            slab.Insert();
            return slab;
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

        //PC ~ Part Cut
        private static void createBeamPartCut(Beam beamPCFather, double beamPCHeight, double beamPCWidth, Point beamPCStartPoint, Point beamPCEndPoint)
        {
            Beam beamPC = new Beam();
            beamPC.Profile.ProfileString = $"{beamPCHeight}*{beamPCWidth}";
            beamPC.Class = BooleanPart.BooleanOperativeClassName;
            beamPC.StartPoint = beamPCStartPoint;
            beamPC.EndPoint = beamPCEndPoint;
            beamPC.Insert();
            BooleanPart beamBooleanPart = new BooleanPart();
            beamBooleanPart.Father = beamPCFather;
            beamBooleanPart.SetOperativePart(beamPC);
            beamBooleanPart.Insert();
            beamPC.Delete();
        }

        private static void createPartCutIntersection(Part partToBeCut, Part partShapeCut)
        {
            string tempBeamClass = partShapeCut.Class;
            partShapeCut.Class = BooleanPart.BooleanOperativeClassName;
            BooleanPart booleanPart = new BooleanPart();
            booleanPart.Father = partToBeCut;
            booleanPart.SetOperativePart(partShapeCut);
            booleanPart.Insert();
            partShapeCut.Class = tempBeamClass;
        }

        private static void createPartCutWithBeam(Model model, Part partToBeCut)
        {
            ModelObjectEnumerator beamEnum = model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);
            OBB partToBeCutObb = createOBB(model, partToBeCut);
            while (beamEnum.MoveNext())
            {
                Beam beam = beamEnum.Current as Beam;
                OBB beamObb = createOBB(model, beam);
                if (partToBeCutObb.Intersects(beamObb))
                {
                    createPartCutIntersection(partToBeCut, beam);
                }
            }
        }

        private static OBB createOBB(Model model, Part part)
        {
            OBB obb = null;
            if (part != null)
            {
                WorkPlaneHandler workPlaneHandler = model.GetWorkPlaneHandler();
                TransformationPlane originalTransformationPlane = workPlaneHandler.GetCurrentTransformationPlane();

                Solid solid = part.GetSolid();
                Point minPointInCurrentPlane = solid.MinimumPoint;
                Point maxPointInCurrentPlane = solid.MaximumPoint;

                Point centerPoint = CalculateCenterPoint(minPointInCurrentPlane, maxPointInCurrentPlane);

                CoordinateSystem coordSys = part.GetCoordinateSystem();
                TransformationPlane localTransformationPlane = new TransformationPlane(coordSys);
                workPlaneHandler.SetCurrentTransformationPlane(localTransformationPlane);

                solid = part.GetSolid();
                Point minPoint = solid.MinimumPoint;
                Point maxPoint = solid.MaximumPoint;
                double extent0 = (maxPoint.X - minPoint.X) / 2;
                double extent1 = (maxPoint.Y - minPoint.Y) / 2;
                double extent2 = (maxPoint.Z - minPoint.Z) / 2;

                workPlaneHandler.SetCurrentTransformationPlane(originalTransformationPlane);

                obb = new OBB(centerPoint, coordSys.AxisX, coordSys.AxisY,
                               coordSys.AxisX.Cross(coordSys.AxisY), extent0, extent1, extent2);
            }

            return obb;
        }

        private static Point CalculateCenterPoint(Point min, Point max)
        {
            double x = min.X + ((max.X - min.X) / 2);
            double y = min.Y + ((max.Y - min.Y) / 2);
            double z = min.Z + ((max.Z - min.Z) / 2);

            return new Point(x, y, z);
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

        private static Beam createWall(string wallName, string wallHeight, string wallThickness, Point wallSP, Point wallEP)
        {
            Beam wall = new Beam(Beam.BeamTypeEnum.PANEL);
            wall.Name = wallName;
            wall.Profile.ProfileString = $"{wallHeight}*{wallThickness}";
            wall.Material.MaterialString = projectMaterial;
            wall.StartPoint = wallSP;
            wall.EndPoint = wallEP;
            if (wall.Name == "PANEL")
            {
                wall.Class = "1";
            }
            else
            {
                wall.Class = "5";
            }
            wall.Position.Depth = Position.DepthEnum.FRONT;
            wall.Insert();
            return wall;
        }

        private static TSG.Point CreateCenterPoint(List<TSG.Point> pointList)
        {
            double XPoint = 0;
            double YPoint = 0;
            pointList.ForEach(point =>
            {
                XPoint += point.X;
                YPoint += point.Y;
            });
            TSG.Point centerPoint =
                new TSG.Point(XPoint / pointList.Count, YPoint / pointList.Count, 500.0);

            ControlPoint controlPoint = new ControlPoint(centerPoint);
            controlPoint.Insert();
            return centerPoint;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Model model = new Model();
            OBB obb = null;

            // In this simplified example, there are two existing beams in the model.
            ModelObjectEnumerator beamsEnumerator =
                model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);

            if (beamsEnumerator != null)
            {
                while (beamsEnumerator.MoveNext())
                {
                    Beam beam = beamsEnumerator.Current as Beam;

                    if (beam != null)
                    {
                        if (obb == null)
                        {
                            obb = CreateOrientedBoundingBox(beam);
                        }
                        else
                        {
                            if (obb.Intersects(CreateOrientedBoundingBox(beam)))
                            {
                                // Boxes intersect.
                                Console.WriteLine("intersect!");
                            }
                            else
                            {
                                // Boxes did not intersect.
                            }
                        }
                    }
                }
            }
            Point CalculateCenterPoint(Point min, Point max)
            {
                double x = min.X + ((max.X - min.X) / 2);
                double y = min.Y + ((max.Y - min.Y) / 2);
                double z = min.Z + ((max.Z - min.Z) / 2);

                return new Point(x, y, z);
            }
            OBB CreateOrientedBoundingBox(Beam beam)
            {
                //OBB obb = null;
                if (beam != null)
                {
                    WorkPlaneHandler workPlaneHandler = model.GetWorkPlaneHandler();
                    TransformationPlane originalTransformationPlane = workPlaneHandler.GetCurrentTransformationPlane();

                    Solid solid = beam.GetSolid();
                    Point minPointInCurrentPlane = solid.MinimumPoint;
                    Point maxPointInCurrentPlane = solid.MaximumPoint;

                    Point centerPoint = CalculateCenterPoint(minPointInCurrentPlane, maxPointInCurrentPlane);

                    CoordinateSystem coordSys = beam.GetCoordinateSystem();
                    TransformationPlane localTransformationPlane = new TransformationPlane(coordSys);
                    workPlaneHandler.SetCurrentTransformationPlane(localTransformationPlane);

                    solid = beam.GetSolid();
                    Point minPoint = solid.MinimumPoint;
                    Point maxPoint = solid.MaximumPoint;
                    double extent0 = (maxPoint.X - minPoint.X) / 2;
                    double extent1 = (maxPoint.Y - minPoint.Y) / 2;
                    double extent2 = (maxPoint.Z - minPoint.Z) / 2;

                    workPlaneHandler.SetCurrentTransformationPlane(originalTransformationPlane);

                    obb = new OBB(centerPoint, coordSys.AxisX, coordSys.AxisY,
                                   coordSys.AxisX.Cross(coordSys.AxisY), extent0, extent1, extent2);
                }

                return obb;
            }

            model.CommitChanges();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cboModuleArea.SelectedIndex = 1;
        }
    }
}