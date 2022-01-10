using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Linq;

using DotSpatial.Projections;
using DotSpatial.Data;
using DotSpatial.Topology;
using DotSpatial.Symbology;
using System.Data;
using Grasshopper;
using Grasshopper.Kernel.Data;

namespace CreateTopo
{
    public class ParsingData : GH_Component
    {

        public ParsingData() : base("Pdata", "FData", "FlorBIM", "FlorBIM", "CreateTopo")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("ShapeFile", "ShapeFile", "ShapeFile", GH_ParamAccess.item);
            pManager.AddTextParameter("Feature", "Feature", "Feature", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Result", "Result", "Result", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Shapefile sf = null;
            string Feature = "";
            DA.GetData(0, ref sf);
            DA.GetData(1, ref Feature);

            if (sf == null || Feature == "") return;

            DataTable dt = sf.DataTable;
            object[] strs = dt.Select().Select(x => x[Feature]).ToArray();

            DA.SetDataList(0, strs);
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
            get { return new Guid("FED5F007-49D1-44AD-A78D-9190C6B3856D"); }
        }
    }
}
