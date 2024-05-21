using System.Drawing;
using System.Windows.Forms;

namespace GestionBBDD
{
    public partial class CreditosForm : Form
    {
        private readonly Font labelFont = new Font("Arial", 12, FontStyle.Bold);
        private readonly Size pictureBoxSize = new Size(150, 150);

        public CreditosForm()
        {
            InitializeComponent();

            // Configura el formulario
            Text = "Créditos";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            ClientSize = new Size(600, 675);

            // Crea una etiqueta para mostrar el nombre
            Label nombreLabel = new Label
            {
                Text = "Juan Luis Domínguez López\nBecario para CEPSA en el área de Cyber OT",
                Dock = DockStyle.Top,
                AutoSize = true,
                Font = labelFont,
                ForeColor = Color.DarkBlue
            };
            Controls.Add(nombreLabel);

            // Crea un PictureBox para mostrar la foto
            PictureBox fotoPictureBox = new PictureBox
            {
                Image = Properties.Resources.ptrjuanlu,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = pictureBoxSize,
                Dock = DockStyle.Top
            };
            Controls.Add(fotoPictureBox);

            // Crea un PictureBox para mostrar el logo de CEPSA
            PictureBox logoPictureBox = new PictureBox
            {
                Image = Properties.Resources.OIP,
                SizeMode = PictureBoxSizeMode.Zoom,
                Dock = DockStyle.Fill
            };
            Controls.Add(logoPictureBox);

            // Ajusta el orden de los controles
            Controls.SetChildIndex(nombreLabel, 0);
            Controls.SetChildIndex(fotoPictureBox, 1);
            Controls.SetChildIndex(logoPictureBox, 2);
        }
    }
}