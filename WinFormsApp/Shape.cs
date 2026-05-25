using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace WinFormsApp {
    [Serializable]
    public class RadiusEventArgs : EventArgs {
        public int OldRadius { get; }
        public int NewRadius { get; }

        public RadiusEventArgs(int oldRadius, int newRadius) {
            OldRadius = oldRadius;
            NewRadius = newRadius;
        }
    }

    [Serializable]
    public class Change {
        public enum ChangeType {
            Add,
            Remove,
            Move
        }

        public ChangeType Type { get; set; }
        public Shape Shape { get; set; }
        public int OldX { get; set; }
        public int OldY { get; set; }
        public int NewX { get; set; }
        public int NewY { get; set; }
        public int Index { get; set; }

        public Change(ChangeType type, Shape shape) {
            Type = type;
            Shape = shape;
        }

        public Change(Shape shape, int oldX, int oldY, int newX, int newY) {
            Type = ChangeType.Move;
            Shape = shape;
            OldX = oldX;
            OldY = oldY;
            NewX = newX;
            NewY = newY;
        }
    }

    [Serializable]
    public abstract class Shape {
        protected int x, y;
        protected int R;
        protected bool IsMov;
        public bool IsVisible = false;

        private Color color = Color.Black;

        public Color Color {
            get => color;
            set => color = value;
        }

        public bool IsHullVertex { get; set; }

        public virtual int Size {
            get { return R; }
            set {
                if (value > 0) {
                    R = value;
                }
            }
        }

        protected Shape() {
            R = 25;
        }

        public Shape(int x, int y) {
            this.x = x;
            this.y = y;
            this.R = 25;
        }

        public abstract void Draw(Graphics g);
        public abstract bool IsInside(int pointX, int pointY);

        public bool IsMoving {
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

    [Serializable]
    public class Circle : Shape {
        [field: NonSerialized]
        public event EventHandler<RadiusEventArgs> RadiusChanged;

        protected Circle() : base() { }

        public Circle(int x, int y) : base(x, y) { }

        public int Radius {
            get { return R; }
            set {
                if (R != value && value > 0) {
                    int oldRadius = R;
                    R = value;
                    OnRadiusChanged(new RadiusEventArgs(oldRadius, R));
                }
            }
        }

        public override int Size {
            get { return Radius; }
            set { Radius = value; }
        }

        protected virtual void OnRadiusChanged(RadiusEventArgs e) {
            RadiusChanged?.Invoke(this, e);
        }

        public override void Draw(Graphics g) {
            if (IsVisible) {
                using (SolidBrush brush = new SolidBrush(Color)) {
                    g.FillEllipse(brush, x - R, y - R, 2 * R, 2 * R);
                }
            }
        }

        public override bool IsInside(int pointX, int pointY) {
            return (pointX - x) * (pointX - x) + (pointY - y) * (pointY - y) <= R * R;
        }
    }

    [Serializable]
    public class Triangle : Shape {
        protected Triangle() : base() { }

        public Triangle(int x, int y) : base(x, y) { }

        public override void Draw(Graphics g) {
            if (IsVisible) {
                Point[] points = {
                    new Point(x, y - R),
                    new Point(x - (int)(R * (Math.Sqrt(3) / 2)), y + R / 2),
                    new Point(x + (int)(R * (Math.Sqrt(3) / 2)), y + R / 2)
                };

                using (SolidBrush brush = new SolidBrush(Color)) {
                    g.FillPolygon(brush, points);
                }
            }
        }

        public override bool IsInside(int pointX, int pointY) {
            Point p1 = new Point(x, y - R);
            Point p2 = new Point(x - (int)(R * (Math.Sqrt(3) / 2)), y + R / 2);
            Point p3 = new Point(x + (int)(R * (Math.Sqrt(3) / 2)), y + R / 2);

            double ab = Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
            double bc = Math.Sqrt((p3.X - p2.X) * (p3.X - p2.X) + (p3.Y - p2.Y) * (p3.Y - p2.Y));
            double ca = Math.Sqrt((p1.X - p3.X) * (p1.X - p3.X) + (p1.Y - p3.Y) * (p1.Y - p3.Y));

            double na = Math.Sqrt((pointX - p1.X) * (pointX - p1.X) + (pointY - p1.Y) * (pointY - p1.Y)),
                   nb = Math.Sqrt((pointX - p2.X) * (pointX - p2.X) + (pointY - p2.Y) * (pointY - p2.Y)),
                   nc = Math.Sqrt((pointX - p3.X) * (pointX - p3.X) + (pointY - p3.Y) * (pointY - p3.Y));

            double p = (ab + bc + ca) / 2;

            double P1 = (ab + nb + na) / 2,
                   P2 = (bc + nb + nc) / 2,
                   P3 = (ca + na + nc) / 2;

            return (Math.Abs(Math.Sqrt(P1 * (P1 - ab) * (P1 - nb) * (P1 - na)) +
                   Math.Sqrt(P2 * (P2 - bc) * (P2 - nb) * (P2 - nc)) +
                   Math.Sqrt(P3 * (P3 - ca) * (P3 - nc) * (P3 - na)) -
                   Math.Sqrt(p * (p - ab) * (p - bc) * (p - ca)))) <= 0.0001;
        }
    }

    [Serializable]
    public class Square : Shape {
        protected Square() : base() { }

        public Square(int x, int y) : base(x, y) { }

        public override void Draw(Graphics g) {
            if (IsVisible) {
                int side = (int)(R * Math.Sqrt(2));
                using (SolidBrush brush = new SolidBrush(Color)) {
                    g.FillRectangle(brush, x - side / 2, y - side / 2, side, side);
                }
            }
        }

        public override bool IsInside(int pointX, int pointY) {
            int side = (int)(R * Math.Sqrt(2));
            int halfSide = side / 2;
            return pointX >= x - halfSide && pointX <= x + halfSide &&
                   pointY >= y - halfSide && pointY <= y + halfSide;
        }
    }

    [Serializable]
    public class SaveData {
        public List<Shape> Shapes { get; set; }
        public System.Drawing.Color SelectedColor { get; set; }
        public int[] CustomColors { get; set; }
        public int DefaultSize { get; set; }
    }

    public class JsonShape {
        public string Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Size { get; set; }
        public int Color { get; set; }
        public bool IsVisible { get; set; }
    }

    public class JsonSaveData {
        public List<JsonShape> Shapes { get; set; }
        public int SelectedColor { get; set; }
        public int[] CustomColors { get; set; }
        public int DefaultSize { get; set; }
    }
}