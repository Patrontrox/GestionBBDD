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

        public DataForm(string connectionString, string tableName, bool isReadOnly = false)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                connection.Open();

                DataTable schemaTable = connection.GetSchema(OdbcMetaDataCollectionNames.Columns, new string[] { null, null, tableName, null });
                DataTable relationships = GetRelationships(connection);

                List<string> fieldNames = new List<string>();
                Dictionary<string, List<(string, string)>> foreignKeys = new Dictionary<string, List<(string, string)>>();

                foreach (DataRow row in schemaTable.Rows)
                {
                    string columnName = row["COLUMN_NAME"].ToString();
                    fieldNames.Add(columnName);

                    foreach (DataRow relRow in relationships.Rows)
                    {
                        if (relRow["FK_COLUMN_NAME"].ToString() == columnName && relRow["FK_TABLE_NAME"].ToString() == tableName)
                        {
                            if (!foreignKeys.ContainsKey(columnName))
                            {
                                foreignKeys[columnName] = new List<(string, string)>();
                            }
                            foreignKeys[columnName].Add((relRow["PK_TABLE_NAME"].ToString(), relRow["PK_COLUMN_NAME"].ToString()));
                        }
                    }
                }

                Text = "Formulario de datos";
                StartPosition = FormStartPosition.CenterScreen;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;

                TableLayoutPanel panel = new TableLayoutPanel
                {
                    ColumnCount = 2,
                    RowCount = fieldNames.Count,
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink
                };
                Controls.Add(panel);

                for (int i = 0; i < fieldNames.Count; i++)
                {
                    Label label = new Label
                    {
                        Text = fieldNames[i],
                        Font = new Font("Arial", 10, FontStyle.Bold),
                        ForeColor = Color.DarkBlue
                    };
                    panel.Controls.Add(label, 0, i);

                    if (foreignKeys.ContainsKey(fieldNames[i]))
                    {
                        ComboBox comboBox = new ComboBox
                        {
                            Font = new Font("Arial", 10, FontStyle.Bold),
                            BackColor = Color.LightGray,
                            DropDownStyle = ComboBoxStyle.DropDownList
                        };
                        panel.Controls.Add(comboBox, 1, i);
                        fields[fieldNames[i]] = comboBox;

                        foreach (var (relatedTable, relatedColumn) in foreignKeys[fieldNames[i]])
                        {
                            try
                            {
                                FillComboBoxWithRelatedData(connection, comboBox, relatedTable, relatedColumn);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error filling ComboBox for {fieldNames[i]}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        TextBox textBox = new TextBox
                        {
                            Font = new Font("Arial", 10, FontStyle.Regular),
                            ReadOnly = (i == 0 && isReadOnly)
                        };
                        textBox.Size = new Size(150, textBox.Size.Height); // Mover la asignación de Size después de la inicialización
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                        panel.Controls.Add(textBox, 1, i);
                        fields[fieldNames[i]] = textBox;

                    }
                }

                okButton = new Button
                {
                    Text = "OK",
                    Dock = DockStyle.Bottom,
                    BackColor = Color.MediumSeaGreen,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Arial", 10, FontStyle.Bold)
                };
                okButton.FlatAppearance.BorderColor = Color.SeaGreen;
                okButton.FlatAppearance.BorderSize = 1;
                okButton.Click += (sender, e) => DialogResult = DialogResult.OK;
                Controls.Add(okButton);

                cancelButton = new Button
                {
                    Text = "Cancelar",
                    Dock = DockStyle.Bottom,
                    BackColor = Color.IndianRed,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Arial", 10, FontStyle.Bold)
                };
                cancelButton.FlatAppearance.BorderColor = Color.Firebrick;
                cancelButton.FlatAppearance.BorderSize = 1;
                cancelButton.Click += (sender, e) => DialogResult = DialogResult.Cancel;
                Controls.Add(cancelButton);

                BackColor = Color.LightGray;

                foreach (Control control in panel.Controls)
                {
                    if (control is TextBox textBoxControl)
                    {
                        textBoxControl.BackColor = Color.LightGray;
                        textBoxControl.ForeColor = Color.Black;
                    }
                    else if (control is Label labelControl)
                    {
                        labelControl.ForeColor = Color.DarkBlue;
                    }
                    else if (control is ComboBox comboBoxControl)
                    {
                        comboBoxControl.BackColor = Color.White;
                        comboBoxControl.ForeColor = Color.Black;
                    }
                }

                int rowHeight = 35;
                int buttonHeight = 35;
                int padding = 25;
                Height = fieldNames.Count * rowHeight + buttonHeight + padding;
            }
        }

        private DataTable GetRelationships(OdbcConnection connection)
        {
            DataTable relationships = new DataTable();
            relationships.Columns.Add("FK_TABLE_NAME");
            relationships.Columns.Add("FK_COLUMN_NAME");
            relationships.Columns.Add("PK_TABLE_NAME");
            relationships.Columns.Add("PK_COLUMN_NAME");

            string query = "SELECT szObject AS FK_TABLE_NAME, szColumn AS FK_COLUMN_NAME, szReferencedObject AS PK_TABLE_NAME, szReferencedColumn AS PK_COLUMN_NAME FROM Relaciones";

            using (OdbcCommand command = new OdbcCommand(query, connection))
            {
                using (OdbcDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DataRow row = relationships.NewRow();
                        row["FK_TABLE_NAME"] = reader["FK_TABLE_NAME"];
                        row["FK_COLUMN_NAME"] = reader["FK_COLUMN_NAME"];
                        row["PK_TABLE_NAME"] = reader["PK_TABLE_NAME"];
                        row["PK_COLUMN_NAME"] = reader["PK_COLUMN_NAME"];
                        relationships.Rows.Add(row);
                    }
                }
            }

            return relationships;
        }

        private void FillComboBoxWithRelatedData(OdbcConnection connection, ComboBox comboBox, string relatedTable, string relatedColumn)
        {
            string query = $"SELECT {relatedColumn} FROM {relatedTable}";
            using (OdbcCommand command = new OdbcCommand(query, connection))
            {
                using (OdbcDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string value = reader[relatedColumn].ToString();
                        System.Diagnostics.Debug.WriteLine($"Adding value '{value}' to ComboBox for column '{relatedColumn}' from table '{relatedTable}'");
                        comboBox.Items.Add(value);
                    }
                }
            }
        }


        public object GetFieldValue(string name)
        {
            return fields[name] is TextBox textBox ? textBox.Text : ((ComboBox)fields[name]).SelectedItem;
        }

        public void IdentifyComboBoxFields(string connectionString, string tableName)
        {
            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                connection.Open();

                DataTable schemaTable = connection.GetSchema(OdbcMetaDataCollectionNames.Columns, new string[] { null, null, tableName, null });

                List<string> fieldNames = new List<string>();

                foreach (DataRow row in schemaTable.Rows)
                {
                    string columnName = row["COLUMN_NAME"].ToString();
                    string dataType = row["DATA_TYPE"].ToString();

                    if (dataType == "3" || dataType == "202" || dataType == "203" || dataType == "7" || dataType == "5" || dataType == "11")
                    {
                        fieldNames.Add(columnName);
                    }
                }

                DataForm dataForm = new DataForm(connectionString, tableName);
                dataForm.ShowDialog();
            }
        }

        public void SetFieldValue(string name, object value)
        {
            if (fields[name] is TextBox textBox)
            {
                textBox.Text = value.ToString();
            }
            else if (fields[name] is ComboBox comboBox)
            {
                comboBox.SelectedItem = value;
            }
        }
    }
}
