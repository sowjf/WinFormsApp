using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using WinFormsApp;
using WinFormsApp1;
using static System.Net.Mime.MediaTypeNames;

namespace WinFormsApp {
    public partial class Form1 : Form {
        public List<Shape> L = new List<Shape>();
        private ShapeType currShapeType = ShapeType.Circle;
        private DrawingMode drawingMode = DrawingMode.byDefinition;

        public Form1() {
            InitializeComponent();
            circleToolStripMenuItem.Checked = true;
            byDefinitionToolStripMenuItem.Checked = true;

            //int centerX = ClientSize.Width / 2;
            //int centerY = ClientSize.Height / 2;
            //L.Add(new Circle(centerX - 50, centerY - 50));
            //L.Add(new Triangle(centerX + 50, centerY - 50));
            //L.Add(new Square(centerX, centerY + 50));

            this.DoubleBuffered = true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e) {

            switch (drawingMode) {
                case DrawingMode.byDefinition:
                    DrawPolygonByDifinition(e.Graphics);
                    break;
                case DrawingMode.jarvis:
                    DrawPolygonJarvis(e.Graphics);
                    break;
                case DrawingMode.graphics:
                    break;
                //default:
                //    break;
            }

            foreach (Shape shape in L) {
                shape.Draw(e.Graphics);
            }
        }

        public double TestJarvis() {
            var stopwatch = Stopwatch.StartNew();
            DrawPolygonJarvis(null);
            stopwatch.Stop();
            return stopwatch.Elapsed.TotalMilliseconds;
        }

        public double TestDefinition() {
            var stopwatch = Stopwatch.StartNew();
            DrawPolygonByDifinition(null);
            stopwatch.Stop();
            return stopwatch.Elapsed.TotalMilliseconds;
        }

        public void DrawPolygonJarvis(Graphics g) {
            //if (L.Count < 3) return;

            Pen polygonPen = new Pen(Color.Blue, 1);

            foreach (Shape shape in L) {
                shape.IsHullVertex = false;
            }

            List<Shape> convexHull = new List<Shape>(); //точки выпуклой оболочки

            Shape start = L[0];
            foreach (Shape point in L) {
                if (point.X < start.X || (point.X == start.X && point.Y < start.Y)) {
                    start = point;
                }
            }

            List<Shape> hull = new List<Shape>();
            Shape curr = start;

            while (true) {
                hull.Add(curr);
                Shape next = L[0];

                foreach (Shape point in L) {
                    if (point == curr) continue;

                    float orientation = (next.X - curr.X) * (point.Y - curr.Y) -
                                        (next.Y - curr.Y) * (point.X - curr.X);

                    if (next == curr || orientation < 0 ||
                        (orientation == 0 && Distance(curr, point) > Distance(curr, next))) {
                        next = point;
                    }
                }

                curr = next;
                if (curr == start) break;
            }

            foreach (Shape shape in hull) {
                shape.IsHullVertex = true;
            }

            if (hull.Count >= 2) {
                PointF[] points = new PointF[hull.Count];
                for (int i = 0; i < hull.Count; i++) {
                    points[i] = new PointF(hull[i].X, hull[i].Y);
                }

                if (g != null) {
                    g.DrawPolygon(Pens.Blue, points);
                }
            }
        }

        private static float Distance(Shape a, Shape b) {
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;
            return dx * dx + dy * dy;
        }

        private void DrawPolygonByDifinition(Graphics g) {
            int n = L.Count;
            //if (n < 3) return;

            Pen polygonPen = null;
            if (g != null) {
                polygonPen = new Pen(Color.Red, 1);
            }


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

                        if (side == 0) {
                            side = currSide;
                        } else if (currSide != side) {
                            oneSide = false;
                            break;
                        }
                    }

                    if (oneSide) {
                        if (!convexHull.Contains(L[i])) {
                            convexHull.Add(L[i]);
                        }
                        if (!convexHull.Contains(L[j])) {
                            convexHull.Add(L[j]);
                        }
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

                            if (side == 0) {
                                side = currSide;
                            } else if (currSide != side) {
                                isEdge = false;
                                break;
                            }
                        }

