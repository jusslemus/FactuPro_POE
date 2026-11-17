using MySqlConnector;

namespace FactuPro
{
    public partial class Form1 : Form
    {
        private readonly Color colorOscuro = Color.FromArgb(30, 41, 59);
        private readonly Color colorPrimario = Color.FromArgb(37, 99, 235);
        private readonly Color colorTexto = Color.FromArgb(51, 65, 85);
        private readonly Color colorSuave = Color.FromArgb(100, 116, 139);

        private TextBox txtUsuario = null!;
        private TextBox txtClave = null!;
        private CheckBox chkMostrar = null!;
        private Button btnIngresar = null!;
        private Button btnSalir = null!;
        private ErrorProvider errorProvider = null!;

        public Form1()
        {
            InitializeComponent();
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            // Formulario
            Text = "FactuPro - Iniciar sesión";
            ClientSize = new Size(720, 440);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10);

            errorProvider = new ErrorProvider
            {
                ContainerControl = this,
                BlinkStyle = ErrorBlinkStyle.NeverBlink
            };

            // Panel izquierdo (marca)
            var panelMarca = new Panel
            {
                Dock = DockStyle.Left,
                Width = 280,
                BackColor = colorOscuro
            };
            var lblMarca = new Label
            {
                Text = "FactuPro",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(30, 150)
            };
            var lblLema = new Label
            {
                Text = "Sistema de facturación",
                ForeColor = Color.FromArgb(148, 163, 184),
                Font = new Font("Segoe UI", 11),
                AutoSize = true,
                Location = new Point(34, 205)
            };
            panelMarca.Controls.Add(lblMarca);
            panelMarca.Controls.Add(lblLema);

            // Panel derecho (formulario)
            var panelForm = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            var lblTitulo = new Label
            {
                Text = "Iniciar sesión",
                ForeColor = colorTexto,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(60, 50)
            };
            var lblSub = new Label
            {
                Text = "Ingrese sus credenciales para continuar",
                ForeColor = colorSuave,
                AutoSize = true,
                Location = new Point(62, 95)
            };

            var lblUsuario = CrearEtiqueta("Usuario", 145);
            txtUsuario = CrearCaja(170, "Nombre de usuario");

            var lblClave = CrearEtiqueta("Contraseña", 225);
            txtClave = CrearCaja(250, "Contraseña");
            txtClave.UseSystemPasswordChar = true;

            chkMostrar = new CheckBox
            {
                Text = "Mostrar contraseña",
                ForeColor = colorSuave,
                AutoSize = true,
                Location = new Point(60, 290),
                Cursor = Cursors.Hand
            };
            chkMostrar.CheckedChanged += (s, e) =>
                txtClave.UseSystemPasswordChar = !chkMostrar.Checked;

            btnIngresar = new Button
            {
                Text = "Ingresar",
                Size = new Size(320, 42),
                Location = new Point(60, 330),
                BackColor = colorPrimario,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnIngresar.FlatAppearance.BorderSize = 0;
            btnIngresar.Click += btnIngresar_Click;

            btnSalir = new Button
            {
                Text = "Salir",
                Size = new Size(320, 32),
                Location = new Point(60, 382),
                BackColor = Color.White,
                ForeColor = colorSuave,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSalir.FlatAppearance.BorderSize = 0;
            btnSalir.Click += (s, e) => Application.Exit();

            panelForm.Controls.AddRange(new Control[]
            {
                lblTitulo, lblSub, lblUsuario, txtUsuario, lblClave,
                txtClave, chkMostrar, btnIngresar, btnSalir
            });

            Controls.Add(panelForm);
            Controls.Add(panelMarca);

            AcceptButton = btnIngresar;   // Enter = Ingresar
            CancelButton = btnSalir;      // Esc = Salir
        }

        private Label CrearEtiqueta(string texto, int y)
        {
            return new Label
            {
                Text = texto,
                ForeColor = colorTexto,
                AutoSize = true,
                Location = new Point(60, y)
            };
        }

        private TextBox CrearCaja(int y, string placeholder)
        {
            return new TextBox
            {
                Location = new Point(60, y),
                Width = 320,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = placeholder,
                MaxLength = 50
            };
        }
        private void Form1_Load(object? sender, EventArgs e)
        {
            var (hash, salt) = PasswordHelper.Crear("admin123");
            using var cn = Conexion.Obtener();
            cn.Open();
            var cmd = new MySqlCommand(
                "INSERT INTO Usuario (Nombre, NombreCompleto, PasswordHash, PasswordSalt) VALUES ('admin', 'Administrador', @h, @s)", cn);
            cmd.Parameters.AddWithValue("@h", hash);
            cmd.Parameters.AddWithValue("@s", salt);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Usuario admin creado");
        }
        private void btnIngresar_Click(object? sender, EventArgs e)
        {
            errorProvider.Clear();
            string usuario = txtUsuario.Text.Trim();
            string clave = txtClave.Text;
            bool valido = true;

            if (usuario == "")
            {
                errorProvider.SetError(txtUsuario, "Ingrese el usuario");
                valido = false;
            }
            if (clave == "")
            {
                errorProvider.SetError(txtClave, "Ingrese la contraseña");
                valido = false;
            }
            if (!valido) return;

            try
            {
                bool correcto = false;
                string nombreCompleto = "";

                using (var cn = Conexion.Obtener())
                {
                    cn.Open();
                    var cmd = new MySqlCommand(
                        "SELECT NombreCompleto, PasswordHash, PasswordSalt FROM Usuario WHERE Nombre = @n", cn);
                    cmd.Parameters.AddWithValue("@n", usuario);

                    using var rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        correcto = PasswordHelper.Verificar(
                            clave, rd.GetString("PasswordHash"), rd.GetString("PasswordSalt"));
                        nombreCompleto = rd.GetString("NombreCompleto");
                    }
                }

                if (!correcto)
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Acceso denegado",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtClave.Clear();
                    txtClave.Focus();
                    return;
                }

                Hide();
                var principal = new frmPrincipal(nombreCompleto);
                principal.FormClosed += (s, args) => Application.Exit();
                principal.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo conectar con la base de datos:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}