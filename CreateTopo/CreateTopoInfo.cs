using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace CreateTopo
{
    public class CreateTopoInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "CreateTopo";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("da488550-6ace-4c9c-8db1-d7a81dd1d44e");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
