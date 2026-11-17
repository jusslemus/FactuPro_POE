namespace FactuPro
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!Conexion.Probar(out string err))
                MessageBox.Show("No se pudo conectar: " + err);
            else
                MessageBox.Show("Conexión exitosa");
        }
    }
}
