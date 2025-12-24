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

            foreach (Shape shape in L)
            {
                shape.Draw(e.Graphics);
            }
        }

        private void DrawPolygon(Graphics g) {
            int n = L.Count;
            Pen polygonPen = new Pen(Color.Black, 2);

            for (int i = 0; i < n; i++) {
                for (int j = i + 1; j < n; j++) {

                    bool oneSide = true;
                    int side = 0;

                    if (L[i].X != L[j].X) {
                        double k = (double)(L[i].Y - L[j].Y) / (L[i].X - L[j].X);
                        double b = L[i].Y - k * L[i].X;

                        for (int z = 0; z < n; z++) {
                            if (z == i || z == j) continue;

                            double Y_z = k * L[z].X + b;
                            double delta = L[z].Y - Y_z;

                            int currSide;
                            if (delta > 0) currSide = 1;
                            else currSide = -1;

                            if (side == 0) { side = currSide; }
                            else if (currSide != side) {
                                oneSide = false;
                                break;
                            }
                            if (oneSide) {
                                g.DrawLine(polygonPen, L[i].X, L[i].Y, L[j].X, L[j].Y);
                                L[i].IsInsidePolygon = true;
                                L[j].IsInsidePolygon = true;
                            }
                        }
                    }

                    else {
                        double x_const = L[i].X;

                        for (int z = 0; z < n; z++) {
                            if (z == i || z == j) continue;

                            double delta = L[z].X - x_const;

                            int currSide;
                            if (delta > 0) currSide = 1;
                            else currSide = -1;

                            if (side == 0) { side = currSide; }
                            else if (currSide != side) {
                                oneSide = false;
                                break;
                            }
                        }
                    }

                    if (oneSide) {
                        g.DrawLine(polygonPen, L[i].X, L[i].Y, L[j].X, L[j].Y);
                        L[i].IsInsidePolygon = true;
                        L[j].IsInsidePolygon = true;
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

        private void Form1_Load(object sender, EventArgs e) {
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e) {
        }
    }

    public enum ShapeType {
        Circle,
        Triangle,
        Square
    }
}