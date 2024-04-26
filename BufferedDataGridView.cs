using System.Windows.Forms;

namespace GestionBBDD
{
    public partial class BufferedDataGridView : Form
    {
        public BufferedDataGridView()
        {
            InitializeComponent();

            DoubleBuffered = true;
        }

        public static implicit operator DataGridView(BufferedDataGridView v)
        {
            DataGridView dgv = new DataGridView();
            return dgv;
        }
    }
}
