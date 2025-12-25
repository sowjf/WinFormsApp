using System.Drawing;
using System.Numerics;
using WinFormsApp;

namespace WinFormsApp {
    public partial class Form1 : Form {
        private List<Shape> L = new List<Shape>();
        private ShapeType currentShapeType = ShapeType.Circle;

        public Form1() {
            InitializeComponent();

            int centerX = ClientSize.Width / 2;
            int centerY = ClientSize.Height / 2;
            L.Add(new Circle(centerX - 50, centerY - 50));
            L.Add(new Triangle(centerX + 50, centerY - 50));
            L.Add(new Square(centerX, centerY + 50));

            CreateShapeMenu();
            this.DoubleBuffered = true;
        }

        private void CreateShapeMenu() {
            MenuStrip menuStrip = new MenuStrip();
            ToolStripMenuItem shapeMenu = new ToolStripMenuItem("Shape Type");

            ToolStripMenuItem circleItem = new ToolStripMenuItem("Circle");
            circleItem.Click += (s, e) => { currentShapeType = ShapeType.Circle; };

            ToolStripMenuItem triangleItem = new ToolStripMenuItem("Triangle");
            triangleItem.Click += (s, e) => { currentShapeType = ShapeType.Triangle; };

            ToolStripMenuItem squareItem = new ToolStripMenuItem("Square");
            squareItem.Click += (s, e) => { currentShapeType = ShapeType.Square; };

            shapeMenu.DropDownItems.AddRange(new ToolStripItem[] { circleItem, triangleItem, squareItem });
            menuStrip.Items.Add(shapeMenu);

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void Form1_Paint(object sender, PaintEventArgs e) {
            DrawPolygon(e.Graphics);

            foreach (Shape shape in L) {
                shape.Draw(e.Graphics);
            }
        }

        private void DrawPolygon(Graphics g) {
            int n = L.Count;
            if (n < 3) return;

            Pen polygonPen = new Pen(Color.Black, 2);

            foreach (Shape shape in L) {
                shape.IsHullVertex = false;
            }

            List<Shape> hullVertices = new List<Shape>();

            for (int i = 0; i < n; i++) {
                for (int j = i + 1; j < n; j++) {
                    bool oneSide = true;
                    int side = 0;
                    double delta;

                    for (int z = 0; z < n; z++) {
                        if (i == z || j == z) continue;

                        if (L[i].X != L[j].X) {
                            double k = (double)(L[i].Y - L[j].Y) / (L[i].X - L[j].X);
                            double b = L[i].Y - k * L[i].X;
                            double Y_z = k * L[z].X + b;
                            delta = L[z].Y - Y_z;
                        } else {
                            delta = L[z].X - (double)L[i].X;
                        }

                        int currSide = Math.Sign(delta);
                        if (currSide == 0) continue;

                        if (side == 0) side = currSide;
                        else if (currSide != side) {
                            oneSide = false;
                            break;
                        }
                    }

                    if (oneSide) {
                        if (!hullVertices.Contains(L[i])) hullVertices.Add(L[i]);
                        if (!hullVertices.Contains(L[j])) hullVertices.Add(L[j]);
                        L[i].IsHullVertex = true;
                        L[j].IsHullVertex = true;
                    }
                }
            }

            if (hullVertices.Count >= 2) {
                for (int i = 0; i < hullVertices.Count; i++) {
                    for (int j = i + 1; j < hullVertices.Count; j++) {
                        bool isEdge = true;
                        int side = 0;

                        for (int z = 0; z < n; z++) {
                            if (L[z] == hullVertices[i] || L[z] == hullVertices[j]) continue;

                            double delta;
                            if (hullVertices[i].X != hullVertices[j].X) {
                                double k = (double)(hullVertices[i].Y - hullVertices[j].Y) /
                                          (hullVertices[i].X - hullVertices[j].X);
                                double b = hullVertices[i].Y - k * hullVertices[i].X;
                                double Y_z = k * L[z].X + b;
                                delta = L[z].Y - Y_z;
                            } else {
                                delta = L[z].X - (double)hullVertices[i].X;
                            }

                            int currSide = Math.Sign(delta);
                            if (currSide == 0) continue;

                            if (side == 0) side = currSide;
                            else if (currSide != side) {
                                isEdge = false;
                                break;
                            }
                        }

                        if (isEdge) {
                            g.DrawLine(polygonPen, hullVertices[i].X, hullVertices[i].Y,
                                      hullVertices[j].X, hullVertices[j].Y);
                        }
                    }
                }
            }

            for (int i = L.Count - 1; i >= 0; i--) {
                if (!L[i].IsHullVertex) {
                    L.RemoveAt(i);
                }
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e) {
            bool hit = false;

            foreach (Shape shape in L) {
                if (shape.IsInside(e.X, e.Y)) {
                    shape.IsMoved = true;
                    hit = true;
                    break;
                }
            }

            if (!hit) {
                Shape newShape;
                switch (currentShapeType) {
                    case ShapeType.Circle:
                        newShape = new Circle(e.X, e.Y);
                        break;
                    case ShapeType.Triangle:
                        newShape = new Triangle(e.X, e.Y);
                        break;
                    case ShapeType.Square:
                        newShape = new Square(e.X, e.Y);
                        break;
                    default:
                        newShape = new Circle(e.X, e.Y);
                        break;
                }

                L.Add(newShape);
                newShape.IsMoved = true;
                Refresh();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e) {
            foreach (Shape shape in L) {
                shape.IsMoved = false;
            }
            Refresh();
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e) {
            bool moved = false;
            foreach (Shape shape in L) {
                if (shape.IsMoved) {
                    shape.X = e.X;
                    shape.Y = e.Y;
                    moved = true;
                }
            }

            if (moved) {
                Refresh();
            }
        }

        private void Form1_Load(object sender, EventArgs e) { }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e) { }
    }

    public enum ShapeType {
        Circle,
        Triangle,
        Square
    }
}