using System;
using System.Drawing;
using System.Windows.Forms;

namespace GestionBBDD
{
    public partial class ExecutionReasonForm : Form
    {
        private Label userNameLabel;
        private TextBox userNameTextBox;
        private Label executionReasonLabel;
        private TextBox executionReasonTextBox;
        private Button okButton;
        private bool forceClose = false;

        public string UserName { get; private set; }
        public string ExecutionReason { get; private set; }

        public ExecutionReasonForm()
        {
            InitializeComponent();

            // Configuración del formulario
            Text = "Registro de acceso";
            BackColor = Color.LightGray;
            Font = new Font("Arial", 10);
            Padding = new Padding(20);
            WindowState = FormWindowState.Normal;

            userNameLabel = new Label();
            userNameLabel.Text = "Nombre:";
            userNameLabel.Location = new Point(10, 20);
            userNameLabel.Font = new Font("Arial", 10, FontStyle.Bold);
            userNameLabel.ForeColor = Color.DarkBlue;

            userNameTextBox = new TextBox();
            userNameTextBox.Location = new Point(10, 50);
            userNameTextBox.Size = new Size(250, 20);

            executionReasonLabel = new Label();
            executionReasonLabel.Text = "Motivo:";
            executionReasonLabel.Location = new Point(10, 90);
            executionReasonLabel.Font = new Font("Arial", 10, FontStyle.Bold);
            executionReasonLabel.ForeColor = Color.DarkBlue;

            executionReasonTextBox = new TextBox();
            executionReasonTextBox.Location = new Point(10, 120);
            executionReasonTextBox.Size = new Size(250, 20);

            okButton = new Button();
            okButton.Text = "OK";
            okButton.Location = new Point(10, 160);
            okButton.Size = new Size(250, 30);
            okButton.BackColor = Color.LightBlue;
            okButton.Click += okButton_Click;

            Controls.Add(userNameLabel);
            Controls.Add(userNameTextBox);
            Controls.Add(executionReasonLabel);
            Controls.Add(executionReasonTextBox);
            Controls.Add(okButton);

            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            StartPosition = FormStartPosition.CenterScreen;

            // Establecer okButton como el botón de aceptación del formulario
            AcceptButton = okButton;

            // Agrega el manejador de eventos FormClosing
            this.FormClosing += ExecutionReasonForm_FormClosing;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(userNameTextBox.Text) || string.IsNullOrWhiteSpace(executionReasonTextBox.Text))
            {
                MessageBox.Show("Por favor, rellene todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                UserName = userNameTextBox.Text;
                ExecutionReason = executionReasonTextBox.Text;
                DialogResult = DialogResult.OK;
                forceClose = true;
                Close();
            }
        }

        private void ExecutionReasonForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Si el usuario está intentando cerrar el formulario y no se ha forzado el cierre, cancela el evento de cierre
            if (e.CloseReason == CloseReason.UserClosing && !forceClose)
            {
                MessageBox.Show("Por favor, rellene todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }
    }
}
