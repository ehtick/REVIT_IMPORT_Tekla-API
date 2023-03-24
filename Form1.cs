﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.Operations;
using WHExtensionElement;

using TSM = Tekla.Structures.Model;
using TSG = Tekla.Structures.Geometry3d;

using Point = Tekla.Structures.Geometry3d.Point;
using System.Drawing;

namespace REVIT_IMPORT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Material apply for all item
        private const string projectMaterial = "C32/40";

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

                #region Reset work plane

                WorkPlaneHandler workPlane = model.GetWorkPlaneHandler();
                TransformationPlane globalPlane = workPlane.GetCurrentTransformationPlane();
                workPlane.SetCurrentTransformationPlane(globalPlane);

                #endregion Reset work plane

                #region Main

                // App sẽ vẽ từ Bottom beam

                const double magicNum = 0.0;
                const bool beamInsert = true;

                string moduleArea = cboModuleArea.SelectedItem.ToString();

                #region Beam Input

                string beamData = File.ReadAllText(@"..\..\Data\PPVC15M_beam.json");
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

                Hashtable beamWidthList = new Hashtable();
                foreach (WhBeam beamDetail in whBeamList)
                {
                    Point beamSP = new Point(beamDetail.StartPoint.X, beamDetail.StartPoint.Y, beamDetail.StartPoint.Z - 29500.00);
                    Point beamEP = new Point(beamDetail.EndPoint.X, beamDetail.EndPoint.Y, beamDetail.EndPoint.Z - 29500.00);
                    string beamPosition = checkPartPosition(beamSP, beamEP, centerPoint);
                    List<Point> beamPoint = formatPoint(beamSP, beamEP, beamPosition);  //Sắp xếp theo chiều vẽ từ dưới lên trên,trái sang phải
                    Point _beamSP = beamPoint[0];
                    Point _beamEP = beamPoint[1];
                    double dropLength = beamDetail.LengthDrop;
                    Beam beamMain = null;

                    beamMain = beamCreate(beamDetail.Name, beamDetail.h, beamDetail.b, beamDetail.Type, projectMaterial, _beamSP, _beamEP, moduleArea, beamPosition);

                    //Convert.ToDouble(beamDetail.h) / 2

                    #region Create beam drop

                    if (beamDetail.DropPoint != null)
                    {
                        //deconstruction tuple
                        (WhPoint whPoint1, WhPoint whPoint2, WhPoint whPoint3, WhPoint whPoint4) = beamDetail.DropPoint;
                        Point point1 = new Point(whPoint1.X, whPoint1.Y, whPoint1.Z - 29500.0);
                        Point point2 = new Point(whPoint2.X, whPoint2.Y, whPoint2.Z - 29500.0);
                        Point point3 = new Point(whPoint3.X, whPoint3.Y, whPoint3.Z - 29500.0);
                        Point point4 = new Point(whPoint4.X, whPoint4.Y, whPoint4.Z - 29500.0);

                        List<Point> firstDropPointGroup = new List<Point>() { point1, point2, point3, point4 };

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

                        Point dropSP = new Point(centerPointX, centerPointY, dropPointZ.Max());

                        Point EpDrop = new Point(beamDetail.EpDrop.X, beamDetail.EpDrop.Y, beamDetail.EpDrop.Z - 29500.0);
                        Point SpDrop = new Point(beamDetail.SpDrop.X, beamDetail.SpDrop.Y, beamDetail.SpDrop.Z - 29500.0);

                        bool isEpFromInputCorrect = false;  //Để kiểm tra EP từ Input có đúng là EP của drop ko hay là SP của drop vì có trường hợp đồi chỗ cho nhau
                        Point EpFromInputChecked = SpDrop;
                        firstDropPointGroup.ForEach(point =>
                        {
                            if (point == SpDrop)
                            {
                                isEpFromInputCorrect = true;
                                EpFromInputChecked = EpDrop;
                            }
                        });

                        ControlPoint controlPoint6 = new ControlPoint(EpFromInputChecked);
                        controlPoint6.Insert();

                        controlPoint6.Insert();

                        Point dropEP = null;

                        bool isDropAtSP = false;
                        if (beamPosition == "top" || beamPosition == "bottom")
                        {
                            double gapDropEPToSP = Math.Abs(EpFromInputChecked.X - _beamSP.X);
                            double gapDropEPToEP = Math.Abs(EpFromInputChecked.X - _beamEP.X);
                            isDropAtSP = gapDropEPToSP < gapDropEPToEP;
                            if (isDropAtSP)
                            {
                                dropEP = new Point(dropSP.X - dropLength, dropSP.Y, dropSP.Z);
                            }
                            else
                            {
                                dropEP = new Point(dropSP.X + dropLength, dropSP.Y, dropSP.Z);
                            }
                        }
                        else if (beamPosition == "left" || beamPosition == "right")
                        {
                            double gapDropEPToSP = Math.Abs(EpFromInputChecked.Y - _beamSP.Y);
                            double gapDropEPToEP = Math.Abs(EpFromInputChecked.Y - _beamEP.Y);
                            isDropAtSP = gapDropEPToSP < gapDropEPToEP;
                            if (isDropAtSP)
                            {
                                dropEP = new Point(dropSP.X, dropSP.Y - dropLength, dropSP.Z);
                            }
                            else
                            {
                                dropEP = new Point(dropSP.X, dropSP.Y + dropLength, dropSP.Z);
                            }
                        }

                        ControlPoint controlPoint5 = new ControlPoint(dropEP);
                        controlPoint5.Insert();
                        createBeamPartCut(beamMain, dropHeight, beamDetail.b, dropSP, dropEP);

                        #endregion find dropBeamShape profile

                        model.CommitChanges();
                    }

                    #endregion Create beam drop
                }

                #endregion Beam Input

                #region Slab Input

                string slabData = File.ReadAllText(@"..\..\Data\PPVC15M_slab.json");
                ICollection<WhSlab> whSlabList = JsonConvert.DeserializeObject<ICollection<WhSlab>>(slabData);
                foreach (WhSlab slabDetail in whSlabList)
                {
                    List<TSG.Point> slabPointBoundaryList = new List<Point>()
                    {
                    new Point(slabDetail.SketchOut[0].CurvePoint[0].X, slabDetail.SketchOut[0].CurvePoint[0].Y, slabDetail.SketchOut[0].CurvePoint[0].Z - 29500.00)
                    };
                    double slabThickness;
                    if (slabDetail.Slope == true && slabDetail.ThickNess == 130.0)
                    {
                        slabThickness = 135.0;
                    }
                    else
                    {
                        slabThickness = slabDetail.ThickNess;
                    }
                    foreach (WhCurve slabSketchOut in slabDetail.SketchOut)
                    {
                        slabPointBoundaryList.Add(new Point(slabSketchOut.CurvePoint[1].X, slabSketchOut.CurvePoint[1].Y, slabSketchOut.CurvePoint[1].Z - 29500.00));

                        Console.WriteLine(slabSketchOut.CurvePoint[1].Z - 29500.0);
                    }
                    ContourPlate slab = slabCreate(slabThickness, projectMaterial, slabPointBoundaryList, slabDetail.Slope);

                    #region Create slope for ledge slab

                    if (slabDetail.Slope)
                    {
                        slopeCreate(slab, slabPointBoundaryList, centerPoint, moduleArea);
                    }

                    #endregion Create slope for ledge slab

                    #region Create Slab Opening

                    if (slabDetail.SketchIn.Count > 0)
                    {
                        foreach (List<WhCurve> slabSketchInList in slabDetail.SketchIn)
                        {
                            List<Point> slabOpeningPointList = new List<Point>()
                            {
                                new Point(slabSketchInList[0].CurvePoint[0].X, slabSketchInList[0].CurvePoint[0].Y, slabSketchInList[0].CurvePoint[0].Z - 29500.00)
                            };

                            foreach (WhCurve slabSketchIn in slabSketchInList)
                            {
                                slabOpeningPointList.Add(new Point(slabSketchIn.CurvePoint[1].X, slabSketchIn.CurvePoint[1].Y, slabSketchIn.CurvePoint[1].Z - 29500.0));
                            }
                            ContourPlate slabOpening = slabCreate(slabThickness, projectMaterial, slabOpeningPointList, slabDetail.Slope);
                            createPartCutIntersection(slab, slabOpening);
                            slabOpening.Delete();
                        }
                    }

                    #endregion Create Slab Opening

                    createPartCutWithBeam(model, slab);
                    model.CommitChanges();
                }

                #endregion Slab Input

                #region Wall Input

                string wallData = File.ReadAllText(@"..\..\Data\PPVC15M_wall.json");
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

                        Point _wallSP = new Point(wallSP.X, wallSP.Y, wallSP.Z - 29500.0 - magicNum);
                        Point _wallEP = new Point(wallEP.X, wallEP.Y, wallEP.Z - 29500.0 - magicNum);
                        double wallHeight = Convert.ToDouble(wallDetail.Height);
                        bool isRecess = true;
                        Beam wall = wallCreate(wallDetail.Name, wallHeight.ToString(), wallDetail.ThickNess.ToString(), _wallSP, _wallEP);
                        model.CommitChanges();
                        string wallPosition = checkPartPosition(_wallSP, _wallEP, centerPoint);
                        wall.SetUserProperty("comment", wallPosition);
                        createPartCutWithBeam(model, wall);

                        #region Create Wall Opening

                        if (wallDetail.WhOpenings.Count > 0)
                        {
                            foreach (WhOpening slabSketchInList in wallDetail.WhOpenings)
                            {
                                List<Point> wallOpeningPointList = new List<Point>();
                                foreach (WhPoint point in slabSketchInList.OpeningPoint)
                                {
                                    wallOpeningPointList.Add(new Point(point.X, point.Y, point.Z));
                                }
                                Point wallOpeningFirstPoint = wallOpeningPointList[0];
                                Point wallOpeningSecondPoint = wallOpeningPointList[1];
                                double zValueWallOpening = wallOpeningFirstPoint.Z > wallOpeningSecondPoint.Z ? wallOpeningSecondPoint.Z : wallOpeningFirstPoint.Z;
                                Point wallOpeningSP = new Point(wallOpeningFirstPoint.X, wallOpeningFirstPoint.Y, zValueWallOpening);
                                Point wallOpeningEP = new Point(wallOpeningSecondPoint.X, wallOpeningSecondPoint.Y, zValueWallOpening);

                                double openingHeight = Math.Abs(wallOpeningFirstPoint.Z - wallOpeningSecondPoint.Z);
                                Beam wallOpening = wallCreate("wall Opening", openingHeight.ToString(), (2 * wallDetail.ThickNess).ToString(), wallOpeningSP, wallOpeningEP);
                                createPartCutIntersection(wall, wallOpening);
                                wallOpening.Delete();
                            }
                        }

                        #endregion Create Wall Opening

                        if (wallDetail.Recess != null)
                        {
                            wallDetail.Recess.ForEach(recessPoint =>
                            {
                                Point _recessPoint = new Point(recessPoint.X, recessPoint.Y, recessPoint.Z);
                                createRecess(wall, wallHeight, wallPosition, wallDetail.ThickNess, _wallSP, _recessPoint, model, moduleArea);
                            });
                        }
                    }
                }

                /*
                 * In future I want to just check 1 or 2 beams relate to wall, not loop through all beam
                 */

                #endregion Wall Input

                #endregion Main

                model.CommitChanges();
                MessageBox.Show("Done!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static Beam beamCreate(string beamName, string beamHeight, string beamWidth, string beamMark, string beamMaterial,
            TSG.Point beamSP, TSG.Point beamEP, string moduleArea, string beamPosition)
        {
            Beam beam = new Beam(Beam.BeamTypeEnum.BEAM); beam.Name = beamName;
            beam.Profile.ProfileString = $"{beamHeight}*{beamWidth}";
            beam.Material.MaterialString = beamMaterial;
            beam.Class = "6";
            beam.StartPoint = beamSP;
            beam.EndPoint = beamEP;
            beam.Insert();

            beam.SetUserProperty("comment", beamPosition);
            beam.SetUserProperty("USER_FIELD_2", beamMark);
            return beam;
        }

        private static ContourPlate slabCreate(double slabThickness, string slabMaterial, List<Point> slabPointsList, bool slabSlope)
        {
            ContourPlate slab = new ContourPlate();
            if (slabSlope)
            {
                double maxZOfPoint = -1000.0;
                slabPointsList.ForEach(maxZ =>
                {
                    if (maxZ.Z > maxZOfPoint)
                    {
                        maxZOfPoint = maxZ.Z;
                    }
                });
                slabPointsList.ForEach(point =>
                {
                    if (point.Z < maxZOfPoint)
                    {
                        point.Z = maxZOfPoint;
                    }
                });
            }

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
                if (partToBeCutObb.Intersects(beamObb) && beam.Name != "LINTEL BEAM")
                {
                    createPartCutIntersection(partToBeCut, beam);
                }
                else if (partToBeCutObb.Intersects(beamObb) && beam.Name == "LINTEL BEAM")
                {
                    createPartCutIntersection(beam, partToBeCut); //partToBeCut here now is partShapeTheCut. Opposite to the name of its self
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

        private static void cutBeamByRecess(Model model, string moduleArea, Beam wallRecess, string wallPosition)
        {
            ModelObjectEnumerator beamEnum = model.GetModelObjectSelector().GetAllObjectsWithType(ModelObject.ModelObjectEnum.BEAM);
            while (beamEnum.MoveNext())
            {
                Beam beam = beamEnum.Current as Beam;
                string beamPosition = null;
                if (moduleArea == "MiddleArea")
                {
                    beam.GetUserProperty("comment", ref beamPosition);
                    if ((wallPosition == "top" || wallPosition == "bottom") && wallPosition == beamPosition)
                    {
                        createPartCutIntersection(beam, wallRecess);
                    }
                    else if ((wallPosition == "left" || wallPosition == "right") && (wallPosition == beamPosition || beamPosition == "top" || beamPosition == "bottom"))
                    {
                        createPartCutIntersection(beam, wallRecess);
                    }
                }
                else
                {
                    beam.GetUserProperty("comment", ref beamPosition);
                    if ((wallPosition == "left" || wallPosition == "right") && wallPosition == beamPosition)
                    {
                        createPartCutIntersection(beam, wallRecess);
                    }
                    else if ((wallPosition == "top" || wallPosition == "bottom") && (wallPosition == beamPosition || beamPosition == "left" || beamPosition == "right"))
                    {
                        createPartCutIntersection(beam, wallRecess);
                    }
                }
            }
        }

        private static Beam wallCreate(string wallName, string wallHeight, string wallThickness, Point wallSP, Point wallEP)
        {
            Beam wall = new Beam(Beam.BeamTypeEnum.PANEL);
            wall.Name = wallName == null ? "PANEL" : wallName;
            wall.Profile.ProfileString = $"{wallHeight}*{wallThickness}";
            wall.Material.MaterialString = projectMaterial;
            wall.StartPoint = wallSP;
            wall.EndPoint = wallEP;
            wall.Class = wall.Name == "PANEL" ? "1" : "5";
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

        private static string checkPartPosition(Point partSP, Point partEP, Point centerPoint)
        {
            double gapX = Math.Abs(partSP.X - partEP.X);
            double gapY = Math.Abs(partSP.Y - partEP.Y);
            bool isHor = false;
            bool isVer = false;
            string partOrientation = null;
            string partPosition = null;

            if (gapX >= 0.0 && gapX < 1.0)
            {
                isVer = true;
            }

            if (gapY >= 0.0 && gapY < 1.0)
            {
                isHor = true;
            }

            if (isVer)
            {
                partOrientation = "ver";
            }

            if (isHor)
            {
                partOrientation = "hor";
            }

            switch (partOrientation)
            {
                case "hor":
                    if (partSP.Y > centerPoint.Y)
                    {
                        partPosition = "top";
                    }
                    else
                    {
                        partPosition = "bottom";
                    }
                    break;

                case "ver":
                    if (partSP.X > centerPoint.X)
                    {
                        partPosition = "right";
                    }
                    else
                    {
                        partPosition = "left";
                    }
                    break;

                default:
                    break;
            }
            return partPosition;
        }

        private static void createRecess(Beam wall, double wallHeight, string wallPosition, double wallThickness,
            Point _wallSP, Point recessPoint, Model model, string moduleArea)
        {
            Beam wallRecess = new Beam(Beam.BeamTypeEnum.BEAM);
            wallRecess.Profile.ProfileString = "TRI_E200*30*0*15";
            wallRecess.Class = "6";
            //Point recessSP = null;
            //Point recessEP = null;

            Point recessSP = null;
            Point recessEP = null;
            switch (wallPosition)
            {
                case "top":
                    wallRecess.Position.Rotation = Position.RotationEnum.BACK;
                    wallRecess.Position.Plane = Position.PlaneEnum.RIGHT;
                    wallRecess.Position.Depth = Position.DepthEnum.MIDDLE;
                    break;

                case "bottom":
                    wallRecess.Position.Rotation = Position.RotationEnum.FRONT;
                    wallRecess.Position.Plane = Position.PlaneEnum.LEFT;
                    wallRecess.Position.Depth = Position.DepthEnum.MIDDLE;
                    break;

                case "left":
                    wallRecess.Position.Rotation = Position.RotationEnum.BELOW;
                    wallRecess.Position.Plane = Position.PlaneEnum.MIDDLE;
                    wallRecess.Position.Depth = Position.DepthEnum.BEHIND;
                    break;

                case "right":
                    wallRecess.Position.Rotation = Position.RotationEnum.TOP;
                    wallRecess.Position.Plane = Position.PlaneEnum.MIDDLE;
                    wallRecess.Position.Depth = Position.DepthEnum.FRONT;

                    break;
            }
            recessSP = new Point(recessPoint.X, recessPoint.Y, _wallSP.Z);
            recessEP = new Point(recessSP.X, recessSP.Y, recessSP.Z + wallHeight);
            wallRecess.StartPoint = recessSP;
            wallRecess.EndPoint = recessEP;
            wallRecess.Insert();
            createPartCutIntersection(wall, wallRecess);

            cutBeamByRecess(model, moduleArea, wallRecess, wallPosition);
            wallRecess.Delete();
        }

        private static List<Point> formatPoint(Point startPoint, Point endPoint, string beamPosition)
        {
            Point tempPoint = null;

            if (beamPosition == "left" || beamPosition == "right")
            {
                if (startPoint.Y > endPoint.Y)
                {
                    tempPoint = endPoint;
                    endPoint = startPoint;
                    startPoint = tempPoint;
                }
            }
            else if ((beamPosition == "top" || beamPosition == "bottom") && startPoint.X > endPoint.X)
            {
                tempPoint = endPoint;
                endPoint = startPoint;
                startPoint = tempPoint;
            }

            return new List<Point>() { startPoint, endPoint };
        }

        private void slopeCreate(ContourPlate slab, List<Point> slabPointList, Point centerPoint, string moduleArea)
        {
            //format slabpoint
            List<double> xValueList = new List<double>();
            List<double> yValueList = new List<double>();
            slabPointList.ForEach(point =>
            {
                xValueList.Add(point.X);
                yValueList.Add(point.Y);
            });
            double maxX = xValueList.Max();
            double maxY = yValueList.Max();
            double minX = xValueList.Min();
            double minY = yValueList.Min();
            double normalZ = slabPointList[0].Z;

            Beam slope = new Beam();
            slope.Profile.ProfileString = "500*230";
            slope.StartPoint = new Point(0.0, 0.0, 0.0);
            slope.EndPoint = new Point(0.0, 1000.0, 0.0);
            string ledgeSlabPosition = null;
            if (moduleArea == "MiddleArea")
            {
                ledgeSlabPosition = slabPointList[0].X > centerPoint.X ? "right" : "left";
            }
            else
            {
                ledgeSlabPosition = slabPointList[0].Y > centerPoint.Y ? "top" : "bottom";
            }

            switch (ledgeSlabPosition)
            {
                case "top":
                    slope.Profile.ProfileString = "IRR_J850*0*10";
                    slope.Position.Plane = Position.PlaneEnum.RIGHT;
                    slope.Position.Rotation = Position.RotationEnum.FRONT;
                    slope.Position.Depth = Position.DepthEnum.MIDDLE;
                    slope.StartPoint = new Point(maxX, minY, normalZ);
                    slope.EndPoint = new Point(minX, minY, normalZ);
                    break;

                case "bottom":
                    slope.Profile.ProfileString = "IRR_J850*0*10";
                    slope.Position.Plane = Position.PlaneEnum.LEFT;
                    slope.Position.Rotation = Position.RotationEnum.FRONT;
                    slope.Position.Depth = Position.DepthEnum.MIDDLE;
                    slope.StartPoint = new Point(minX, minY, normalZ);
                    slope.EndPoint = new Point(maxX, minY, normalZ);
                    break;

                case "right":
                    slope.Profile.ProfileString = "IRR_J850*0*10";
                    slope.Position.Plane = Position.PlaneEnum.LEFT;
                    slope.Position.Rotation = Position.RotationEnum.FRONT;
                    slope.Position.Depth = Position.DepthEnum.MIDDLE;
                    slope.StartPoint = new Point(maxX, minY, normalZ);
                    slope.EndPoint = new Point(maxX, maxY, normalZ);
                    break;

                case "left":
                    slope.Profile.ProfileString = "IRR_J850*0*10";
                    slope.Position.Plane = Position.PlaneEnum.RIGHT;
                    slope.Position.Rotation = Position.RotationEnum.FRONT;
                    slope.Position.Depth = Position.DepthEnum.MIDDLE;
                    slope.StartPoint = new Point(maxX, maxY, normalZ);
                    slope.EndPoint = new Point(maxX, minY, normalZ);
                    break;

                default:
                    break;
            }
            slope.Insert();
            createPartCutIntersection(slab, slope);
            slope.Delete();
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