using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SchetsEditor
{   public class SchetsControl : UserControl
    {   private Schets schets;
        private Color penkleur;
        private int pendikte;

        public Color PenKleur
        { get { return penkleur; }
        }

		public int PenDikte
		{
			get { return pendikte; }
		}

        public Schets Schets
        { get { return schets;   }
        }

        public SchetsControl()
        {   this.BorderStyle = BorderStyle.Fixed3D;
            this.schets = new Schets();
            this.Paint += this.teken;
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);
            this.pendikte = 3;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        private void teken(object o, PaintEventArgs pea)
        {   
            schets.Teken(pea.Graphics);
        }

        private void veranderAfmeting(object o, EventArgs ea)
        {   schets.VeranderAfmeting(this.ClientSize);
            this.Invalidate();
        }

        public Graphics MaakBitmapGraphics()
        {   Graphics g = schets.BitmapGraphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            return g;
        }

        public void Schoon(object o, EventArgs ea)
        {   schets.Schoon();
            this.Invalidate();
        }

        public void Roteer(object o, EventArgs ea)
        {   schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
            schets.Roteer();
            this.Invalidate();
        }

        public void VeranderKleur(object obj, EventArgs ea)
        {   string kleurNaam = ((ComboBox)obj).Text;
            penkleur = Color.FromName(kleurNaam);
        }

        public void VeranderKleurViaMenu(object obj, EventArgs ea)
        {   string kleurNaam = ((ToolStripMenuItem)obj).Text;
            penkleur = Color.FromName(kleurNaam);
        }

		public void VeranderDikte(object obj, EventArgs ea)
		{
			string dikte = ((ComboBox)obj).Text;
			if (dikte == "slank")
			{
				this.pendikte = 1;
			}
			else if (dikte == "normaal")
			{
				this.pendikte = 3;
			}
			else if (dikte == "dik")
			{
				this.pendikte = 5;
			}

		}

        public void VeranderDikteViaMenu(object obj, EventArgs ea)
        {
            string dikte = ((ToolStripMenuItem)obj).Text;
            if (dikte == "slank")
            {
                this.pendikte = 1;
            }
            else if (dikte == "normaal")
            {
                this.pendikte = 3;
            }
            else if (dikte == "dik")
            {
                this.pendikte = 5;
            }

        }

		public void Undo(object o, EventArgs ea)
		{
            if (schets.acties.Count > 0)
            {
                schets.acties.RemoveAt(schets.acties.Count - 1);
                this.Invalidate();
            }
		}
    }
}
