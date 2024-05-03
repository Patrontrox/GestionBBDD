using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestionBBDD
{
    public partial class CreateRelationForm : Form
    {
        private ComboBox table1ComboBox;
        private ComboBox column1ComboBox;
        private ComboBox table2ComboBox;
        private ComboBox column2ComboBox;
        private Button okButton;
        private Button cancelButton;
        private OdbcConnection conn;

        public string Table1 => table1ComboBox.SelectedItem?.ToString();
        public string Column1 => column1ComboBox.SelectedItem?.ToString();
        public string Table2 => table2ComboBox.SelectedItem?.ToString();
        public string Column2 => column2ComboBox.SelectedItem?.ToString();

        public CreateRelationForm(OdbcConnection conn)
        {
            InitializeComponent();
            this.conn = conn;
            ConfigureForm();
            LoadTableAndColumnNames();
        }

        private void ConfigureForm()
        {
            Text = "Crear relación entre tablas";
            BackColor = Color.LightGray;
            Font = new Font("Arial", 10);

            var tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.RowCount = 4;
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            Controls.Add(tableLayoutPanel);

            AddLabelAndComboBox(tableLayoutPanel, "Tabla 1:", out table1ComboBox);
            AddLabelAndComboBox(tableLayoutPanel, "Columna 1:", out column1ComboBox);
            AddLabelAndComboBox(tableLayoutPanel, "Tabla 2:", out table2ComboBox);
            AddLabelAndComboBox(tableLayoutPanel, "Columna 2:", out column2ComboBox);

            okButton = CreateButton("OK", Color.LightGreen, DialogResult.OK);
            cancelButton = CreateButton("Cancelar", Color.LightCoral, DialogResult.Cancel);
            var editButton = CreateButton("Editar", Color.LightYellow);
            editButton.Click += EditButton_Click;
            var deleteButton = CreateButton("Eliminar", Color.LightCoral);
            deleteButton.Click += DeleteButton_Click;

            Controls.Add(okButton);
            Controls.Add(cancelButton);
            Controls.Add(editButton);
            Controls.Add(deleteButton);

            table1ComboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            table2ComboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
        }

        private Button CreateButton(string text, Color backColor, DialogResult dialogResult = DialogResult.None)
        {
            var button = new Button();
            button.Text = text;
            button.Dock = DockStyle.Bottom;
            button.BackColor = backColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = backColor == Color.LightGreen ? Color.Green : backColor == Color.LightCoral ? Color.Red : Color.Yellow;
            button.FlatAppearance.BorderSize = 1;
            button.Font = new Font("Arial", 10, FontStyle.Bold);
            button.DialogResult = dialogResult;
            return button;
        }

        private void AddLabelAndComboBox(TableLayoutPanel tableLayoutPanel, string labelText, out ComboBox comboBox)
        {
            var label = new Label();
            label.Text = labelText;
            tableLayoutPanel.Controls.Add(label);

            comboBox = new ComboBox();
            comboBox.Dock = DockStyle.Fill;
            tableLayoutPanel.Controls.Add(comboBox);
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
            DataTable tables = conn.GetSchema("Tables");
            foreach (DataRow row in tables.Rows)
            {
                string tableName = row.Field<string>("TABLE_NAME");
                comboBox.Items.Add(tableName);
            }
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var comboBox = (ComboBox)sender;
            ComboBox columnComboBox = null;
            string selectedTable = comboBox.SelectedItem?.ToString();

            if (comboBox == table1ComboBox)
                columnComboBox = column1ComboBox;
            else if (comboBox == table2ComboBox)
                columnComboBox = column2ComboBox;

            if (selectedTable != null)
                LoadColumnNames(selectedTable, columnComboBox);
        }

        private void LoadColumnNames(string tableName, ComboBox columnComboBox)
        {
            columnComboBox.Items.Clear();
            DataTable columns = conn.GetSchema("Columns", new string[] { null, null, tableName });
            foreach (DataRow row in columns.Rows)
            {
                string columnName = row.Field<string>("COLUMN_NAME");
                columnComboBox.Items.Add(columnName);
            }
            if (columnComboBox.Items.Count > 0)
                columnComboBox.SelectedIndex = 0;
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            string table1 = Table1;
            string column1 = Column1;
            string table2 = Table2;
            string column2 = Column2;

            var editForm = new EditRelationForm(table1, column1, table2, column2, conn);
            editForm.Show();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            string table1 = Table1;
            string table2 = Table2;

            try
            {
                using (var command = new OdbcCommand($"ALTER TABLE {table1} DROP CONSTRAINT fk_{table1}_{table2}", conn))
                {
                    command.ExecuteNonQuery();
                }
                MessageBox.Show("Relación eliminada con éxito.");

                table1ComboBox.Items.Clear();
                table2ComboBox.Items.Clear();
                LoadTableNames(table1ComboBox);
                LoadTableNames(table2ComboBox);
            }
            catch (OdbcException ex)
            {
                MessageBox.Show("Error al eliminar la relación: " + ex.Message);
            }
        }
    }
}
