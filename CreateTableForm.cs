using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace GestionBBDD
{
    public partial class CreateTableForm : Form
    {
        private TextBox tableNameTextBox;
        private DataGridView columnsDataGridView;
        private Button okButton;
        private Button cancelButton;

        public string TableName => tableNameTextBox.Text;

        public List<(string ColumnName, string ColumnType, bool PrimaryKey, bool Unique, bool NotNull, bool Check)> Columns
        {
            get
            {
                List<(string ColumnName, string ColumnType, bool PrimaryKey, bool Unique, bool NotNull, bool Check)> columns = new List<(string ColumnName, string ColumnType, bool PrimaryKey, bool Unique, bool NotNull, bool Check)>();
                foreach (DataGridViewRow row in columnsDataGridView.Rows)
                {
                    if (row.IsNewRow) continue;
                    string columnName = (string)row.Cells[0].Value;
                    string columnType = (string)row.Cells[1].Value;
                    bool primaryKey = Convert.ToBoolean(row.Cells["PrimaryKey"].Value);
                    bool unique = Convert.ToBoolean(row.Cells["Unique"].Value);
                    bool notNull = Convert.ToBoolean(row.Cells["NotNull"].Value);
                    bool check = Convert.ToBoolean(row.Cells["Check"].Value);
                    columns.Add((columnName, columnType, primaryKey, unique, notNull, check));
                }
                return columns;
            }
        }

        public CreateTableForm()
        {
            InitializeComponent();
            InitializeControls();
            InitializeEventHandlers();
        }

        private void InitializeControls()
        {
            Text = "Crear tabla";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            tableNameTextBox = new TextBox();
            tableNameTextBox.Dock = DockStyle.Top;
            tableNameTextBox.Font = new Font("Arial", 10, FontStyle.Bold);

            Label tableNameLabel = new Label();
            tableNameLabel.Text = "Nombre de la tabla:";
            tableNameLabel.Dock = DockStyle.Top;

            Label columnsLabel = new Label();
            columnsLabel.Text = "Columnas:";
            columnsLabel.Dock = DockStyle.Top;

            columnsDataGridView = new DataGridView();
            columnsDataGridView.Dock = DockStyle.Fill;
            columnsDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            columnsDataGridView.Columns.Add("ColumnName", "Nombre de la columna");
            DataGridViewComboBoxColumn columnTypeColumn = new DataGridViewComboBoxColumn();
            columnTypeColumn.Name = "ColumnType";
            columnTypeColumn.HeaderText = "Tipo de la columna";
            columnTypeColumn.Items.AddRange("INT", "VARCHAR", "TEXT", "DATE", "REAL", "BOOLEAN");
            columnsDataGridView.Columns.Add(columnTypeColumn);

            DataGridViewCheckBoxColumn primaryKeyColumn = new DataGridViewCheckBoxColumn();
            primaryKeyColumn.Name = "PrimaryKey";
            primaryKeyColumn.HeaderText = "PRIMARY KEY";
            columnsDataGridView.Columns.Add(primaryKeyColumn);

            DataGridViewCheckBoxColumn uniqueColumn = new DataGridViewCheckBoxColumn();
            uniqueColumn.Name = "Unique";
            uniqueColumn.HeaderText = "UNIQUE";
            columnsDataGridView.Columns.Add(uniqueColumn);

            DataGridViewCheckBoxColumn notNullColumn = new DataGridViewCheckBoxColumn();
            notNullColumn.Name = "NotNull";
            notNullColumn.HeaderText = "NOT NULL";
            columnsDataGridView.Columns.Add(notNullColumn);

            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
            checkColumn.Name = "Check";
            checkColumn.HeaderText = "CHECK";
            columnsDataGridView.Columns.Add(checkColumn);

            FlowLayoutPanel buttonsPanel = new FlowLayoutPanel();
            buttonsPanel.Dock = DockStyle.Bottom;

            cancelButton = CreateButton("Cancelar", Color.LightGray, Color.DarkGray);
            cancelButton.Click += (sender, e) => this.DialogResult = DialogResult.Cancel; // Cierra el formulario y establece el resultado del diálogo en Cancelar
            okButton = CreateButton("OK", Color.LightBlue, Color.DarkBlue);
            okButton.Click += (sender, e) =>
            {
                if (string.IsNullOrEmpty(TableName)) // Verifica que se haya introducido un nombre de tabla
                {
                    MessageBox.Show("Por favor, introduce un nombre de tabla.");
                    return;
                }
                foreach (var column in Columns)
                {
                    if (string.IsNullOrEmpty(column.ColumnName) || string.IsNullOrEmpty(column.ColumnType)) // Verifica que se haya introducido un nombre y un tipo de columna
                    {
                        MessageBox.Show("Por favor, introduce un nombre y un tipo para cada columna.");
                        return;
                    }
                }
                this.DialogResult = DialogResult.OK; // Cierra el formulario y establece el resultado del diálogo en OK
            };
            Button addRowButton = CreateButton("Añadir fila", Color.LightGreen, Color.DarkGreen);
            addRowButton.Click += (sender, e) => columnsDataGridView.Rows.Add(); // Añade una nueva fila al hacer clic en el botón
            Button deleteRowButton = CreateButton("Eliminar fila", Color.LightCoral, Color.DarkRed);
            deleteRowButton.Click += (sender, e) =>
            {
                // Verifica que haya una fila seleccionada
                if (columnsDataGridView.SelectedRows.Count > 0)
                {
                    // Recorre todas las filas seleccionadas
                    foreach (DataGridViewRow row in columnsDataGridView.SelectedRows)
                    {
                        if (!row.IsNewRow) // No elimina la fila de nueva entrada
                        {
                            columnsDataGridView.Rows.Remove(row);
                        }
                    }
                }
            };

            buttonsPanel.Controls.AddRange(new Control[] { cancelButton, okButton, addRowButton, deleteRowButton });

            Controls.AddRange(new Control[] { buttonsPanel, columnsDataGridView, columnsLabel, tableNameTextBox, tableNameLabel });
        }

        private void InitializeEventHandlers()
        {
            columnsDataGridView.CellValidating += ColumnsDataGridView_CellValidating;
        }

        private Button CreateButton(string text, Color backColor, Color borderColor)
        {
            Button button = new Button();
            button.Text = text;
            button.BackColor = backColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = borderColor;
            button.FlatAppearance.BorderSize = 1;
            button.Font = new Font("Arial", 10, FontStyle.Bold);
            button.AutoSize = true;
            return button;
        }

        private void ColumnsDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (columnsDataGridView.Columns[e.ColumnIndex].Name == "Constraint")
            {
                string constraint = (string)e.FormattedValue;
                if (!string.IsNullOrEmpty(constraint) && !IsValidConstraint(constraint))
                {
                    e.Cancel = true;
                    MessageBox.Show("La restricción introducida no es válida. Por favor, introduce una restricción válida (PRIMARY KEY, UNIQUE, etc.).");
                }
            }
        }

        private bool IsValidConstraint(string constraint)
        {
            return constraint == "PRIMARY KEY" || constraint == "UNIQUE" || constraint == "NOT NULL" || constraint.StartsWith("CHECK");
        }
    }
}
