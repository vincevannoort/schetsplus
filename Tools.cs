using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;

namespace SchetsEditor
{
    public interface ISchetsTool
    {
        void MuisVast(SchetsControl s, Point p);
        void MuisDrag(SchetsControl s, Point p);
        void MuisLos(SchetsControl s, Point p);
        void Letter(SchetsControl s, char c);
        void VoegActieToe(SchetsObject s);
    }

    public abstract class StartpuntTool : ISchetsTool
    {

        protected Schets schets;
        protected Point startpunt;
        protected Brush kwast;

		public StartpuntTool(Schets s)
		{
			this.schets = s;
		}

        public virtual void MuisVast(SchetsControl s, Point p)
        {   
            startpunt = p;
        }

        public virtual void MuisLos(SchetsControl s, Point p)
        {   
            kwast = new SolidBrush(s.PenKleur);
        }

        public virtual void VoegActieToe(SchetsObject s)
        {
            this.schets.acties.Add(s);
            Console.WriteLine("------ACTIES------");
            foreach (SchetsObject actie in this.schets.acties)
            {
                Console.WriteLine(actie);
            }
        }

        public abstract void MuisDrag(SchetsControl s, Point p);
        public abstract void Letter(SchetsControl s, char c);
    }

    public class TekstTool : StartpuntTool
    {
		public TekstTool(Schets s): base(s) { }

        public override string ToString() { return "tekst"; }

        public override void MuisDrag(SchetsControl s, Point p) { }

        public override void Letter(SchetsControl s, char c)
        {
            if (c >= 32)
            {
                Graphics gr = s.MaakBitmapGraphics();
                Font font = new Font("Tahoma", 40);
                string tekst = c.ToString();
                SizeF sz = gr.MeasureString(tekst, font, startpunt, StringFormat.GenericTypographic);
                // gr.DrawString(tekst, font, kwast, startpunt, StringFormat.GenericTypographic);
                startpunt.X += (int)sz.Width;
                s.Invalidate();

                this.VoegActieToe(new TekstObject(kwast, tekst, font, startpunt));
            }
        }
    }

    public abstract class TweepuntTool : StartpuntTool
    {
		public TweepuntTool(Schets s): base(s) { }

        public static Rectangle Punten2Rechthoek(Point p1, Point p2)
        {   
            return new Rectangle( new Point(Math.Min(p1.X,p2.X), Math.Min(p1.Y,p2.Y)), 
                                  new Size(Math.Abs(p1.X-p2.X), Math.Abs(p1.Y-p2.Y)));
        }
        public static Pen MaakPen(Brush b, int dikte)
        {   
            Pen pen = new Pen(b, dikte);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }
        public override void MuisVast(SchetsControl s, Point p)
        {   
            base.MuisVast(s, p);
            kwast = Brushes.Gray;
        }

        public override void MuisDrag(SchetsControl s, Point p)
        {   
            s.Refresh();
            this.Bezig(s.CreateGraphics(), this.startpunt, p);
        }

        public override void MuisLos(SchetsControl s, Point p)
        {   
            base.MuisLos(s, p);
            this.Compleet(s.MaakBitmapGraphics(), this.startpunt, p);
            s.Invalidate();
        }
        
        public virtual void Compleet(Graphics g, Point p1, Point p2)
        {   
            this.Bezig(g, p1, p2);
        }
        
        public override void Letter(SchetsControl s, char c) {}
        public abstract void Bezig(Graphics g, Point p1, Point p2);
    }

    public class RechthoekTool : TweepuntTool
    {
        public RechthoekTool(Schets s): base(s) {}
        
        public override string ToString() { return "kader"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {   
            // g.DrawRectangle(MaakPen(kwast,3), TweepuntTool.Punten2Rechthoek(p1, p2));
        }

        public override void Compleet(Graphics g, Point p1, Point p2)
        {   
            this.Bezig(g, p1, p2);
            this.VoegActieToe(new RechthoekObject(MaakPen(kwast, 3), p1, p2));
        }
    }
    
    public class VolRechthoekTool : RechthoekTool
    {
        public VolRechthoekTool(Schets s): base(s) { }
        
        public override string ToString() { return "vlak"; }

        public override void Compleet(Graphics g, Point p1, Point p2)
        {   
            // g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
            this.VoegActieToe(new RechthoekObject(kwast, p1, p2));
        }
    }

	public class CirkelTool : TweepuntTool
	{
        public CirkelTool(Schets s): base(s) { }

		public override string ToString() { return "cirkel"; }

		public override void Bezig(Graphics g, Point p1, Point p2)
		{
            // g.DrawEllipse(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));
		}

        public override void Compleet(Graphics g, Point p1, Point p2)
        {   
            this.Bezig(g, p1, p2);
            this.VoegActieToe(new CirkelObject(MaakPen(kwast, 3), p1, p2));
        }
	}

    public class VolCirkelTool : CirkelTool
	{
        public VolCirkelTool(Schets s): base(s) { }

		public override string ToString() { return "volcirkel"; }

		public override void Compleet(Graphics g, Point p1, Point p2)
		{
            // g.FillEllipse(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
            this.VoegActieToe(new CirkelObject(kwast, p1, p2));
		}
	}

    public class LijnTool : TweepuntTool
    {
        public LijnTool(Schets s): base(s) { }

        public override string ToString() { return "lijn"; }

        public override void Bezig(Graphics g, Point p1, Point p2)
        {   
            // g.DrawLine(MaakPen(this.kwast,3), p1, p2);
        }

        public override void Compleet(Graphics g, Point p1, Point p2)
        {
            this.Bezig(g, p1, p2);
            this.VoegActieToe(new LijnObject(MaakPen(this.kwast,3), p1, p2));
        }
    }

    public class PenTool : LijnTool
    {
        protected List<LijnObject> lijnen;
        public PenTool(Schets s): base(s) { }

        public override string ToString() { return "pen"; }

        public override void MuisVast(SchetsControl s, Point p)
        {
            startpunt = p;
            kwast = new SolidBrush(s.PenKleur);
            lijnen = new List<LijnObject>();
        }

        public override void MuisDrag(SchetsControl s, Point p)
        {
            lijnen.Add(new LijnObject(MaakPen(this.kwast, 3), startpunt, p));
            startpunt = p;
        }

		public override void Compleet(Graphics g, Point p1, Point p2)
		{
            this.VoegActieToe(new PenObject(MaakPen(this.kwast, 3), lijnen, p1, p2));
		}
    }
    
    public class GumTool : StartpuntTool
    {
        public GumTool(Schets s): base(s) { }

        public override string ToString() { return "gum"; }

        public override void MuisDrag(SchetsControl s, Point p) { }
        public override void Letter(SchetsControl s, char c) { }
    }
}