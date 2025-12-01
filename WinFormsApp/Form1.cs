using System.Drawing;
using WinFormsApp;

namespace WinFormsApp {
    public partial class Form1 : Form {
        private List<Shape> shapes = new List<Shape>();
        private List<Shape> selectedShapes = new List<Shape>();
        private ShapeType currentShapeType = ShapeType.Circle;
        private Point lastMousePosition;
        private bool isDragging = false;

        public Form1() {
            InitializeComponent();
            
            int centerX = ClientSize.Width / 2;
            int centerY = ClientSize.Height / 2;
            shapes.Add(new Circle(centerX - 100, centerY));
            shapes.Add(new Triangle(centerX, centerY));
            shapes.Add(new Square(centerX + 100, centerY));
            
            CreateShapeMenu();
        }

        private void CreateShapeMenu() {
            MenuStrip menuStrip = new MenuStrip();
            ToolStripMenuItem shapeMenu = new ToolStripMenuItem("Shapes");
            
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
            foreach (Shape shape in shapes) {
                if (selectedShapes.Contains(shape)) {
                    shape.Draw(e.Graphics, Color.Red);
                } else {
                    shape.Draw(e.Graphics, Color.Black);
                }
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                lastMousePosition = e.Location;
                isDragging = false;
                
                Shape clickedShape = null;
                foreach (Shape shape in shapes) {
                    if (shape.IsInside(e.X, e.Y)) {
                        clickedShape = shape;
                        break;
                    }
                }
                
                if (clickedShape != null) {
                    if (Control.ModifierKeys == Keys.Control) {
                        if (selectedShapes.Contains(clickedShape)) {
                            selectedShapes.Remove(clickedShape);
                        } else {
                            selectedShapes.Add(clickedShape);
                        }
                    } else {
                        if (!selectedShapes.Contains(clickedShape)) {
                            ClearSelection();
                            selectedShapes.Add(clickedShape);
                        }
                    }
                    
                    foreach (Shape shape in selectedShapes) {
                        shape.IsMoved = true;
                    }
                } else {
                    if (Control.ModifierKeys != Keys.Control) {
                        ClearSelection();
                    }
                    
                    Shape newShape = CreateShapeByType(currentShapeType, e.X, e.Y);
                    shapes.Add(newShape);
                    selectedShapes.Add(newShape);
                    newShape.IsMoved = true;
                }
                
                Refresh();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                foreach (Shape shape in shapes) {
                    shape.IsMoved = false;
                }
                isDragging = false;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left && selectedShapes.Count > 0) {
                isDragging = true;
                int deltaX = e.X - lastMousePosition.X;
                int deltaY = e.Y - lastMousePosition.Y;
                
                foreach (Shape shape in selectedShapes) {
                    shape.X += deltaX;
                    shape.Y += deltaY;
                }
                
                lastMousePosition = e.Location;
                Refresh();
            }
        }

        private void ClearSelection() {
            foreach (Shape shape in selectedShapes) {
                shape.IsMoved = false;
            }
            selectedShapes.Clear();
        }

        private Shape CreateShapeByType(ShapeType type, int x, int y) {
            switch (type) {
                case ShapeType.Circle:
                    return new Circle(x, y);
                case ShapeType.Triangle:
                    return new Triangle(x, y);
                case ShapeType.Square:
                    return new Square(x, y);
                default:
                    return new Circle(x, y);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Delete && selectedShapes.Count > 0) {
                foreach (Shape shape in selectedShapes) {
                    shapes.Remove(shape);
                }
                selectedShapes.Clear();
                Refresh();
            }
        }

        private void Form1_Load(object sender, EventArgs e) {
            this.KeyPreview = true;
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