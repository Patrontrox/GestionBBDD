using System.Drawing;
using System.Windows.Forms;

namespace GestionBBDD
{
    public partial class CreditosForm : Form
    {
        public CreditosForm()
        {
            InitializeComponent();

            // Configura el formulario
            Text = "Créditos";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.LightGray; // Cambia el color de fondo del formulario

            // Crea una etiqueta para mostrar el nombre
            Label nombreLabel = new Label();
            nombreLabel.Text = "Juan Luis Domínguez López\nBecario para CEPSA en el área de Cyber";
            nombreLabel.Dock = DockStyle.Top;
            nombreLabel.AutoSize = true; // Ajusta el tamaño de la etiqueta al texto
            nombreLabel.Font = new Font("Arial", 12, FontStyle.Bold); // Cambia la fuente
            nombreLabel.ForeColor = Color.DarkBlue; // Cambia el color de la fuente
            Controls.Add(nombreLabel);

            // Crea un PictureBox para mostrar la foto
            PictureBox fotoPictureBox = new PictureBox();
            fotoPictureBox.Image = Properties.Resources.ptrjuanlu; // Asume que ptrjuanlu es el nombre de tu imagen
            fotoPictureBox.SizeMode = PictureBoxSizeMode.Zoom; // Ajusta la imagen al tamaño del PictureBox
            fotoPictureBox.Size = new Size(150, 150); // Establece un tamaño fijo para el PictureBox
            fotoPictureBox.Dock = DockStyle.Top;
            Controls.Add(fotoPictureBox);

            // Crea un PictureBox para mostrar el logo de CEPSA
            PictureBox logoPictureBox = new PictureBox();
            logoPictureBox.Image = Properties.Resources.OIP; // Asume que OIP es el nombre de tu imagen
            logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom; // Ajusta la imagen al tamaño del PictureBox
            logoPictureBox.Dock = DockStyle.Fill; // Llena el espacio restante del formulario
            Controls.Add(logoPictureBox);


            // Ajusta el tamaño del formulario a un tamaño fijo
            ClientSize = new Size(600, 675); // Reemplaza 500 y 600 con el ancho y la altura que desees

            // Ajusta el orden de los controles
            Controls.SetChildIndex(nombreLabel, 0);
            Controls.SetChildIndex(fotoPictureBox, 1);
            Controls.SetChildIndex(logoPictureBox, 2);
        }
    }



}