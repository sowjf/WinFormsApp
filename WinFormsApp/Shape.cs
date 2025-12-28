using System;
using System.Drawing;

namespace WinFormsApp {
    public abstract class Shape {
        protected int x, y;
        protected static int R;
        protected bool IsMov;

        public bool IsHullVertex { get; set; }

        static Shape() {
            R = 25;
        }

        public Shape(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public abstract void Draw(Graphics g);
        public abstract bool IsInside(int pointX, int pointY);

        public bool IsMoved {
            get { return IsMov; }
            set { IsMov = value; }
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

        public override void Draw(Graphics g) {
            g.DrawEllipse(Pens.Black, x - R, y - R, 2 * R, 2 * R);
        }

        public override bool IsInside(int pointX, int pointY) {
            return (pointX - x) * (pointX - x) + (pointY - y) * (pointY - y) <= R * R;
        }
    }

    public class Triangle : Shape {
        public Triangle(int x, int y) : base(x, y) { }

        public override void Draw(Graphics g) {
            Point[] points = {
                new Point(x, y - R),
                new Point(x - (int)(R * (Math.Sqrt(3) / 2)), y + R / 2),
                new Point(x + (int)(R * (Math.Sqrt(3) / 2)), y + R / 2)
            };

            g.DrawPolygon(Pens.Black, points);
        }

        public override bool IsInside(int pointX, int pointY) {
            Point p1 = new Point(x, y - R); // up vertex
            Point p2 = new Point(x - (int)(R * (Math.Sqrt(3) / 2)), y + R / 2); // down left vertex
            Point p3 = new Point(x + (int)(R * (Math.Sqrt(3) / 2)), y + R / 2); // down right vertex

            double ab = Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
            double bc = Math.Sqrt((p3.X - p2.X) * (p3.X - p2.X) + (p3.Y - p2.Y) * (p3.Y - p2.Y));
            double ca = Math.Sqrt((p1.X - p3.X) * (p1.X - p3.X) + (p1.Y - p3.Y) * (p1.Y - p3.Y));

            double na = Math.Sqrt((pointX - p1.X) * (pointX - p1.X) + (pointY - p1.Y) * (pointY - p1.Y)),
                   nb = Math.Sqrt((pointX - p2.X) * (pointX - p2.X) + (pointY - p2.Y) * (pointY - p2.Y)),
                   nc = Math.Sqrt((pointX - p3.X) * (pointX - p3.X) + (pointY - p3.Y) * (pointY - p3.Y));

            double p = (ab +  bc + ca) / 2;

            double P1 = (ab + nb + na) / 2,
                   P2 = (bc + nb + nc) / 2,
                   P3 = (ca + na + nc) / 2;

            return (Math.Abs(Math.Sqrt(P1 * (P1 - ab) * (P1 - nb) * (P1 - na)) + Math.Sqrt(P2 * (P2 - bc) * (P2 - nb) * (P2 - nc)) + Math.Sqrt(P3 * (P3 - ca) * (P3 - nc) * (P3 - na)) - Math.Sqrt(p * (p - ab) * (p - bc) * (p - ca)))) <= 0.0001;
        }
    }

    public class Square : Shape {
        public Square(int x, int y) : base(x, y) { }

        public override void Draw(Graphics g) {
            int side = (int)(R * Math.Sqrt(2));
            g.DrawRectangle(Pens.Black, x - side / 2, y - side / 2, side, side);
        }

        public override bool IsInside(int pointX, int pointY) {
            int side = (int)(R * Math.Sqrt(2));
            int halfSide = side / 2;
            return pointX >= x - halfSide && pointX <= x + halfSide &&
                   pointY >= y - halfSide && pointY <= y + halfSide;
        }
    }
}