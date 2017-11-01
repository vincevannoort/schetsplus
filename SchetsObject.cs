using System;
using System.Drawing;

namespace SchetsEditor
{
    public class SchetsObject
    {
        protected Point startpunt, eindpunt;

        public SchetsObject(Point start)
        {
            this.startpunt = start;
        }

        public SchetsObject(Point start, Point eind)
        {
            this.startpunt = start;
            this.eindpunt = eind;
        }
    }

    public class TekstObject : SchetsObject
    {
        protected Brush kwast;
        protected Font font;
        string tekst;

        public TekstObject(Brush kwast, String tekst, Point start) : base(start)
        {
            this.kwast = kwast;
            this.tekst = tekst;
        }
    }

    public class LijnObject : SchetsObject
    {
        protected Pen pen;
        public LijnObject(Pen pen, Point start, Point eind) : base(start)
        {
            this.pen = pen;
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
    }
}