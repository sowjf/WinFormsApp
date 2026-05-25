using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace WinFormsApp {
    public partial class Form2 : Form {
        public delegate int SomeDelegate(int s, bool isFinal);
        public event SomeDelegate RadiusChanged;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SomeDelegate S { get; set; }

        private TrackBar trackBar;
        private Label valueLabel;
        private int currentRadius = 25;

        public Form2() {
            this.Text = "Radius settings";
            this.Size = new System.Drawing.Size(400, 200);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label titleLabel = new Label {
                Text = "Changing radius:",
                Location = new System.Drawing.Point(50, 20),
                Size = new System.Drawing.Size(200, 20),
                Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold)
            };

            trackBar = new TrackBar {
                Location = new System.Drawing.Point(50, 50),
                Size = new System.Drawing.Size(250, 45),
                Minimum = 10,
                Maximum = 100,
                Value = currentRadius,
                TickFrequency = 10,
                LargeChange = 10,
                SmallChange = 5
            };
            trackBar.Scroll += TrackBar_Scroll;
            trackBar.MouseUp += TrackBar_MouseUp;

            valueLabel = new Label {
                Location = new System.Drawing.Point(50, 100),
                Size = new System.Drawing.Size(200, 25),
                Text = $"Current radius: {currentRadius}",
                Font = new System.Drawing.Font("Arial", 10)
            };

            this.Controls.Add(titleLabel);
            this.Controls.Add(trackBar);
            this.Controls.Add(valueLabel);

            S = new SomeDelegate(CalculateRadius);
        }

        public void SetRadius(int radius) {
            currentRadius = radius;
            trackBar.Value = radius;
            valueLabel.Text = $"Current radius: {radius}";
        }

        private int CalculateRadius(int s, bool isFinal) {
            return s;
        }

        private void TrackBar_Scroll(object sender, EventArgs e) {
            currentRadius = trackBar.Value;
            valueLabel.Text = $"Current radius: {currentRadius}";
            // Передаем isFinal = false - обновляем радиус但没有 сохраняем в стек
            RadiusChanged?.Invoke(currentRadius, false);
        }

        private void TrackBar_MouseUp(object sender, MouseEventArgs e) {
            // Передаем isFinal = true - сохраняем только это значение в стек
            RadiusChanged?.Invoke(currentRadius, true);
        }
    }
}