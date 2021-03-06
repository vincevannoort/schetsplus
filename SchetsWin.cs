﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;

namespace SchetsEditor
{
    public class SchetsWin : Form
    {   
        MenuStrip menuStrip;
        public SchetsControl schetscontrol; // LATER ANDERS OPLOSSEN DAN MET PUBLIC? GETTER AND SETTER?
        ISchetsTool huidigeTool;
        Panel paneel;
        bool vast;
        ResourceManager resourcemanager
            = new ResourceManager("SchetsEditor.Properties.Resources"
                                 , Assembly.GetExecutingAssembly()
                                 );

        private void veranderAfmeting(object o, EventArgs ea)
        {
            schetscontrol.Size = new Size ( this.ClientSize.Width  - 70
                                          , this.ClientSize.Height - 50);
            paneel.Location = new Point(64, this.ClientSize.Height - 30);
        }

        private void klikToolMenu(object obj, EventArgs ea)
        {
            this.huidigeTool = (ISchetsTool)((ToolStripMenuItem)obj).Tag;
        }

        private void klikToolButton(object obj, EventArgs ea)
        {
            this.huidigeTool = (ISchetsTool)((RadioButton)obj).Tag;
        }

        private void afsluiten(object obj, EventArgs ea)
        {
            this.Close();
        }

        public SchetsWin()
        {
            schetscontrol = new SchetsControl();

            ISchetsTool[] deTools = { new PenTool(schetscontrol.Schets), 
                                      new LijnTool(schetscontrol.Schets), 
                                      new RechthoekTool(schetscontrol.Schets), 
                                      new VolRechthoekTool(schetscontrol.Schets), 
                                      new CirkelTool(schetscontrol.Schets), 
                                      new VolCirkelTool(schetscontrol.Schets), 
                                      new TekstTool(schetscontrol.Schets), 
                                      new GumTool(schetscontrol.Schets) };

            String[] deKleuren = { "Black", "Red", "Green", "Blue", "Yellow", "Magenta", "Cyan" };
            String[] deDiktes = { "slank", "normaal", "dik" };

            this.ClientSize = new Size(700, 500);
            huidigeTool = deTools[0];

            schetscontrol.Location = new Point(64, 10);
            schetscontrol.MouseDown += (object o, MouseEventArgs mea) => {   
                                            vast=true;  
                                            huidigeTool.MuisVast(schetscontrol, mea.Location); 
                                       };
            schetscontrol.MouseMove += (object o, MouseEventArgs mea) => {   
                                            if (vast)
                                            huidigeTool.MuisDrag(schetscontrol, mea.Location); 
                                       };
            schetscontrol.MouseUp   += (object o, MouseEventArgs mea) => {   
                                            if (vast)
                                            huidigeTool.MuisLos (schetscontrol, mea.Location);
                                            vast = false; 
                                       };
            schetscontrol.KeyPress +=  (object o, KeyPressEventArgs kpea) =>  {   
                                            huidigeTool.Letter  (schetscontrol, kpea.KeyChar); 
                                        };
            this.Controls.Add(schetscontrol);

            menuStrip = new MenuStrip();
            menuStrip.Visible = false;
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakToolMenu(deTools);
            this.maakAktieMenu(deKleuren, deDiktes);
            this.maakToolButtons(deTools);
            this.maakAktieButtons(deKleuren, deDiktes);
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);
        }

        public void importeerBitmap(String filenaam)
        {
            Bitmap bitmap = new Bitmap(filenaam);
            schetscontrol.Schets.VeranderAfmeting(bitmap.Size);
            this.Size = new Size(bitmap.Width + 70, bitmap.Height + 50);
            schetscontrol.Schets.BitmapGraphics.DrawImage(bitmap, 0, 0);
        }

        public void importeerSchets()
        {

        }

