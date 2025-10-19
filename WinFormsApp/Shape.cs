using System;
using System.Collections.Generic;
using System.Text;

namespace WinFormsApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace WinFormsApp1 {
        public abstract class Shape {
            protected int x, y;
            protected static int R = 5;

            public Shape(int x, int y) {
                this.x = x;
                this.y = y;
            }

            public abstract void draw();
        }

        public class Circle : Shape {
            public Circle(int x, int y) : base(x, y) { }

            public override void draw() { }
        }

        public class Triangle : Shape {
            public Triangle(int x, int y) : base(x, y) { }

            public override void draw() { }
        }

        public class Square : Shape {
            public Square(int x, int y) : base(x, y) { }

            public override void draw() { }
        }
    }
}
