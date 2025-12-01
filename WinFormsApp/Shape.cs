using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp {
    public abstract class Shape {
        protected int x, y;
        protected static int R;
        protected bool IsMov;

        public Shape(int x, int y) {
            this.x = x;
            this.y = y;
        }

        static Shape() { R = 25; }

        public abstract void Draw(Graphics g, Color color);
        
        public void Draw(Graphics g) {
            Draw(g, Color.Black);
        }

        public abstract bool IsInside(int pointX, int pointY);

        public bool IsMoved {
            get { return IsMov; }
            set { IsMov = value; }
        }

        public int getR {
            get { return R; }
        }

        public int X {
            get { return x; }
            set { x = value; }
        }

        public int Y {
            get { return y; }
            set { y = value; }
        }
    }

    public class Circle : Shape {
        public Circle(int x, int y) : base(x, y) { }

        public override void Draw(Graphics g, Color color) {
            g.DrawEllipse(new Pen(color, 2), x - R, y - R, 2 * R, 2 * R);
        }

        public override bool IsInside(int pointX, int pointY) {
            return (pointX - x) * (pointX - x) + (pointY - y) * (pointY - y) <= R * R;
        }
    }

    public class Triangle : Shape {
        public Triangle(int x, int y) : base(x, y) { }

        public override void Draw(Graphics g, Color color) {
            Point[] points = {
                new Point(x, y - R), // up
                new Point(x - (int)(R * (Math.Sqrt(3) / 2)), y + R / 2), // left
                new Point(x + (int)(R * (Math.Sqrt(3) / 2)), y + R / 2) // right
            };

            g.DrawPolygon(new Pen(color, 2), points);
        }

        public override bool IsInside(int pointX, int pointY) {
            Point p1 = new Point(x, y - R);
            Point p2 = new Point(x - (int)(R * (Math.Sqrt(3) / 2)), y + R / 2);
            Point p3 = new Point(x + (int)(R * (Math.Sqrt(3) / 2)), y + R / 2);

            double denominator = (p2.Y - p3.Y) * (p1.X - p3.X) + (p3.X - p2.X) * (p1.Y - p3.Y);
            if (denominator == 0) return false;

            double a = ((p2.Y - p3.Y) * (pointX - p3.X) + (p3.X - p2.X) * (pointY - p3.Y)) / denominator;
            double b = ((p3.Y - p1.Y) * (pointX - p3.X) + (p1.X - p3.X) * (pointY - p3.Y)) / denominator;
            double c = 1 - a - b;

            return a >= 0 && a <= 1 && b >= 0 && b <= 1 && c >= 0 && c <= 1;
        }
    }

    public class Square : Shape {
        public Square(int x, int y) : base(x, y) { }

        public override void Draw(Graphics g, Color color) {
            int side = (int)(R * Math.Sqrt(2));
            g.DrawRectangle(new Pen(color, 2), x - side / 2, y - side / 2, side, side);
        }

        public override bool IsInside(int pointX, int pointY) {
            int side = (int)(R * Math.Sqrt(2));
            int halfSide = side / 2;
            return pointX >= x - halfSide && pointX <= x + halfSide && 
                   pointY >= y - halfSide && pointY <= y + halfSide;
        }
    }
}