using System;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Windows.Forms;

namespace GestionBBDD
{
    public partial class EditRelationForm : Form
    {
        private ComboBox table1ComboBox;
        private ComboBox column1ComboBox;
        private ComboBox table2ComboBox;
        private ComboBox column2ComboBox;
        private OdbcConnection conn;
        private Button okButton;
        private Button cancelButton;

        public EditRelationForm(string table1, string column1, string table2, string column2, OdbcConnection conn)
        {
            InitializeComponent();
            this.conn = conn;

            table1ComboBox = CreateComboBox(new Point(150, 30));
            column1ComboBox = CreateComboBox(new Point(150, 70));
            table2ComboBox = CreateComboBox(new Point(150, 110));
            column2ComboBox = CreateComboBox(new Point(150, 150));

            table1ComboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            table2ComboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;

            Label table1Label = CreateLabel("Tabla 1:", new Point(30, 30));
            Label column1Label = CreateLabel("Columna 1:", new Point(30, 70));
            Label table2Label = CreateLabel("Tabla 2:", new Point(30, 110));
            Label column2Label = CreateLabel("Columna 2:", new Point(30, 150));

            okButton = CreateButton("OK", Color.LightGreen);
            okButton.Click += OkButton_Click;
            cancelButton = CreateButton("Cancelar", Color.LightCoral);
            cancelButton.Click += CancelButton_Click;

            Controls.AddRange(new Control[] { table1ComboBox, column1ComboBox, table2ComboBox, column2ComboBox,
                table1Label, column1Label, table2Label, column2Label, okButton, cancelButton });

            LoadTableAndColumnNames();

            table1ComboBox.SelectedItem = table1;
            column1ComboBox.SelectedItem = column1;
            table2ComboBox.SelectedItem = table2;
            column2ComboBox.SelectedItem = column2;
        }

        private ComboBox CreateComboBox(Point location)
        {
            return new ComboBox { Location = location, Width = 200 };
        }

        private Label CreateLabel(string text, Point location)
        {
            return new Label { Text = text, Location = location };
        }

        private Button CreateButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Dock = DockStyle.Bottom,
                BackColor = backColor,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderColor = backColor == Color.LightGreen ? Color.Green : Color.Red },
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
        }

        private void LoadTableAndColumnNames()
        {
            try
            {
                LoadTableNames(table1ComboBox);
                LoadTableNames(table2ComboBox);
            }
            catch (OdbcException ex)
            {
                MessageBox.Show("Error al obtener los nombres de las tablas y columnas: " + ex.Message);
            }
        }

        private void LoadTableNames(ComboBox comboBox)
        {
            DataTable items = conn.GetSchema("Tables");
            foreach (DataRow row in items.Rows)
            {
                string itemName = row.Field<string>("TABLE_NAME");
                comboBox.Items.Add(itemName);
            }
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        private void LoadComboBoxItems(ComboBox comboBox)
        {
            DataTable items = conn.GetSchema(comboBox == table1ComboBox ? "Tables" : "Columns");
            foreach (DataRow row in items.Rows)
            {
                string itemName = row.Field<string>(comboBox == table1ComboBox ? "TABLE_NAME" : "COLUMN_NAME");
                comboBox.Items.Add(itemName);
            }
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var relatedComboBox = comboBox == table1ComboBox ? column1ComboBox : column2ComboBox;

            // Limpiar ComboBox de columna relacionada
            relatedComboBox.Items.Clear();

            if (comboBox.SelectedItem != null)
            {
                // Cargar las columnas relacionadas
                string tableName = comboBox.SelectedItem.ToString();
                LoadColumnNames(relatedComboBox, tableName);
            }
        }

        private void LoadColumnNames(ComboBox comboBox, string tableName)
        {
            DataTable items = conn.GetSchema("Columns", new string[] { null, null, tableName });
            foreach (DataRow row in items.Rows)
            {
                string itemName = row.Field<string>("COLUMN_NAME");
                comboBox.Items.Add(itemName);
            }
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }



        private void OkButton_Click(object sender, EventArgs e)
        {
            string oldTable1 = table1ComboBox.SelectedItem.ToString();
            string oldTable2 = table2ComboBox.SelectedItem.ToString();
            string newTable1 = table1ComboBox.SelectedItem.ToString();
            string newTable2 = table2ComboBox.SelectedItem.ToString();
            string newColumn1 = column1ComboBox.SelectedItem.ToString();
            string newColumn2 = column2ComboBox.SelectedItem.ToString();

            try
            {
                using (var command = new OdbcCommand($"ALTER TABLE {oldTable1} DROP CONSTRAINT fk_{oldTable1}_{oldTable2}", conn))
                    command.ExecuteNonQuery();

                using (var command = new OdbcCommand($"ALTER TABLE {newTable1} ADD CONSTRAINT fk_{newTable1}_{newTable2} FOREIGN KEY ({newColumn1}) REFERENCES {newTable2}({newColumn2})", conn))
                    command.ExecuteNonQuery();

                MessageBox.Show("Relación editada con éxito.");
                DialogResult = DialogResult.OK;
            }
            catch (OdbcException ex)
            {
                MessageBox.Show("Error al editar la relación: " + ex.Message);
            }
        }

        private void LoadComboBoxItems(ComboBox comboBox, string tableName)
        {
            DataTable items = conn.GetSchema("Columns", new string[] { null, null, tableName });
            foreach (DataRow row in items.Rows)
            {
                string itemName = row.Field<string>("COLUMN_NAME");
                comboBox.Items.Add(itemName);
            }
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }


        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
