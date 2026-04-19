using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Windows.Forms;
using WinFormsApp1;

namespace WinFormsApp {
    public partial class Form1 : Form {
        public List<Shape> L = new List<Shape>();
        private ShapeType currShapeType = ShapeType.Circle;
        private DrawingMode drawingMode = DrawingMode.byDefinition;
        Form2 form2 = new Form2();
        private System.Windows.Forms.Timer movementTimer;
        private bool isPlaying = false;
        private Random random = new Random();
        private Color selectedColor = Color.Black;
        private string currentFilePath = null;
        private bool isFileModified = false;
        private int[] customColors = null;
        private int currentSize = 25;

        public Form1() {
            InitializeComponent();
            circleToolStripMenuItem.Checked = true;
            byDefinitionToolStripMenuItem.Checked = true;
            this.DoubleBuffered = true;
            movementTimer = new System.Windows.Forms.Timer();
            movementTimer.Interval = 50;
            movementTimer.Tick += MovementTimer_Tick;
        }

        private void colorToolStripMenuItem1_Click(object sender, EventArgs e) {
            ColorDialog colorDialog = new ColorDialog();

            colorDialog.Color = selectedColor;
            colorDialog.FullOpen = false;

            if (customColors != null) {
                colorDialog.CustomColors = (int[])customColors.Clone();
            }

            if (colorDialog.ShowDialog() == DialogResult.OK) {
                selectedColor = colorDialog.Color;

                foreach (Shape shape in L) {
                    shape.Color = selectedColor;
                }

                customColors = (int[])colorDialog.CustomColors.Clone();
                isFileModified = true;
                Refresh();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            if (L.Count == 0) {
                MessageBox.Show("Shapes haven't found!",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!string.IsNullOrEmpty(currentFilePath)) {
                SaveToFile(currentFilePath);
            } else {
                SaveAs();
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            if (L.Count == 0) {
                MessageBox.Show("Shapes haven't found!",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SaveAs();
        }

        private void SaveAs() {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog()) {
                saveFileDialog.Filter = "Binary files (*.bin)|*.bin|JSON files (*.json)|*.json|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.DefaultExt = "bin";
                saveFileDialog.Title = "Save as...";

                if (!string.IsNullOrEmpty(currentFilePath)) {
                    saveFileDialog.FileName = Path.GetFileName(currentFilePath);
                    saveFileDialog.InitialDirectory = Path.GetDirectoryName(currentFilePath);
                } else {
                    saveFileDialog.FileName = "shapes.bin";
                }

                if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                    currentFilePath = saveFileDialog.FileName;
                    string extension = Path.GetExtension(currentFilePath).ToLower();
                    if (extension == ".json") {
                        SaveToJsonFile(currentFilePath);
                    } else {
                        SaveToFile(currentFilePath);
                    }
                }
            }
        }

        private void SaveToFile(string filePath) {
            try {
                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write)) {
                    var saveData = new SaveData {
                        Shapes = L,
                        SelectedColor = selectedColor,
                        CustomColors = customColors,
                        DefaultSize = currentSize
                    };
                    bf.Serialize(fs, saveData);
                }
                isFileModified = false;
                UpdateFormTitle();
                MessageBox.Show($"File has been saved:\n{filePath}", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (Exception ex) {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveToJsonFile(string filePath) {
            try {
                var saveData = new JsonSaveData {
                    Shapes = new List<JsonShape>(),
                    SelectedColor = selectedColor.ToArgb(),
                    CustomColors = customColors,
                    DefaultSize = currentSize
                };

                foreach (var shape in L) {
                    var jsonShape = new JsonShape {
                        Type = shape.GetType().Name,
                        X = shape.X,
                        Y = shape.Y,
                        Size = shape.Size,
                        Color = shape.Color.ToArgb(),
                        IsVisible = shape.IsVisible
                    };
                    saveData.Shapes.Add(jsonShape);
                }

                string jsonString = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, jsonString);

                isFileModified = false;
                UpdateFormTitle();
                MessageBox.Show($"File has been saved as JSON:\n{filePath}", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (Exception ex) {
                MessageBox.Show($"Error saving JSON: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            if (isFileModified && L.Count > 0) {
                DialogResult result = MessageBox.Show("There are unsaved changes. Save before uploading?",
                    "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes) {
                    saveToolStripMenuItem_Click(sender, e);
                    if (isFileModified) return;
                } else if (result == DialogResult.Cancel) {
                    return;
                }
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.Filter = "Binary files (*.bin)|*.bin|JSON files (*.json)|*.json|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Choose file to load";

                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    string extension = Path.GetExtension(openFileDialog.FileName).ToLower();
                    if (extension == ".json") {
                        LoadFromJsonFile(openFileDialog.FileName);
                    } else {
                        LoadFromFile(openFileDialog.FileName);
                    }
                }
            }
        }

        private void LoadFromFile(string filePath) {
            try {
                if (!File.Exists(filePath)) {
                    MessageBox.Show($"File hasn't found:\n{filePath}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                BinaryFormatter bf = new BinaryFormatter();
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
                    var saveData = (SaveData)bf.Deserialize(fs);
                    L = saveData.Shapes;
                    selectedColor = saveData.SelectedColor;
                    customColors = saveData.CustomColors;
                    if (saveData.DefaultSize > 0) {
                        currentSize = saveData.DefaultSize;
                    }
                }

                foreach (Shape shape in L) {
                    if (shape is Circle circle) {
                        circle.RadiusChanged += Circle_RadiusChanged;
                    }
                    shape.Color = selectedColor;
                    shape.IsVisible = true;
                }

                currentFilePath = filePath;
                isFileModified = false;
                UpdateFormTitle();
                Refresh();
                MessageBox.Show($"Loaded successfully from file:\n{filePath}\nShapes: {L.Count}",
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (Exception ex) {
                MessageBox.Show($"Error to load: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadFromJsonFile(string filePath) {
            try {
                if (!File.Exists(filePath)) {
                    MessageBox.Show($"File hasn't found:\n{filePath}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string jsonString = File.ReadAllText(filePath);
                var saveData = JsonSerializer.Deserialize<JsonSaveData>(jsonString);

                L.Clear();
                selectedColor = Color.FromArgb(saveData.SelectedColor);
                customColors = saveData.CustomColors;
                if (saveData.DefaultSize > 0) {
                    currentSize = saveData.DefaultSize;
                }

                foreach (var jsonShape in saveData.Shapes) {
                    Shape shape = null;
                    switch (jsonShape.Type) {
                        case "Circle":
                            var circle = new Circle(jsonShape.X, jsonShape.Y);
                            circle.Size = jsonShape.Size;
                            circle.RadiusChanged += Circle_RadiusChanged;
                            shape = circle;
                            break;
                        case "Triangle":
                            shape = new Triangle(jsonShape.X, jsonShape.Y);
                            shape.Size = jsonShape.Size;
                            break;
                        case "Square":
                            shape = new Square(jsonShape.X, jsonShape.Y);
                            shape.Size = jsonShape.Size;
                            break;
                    }
                    if (shape != null) {
                        shape.Color = Color.FromArgb(jsonShape.Color);
                        shape.IsVisible = jsonShape.IsVisible;
                        L.Add(shape);
                    }
                }

                currentFilePath = filePath;
                isFileModified = false;
                UpdateFormTitle();
                Refresh();
                MessageBox.Show($"Loaded successfully from JSON file:\n{filePath}\nShapes: {L.Count}",
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (Exception ex) {
                MessageBox.Show($"Error loading JSON: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateFormTitle() {
            string fileName = string.IsNullOrEmpty(currentFilePath) ? "New file" : Path.GetFileName(currentFilePath);
            string modifiedMark = isFileModified ? "*" : "";
            this.Text = $"{fileName}{modifiedMark} - Shapes";
        }

        private void MarkAsModified() {
            if (!isFileModified) {
                isFileModified = true;
                UpdateFormTitle();
            }
        }

        private void MovementTimer_Tick(object sender, EventArgs e) {
            if (!isPlaying) return;
            foreach (Shape shape in L) {
                shape.X += random.Next(-1, 2);
                shape.Y += random.Next(-1, 2);
            }
            MarkAsModified();
            Refresh();
        }

        private void toolStripButton1_Click(object sender, EventArgs e) {
            isPlaying = true;
            movementTimer.Start();
            playToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Enabled = true;
        }

        private void toolStripButton2_Click(object sender, EventArgs e) {
            isPlaying = false;
            movementTimer.Stop();
            playToolStripMenuItem.Enabled = true;
            stopToolStripMenuItem.Enabled = false;
        }

        private void Circle_RadiusChanged(object sender, RadiusEventArgs e) {
            if (sender is Circle circle) {
                System.Diagnostics.Debug.WriteLine($"Radius changed: {e.OldRadius} -> {e.NewRadius}");
                MarkAsModified();
                Refresh();
            }
        }

        private int OnRadiusChanged(int s, bool b) {
            if (s > 0) {
                currentSize = s;
                foreach (Shape shape in L) {
                    shape.Size = s;
                }
                MarkAsModified();
                Refresh();
            }
            return s;
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
            if (L.Count < 1) return;
            foreach (Shape shape in L) {
                shape.IsHullVertex = false;
            }
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
            Pen polygonPen = null;
            if (g != null) {
                polygonPen = new Pen(Color.Red, 1);
            }
            foreach (Shape shape in L) {
                shape.IsHullVertex = false;
            }
            List<Shape> convexHull = new List<Shape>();
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
            bool removed = false;
            if (e.Button == MouseButtons.Left) {
                bool hit = false;
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
                            var circle = new Circle(e.X, e.Y);
                            circle.Size = currentSize;
                            circle.RadiusChanged += Circle_RadiusChanged;
                            newShape = circle;
                            break;
                        case ShapeType.Triangle:
                            newShape = new Triangle(e.X, e.Y);
                            newShape.Size = currentSize;
                            break;
                        case ShapeType.Square:
                            newShape = new Square(e.X, e.Y);
                            newShape.Size = currentSize;
                            break;
                        default:
                            var defaultCircle = new Circle(e.X, e.Y);
                            defaultCircle.Size = currentSize;
                            defaultCircle.RadiusChanged += Circle_RadiusChanged;
                            newShape = defaultCircle;
                            break;
                    }
                    newShape.Color = selectedColor;
                    L.Add(newShape);
                    newShape.IsMoving = true;
                    if (L.Count == 1) {
                        L[0].IsVisible = true;
                    }
                    MarkAsModified();
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
                if (removed) {
                    Refresh();
                }
            } else if (e.Button == MouseButtons.Right) {
                for (int i = L.Count - 1; i >= 0; i--) {
                    if (L[i].IsInside(e.X, e.Y)) {
                        L.RemoveAt(i);
                        removed = true;
                        MarkAsModified();
                    }
                }
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
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
                    MarkAsModified();
                    Refresh();
                }
            }
            return;
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
                        MarkAsModified();
                    }
                }
            }
            Refresh();
        }

        private void Form1_Load(object sender, EventArgs e) {
            UpdateFormTitle();
        }

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

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }

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

        private void radiusToolStripMenuItem_Click(object sender, EventArgs e) {
            int currentSizeValue = (L.Count > 0) ? L[0].Size : currentSize;
            if (form2 == null || form2.IsDisposed) {
                form2 = new Form2();
            }
            form2.SetRadius(currentSizeValue);
            form2.RadiusChanged += OnRadiusChanged;
            form2.Show();
        }

        protected override void OnFormClosing(FormClosingEventArgs e) {
            if (isFileModified && L.Count > 0) {
                DialogResult result = MessageBox.Show("There are unsaved changes. Save before uploading?",
                    "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes) {
                    saveToolStripMenuItem_Click(null, null);
                    if (isFileModified) {
                        e.Cancel = true;
                    }
                } else if (result == DialogResult.Cancel) {
                    e.Cancel = true;
                }
            }
            base.OnFormClosing(e);
        }

        private void binToolStripMenuItem_Click(object sender, EventArgs e) {
            if (L.Count == 0) {
                MessageBox.Show("No shapes to save!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog()) {
                saveFileDialog.Filter = "Binary files (*.bin)|*.bin";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.DefaultExt = "bin";
                saveFileDialog.Title = "Save as BIN...";
                saveFileDialog.FileName = "shapes.bin";

                if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                    currentFilePath = saveFileDialog.FileName;
                    SaveToFile(currentFilePath);
                }
            }
        }

        private void jsonToolStripMenuItem_Click(object sender, EventArgs e) {
            if (L.Count == 0) {
                MessageBox.Show("No shapes to save!", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog()) {
                saveFileDialog.Filter = "JSON files (*.json)|*.json";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.DefaultExt = "json";
                saveFileDialog.Title = "Save as JSON...";
                saveFileDialog.FileName = "shapes.json";

                if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                    currentFilePath = saveFileDialog.FileName;
                    SaveToJsonFile(currentFilePath);
                }
            }
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