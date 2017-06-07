using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CruPhysics;
using CruPhysics.Shapes;

using System.Windows;

namespace UnitTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void VectorRotateTest()
        {
            Vector vector = new Vector(0.0, 1.0);

            Assert.AreEqual(new Vector(1.0, 0.0), Common.Rotate(vector, Math.PI / 2.0));
            Assert.AreEqual(
                new Vector(
                    Math.Cos(Math.PI * (1.0 / 2.0 + 1.0 / 6.0)),
                    Math.Sin(Math.PI * (1.0 / 2.0 + 1.0 / 6.0))
                    ),
                Common.Rotate(vector, -Math.PI / 6.0));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CircleSetTest()
        {
            var circle = new CruCircle();
            circle.Set(new Point(), -1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RectangleSetTest()
        {
            var rect = new CruRectangle();
            rect.Set(10, -10, 0, 0);
        }
    }
}
