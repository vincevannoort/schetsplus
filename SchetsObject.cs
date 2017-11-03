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
    }
}