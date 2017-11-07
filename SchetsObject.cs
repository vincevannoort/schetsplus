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

        private double Afstand(Point point, Point segmentstart, Point segmentend)
		{
			// credits to: http://csharphelper.com/blog/2016/09/find-the-shortest-distance-between-a-point-and-a-line-segment-in-c/
			Point closest;
			float dx = segmentend.X - segmentstart.X;
			float dy = segmentend.Y - segmentstart.Y;

			if ((dx == 0) && (dy == 0))
			{
				// It's a point not a line segment.
				closest = segmentstart;
				dx = point.X - segmentstart.X;
				dy = point.Y - segmentstart.Y;
				return Math.Sqrt(dx * dx + dy * dy);
			}

			// Calculate the t that minimizes the distance.
			float t = ((point.X - segmentstart.X) * dx + (point.Y - segmentstart.Y) * dy) /
				(dx * dx + dy * dy);

			// See if this represents one of the segment's
			// end points or a point in the middle.
			if (t < 0)
			{
                closest = new Point(segmentstart.X, segmentstart.Y);
				dx = point.X - segmentstart.X;
				dy = point.Y - segmentstart.Y;
			}
			else if (t > 1)
			{
                closest = new Point(segmentend.X, segmentend.Y);
				dx = point.X - segmentend.X;
				dy = point.Y - segmentend.Y;
			}
			else
			{
                closest = new Point(
                    (int)((segmentstart.X) + t * dx), 
                    (int)((segmentstart.Y) + t * dy));
				dx = point.X - closest.X;
				dy = point.Y - closest.Y;
			}

			return Math.Sqrt(dx * dx + dy * dy);
		}

        public override bool Geklikt(Point p) {
            return this.Afstand(p, startpunt, eindpunt) < 5;
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
            Point normalized = new Point(p.X - center.X, p.Y - center.Y);
            double normalizedDistance = ((double)(normalized.X * normalized.X) / (xRadius * xRadius)) + ((double)(normalized.Y * normalized.Y) / (yRadius * yRadius));
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