        private void maakFileMenu()
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("File");
            menu.MergeAction = MergeAction.MatchOnly;
            menu.DropDownItems.Add("Sluiten", null, this.afsluiten);
            menuStrip.Items.Add(menu);
        }

        private void maakToolMenu(ICollection<ISchetsTool> tools)
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("Tool");
            foreach (ISchetsTool tool in tools)
            {   ToolStripItem item = new ToolStripMenuItem();
                item.Tag = tool;
                item.Text = tool.ToString();
                item.Image = (Image)resourcemanager.GetObject(tool.ToString());
                item.Click += this.klikToolMenu;
                menu.DropDownItems.Add(item);
            }
            menuStrip.Items.Add(menu);
        }

        private void maakAktieMenu(String[] kleuren, String[] diktes)
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("Aktie");
            menu.DropDownItems.Add("Clear", null, schetscontrol.Schoon );
            menu.DropDownItems.Add("Roteer", null, schetscontrol.Roteer );
            ToolStripMenuItem submenu1 = new ToolStripMenuItem("Kies kleur");
            foreach (string k in kleuren)
                submenu1.DropDownItems.Add(k, null, schetscontrol.VeranderKleurViaMenu);
            menu.DropDownItems.Add(submenu1);
			ToolStripMenuItem submenu2 = new ToolStripMenuItem("Kies dikte");
			foreach (string d in diktes)
				submenu2.DropDownItems.Add(d, null, schetscontrol.VeranderDikteViaMenu);
			menu.DropDownItems.Add(submenu2);
            menuStrip.Items.Add(menu);
        }

        private void maakToolButtons(ICollection<ISchetsTool> tools)
        {
            int t = 0;
            foreach (ISchetsTool tool in tools)
            {
                RadioButton b = new RadioButton();
                b.Appearance = Appearance.Button;
                b.Size = new Size(45, 62);
                b.Location = new Point(10, 10 + t * 62);
                b.Tag = tool;
                b.Text = tool.ToString();
                b.Image = (Image)resourcemanager.GetObject(tool.ToString());
                b.TextAlign = ContentAlignment.TopCenter;
                b.ImageAlign = ContentAlignment.BottomCenter;
                b.Click += this.klikToolButton;
                this.Controls.Add(b);
                if (t == 0) b.Select();
                t++;
            }
        }

        private void maakAktieButtons(String[] kleuren, String[] diktes)
        {   
            paneel = new Panel();
            paneel.Size = new Size(600, 24);
            this.Controls.Add(paneel);
            
            Button b; Label l; ComboBox cbb;
            b = new Button(); 
            b.Text = "Clear";  
            b.Location = new Point(  0, 0); 
            b.Click += schetscontrol.Schoon; 
            paneel.Controls.Add(b);

            
            b = new Button(); 
            b.Text = "Rotate"; 
            b.Location = new Point( 80, 0); 
            b.Click += schetscontrol.Roteer; 
            paneel.Controls.Add(b);

			b = new Button();
			b.Text = "Undo";
			b.Location = new Point(160, 0);
			b.Click += schetscontrol.Undo;
			paneel.Controls.Add(b);

			b = new Button();
			b.Text = "Redo";
			b.Location = new Point(240, 0);
			//b.Click += schetscontrol.Schoon;
			paneel.Controls.Add(b);
            
            l = new Label();  
            l.Text = "Kleur:"; 
            l.Location = new Point(340, 3); 
            l.AutoSize = true;               
            paneel.Controls.Add(l);
            
            cbb = new ComboBox(); cbb.Location = new Point(380, 0); cbb.Size = new Size(60, 40); 
            cbb.DropDownStyle = ComboBoxStyle.DropDownList; 
            cbb.SelectedValueChanged += schetscontrol.VeranderKleur;
            foreach (string k in kleuren)
                cbb.Items.Add(k);
            cbb.SelectedIndex = 0;
            paneel.Controls.Add(cbb);

			l = new Label();
			l.Text = "Dikte:";
			l.Location = new Point(460, 3);
			l.AutoSize = true;
			paneel.Controls.Add(l);

			cbb = new ComboBox(); cbb.Location = new Point(500, 0); cbb.Size = new Size(60, 40);
			cbb.DropDownStyle = ComboBoxStyle.DropDownList;
			cbb.SelectedValueChanged += schetscontrol.VeranderDikte;
			foreach (string d in diktes)
				cbb.Items.Add(d);
			cbb.SelectedIndex = 1;
			paneel.Controls.Add(cbb);
        }
    }
}
