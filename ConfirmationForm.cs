﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestionBBDD
{
    public partial class ConfirmationForm : Form
    {
        public bool DoNotAskAgain { get; private set; }

        public ConfirmationForm()
        {
            InitializeComponent();
            Text = "Confirmación";
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.LightGray;

            // Agrega el manejador de eventos FormClosing
            this.FormClosing += ConfirmationForm_FormClosing;

            TableLayoutPanel panel = new TableLayoutPanel() { Dock = DockStyle.Fill, ColumnCount = 1, Padding = new Padding(20), AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            Controls.Add(panel);

            Label label = new Label() { Text = "¿Estás seguro de que quieres eliminar la fila seleccionada?", AutoSize = true, TextAlign = ContentAlignment.MiddleCenter, Padding = new Padding(20), Font = new Font("Arial", 14) };
            panel.Controls.Add(label, 0, 0);

            CheckBox checkBox = new CheckBox() { Text = "No volver a preguntar hasta el reinicio del programa", AutoSize = true, Padding = new Padding(20), Font = new Font("Arial", 12) };
            checkBox.CheckedChanged += (sender, e) => DoNotAskAgain = checkBox.Checked;
            panel.Controls.Add(checkBox, 0, 1);

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel() { FlowDirection = FlowDirection.RightToLeft, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(20) };
            panel.Controls.Add(buttonPanel, 0, 2);

            Button noButton = new Button() { Text = "No", DialogResult = DialogResult.No, Margin = new Padding(5), Font = new Font("Arial", 12), Size = new Size(100, 30), BackColor = Color.LightSalmon };
            buttonPanel.Controls.Add(noButton);

            Button yesButton = new Button() { Text = "Sí", DialogResult = DialogResult.Yes, Margin = new Padding(5), Font = new Font("Arial", 12), Size = new Size(100, 30), BackColor = Color.LightGreen };
            buttonPanel.Controls.Add(yesButton);
        }



        private void ConfirmationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Si el usuario cierra el formulario sin seleccionar una opción, cambia el DialogResult a Cancel
            if (this.DialogResult == DialogResult.None)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}