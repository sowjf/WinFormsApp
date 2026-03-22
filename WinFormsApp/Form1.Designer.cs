namespace WinFormsApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            shapesToolStripMenuItem = new ToolStripMenuItem();
            circleToolStripMenuItem = new ToolStripMenuItem();
            triangleToolStripMenuItem = new ToolStripMenuItem();
            squareToolStripMenuItem = new ToolStripMenuItem();
            drawingModeToolStripMenuItem = new ToolStripMenuItem();
            byDefinitionToolStripMenuItem = new ToolStripMenuItem();
            jarvisToolStripMenuItem = new ToolStripMenuItem();
            graphicsToolStripMenuItem = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem1 = new ToolStripMenuItem();
            settingsToolStripMenuItem1 = new ToolStripMenuItem();
            radiusToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, shapesToolStripMenuItem, drawingModeToolStripMenuItem, settingsToolStripMenuItem, settingsToolStripMenuItem1 });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1003, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            menuStrip1.ItemClicked += menuStrip1_ItemClicked;
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem });
            fileToolStripMenuItem.Enabled = false;
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.Size = new Size(112, 22);
            newToolStripMenuItem.Text = "New";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(112, 22);
            openToolStripMenuItem.Text = "Open";
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(112, 22);
            saveToolStripMenuItem.Text = "Save";
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new Size(112, 22);
            saveAsToolStripMenuItem.Text = "Save as";
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.Enabled = false;
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(39, 20);
            editToolStripMenuItem.Text = "Edit";
            // 
            // shapesToolStripMenuItem
            // 
            shapesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { circleToolStripMenuItem, triangleToolStripMenuItem, squareToolStripMenuItem });
            shapesToolStripMenuItem.Name = "shapesToolStripMenuItem";
            shapesToolStripMenuItem.Size = new Size(56, 20);
            shapesToolStripMenuItem.Text = "Shapes";
            // 
            // circleToolStripMenuItem
            // 
            circleToolStripMenuItem.Name = "circleToolStripMenuItem";
            circleToolStripMenuItem.Size = new Size(116, 22);
            circleToolStripMenuItem.Text = "Circle";
            circleToolStripMenuItem.Click += circleToolStripMenuItem_Click;
            // 
            // triangleToolStripMenuItem
            // 
            triangleToolStripMenuItem.Name = "triangleToolStripMenuItem";
            triangleToolStripMenuItem.Size = new Size(116, 22);
            triangleToolStripMenuItem.Text = "Triangle";
            triangleToolStripMenuItem.Click += triangleToolStripMenuItem_Click;
            // 
            // squareToolStripMenuItem
            // 
            squareToolStripMenuItem.Name = "squareToolStripMenuItem";
            squareToolStripMenuItem.Size = new Size(116, 22);
            squareToolStripMenuItem.Text = "Square";
            squareToolStripMenuItem.Click += squareToolStripMenuItem_Click;
            // 
            // drawingModeToolStripMenuItem
            // 
            drawingModeToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { byDefinitionToolStripMenuItem, jarvisToolStripMenuItem, graphicsToolStripMenuItem });
            drawingModeToolStripMenuItem.Name = "drawingModeToolStripMenuItem";
            drawingModeToolStripMenuItem.Size = new Size(97, 20);
            drawingModeToolStripMenuItem.Text = "Drawing mode";
            // 
            // byDefinitionToolStripMenuItem
            // 
            byDefinitionToolStripMenuItem.Name = "byDefinitionToolStripMenuItem";
            byDefinitionToolStripMenuItem.Size = new Size(142, 22);
            byDefinitionToolStripMenuItem.Text = "By Definition";
            byDefinitionToolStripMenuItem.Click += byDefinitionToolStripMenuItem_Click;
            // 
            // jarvisToolStripMenuItem
            // 
            jarvisToolStripMenuItem.Name = "jarvisToolStripMenuItem";
            jarvisToolStripMenuItem.Size = new Size(142, 22);
            jarvisToolStripMenuItem.Text = "Jarvis";
            jarvisToolStripMenuItem.Click += jarvisToolStripMenuItem_Click;
            // 
            // graphicsToolStripMenuItem
            // 
            graphicsToolStripMenuItem.Name = "graphicsToolStripMenuItem";
            graphicsToolStripMenuItem.Size = new Size(142, 22);
            graphicsToolStripMenuItem.Text = "Graphics";
            graphicsToolStripMenuItem.Click += graphicsToolStripMenuItem_Click;
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(12, 20);
            // 
            // editToolStripMenuItem1
            // 
            editToolStripMenuItem1.Name = "editToolStripMenuItem1";
            editToolStripMenuItem1.Size = new Size(32, 19);
            // 
            // settingsToolStripMenuItem1
            // 
            settingsToolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { radiusToolStripMenuItem });
            settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
            settingsToolStripMenuItem1.Size = new Size(61, 20);
            settingsToolStripMenuItem1.Text = "Settings";
            // 
            // radiusToolStripMenuItem
            // 
            radiusToolStripMenuItem.Name = "radiusToolStripMenuItem";
            radiusToolStripMenuItem.Size = new Size(180, 22);
            radiusToolStripMenuItem.Text = "Radius";
            radiusToolStripMenuItem.Click += radiusToolStripMenuItem_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1003, 649);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Shapes";
            Load += Form1_Load;
            Paint += Form1_Paint;
            MouseDown += Form1_MouseDown;
            MouseMove += Form1_MouseMove;
            MouseUp += Form1_MouseUp;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private void Form1_Paint1(object sender, PaintEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem shapesToolStripMenuItem;
        private ToolStripMenuItem circleToolStripMenuItem;
        private ToolStripMenuItem triangleToolStripMenuItem;
        private ToolStripMenuItem squareToolStripMenuItem;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripMenuItem drawingModeToolStripMenuItem;
        private ToolStripMenuItem byDefinitionToolStripMenuItem;
        private ToolStripMenuItem jarvisToolStripMenuItem;
        private ToolStripMenuItem graphicsToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem1;
        private ToolStripMenuItem settingsToolStripMenuItem1;
        private ToolStripMenuItem radiusToolStripMenuItem;
    }
}
