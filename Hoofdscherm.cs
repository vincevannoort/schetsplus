using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace SchetsEditor
{
    public class Hoofdscherm : Form
    {
        MenuStrip menuStrip;

        public Hoofdscherm()
        {   this.ClientSize = new Size(800, 600);
            menuStrip = new MenuStrip();
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakHelpMenu();
            this.Text = "Schets editor";
            this.IsMdiContainer = true;
            this.MainMenuStrip = menuStrip;
        }

        private void maakFileMenu()
        {   ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem("File");
            menu.DropDownItems.Add("Nieuw", null, this.nieuw);
            menu.DropDownItems.Add("Importeren", null, this.Importeren);
            menu.DropDownItems.Add("Exporteren", null, this.Exporteren);
            menu.DropDownItems.Add("Afsluiten", null, this.afsluiten);
            menuStrip.Items.Add(menu);
        }

        private void Importeren(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "PNG Images (*.png)|*.png|JPEG Images (*.jpg)|*.jpg|BMP Images (*.bmp)|*.bmp";
            if (openDialog.ShowDialog() == DialogResult.OK 
                && openDialog.FileName.Length > 0)
            {
                SchetsWin s = new SchetsWin();
                s.MdiParent = this;
                s.Show();
                SchetsWin activeChild = (SchetsWin) this.ActiveMdiChild;

                if (Path.GetExtension(openDialog.FileName) != ".sp")
                {
                    activeChild.importeerBitmap(openDialog.FileName);
                } 
                else 
                {
                    // laad special bestand
                    // activeChild.importeerSchets();
                }
            }
        }

        private void Exporteren(object sender, EventArgs e)
        {
            SchetsWin activeChild = (SchetsWin) this.ActiveMdiChild;
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "PNG Images (*.png)|*.png|JPEG Images (*.jpg)|*.jpg|BMP Images (*.bmp)|*.bmp";
            if (saveDialog.ShowDialog() == DialogResult.OK 
                && saveDialog.FileName.Length > 0)
            {
                Bitmap bitmap = activeChild.schetscontrol.Schets.getBitmap();
                activeChild.schetscontrol.Schets.Teken(activeChild.schetscontrol.Schets.BitmapGraphics);
                switch (saveDialog.FilterIndex)
                {
                    case 1:
                        bitmap.Save(saveDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case 2:
                        bitmap.Save(saveDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case 3:
                        bitmap.Save(saveDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                }
                activeChild.schetscontrol.Schets.Schoon();
            }
        }

        private void maakHelpMenu()
        {   
            ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem("Help");
            menu.DropDownItems.Add("Over \"Schets\"", null, this.about);
            menuStrip.Items.Add(menu);
        }

        private void about(object o, EventArgs ea)
        {   
            MessageBox.Show("Schets versie 1.0\n(c) UU Informatica 2010"
                           , "Over \"Schets\""
                           , MessageBoxButtons.OK
                           , MessageBoxIcon.Information
                           );
        }

        private void nieuw(object sender, EventArgs e)
        {   
            SchetsWin s = new SchetsWin();
            s.MdiParent = this;
            s.Show();
        }
        private void afsluiten(object sender, EventArgs e)
        {   
            this.Close();
        }
    }
}
