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

        public override void CreateAttributes()
        {
            m_attributes = new ButtonUIAttributes(this, "옵션선택", FunctionToRunClick, "옵션");
        }

        public void FunctionToRunClick()
        {
            m_filepaths.Clear();

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
            pManager.AddPointParameter("Point", "Point", "Point", GH_ParamAccess.list);
            pManager.AddCurveParameter("Curve", "Curve", "Curve", GH_ParamAccess.list);
            pManager.AddTextParameter("CRS", "CRS", "CRS", GH_ParamAccess.item);
            pManager.AddTextParameter("Feature", "Feature", "Feature", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (m_filepaths.Count == 0) return;
            var sf = Shapefile.OpenFile(m_filepaths[0], null);
            string srs_re = "";
            if(CRS == "")
            {
                srs_re = sf.ProjectionString;
            }
            else
            {
                srs_re = CRS;
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
                List<Point3d> pprs = new List<Point3d>();
                foreach (Coordinate p in cd)
                {
                    pts.Add(new Point3d(p.X, p.Y, 0));
                    pprs.Add(new Point3d(p.X, p.Y, 0));
                }
                Point3d[] removeDuplicate = Point3d.CullDuplicates(pprs, 0.001);
                Polyline pp = new Polyline(removeDuplicate);
                ploys.Add(pp);
            }

            Point3d[] removeDuplicate_re = Point3d.CullDuplicates(pts, 0.001);

            DA.SetDataList(0, removeDuplicate_re);
            DA.SetDataList(1, ploys);
            DA.SetDataList(2, srs_re);
            DA.SetDataList(3, features);
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
    }
}
