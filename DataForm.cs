using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace GestionBBDD
{
    public partial class DataForm : Form
    {
        private Dictionary<string, Control> fields = new Dictionary<string, Control>();
        private Button okButton;
        private Button cancelButton;

        public DataForm()
        {
            InitializeComponent();
        }

        // Constructor que recibe un array de nombres de campos y un booleano para indicar si el formulario es de solo lectura
        public DataForm(string connectionString, string tableName, bool isReadOnly = false)
        {
            // Conexión a la base de datos Access
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                connection.Open();

                // Obtiene la información del esquema de la tabla
                DataTable schemaTable = connection.GetSchema(OdbcMetaDataCollectionNames.Columns, new string[] { null, null, tableName, null });

                // Crea una lista para almacenar los nombres de los campos
                List<string> fieldNames = new List<string>();

                // Recorre las filas de la tabla de esquema
                foreach (DataRow row in schemaTable.Rows)
                {
                    // Obtiene el nombre de la columna
                    string columnName = row["COLUMN_NAME"].ToString();

                    // Añade el nombre de la columna a la lista de nombres de campos
                    fieldNames.Add(columnName);
                }

                // Configura el formulario
                Text = "Formulario de datos";
                StartPosition = FormStartPosition.CenterScreen;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;

                // Crea un TableLayoutPanel para contener los TextBoxes y Labels
                TableLayoutPanel panel = new TableLayoutPanel();
                panel.ColumnCount = 2;
                panel.RowCount = fieldNames.Count;
                panel.Dock = DockStyle.Fill;
                panel.AutoSize = true;
                panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                Controls.Add(panel);



                // Crea los TextBoxes y Labels
                for (int i = 0; i < fieldNames.Count; i++)
                {

                    Label label = new Label();
                    label.Text = fieldNames[i];
                    label.Font = new Font("Arial", 10, FontStyle.Bold); // Cambia la fuente
                    label.ForeColor = Color.DarkBlue; // Cambia el color de la fuente
                    panel.Controls.Add(label, 0, i);

                    if (fieldNames[i] == "ID")
                    {
                        TextBox textBox = new TextBox();
                        textBox.Size = new Size(150, textBox.Size.Height); // Cambia el ancho del TextBox a 200
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                        textBox.Font = new Font("Arial", 10, FontStyle.Regular); // Cambia la fuente
                        textBox.ReadOnly = false; // Hace que el campo ID sea de solo lectura siempre
                        panel.Controls.Add(textBox, 1, i);
                        fields[fieldNames[i]] = textBox;

                    }

                    else if (fieldNames[i] == "Type")
                    {
                        ComboBox comboBox = new ComboBox();
                        comboBox.Items.AddRange(new object[] { "10/100/1000BaseTX", "Unknown" });
                        comboBox.Font = new Font("Arial", 10, FontStyle.Bold); // Cambia la fuente
                        comboBox.BackColor = Color.LightGray; // Cambia el color de fondo
                        comboBox.DropDownStyle = ComboBoxStyle.DropDownList; // Cambia el estilo de la caja de combinación
                        panel.Controls.Add(comboBox, 1, i);
                        fields[fieldNames[i]] = comboBox;


                    }
                    else if (fieldNames[i] == "Speed")
                    {
                        ComboBox comboBox = new ComboBox();
                        comboBox.Items.AddRange(new object[] { "a-100", "10", "a-10", "auto", "100" });
                        comboBox.Font = new Font("Arial", 10, FontStyle.Bold); // Cambia la fuente
                        comboBox.BackColor = Color.LightGray; // Cambia el color de fondo
                        comboBox.DropDownStyle = ComboBoxStyle.DropDownList; // Cambia el estilo de la caja de combinación
                        panel.Controls.Add(comboBox, 1, i);
                        fields[fieldNames[i]] = comboBox;
                    }
                    else if (fieldNames[i] == "Duplex")
                    {
                        ComboBox comboBox = new ComboBox();
                        comboBox.Items.AddRange(new object[] { "a-full", "full", "a-half", "auto" });
                        comboBox.Font = new Font("Arial", 10, FontStyle.Bold); // Cambia la fuente
                        comboBox.BackColor = Color.LightGray; // Cambia el color de fondo
                        comboBox.DropDownStyle = ComboBoxStyle.DropDownList; // Cambia el estilo de la caja de combinación
                        panel.Controls.Add(comboBox, 1, i);
                        fields[fieldNames[i]] = comboBox;
                    }
                    else if (fieldNames[i] == "Tipo")
                    {
                        ComboBox comboBox = new ComboBox();
                        comboBox.Items.AddRange(new object[] { "Trunk", "Access" });
                        comboBox.Font = new Font("Arial", 10, FontStyle.Bold); // Cambia la fuente
                        comboBox.BackColor = Color.LightGray; // Cambia el color de fondo
                        comboBox.DropDownStyle = ComboBoxStyle.DropDownList; // Cambia el estilo de la caja de combinación
                        panel.Controls.Add(comboBox, 1, i);
                        fields[fieldNames[i]] = comboBox;
                    }
                    else if (fieldNames[i] == "Estado")
                    {
                        ComboBox comboBox = new ComboBox();
                        comboBox.Items.AddRange(new object[] { "Connected", "Disabled", "Notconnect" });
                        comboBox.Font = new Font("Arial", 10, FontStyle.Bold); // Cambia la fuente
                        comboBox.BackColor = Color.LightGray; // Cambia el color de fondo
                        comboBox.DropDownStyle = ComboBoxStyle.DropDownList; // Cambia el estilo de la caja de combinación
                        panel.Controls.Add(comboBox, 1, i);
                        fields[fieldNames[i]] = comboBox;
                    }
                    else
                    {
                        TextBox textBox = new TextBox();
                        textBox.Size = new Size(150, textBox.Size.Height); // Cambia el ancho del TextBox a 200
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                        textBox.Font = new Font("Arial", 10, FontStyle.Regular); // Cambia la fuente
                        textBox.MaxLength = 255; // Limita la longitud de entrada a 255 caracteres
                        if (i == 0 && isReadOnly) // Si es la primera columna y el formulario es de solo lectura
                        {
                            textBox.ReadOnly = true; // Hace que el campo sea de solo lectura
                        }
                        panel.Controls.Add(textBox, 1, i);
                        fields[fieldNames[i]] = textBox;

                    }
                }

                // Crea los botones OK y Cancelar
                okButton = new Button();
                okButton.Text = "OK";
                okButton.Dock = DockStyle.Bottom;
                okButton.Click += (sender, e) => DialogResult = DialogResult.OK;
                Controls.Add(okButton);

                cancelButton = new Button();
                cancelButton.Text = "Cancelar";
                cancelButton.Dock = DockStyle.Bottom;
                cancelButton.Click += (sender, e) => DialogResult = DialogResult.Cancel;
                Controls.Add(cancelButton);

                // Configura los colores y estilos del formulario
                BackColor = Color.LightGray;

                // Configura los colores y estilos de los TextBoxes y Labels
                foreach (Control control in panel.Controls)
                {
                    if (control is TextBox)
                    {
                        control.BackColor = Color.LightGray;
                        control.ForeColor = Color.Black;
                        control.Font = new Font("Arial", 10, FontStyle.Regular);

                    }
                    else if (control is Label)
                    {
                        control.ForeColor = Color.DarkBlue;
                    }
                    else if (control is ComboBox)
                    {
                        control.BackColor = Color.White;
                        control.ForeColor = Color.Black;
                    }
                }

                // Configura los colores y estilos de los botones
                okButton.BackColor = Color.MediumSeaGreen;
                okButton.FlatStyle = FlatStyle.Flat;
                okButton.FlatAppearance.BorderColor = Color.SeaGreen;
                okButton.FlatAppearance.BorderSize = 1;
                okButton.Font = new Font("Arial", 10, FontStyle.Bold);

                cancelButton.BackColor = Color.IndianRed;
                cancelButton.FlatStyle = FlatStyle.Flat;
                cancelButton.FlatAppearance.BorderColor = Color.Firebrick;
                cancelButton.FlatAppearance.BorderSize = 1;
                cancelButton.Font = new Font("Arial", 10, FontStyle.Bold);

                // Ajusta la altura del formulario en función del número de filas
                int rowHeight = 35; // Altura estimada de una fila
                int buttonHeight = 35; // Altura estimada de un botón
                int padding = 25; // Espacio adicional para evitar que el formulario esté demasiado apretado
                Height = fieldNames.Count * rowHeight + buttonHeight + padding;

            }
        }

        // Método para obtener el valor de un campo
        public object GetFieldValue(string name)
        {
            if (fields[name] is TextBox)
                return ((TextBox)fields[name]).Text;
            else
                return ((ComboBox)fields[name]).SelectedItem;
        }

        public void IdentifyComboBoxFields(string connectionString, string tableName)
        {
            // Conexión a la base de datos Access
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                connection.Open();

                // Obtiene la información del esquema de la tabla
                DataTable schemaTable = connection.GetSchema(OdbcMetaDataCollectionNames.Columns, new string[] { null, null, tableName, null });

                // Crea una lista para almacenar los nombres de los campos
                List<string> fieldNames = new List<string>();

                // Recorre las filas de la tabla de esquema
                foreach (DataRow row in schemaTable.Rows)
                {
                    // Obtiene el nombre de la columna
                    string columnName = row["COLUMN_NAME"].ToString();

                    // Obtiene el tipo de datos de la columna
                    string dataType = row["DATA_TYPE"].ToString();

                    // Si el tipo de datos es un tipo que debe ser representado por un ComboBox, añade el nombre de la columna a la lista de nombres de campos
                    if (dataType == "3" || dataType == "202" || dataType == "203" || dataType == "7" || dataType == "5" || dataType == "11")
                    {
                        fieldNames.Add(columnName);
                    }
                }

                // Crea un nuevo formulario de datos con los nombres de los campos
                DataForm dataForm = new DataForm(connectionString, tableName);

                // Muestra el formulario de datos
                dataForm.ShowDialog();
            }
        }

        // Método para establecer el valor de un campo
        public void SetFieldValue(string name, object value)
        {
            // Debug line to print out the keys in the fields dictionary and the name being passed
            System.Diagnostics.Debug.WriteLine("Fields keys: " + String.Join(", ", fields.Keys));
            System.Diagnostics.Debug.WriteLine("Name passed: " + name);

            if (!fields.ContainsKey(name))
            {
                throw new ArgumentException($"The field '{name}' does not exist.");
            }

            if (fields[name] is TextBox)
            {
                ((TextBox)fields[name]).Text = value.ToString();
            }
            else if (fields[name] is ComboBox)
            {
                ComboBox comboBox = (ComboBox)fields[name];
                if (comboBox.Items.Contains(value))
                {
                    comboBox.SelectedItem = value;
                }
                else
                {
                    throw new ArgumentException($"The value '{value}' does not exist in the ComboBox '{name}'.");
                }
            }
        }
    }
}
