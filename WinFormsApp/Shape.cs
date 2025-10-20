using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp
{
    public abstract class Shape {
        protected int x, y;
        protected static int R;

        public Shape(int x, int y) {
            this.x = x;
            this.y = y;
        }

        static Shape() { R = 50; }
        public abstract void Draw(Graphics g);
        public abstract bool IsInside(int pointX, int pointY);
    }

    public class Circle : Shape {
        public Circle(int x, int y) : base(x, y) { }

        public override void Draw(Graphics g) { g.DrawEllipse(Pens.Black, this.x - R, this.y - R, 2 * R, 2 * R); }

        public override bool IsInside(int pointX, int pointY) { return (pointX - x) * (pointX - x) + (pointY - y) * (pointY - y) <= R * R; }
    }

    public class Triangle : Shape {
        public Triangle(int x, int y) : base(x, y) { }

        public override void Draw(Graphics g) {
            Point[] points = {
                new Point(x, y - R), // up
                new Point(x - R, y + R), // left
                new Point(x + R, y + R) // right
            };

           g.DrawPolygon(Pens.Black, points);
        }

        public override bool IsInside(int pointX, int pointY) { 
            int a = (int)(Math.Sqrt(R * R / 2));
            if ((pointX < this.x + a) & (pointX > this.x - a) & (pointX < y + a) & (pointY > y + a)) { return true; }
            return false;
        }
    }

    public class Square : Shape {
        public Square(int x, int y) : base(x, y) { }

        public override void Draw(Graphics g) { g.DrawRectangle(Pens.Black, x - R, y - R, 2 * R, 2 * R); }

        public override bool IsInside(int pointX, int pointY) { return pointX >= x - R && pointX <= x + R && pointY >= y - R && pointY <= y + R; }
    }

}
