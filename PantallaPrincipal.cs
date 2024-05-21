using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestionBBDD
{
    public partial class PantallaPrincipal : Form
    {
        private OdbcConnection conn;
        private DataGridView dataGridView;
        private ComboBox tableNameComboBox;
        private Button insertButton;
        private Button editButton;
        private Button deleteButton;
        private bool? deleteConfirmation = null;

        public PantallaPrincipal()
        {
            InitializeComponent();
            conn = ConnectDatabase();

            // Crea un nuevo Panel con AutoScroll habilitado
            Panel panel = new Panel
            {
                AutoScroll = true,
                Dock = DockStyle.Fill
            };
            Controls.Add(panel);
            HScroll = true;
            // Llama a CreateControls pasando el panel como argumento
            CreateControls(panel);

            LoadTableNames();
        }

        private OdbcConnection ConnectDatabase()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Access Files|*.accdb";
                openFileDialog.Title = "Selecciona una base de datos Access";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string dbPath = openFileDialog.FileName;

                    // Crear una copia de seguridad de la base de datos
                    string backupPath = Path.Combine(Path.GetDirectoryName(dbPath), Path.GetFileNameWithoutExtension(dbPath) + "_backup.accdb");
                    File.Copy(dbPath, backupPath, true);

                    OdbcConnection conn = new OdbcConnection(
                        $@"Driver={{Microsoft Access Driver (*.mdb, *.accdb)}};Dbq={dbPath};");

                    conn.Open();

                    // Mostrar el formulario para recoger el nombre y el motivo de la ejecución
                    ExecutionReasonForm form = new ExecutionReasonForm();
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // Insertar los datos en la base de datos
                        string sql = "INSERT INTO Registros_Acceso (Nombre, Motivo, FechaHora) VALUES (?, ?, ?)";
                        using (OdbcCommand cmd = new OdbcCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@Nombre", form.UserName);
                            cmd.Parameters.AddWithValue("@Motivo", form.ExecutionReason);
                            cmd.Parameters.AddWithValue("@FechaHora", DateTime.Now);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    return conn;
                }
                else
                {
                    MessageBox.Show("No se seleccionó ninguna base de datos.");
                    Application.Exit();
                    return null;
                }
            }
            catch (OdbcException ex)
            {
                MessageBox.Show("No es posible conectar con la base de datos: " + ex.Message);
                Application.Exit();
                return null;
            }
            catch (IOException ex)
            {
                MessageBox.Show("No se pudo crear una copia de seguridad de la base de datos: " + ex.Message);
                Application.Exit();
                return null;
            }
        }

        //Método para crear los controles de la interfaz
        private void CreateControls(Panel panel)
        {
            tableNameComboBox = new ComboBox();
            tableNameComboBox.Dock = DockStyle.Top;
            tableNameComboBox.SelectedIndexChanged += tableNameComboBox_SelectedIndexChanged; // Asigna el manejador de eventos
            tableNameComboBox.DropDownStyle = ComboBoxStyle.DropDownList; // Cambia el estilo de la caja de combinación
            tableNameComboBox.Font = new Font("Arial", 10, FontStyle.Bold); // Cambia la fuente
            tableNameComboBox.BackColor = Color.LightGray; // Cambia el color de fondo
            panel.Controls.Add(tableNameComboBox);

            // Crea los botones para insertar, editar y eliminar datos
            insertButton = new Button();
            insertButton.Text = "Insertar datos";
            insertButton.Dock = DockStyle.Top;
            insertButton.Click += insertButton_Click; // Asigna el manejador de eventos
            insertButton.AutoSize = true;
            panel.Controls.Add(insertButton);

            editButton = new Button();
            editButton.Text = "Editar datos";
            editButton.Dock = DockStyle.Top;
            editButton.Click += editButton_Click; // Asigna el manejador de eventos
            editButton.AutoSize = true;
            panel.Controls.Add(editButton);

            deleteButton = new Button();
            deleteButton.Text = "Eliminar datos";
            deleteButton.Dock = DockStyle.Top;
            deleteButton.Click += deleteButton_Click; // Asigna el manejador de eventos
            deleteButton.AutoSize = true;
            panel.Controls.Add(deleteButton);

            // Crea el botón de créditos
            Button creditosButton = new Button();
            creditosButton.Text = "Créditos";
            creditosButton.Dock = DockStyle.Top;
            creditosButton.Click += creditosButton_Click; // Asigna el manejador de eventos
            creditosButton.AutoSize = true;
            panel.Controls.Add(creditosButton);

            // Configura los colores y estilos del botón de créditos
            creditosButton.BackColor = Color.Khaki;
            creditosButton.FlatStyle = FlatStyle.Flat;
            creditosButton.FlatAppearance.BorderColor = Color.DeepSkyBlue;
            creditosButton.FlatAppearance.BorderSize = 1;
            creditosButton.Font = new Font("Arial", 10, FontStyle.Bold);

            // Crea el botón para crear tablas
            Button createTableButton = new Button();
            createTableButton.Text = "Crear tabla";
            createTableButton.Dock = DockStyle.Top;
            createTableButton.Click += createTableButton_Click; // Asigna el manejador de eventos
            createTableButton.AutoSize = true;
            panel.Controls.Add(createTableButton);

            // Configura los colores y estilos del botón para crear tablas
            createTableButton.BackColor = Color.LightBlue;
            createTableButton.FlatStyle = FlatStyle.Flat;
            createTableButton.FlatAppearance.BorderColor = Color.LightBlue;
            createTableButton.FlatAppearance.BorderSize = 1;
            createTableButton.Font = new Font("Arial", 10, FontStyle.Bold);

            // Crea el botón para crear relaciones
            Button createRelationButton = new Button();
            createRelationButton.Text = "Crear relación";
            createRelationButton.Dock = DockStyle.Top;
            createRelationButton.Click += createRelationButton_Click; // Asigna el manejador de eventos
            createRelationButton.AutoSize = true;
            panel.Controls.Add(createRelationButton);

            // Configura los colores y estilos del botón para crear relaciones
            createRelationButton.BackColor = Color.LightCoral;
            createRelationButton.FlatStyle = FlatStyle.Flat;
            createRelationButton.FlatAppearance.BorderColor = Color.LightCoral;
            createRelationButton.FlatAppearance.BorderSize = 1;
            createRelationButton.Font = new Font("Arial", 10, FontStyle.Bold);

            dataGridView = new BufferedDataGridView(); // Utiliza un DataGridView personalizado para mejorar el rendimiento
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.ReadOnly = true; // Hace que todas las celdas sean de solo lectura
            panel.Controls.Add(dataGridView);
            FormBorderStyle = FormBorderStyle.Sizable;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.MultiSelect = true;

            panel.Controls.SetChildIndex(dataGridView, 0);
            panel.Controls.SetChildIndex(deleteButton, 1);
            panel.Controls.SetChildIndex(editButton, 2);
            panel.Controls.SetChildIndex(insertButton, 3);
            panel.Controls.SetChildIndex(creditosButton, 4);
            panel.Controls.SetChildIndex(createTableButton, 5);
            panel.Controls.SetChildIndex(createRelationButton, 6);
            panel.Controls.SetChildIndex(tableNameComboBox, 7);

            // Configura los colores y estilos de los botones
            insertButton.BackColor = Color.MediumSeaGreen;
            insertButton.FlatStyle = FlatStyle.Flat;
            insertButton.FlatAppearance.BorderColor = Color.SeaGreen;
            insertButton.FlatAppearance.BorderSize = 1;
            insertButton.Font = new Font("Arial", 10, FontStyle.Bold);

            editButton.BackColor = Color.CornflowerBlue;
            editButton.FlatStyle = FlatStyle.Flat;
            editButton.FlatAppearance.BorderColor = Color.RoyalBlue;
            editButton.FlatAppearance.BorderSize = 1;
            editButton.Font = new Font("Arial", 10, FontStyle.Bold);

            deleteButton.BackColor = Color.IndianRed;
            deleteButton.FlatStyle = FlatStyle.Flat;
            deleteButton.FlatAppearance.BorderColor = Color.Firebrick;
            deleteButton.FlatAppearance.BorderSize = 1;
            deleteButton.Font = new Font("Arial", 10, FontStyle.Bold);


            // Configura los colores y estilos del DataGridView
            dataGridView.EnableHeadersVisualStyles = false; // Permite cambiar el estilo de las cabeceras
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy; // Cambia el color de fondo de las cabeceras
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White; // Cambia el color de la fuente de las cabeceras
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView.Font, FontStyle.Bold); // Cambia la fuente de las cabeceras
            dataGridView.DefaultCellStyle.Font = new Font("Arial", 10); // Cambia la fuente de las celdas
            dataGridView.DefaultCellStyle.BackColor = Color.Beige; // Cambia el color de fondo de las celdas
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.Teal; // Cambia el color de fondo de las celdas seleccionadas
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.White; // Cambia el color de la fuente de las celdas seleccionadas
        }

        private async void refreshButton_Click(object sender, EventArgs e)
        {
            LoadTableNames();
            await FetchAndDisplayDataAsync();
        }

        // Manejador de eventos para el botón de creación de relaciones
        private void createRelationButton_Click(object sender, EventArgs e)
        {
            // Crea una nueva instancia del formulario de creación de relaciones y lo muestra como un diálogo modal
            CreateRelationForm form = new CreateRelationForm(conn);
            form.StartPosition = FormStartPosition.CenterParent;       
            if (form.ShowDialog() == DialogResult.OK)
            {
                // Recoge la información del formulario
                string table1 = form.Table1;
                string column1 = form.Column1;
                string table2 = form.Table2;
                string column2 = form.Column2;

                // Crea la relación en la base de datos
                string sql = $"ALTER TABLE [{table1}] ADD CONSTRAINT FK_{table1}_{table2} FOREIGN KEY ([{column1}]) REFERENCES [{table2}] ([{column2}])";
                using (OdbcCommand cmd = new OdbcCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Manejador de eventos para el botón de creación de tablas
        private void createTableButton_Click(object sender, EventArgs e)
        {
            // Crea una nueva instancia del formulario de creación de tablas y lo muestra como un diálogo modal
            CreateTableForm form = new CreateTableForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                // Recoge la información del formulario
                string tableName = form.TableName;
                List<(string ColumnName, string ColumnType, bool PrimaryKey, bool Unique, bool NotNull, bool Check)> columns = form.Columns;

                // Check if tableName and columns are not empty
                if (string.IsNullOrWhiteSpace(tableName) || columns.Count == 0)
                {
                    MessageBox.Show("El nombre de la tabla y las columnas no pueden estar vacíos.");
                    return;
                }

                // Crea la tabla en la base de datos
                string sql = $"CREATE TABLE [{tableName}] ({string.Join(", ", columns.Select(column => $"[{column.ColumnName}] {column.ColumnType} " + (column.PrimaryKey ? "PRIMARY KEY, " : "") + (column.Unique ? "UNIQUE, " : "") + (column.NotNull ? "NOT NULL, " : "") + (column.Check ? "CHECK" : "")))})";
                using (OdbcCommand cmd = new OdbcCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Recarga los nombres de las tablas
                LoadTableNames();
            }
        }

        // Manejador de eventos para el botón de créditos
        private void creditosButton_Click(object sender, EventArgs e)
        {
            // Crea una nueva instancia del formulario de créditos y lo muestra como un diálogo modal
            CreditosForm form = new CreditosForm();
            form.ShowDialog();
        }

        //Método para cargar los datos de la tabla seleccionada
        private async void tableNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            await FetchAndDisplayDataAsync();
        }

        //Método para cargar los nombres de las tablas
        private void LoadTableNames()
        {
            try
            {
                // Limpia los elementos existentes
                tableNameComboBox.Items.Clear();

                DataTable schema = conn.GetSchema("Tables");
                foreach (DataRow row in schema.Rows)
                {
                    string tableName = (string)row[2];
                    tableNameComboBox.Items.Add(tableName);
                }
            }
            catch (OdbcException ex)
            {
                MessageBox.Show("Error al obtener los nombres de las tablas: " + ex.Message);
            }
        }

        //Método para obtener y mostrar los datos de la tabla seleccionada
        private async Task FetchAndDisplayDataAsync()
        {
            string tableName = (string)tableNameComboBox.SelectedItem;
            try
            {
                using (OdbcCommand cmd = new OdbcCommand($"SELECT * FROM [{tableName}]", conn))
                {
                    using (OdbcDataReader reader = (OdbcDataReader)await cmd.ExecuteReaderAsync())
                    {
                        DataTable table = new DataTable();
                        table.Load(reader);
                        dataGridView.DataSource = table;
                    }
                }
            }
            catch (OdbcException ex)
            {
                MessageBox.Show("Error al obtener datos de la base de datos: " + ex.Message);
            }
        }

        //Manejador de eventos para el botón de inserción
        private async void insertButton_Click(object sender, EventArgs e)
        {
            DataTable schema = conn.GetSchema("Columns", new string[] { null, null, tableNameComboBox.SelectedItem as string });
            List<string> columnNames = new List<string>();
            foreach (DataRow row in schema.Rows)
            {
                string columnName = row["COLUMN_NAME"] as string;
                columnNames.Add(columnName);
            }

            string connectionString = conn.ConnectionString;
            string tableName = tableNameComboBox.SelectedItem?.ToString();
            if (tableName == null)
            {
                MessageBox.Show("Por favor, selecciona una tabla.");
                return;
            }

            if (tableName == "Registros_Acceso")
            {
                MessageBox.Show("No se pueden añadir datos a la tabla Registros_Acceso.");
                return;
            }

            DataForm form = new DataForm(connectionString, tableName, false);
            if (form.ShowDialog() == DialogResult.OK)
            {
                string sql = $"INSERT INTO [{tableNameComboBox.SelectedItem}] ({string.Join(", ", columnNames)}) VALUES ({string.Join(", ", columnNames.Select(name => "?"))})";

                using (OdbcCommand cmd = new OdbcCommand(sql, conn))
                {
                    foreach (string name in columnNames)
                    {
                        var value = form.GetFieldValue(name).ToString();
                        int maxLength = 255;
                        if (value.Length > maxLength)
                        {
                            MessageBox.Show($"El valor para {name} excede la longitud máxima permitida. Se truncará a {maxLength} caracteres.");
                            value = value.Substring(0, maxLength);
                        }
                        cmd.Parameters.AddWithValue("@" + name, value);
                    }
                    cmd.ExecuteNonQuery();
                }

                await FetchAndDisplayDataAsync();
            }
        }

        //Manejador de eventos para el botón de edición
        private async void editButton_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, selecciona una fila para editar.");
                return;
            }

            DataTable schema = conn.GetSchema("Columns", new string[] { null, null, tableNameComboBox.SelectedItem as string });
            List<string> columnNames = new List<string>();
            foreach (DataRow row in schema.Rows)
            {
                columnNames.Add(row["COLUMN_NAME"] as string);
            }

            // Exclude 'Id' from the columnNames list
            columnNames.Remove("Id");

            string connectionString = conn.ConnectionString;
            string tableName = tableNameComboBox.SelectedItem.ToString();
            if (tableName == "Registros_Acceso")
            {
                MessageBox.Show("No se pueden editar datos en la tabla Registros_Acceso.");
                return;
            }

            DataForm form = new DataForm(connectionString, tableName, false);
            foreach (string name in columnNames)
            {
                form.SetFieldValue(name, dataGridView.SelectedRows[0].Cells[name].Value);
            }
            if (form.ShowDialog() == DialogResult.OK)
            {
                string sql = $"UPDATE [{tableNameComboBox.SelectedItem}] SET {string.Join(", ", columnNames.Select(name => name + " = ?"))} WHERE {columnNames[0]} = ?";

                using (OdbcCommand cmd = new OdbcCommand(sql, conn))
                {
                    foreach (string name in columnNames)
                    {
                        var value = form.GetFieldValue(name).ToString();
                        int maxLength = 255;
                        if (value.Length > maxLength)
                        {
                            MessageBox.Show($"El valor para {name} excede la longitud máxima permitida. Se truncará a {maxLength} caracteres.");
                            value = value.Substring(0, maxLength);
                        }
                        cmd.Parameters.AddWithValue("@" + name, value);
                    }
                    cmd.Parameters.AddWithValue("@" + columnNames[0], dataGridView.SelectedRows[0].Cells[columnNames[0]].Value);
                    cmd.ExecuteNonQuery();
                }

                await FetchAndDisplayDataAsync();
            }
        }

        //Manejador de eventos para el botón de eliminación
        private async void deleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, selecciona al menos una fila para eliminar.");
                return;
            }
            string tableName = tableNameComboBox.SelectedItem?.ToString();

            if (tableName == "Registros_Acceso")
            {
                MessageBox.Show("No se pueden eliminar datos de la tabla Registros_Acceso.");
                return;
            }

            if (deleteConfirmation == null || deleteConfirmation == false)
            {
                ConfirmationForm form = new ConfirmationForm();
                var result = form.ShowDialog();
                if (result == DialogResult.No || result == DialogResult.Cancel)
                {
                    return;
                }
                deleteConfirmation = form.DoNotAskAgain;
            }

            DataTable schema = conn.GetSchema("Columns", new string[] { null, null, tableNameComboBox.SelectedItem as string });
            List<string> columnNames = new List<string>();
            foreach (DataRow row in schema.Rows)
            {
                columnNames.Add(row["COLUMN_NAME"] as string);
            }

            string sql = $"DELETE FROM [{tableNameComboBox.SelectedItem}] WHERE {columnNames[0]} = ?";

            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                using (OdbcCommand cmd = new OdbcCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@" + columnNames[0], row.Cells[columnNames[0]].Value);
                    cmd.ExecuteNonQuery();
                }
            }

            await FetchAndDisplayDataAsync();
        }
    }
}
