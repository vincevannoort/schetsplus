using System;
using System.Drawing;
using System.Collections.Generic;

namespace SchetsEditor
{
    public abstract class SchetsObject
    {
        protected Point startpunt;
        protected Point eindpunt;

        public Point Startpunt {
            get { return startpunt; }
        }

		public Point Eindpunt
		{
			get { return eindpunt; }
		}

        public SchetsObject(Point start)
        {
            this.startpunt = start;
        }

        public SchetsObject(Point start, Point eind)
        {
            this.startpunt = start;
            this.eindpunt = eind;
        }

        public abstract void Teken(Graphics g);

        public abstract bool Geklikt(Point p);
    }

    public class TekstObject : SchetsObject
    {
        protected Brush kwast;
        protected Font font;
        string tekst;

        public TekstObject(Brush kwast, String tekst, Font font, Point start) : base(start)
        {
            this.kwast = kwast;
            this.tekst = tekst;
            this.font = font;
        }

        public override void Teken(Graphics g)
        {
            g.DrawString(tekst, font, kwast, startpunt, StringFormat.GenericTypographic);
        }

        public override bool Geklikt(Point p) { return false; }
    }

    public class LijnObject : SchetsObject
    {
        protected Pen pen;

        public LijnObject(Pen pen, Point start, Point eind) : base(start, eind)
        {
            this.pen = pen;
        }

        public override void Teken(Graphics g)
        {
            g.DrawLine(pen, startpunt, eindpunt);
        }

        public static double Afstand(Point a, Point b)
        {
            double fa = (a.X - b.X);
            double fb = (a.Y - b.Y);
            return Math.Sqrt(fa * fa + fb * fb);
        }

        // WERKT NOG NIET
        public override bool Geklikt(Point p) {

			// algorithm based on https://stackoverflow.com/questions/328107/how-can-you-determine-a-point-is-between-two-other-points-on-a-line-segment

			Point a = startpunt;
            Point b = eindpunt;
            Point c = p;

            //float epsilon = 0.0000006F;

            //float crossproduct = (c.Y - a.Y) * (b.X - a.X) - (c.X - a.X) * (b.Y - a.Y);
            //if (Math.Abs(crossproduct) > epsilon) { return false; }

            //float dotproduct = (c.X - a.X) * (b.X - a.X) + (c.Y - a.Y) * (b.Y - a.Y);
            //if (dotproduct < 0) { return false; }

            //float squaredlengthba = (b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y);
            //if (dotproduct > squaredlengthba) { return false; }

            //return true;

            return (LijnObject.Afstand(a, c) + LijnObject.Afstand(c, b)) == LijnObject.Afstand(a, b);
		}
    }

	public class PenObject : SchetsObject
	{
		protected Pen pen;
        protected List<LijnObject> lijnen;

		public PenObject(Pen pen, List<LijnObject> lijnen, Point start, Point eind) : base(start, eind)
		{
			this.pen = pen;
            this.lijnen = lijnen;
		}

		public override void Teken(Graphics g)
		{
            foreach (LijnObject lijn in lijnen)
            {
                g.DrawLine(pen, lijn.Startpunt, lijn.Eindpunt);       
            }
		}

        public override bool Geklikt(Point p) {
            foreach(LijnObject lijn in lijnen)
            {
                if (lijn.Geklikt(p))
                {
                    return true;
                }
            }
            return false;
        }
	}

    public class RechthoekObject : SchetsObject
    {
        protected Pen pen;
        protected Brush kwast;

        // rechthoek met omlijning
        public RechthoekObject(Pen pen, Point start, Point eind) : base (start, eind)
        {
            this.pen = pen;
        }

        // rechthoek gevuld
        public RechthoekObject(Brush kwast, Point start, Point eind) : base (start, eind)
        {
            this.kwast = kwast;
        }

        public override void Teken(Graphics g)
        {
            if (pen != null)
            {
                g.DrawRectangle(pen, TweepuntTool.Punten2Rechthoek(startpunt, eindpunt));
            }
            else if (kwast != null) {
                g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(startpunt, eindpunt));
            }
        }

        public override bool Geklikt(Point p) {
            if (pen != null)
            {
                int marge = 5;
                Rectangle buitenrechthoek = TweepuntTool.Punten2Rechthoek(new Point(startpunt.X - marge, startpunt.Y - marge), new Point(eindpunt.X + marge, eindpunt.Y + marge));
                Rectangle binnenrechthoek = TweepuntTool.Punten2Rechthoek(new Point(startpunt.X + marge, startpunt.Y + marge), new Point(eindpunt.X - marge, eindpunt.Y - marge));
                if (buitenrechthoek.Contains(p) && !binnenrechthoek.Contains(p))
				{
					return true;
				}
            }
            else if (kwast != null)
            {
                if (TweepuntTool.Punten2Rechthoek(startpunt, eindpunt).Contains(p))
                {
                    return true;

                }
            }
            return false;
        }
    }

    public class CirkelObject : SchetsObject
    {
        protected Pen pen;
        protected Brush kwast;

        // Cirkel met omlijning
        public CirkelObject(Pen pen, Point start, Point eind) : base (start, eind)
        {
            this.pen = pen;
        }

        // Cirkel gevuld
        public CirkelObject(Brush kwast, Point start, Point eind) : base (start, eind)
        {
            this.kwast = kwast;
        }

        public override void Teken(Graphics g)
        {
             
            if (pen != null)
            {
                g.DrawEllipse(pen, TweepuntTool.Punten2Rechthoek(startpunt, eindpunt));
            }
            else if (kwast != null) {
                g.FillEllipse(kwast, TweepuntTool.Punten2Rechthoek(startpunt, eindpunt));
            }
        }

        public override bool Geklikt(Point p) {
			// credits to: https://stackoverflow.com/questions/13285007/how-to-determine-if-a-point-is-within-an-ellipse
			Rectangle rechthoek = TweepuntTool.Punten2Rechthoek(startpunt, eindpunt);
			Point center = new Point(rechthoek.X + rechthoek.Width / 2, rechthoek.Y + rechthoek.Height / 2);
			double xRadius = rechthoek.Width / 2;
			double yRadius = rechthoek.Height / 2;
            double normalizedDistance = ((double)(normalized.X * normalized.X) / (xRadius * xRadius)) + ((double)(normalized.Y * normalized.Y) / (yRadius * yRadius));
            Point normalized = new Point(p.X - center.X, p.Y - center.Y);
            float marge = 0.05F;

			if (pen != null)
			{
                return normalizedDistance > 1.0 - marge && normalizedDistance < 1.0 + marge;
			}
			else if (kwast != null)
			{
				return normalizedDistance <= 1.0;
			}

            return false;
        }
    }
}