                        if (isEdge && g != null && polygonPen != null) {
                            g.DrawLine(polygonPen, convexHull[i].X, convexHull[i].Y,
                                      convexHull[j].X, convexHull[j].Y);
                        }
                    }
                }
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e) {
            bool hit = false;
            bool removed = false;

            foreach (Shape shape in L) {
                if (shape.IsInside(e.X, e.Y)) {
                    shape.IsMoving = true;
                    hit = true;
                }
            }

            if (!hit) {
                Shape newShape;

                switch (currShapeType) {
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
                newShape.IsMoving = true;

                if (L.Count == 1) {
                    L[0].IsVisible = true;
                }

                Refresh();

                if (!newShape.IsHullVertex) {
                    foreach (Shape shape in L) {
                        shape.IsMoving = true;
                    }
                } else {
                    newShape.IsVisible = true;
                }

                Refresh();
            }

            if (e.Button == MouseButtons.Right) {
                for (int i = L.Count - 1; i >= 0; i--) {
                    if (L[i].IsInside(e.X, e.Y)) {
                        L.RemoveAt(i);
                        removed = true;
                    }
                }
            }

            if (removed) {
                Refresh();
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e) {
            bool moved = false;

            List<Shape> movedShapes = new List<Shape>();
            foreach (Shape shape in L) {
                if (shape.IsMoving) {
                    movedShapes.Add(shape);
                }
            }

            if (movedShapes.Count > 0) {
                Shape firstMoved;

                if (movedShapes.Count > 0 && !movedShapes[movedShapes.Count - 1].IsHullVertex) {
                    firstMoved = movedShapes[movedShapes.Count - 1];
                } else {
                    firstMoved = movedShapes[0];
                }

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
                    shape.IsMoving = false;
                }
            }

            if (L.Count > 0 && !L[L.Count - 1].IsHullVertex) {
                foreach (Shape shape in L) {
                    shape.IsMoving = false;
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

        private void Form1_Load(object sender, EventArgs e) { }

        private void circleToolStripMenuItem_Click(object sender, EventArgs e) {
            currShapeType = ShapeType.Circle;
            circleToolStripMenuItem.Checked = true;
            triangleToolStripMenuItem.Checked = false;
            squareToolStripMenuItem.Checked = false;
        }

        private void triangleToolStripMenuItem_Click(object sender, EventArgs e) {
            currShapeType = ShapeType.Triangle;
            circleToolStripMenuItem.Checked = false;
            triangleToolStripMenuItem.Checked = true;
            squareToolStripMenuItem.Checked = false;
        }

        private void squareToolStripMenuItem_Click(object sender, EventArgs e) {
            currShapeType = ShapeType.Square;
            circleToolStripMenuItem.Checked = false;
            triangleToolStripMenuItem.Checked = false;
            squareToolStripMenuItem.Checked = true;
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
        }

        private void byDefinitionToolStripMenuItem_Click(object sender, EventArgs e) {
            drawingMode = DrawingMode.byDefinition;
            byDefinitionToolStripMenuItem.Checked = true;
            jarvisToolStripMenuItem.Checked = false;
            graphicsToolStripMenuItem.Checked = false;

            Refresh();
        }

        private void jarvisToolStripMenuItem_Click(object sender, EventArgs e) {
            drawingMode = DrawingMode.jarvis;
            byDefinitionToolStripMenuItem.Checked = false;
            jarvisToolStripMenuItem.Checked = true;
            graphicsToolStripMenuItem.Checked = false;

            Refresh();
        }

        public void Tests() {
            FormChart chartForm = new FormChart();
            chartForm.Show();
        }

        private void graphicsToolStripMenuItem_Click(object sender, EventArgs e) {
            drawingMode = DrawingMode.graphics;
            byDefinitionToolStripMenuItem.Checked = false;
            jarvisToolStripMenuItem.Checked = false;
            graphicsToolStripMenuItem.Checked = true;

            Tests();
        }
    }

    public enum ShapeType {
        Circle,
        Triangle,
        Square
    }

    public enum DrawingMode {
        byDefinition,
        jarvis,
        graphics
    }
}