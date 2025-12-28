using System;
using System.Drawing;
using System.Numerics;
using WinFormsApp;

namespace WinFormsApp {
    public partial class Form1 : Form {
        private List<Shape> L = new List<Shape>();
        private ShapeType currentShapeType = ShapeType.Circle;

        public Form1() {
            InitializeComponent();
            circleToolStripMenuItem.Checked = true;

            int centerX = ClientSize.Width / 2;
            int centerY = ClientSize.Height / 2;
            //L.Add(new Circle(centerX - 50, centerY - 50));
            //L.Add(new Triangle(centerX + 50, centerY - 50));
            //L.Add(new Square(centerX, centerY + 50));

            this.DoubleBuffered = true;
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

            List<Shape> convexHull = new List<Shape>(); //точки выпуклой оболочки

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

                            delta = L[z].Y - (double)(k * L[z].X + b);
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
                        if (!convexHull.Contains(L[i])) convexHull.Add(L[i]);
                        if (!convexHull.Contains(L[j])) convexHull.Add(L[j]);
                        L[i].IsHullVertex = true;
                        L[j].IsHullVertex = true;
                    }
                }
            }

            if (convexHull.Count >= 2) {
                for (int i = 0; i < convexHull.Count; i++) {
                    for (int j = i + 1; j < convexHull.Count; j++) {
                        bool isEdge = true;
                        int side = 0;

                        for (int z = 0; z < n; z++) {
                            if (L[z] == convexHull[i] || L[z] == convexHull[j]) continue;

                            double delta;
                            if (convexHull[i].X != convexHull[j].X) {
                                double k = (double)(convexHull[i].Y - convexHull[j].Y) /
                                          (convexHull[i].X - convexHull[j].X);

                                double b = convexHull[i].Y - k * convexHull[i].X;
                                delta = L[z].Y - (double)(k * L[z].X + b);

                            } else {
                                delta = L[z].X - (double)convexHull[i].X;
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
                            g.DrawLine(polygonPen, convexHull[i].X, convexHull[i].Y,
                                      convexHull[j].X, convexHull[j].Y);
                        }
                    }
                }
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e) {
            bool hit = false;

            foreach (Shape shape in L) {
                if (shape.IsInside(e.X, e.Y)) {
                    shape.IsMoved = true;
                    hit = true;
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

        private void Form1_MouseMove(object sender, MouseEventArgs e) {
            bool moved = false;

            List<Shape> movedShapes = new List<Shape>();
            foreach (Shape shape in L) {
                if (shape.IsMoved) {
                    movedShapes.Add(shape);
                }
            }

            if (movedShapes.Count > 0) {
                Shape firstMoved = movedShapes[0];
                int deltaX = e.X - firstMoved.X;
                int deltaY = e.Y - firstMoved.Y;

                foreach (Shape shape in movedShapes) {
                    shape.X += deltaX;
                    shape.Y += deltaY;
                }

                moved = true;
            }

            if (moved) {
                Refresh();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e) {
            foreach (Shape shape in L) {
                if (shape.IsInside(e.X, e.Y)) {
                    shape.IsMoved = false;
                }
            }

            if (L.Count > 3) {
                for (int i = L.Count - 1; i >= 0; i--) {
                    if (!L[i].IsHullVertex) {
                        L.RemoveAt(i);
                    }
                }
            }

            Refresh();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                    
            } else if (e.Button == MouseButtons.Right) {
                bool removed = false;
                for (int i = L.Count - 1; i >= 0; i--) {
                    if (L[i].IsInside(e.X, e.Y)) {
                        L.RemoveAt(i);
                        removed = true;
                    }
                }

                if (removed) {
                    Refresh();
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e) { }

        //private void fileToolStripMenuItem_Click(object sender, EventArgs e) { }

        private void circleToolStripMenuItem_Click(object sender, EventArgs e) {
            currentShapeType = ShapeType.Circle;
            circleToolStripMenuItem.Checked = true;
            triangleToolStripMenuItem.Checked = false;
            squareToolStripMenuItem.Checked = false;
        }

        private void triangleToolStripMenuItem_Click(object sender, EventArgs e) {
            currentShapeType = ShapeType.Triangle;
            circleToolStripMenuItem.Checked = false;
            triangleToolStripMenuItem.Checked = true;
            squareToolStripMenuItem.Checked = false;
        }

        private void squareToolStripMenuItem_Click(object sender, EventArgs e) {
            currentShapeType = ShapeType.Square;
            circleToolStripMenuItem.Checked = false;
            triangleToolStripMenuItem.Checked = false;
            squareToolStripMenuItem.Checked = true;
        }

    }

    public enum ShapeType {
        Circle,
        Triangle,
        Square
    }
}