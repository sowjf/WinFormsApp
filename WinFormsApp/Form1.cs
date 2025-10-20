using System.Drawing;
using WinFormsApp;

namespace WinFormsApp
{
    public partial class Form1 : Form
    {
        private Shape S;

        private void Form1_Load(object sender, EventArgs e) {

        }

        public Form1() {
            InitializeComponent();

            S = new Square(200, 200);

            Console.WriteLine(S.IsInside(100, 100));
            this.Paint += Form1_Paint;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Вызываем Draw от S, передав e.Graphics
            S.Draw(e.Graphics);
        }


        private void button1_Click(object sender, EventArgs e) {
            Graphics g = this.CreateGraphics();
            g.DrawEllipse(new Pen(Color.Red), 100, 100, 200, 200);
        }
    }
}