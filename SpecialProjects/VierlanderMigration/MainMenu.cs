using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VierlanderMigration
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void btnImportLatestFiles_Click(object sender, EventArgs e)
        {
            ApplicationLayer.ImportAllFiles();
        }
    }
}