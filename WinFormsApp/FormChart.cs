using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WinFormsApp;

namespace WinFormsApp1 {
    public partial class FormChart : Form {
        private List<int> Counts = new List<int>();
        private List<double> jarvisResults = new List<double>();
        private List<double> definitionResults = new List<double>();

        public FormChart() {
            this.Width = 1500;
            this.Height = 1000;
            this.BackColor = Color.White;
            this.Paint += PaintChart;
            Tests();
        }

        public void Tests() {
            Counts.Clear();
            jarvisResults.Clear();
            definitionResults.Clear();

            int points = 200;

            List<int> testCounts = new List<int>();
            testCounts.Add(1);
            for (int i = 1; i < 20; i++) {
                testCounts.Add(i * points);
            }

            foreach (int n in testCounts) {
                Counts.Add(n);
                var testForm = new Form1();
                testForm.L.Clear();

                Random rnd = new Random();
                for (int i = 0; i < n; i++) {
                    int x = rnd.Next(0, 1500);
                    int y = rnd.Next(0, 1000);
                    testForm.L.Add(new Circle(x, y));
                }

                double jarvisTime = testForm.TestJarvis();
                jarvisResults.Add(jarvisTime);

                double definitionTime = testForm.TestDefinition();
                definitionResults.Add(definitionTime);
            }

            this.Invalidate();
        }



        private void PaintChart(object sender, PaintEventArgs e) {
            Graphics g = e.Graphics;

            if (Counts.Count == 0) return;

            int left = 60;
            int down = this.Height - 100;
            int right = this.Width - 40;
            int up = 40;

            Pen linePen = new Pen(Color.Black, 2);
            g.DrawLine(linePen, left, down, right, down);
            g.DrawLine(linePen, left, up, left, down);

            int maxPoints = 0;
            double maxTime = 0;

            for (int i = 0; i < Counts.Count; i++) {
                if (Counts[i] > maxPoints)
                    maxPoints = Counts[i];

                if (jarvisResults[i] > maxTime)
                    maxTime = jarvisResults[i];

                if (definitionResults[i] > maxTime)
                    maxTime = definitionResults[i];
            }

            double maxjarvis = 0;
            for (int i = 0; i < jarvisResults.Count; i++) {
                if (jarvisResults[i] > maxjarvis) {
                    maxjarvis = jarvisResults[i];
                }
            }


            int width = right - left;
            int height = down - up;

            DrawJarvis(g, left, down, width, height, maxPoints, maxTime);
            DrawDefinition(g, left, down, width, height, maxPoints, maxTime);
        }

        private void DrawJarvis(Graphics g, int left, int down, int width, int height, int maxPoints, double maxTime) {
            Pen JarvisPen = new Pen(Color.Blue, 2);

            for (int i = 0; i < Counts.Count - 1; i++) {
                double time1 = jarvisResults[i];
                double time2 = jarvisResults[i + 1];

                int x1 = left + (Counts[i] * width / maxPoints);
                int y1 = down - (int)(time1 * height / maxTime);
                int x2 = left + (Counts[i + 1] * width / maxPoints);
                int y2 = down - (int)(time2 * height / maxTime);

                g.DrawLine(JarvisPen, x1, y1, x2, y2);
            }
        }

        private void DrawDefinition(Graphics g, int left, int down, int width, int height, int maxPoints, double maxTime) {
            Pen DefenitionPen = new Pen(Color.Red, 2);

            for (int i = 0; i < Counts.Count - 1; i++) {
                double time1 = definitionResults[i];
                double time2 = definitionResults[i + 1];

                int x1 = left + (Counts[i] * width / maxPoints);
                int y1 = down - (int)(time1 * height / maxTime);
                int x2 = left + (Counts[i + 1] * width / maxPoints);
                int y2 = down - (int)(time2 * height / maxTime);

                g.DrawLine(DefenitionPen, x1, y1, x2, y2);
            }
        }
    }
}