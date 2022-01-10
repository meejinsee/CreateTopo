using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateTopo
{
    public partial class Option : Form
    {
        public static string m_CRS = "";
        public static bool m_Unit = true;
        public Option()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(radioButton1.Checked == true)
            {
                m_Unit = true;
                radioButton2.Checked = false;
            }
            else if(radioButton2.Checked == true)
            {
                m_Unit = false;
                radioButton1.Checked = false;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            m_CRS = textBox1.Text;
        }
    }
}
