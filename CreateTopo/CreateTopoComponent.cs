using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Rhino.Geometry;

using DotSpatial.Projections;
using DotSpatial.Data;
using DotSpatial.Topology;
using DotSpatial.Symbology;
using System.Data;
using Grasshopper;
using Grasshopper.Kernel.Data;

namespace CreateTopo
{
    public class CreateTopoComponent : GH_Component
    {
        public static string CRS = "";
        public static bool unit = true;
        public static List<string> m_filepaths = new List<string>();

        public CreateTopoComponent(): base("CreateTopo", "Shape", "FlorBIM", "FlorBIM", "CreateTopo")
        {
        }

        //protected override void BeforeSolveInstance()
        //{
        //    CRS = "";
        //    unit = true;
        //    m_filepaths.Clear();

        //    base.BeforeSolveInstance();
        //}

        protected override void AfterSolveInstance()
        {
            base.AfterSolveInstance();
            m_filepaths.Clear();
        }

        public override void CreateAttributes()
        {
            m_attributes = new ButtonUIAttributes(this, "옵션선택", FunctionToRunClick, "옵션");
        }

        public void FunctionToRunClick()
        {
            m_filepaths.Clear();
            CRS = "";

            Option op = new Option();
            if (op.ShowDialog() != DialogResult.OK) return;
            CRS = Option.m_CRS;
            unit = Option.m_Unit;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Shapefiles|*.shp";
            ofd.Multiselect = true;
            ofd.RestoreDirectory = true;
            ofd.Title = "Shape 파일을 선택하세요";

            if (ofd.ShowDialog() != DialogResult.OK) return;

            foreach (string item in ofd.FileNames)
            {
                m_filepaths.Add(item);
            }

            GH_Component comp = this;
            comp.ExpireSolution(true);
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Shapefile", "Shapefile", "Shapefile", GH_ParamAccess.list);
            pManager.AddPointParameter("Point", "Point", "Point", GH_ParamAccess.list);
            pManager.AddCurveParameter("Curve", "Curve", "Curve", GH_ParamAccess.list);
            pManager.AddTextParameter("CRS", "CRS", "CRS", GH_ParamAccess.list);
            pManager.AddTextParameter("Feature", "Feature", "Feature", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (m_filepaths.Count == 0) return;

            List<Tuple<Shapefile, List<Point3d>, List<Polyline>, string, List<string>>> data = GetdataFromShape(m_filepaths, unit, CRS);

            DataTree<Shapefile> shapeList = new DataTree<Shapefile>();
            DataTree<Point3d> pts = new DataTree<Point3d>();
            DataTree<Polyline> poylist = new DataTree<Polyline>();
            DataTree<string> SRC = new DataTree<string>();
            DataTree<string> features = new DataTree<string>();

            try
            {
                for (int i = 0; i < data.Count; i++)
                {
                    shapeList.Add(data[i].Item1, new GH_Path(i));
                    pts.AddRange(data[i].Item2, new GH_Path(i));
                    poylist.AddRange(data[i].Item3, new GH_Path(i));
                    SRC.Add(data[i].Item4, new GH_Path(i));
                    features.AddRange(data[i].Item5, new GH_Path(i));
                }

                DA.SetDataTree(0, shapeList);
                DA.SetDataTree(1, pts);
                DA.SetDataTree(2, poylist);
                DA.SetDataTree(3, SRC);
                DA.SetDataTree(4, features);

                ////DA.IncrementIteration();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("864d1b98-ab5c-4abe-aa55-60501817e09f"); }
        }


        public static List<Tuple<Shapefile, List<Point3d>, List<Polyline>, string,List<string>>> GetdataFromShape (List<string> filepaths, bool unit, string CRS)
        {
            List<Tuple<Shapefile, List<Point3d>, List<Polyline>, string, List<string>>> retundata = new List<Tuple<Shapefile, List<Point3d>, List<Polyline>, string, List<string>>>();

            try
            {
                foreach (string path in filepaths)
                {
                    var sf = Shapefile.OpenFile(path, DataManager.DefaultDataManager.ProgressHandler);

                    //string n = "+proj=tmerc +lat_0=38 +lon_0=127 +k=1 +x_0=200000 +y_0=500000 +ellps=GRS80 +units=m +no_defs";
                    //ProjectionInfo pf = ProjectionInfo.FromProj4String(n);
                    //if(sf.CanReproject == true)
                    //{
                    //    sf.Reproject(pf);
                    //}

                    string srs_re = "";
                    if (CRS == "")
                    {
                        srs_re = sf.ProjectionString;
                    }
                    else
                    {
                        srs_re = CRS;
                        sf.ProjectionString = CRS;
                    }

                    DataColumn[] cols = sf.GetColumns();
                    List<string> features = new List<string>();
                    foreach (DataColumn item in cols)
                    {
                        features.Add(item.ColumnName);
                    }

                    var Pdata = sf.Features;
                    List<Point3d> pts = new List<Point3d>();
                    List<Polyline> ploys = new List<Polyline>();

                    foreach (var item in Pdata)
                    {
                        IList<Coordinate> cd = item.Coordinates;
                        var typeF = item.FeatureType;

                        List<Point3d> pprs = new List<Point3d>();

                        if (typeF == FeatureType.Polygon)
                        {
                            var geometry = new Polygon(new LinearRing(cd));
                            IList<Coordinate> cdc = geometry.Coordinates;

                            foreach (Coordinate p in cdc)
                            {
                                if (unit == true)
                                {
                                    pts.Add(new Point3d(p.X, p.Y, 0));
                                    pprs.Add(new Point3d(p.X, p.Y, 0));
                                }
                                else
                                {
                                    pts.Add(new Point3d(p.X * 1000, p.Y * 1000, 0));
                                    pprs.Add(new Point3d(p.X * 1000, p.Y * 1000, 0));
                                }
                            }

                            Polyline pp = new Polyline(pprs);
                            ploys.Add(pp);
                        }

                        //Point3d[] removeDuplicate = Point3d.CullDuplicates(pprs, 0.001);

                        //if (sf.FeatureType == FeatureType.Line)
                        //{
                        //    Polyline pp = new Polyline(removeDuplicate);
                        //    ploys.Add(pp);
                        //}

                        //else if (sf.FeatureType == FeatureType.Polygon)
                        //{
                        //    //List<Point3d> ppts = new List<Point3d>();
                        //    //ppts.AddRange(removeDuplicate);
                        //    //ppts.Add(removeDuplicate[0]);
                        //    Polyline pp = new Polyline(pprs);
                        //    ploys.Add(pp);
                        //}
                    }

                    retundata.Add(new Tuple<Shapefile, List<Point3d>, List<Polyline>, string, List<string>>(sf, pts, ploys, srs_re, features));
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return retundata;
        }
    }
}
