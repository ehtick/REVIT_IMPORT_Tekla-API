using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using WHExtensionElement;

using TSG = Tekla.Structures.Geometry3d;

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

                /*
                string beamData = File.ReadAllText(@"..\..\Data\MockBeam.json");
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
                    Beam beamMain = beamCreate(beamDetail.name, beamDetail.h, beamDetail.b, beamDetail.type, projectMaterial, beamSP, beamEP, moduleArea, centerPoint);

                    /*
                     * Truong hop can xu ly Z axis
                    double beamTopLevel = createZValue(Convert.ToDouble(beamDetail.h));
                    Point beamSP = new Point(beamDetail.StartPoint.X, beamDetail.StartPoint.Y, beamTopLevel);
                    Point beamEP = new Point(beamDetail.EndPoint.X, beamDetail.EndPoint.Y, beamTopLevel);

                if (beamDetail.DropPoint != null)
                {
                    //deconstruction tuple
                    (WhPoint whPoint1, WhPoint whPoint2, WhPoint whPoint3, WhPoint whPoint4) = beamDetail.DropPoint;
                    Point point1 = new Point(whPoint1.X, whPoint1.Y, whPoint1.Z - 29500.0);
                    Point point2 = new Point(whPoint2.X, whPoint2.Y, whPoint2.Z - 29500.0);
                    Point point3 = new Point(whPoint3.X, whPoint3.Y, whPoint3.Z - 29500.0);
                    Point point4 = new Point(whPoint4.X, whPoint4.Y, whPoint4.Z - 29500.0);
                    ControlPoint controlPoint1 = new ControlPoint(point1);
                    ControlPoint controlPoint2 = new ControlPoint(point2);
                    ControlPoint controlPoint3 = new ControlPoint(point3);
                    ControlPoint controlPoint4 = new ControlPoint(point4);
                    controlPoint1.Insert();
                    controlPoint2.Insert();
                    controlPoint3.Insert();
                    controlPoint4.Insert();

                    #region find dropBeamShape profile

                    List<double> dropPointZ = new List<double>() { point1.Z, point2.Z, point3.Z, point4.Z };
                    double dropHeight = dropPointZ.Max() - dropPointZ.Min();
                    List<double> dropPointX = new List<double>() { point1.X, point2.X, point3.X, point4.X };
                    double centerPointX = (dropPointX.Max() + dropPointX.Min()) / 2;
                    List<double> dropPointY = new List<double>() { point1.Y, point2.Y, point3.Y, point4.Y };
                    double centerPointY = (dropPointY.Max() + dropPointY.Min()) / 2;
                    //Xac dinh beam's orientation roi tao profile beam drop
                    //Tim centerPoint cua drop
                    double totalX = 0;
                    dropPointX.ForEach(pointX => totalX += pointX);
                    double averageX = totalX / dropPointX.Count;
                    double gapX = 0;
                    dropPointX.ForEach(pointX =>
                    {
                        gapX = Math.Abs(pointX - averageX);
                    });
                    string beamPosition = null;
                    beamMain.GetUserProperty("comment", ref beamPosition);
                    Point dropSP = new Point(centerPointX, centerPointY, dropPointZ.Max());
                    bool isDropAtSP = false;
                    if (beamPosition == "top" || beamPosition == "bottom")
                    {
                        double gapXFromDropToSP = Math.Abs(averageX - beamSP.X);
                        double gapXFromDropToEP = Math.Abs(averageX - beamEP.X);
                        isDropAtSP = gapXFromDropToSP < gapXFromDropToEP;
                    }
                    else if (beamPosition == "left" || beamPosition == "right")
                    {
                        double gapYFromDropToSP = Math.Abs(centerPointY - beamSP.Y);
                        double gapYFromDropToEP = Math.Abs(centerPointY - beamEP.Y);
                        isDropAtSP = gapYFromDropToSP < gapYFromDropToEP;
                    }
                    Point dropEP;
                    if (isDropAtSP)
                    {
                        dropEP = beamSP;
                    }
                    else
                    {
                        dropEP = beamEP;
                    }
                    //Mock drop point
                    Point beamPCStartPoint = new Point(-1927.42, 3252.39, -20);
                    Point beamPCEndPoint = new Point(-1927.42, 4382.39, -20);
                    createBeamPartCut(beamMain, dropHeight, beamDetail.b, dropSP, dropEP);

                    #endregion find dropBeamShape profile

                    model.CommitChanges();
                }

                //string beamLength = beamDetail.length;   -wait for Beam's Length
            }

            */

                #endregion Beam Input

                #region Slab Input

                /*
                string slabData = File.ReadAllText(@"..\..\Data\Slab.json");
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

                */

                #endregion Slab Input

                #region Wall Input

                string wallData = File.ReadAllText(@"..\..\Data\Wall.json");
                ICollection<WhWall> whWallList = JsonConvert.DeserializeObject<ICollection<WhWall>>(wallData);

                foreach (WhWall wallDetail in whWallList)
                {
                    if (wallDetail.WhCurve == null)
                    {
                        break;
                    }
                    else
                    {
                        var wallSP = wallDetail.WhCurve.CurvePoint[0];
                        var wallEP = wallDetail.WhCurve.CurvePoint[1];

                        Point _wallSP = new Point(wallSP.X, wallSP.Y, wallSP.Z - 29500.0);
                        Point _wallEP = new Point(wallEP.X, wallEP.Y, wallEP.Z - 29500.0);
                        double wallHeight = Convert.ToDouble(wallDetail.LevelTop) - Convert.ToDouble(wallDetail.LevelBot);
                        Beam wall = createWall("PANEL", wallHeight.ToString(), wallDetail.ThickNess.ToString(), _wallSP, _wallEP);
                        createPartCutWithBeam(model, wall);
                    }
                }

                /*
                 * In future I want to just check 1 or 2 beams relate to wall, not loop through all beam
                 */

                #endregion Wall Input

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
            Beam beam = new Beam(Beam.BeamTypeEnum.BEAM); beam.Name = beamName;
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
        private static void createBeamPartCut(Beam beamPCFather, double beamPCHeight, string beamPCWidth, Point beamPCStartPoint, Point beamPCEndPoint)
        {
            Beam beamPC = new Beam();
            //beamHeight dang la double. Co the loi
            beamPC.Profile.ProfileString = $"{beamPCHeight}*{beamPCWidth}";
            beamPC.Class = BooleanPart.BooleanOperativeClassName;
            beamPC.StartPoint = beamPCStartPoint;
            beamPC.EndPoint = beamPCEndPoint;
            beamPC.Insert();
            BooleanPart beamBooleanPart = new BooleanPart();
            beamBooleanPart.Father = beamPCFather;
            beamBooleanPart.SetOperativePart(beamPC);
            beamBooleanPart.Insert();
            //beamPC.Delete();
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