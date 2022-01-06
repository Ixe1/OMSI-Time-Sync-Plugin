using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace OmsiTimeSyncPlugin
{
    public partial class frmMain : Form
    {
        Process process;

        Mem m = new Mem();

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            process = Process.GetProcessesByName("omsi")[0];

            m.OpenProcess(process.Id);
        }

        private void tmrOMSI_Tick(object sender, EventArgs e)
        {
            // Get current in-game time 'minutes' from OMSI
            label1.Text = m.ReadByte("0x0046176D").ToString();

            // Set the in-game OMSI game 'minutes' to 32
            m.WriteMemory("0x0046176D", "byte", "32");
        }
    }
}
