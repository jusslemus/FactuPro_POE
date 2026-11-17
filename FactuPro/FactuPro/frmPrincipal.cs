using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FactuPro
{
    public partial class frmPrincipal : Form
    {
        public frmPrincipal(string nombreUsuario)
        {
            InitializeComponent();
            Text = "FactuPro - Usuario: " + nombreUsuario;
        }
    }
}