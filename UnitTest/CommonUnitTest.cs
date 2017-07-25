using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CruPhysics;

using System.Windows;

namespace UnitTest
{
    public class ClassA : NotifyPropertyChangedObject
    {
        private int a;

        public int A
        {
            get
            {
                return a;
            }
            set
            {
                a = value;
                RaisePropertyChangedEvent("A");
            }
        }
    }

    public class ClassB : NotifyPropertyChangedObject
    {
        private int b;

        public int B
        {
            get
            {
                return b;
            }
            set
            {
                b = value;
                RaisePropertyChangedEvent("B");
            }
        }
    }

    [TestClass]
    public class CommonUnitTest
    {
        public object Property { get; set; }

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
        public void TwoWayBindTest()
        {
            var a = new ClassA();
            var b = new ClassB();

            PropertyManager.TwoWayBind(a, "A", b, "B");

            a.A = 10;

            Assert.IsTrue(a.A == b.B);
        }
    }
}
