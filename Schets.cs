﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace SchetsEditor
{
    public class Schets
    {
        private Bitmap bitmap;
        public List<SchetsObject> acties = new List<SchetsObject>();
        
        public Schets()
        {
            bitmap = new Bitmap(1, 1);
        }

        public Bitmap getBitmap()
        {
            return bitmap;
        }

        public Graphics BitmapGraphics
        {
            get { return Graphics.FromImage(bitmap); }
        }

        public void VeranderAfmeting(Size sz)
        {
            if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
            {
                Bitmap nieuw = new Bitmap( Math.Max(sz.Width,  bitmap.Size.Width)
                                         , Math.Max(sz.Height, bitmap.Size.Height)
                                         );
                Graphics gr = Graphics.FromImage(nieuw);
                gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
                gr.DrawImage(bitmap, 0, 0);
                bitmap = nieuw;
            }
        }

		public void Teken()
		{
            this.Teken(BitmapGraphics);
		}


		public void Teken(Graphics gr)
        {
            gr.DrawImage(bitmap, 0, 0);
            foreach(SchetsObject actie in this.acties)
            {
                actie.Teken(gr);
            }
        }

        public void Schoon()
        {
            Graphics gr = Graphics.FromImage(bitmap);
            acties.Clear();
            gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
        }

        public void Roteer()
        {
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }

        public void VerwijderSchetsObject(Point p)
        {
            Console.WriteLine("Alle objecten checken");
            SchetsObject schetsobject;
            foreach (SchetsObject actie in Enumerable.Reverse(this.acties))
			{
                if (actie.Geklikt(p)) { 
                    Console.WriteLine("Object gevonden");
                    schetsobject = actie; 
                    acties.Remove(schetsobject);
                    break;
                }

			}
        }
    }
}
