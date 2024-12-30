using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;

namespace Mapping
{
    [TestFixture]
    [TestOf(typeof(MyDTW))]
    public class MyDTWTest
    {
        [Test]
        public void DTW()
        {
            var random = new Random();
            var left = new List<Vector3>();
            for (int i = 0; i < 10; i++)
            {
                left.Add(
                    new Vector3(random.Next(0, i * 10 + 5), random.Next(0, i * 10 + 5), random.Next(0, i * 10 + 5)));
            }

            var right = new List<Vector3>();
            for (int i = 0; i < 10; i++)
            {
                right.Add(new Vector3(random.Next(0, i * 10 + 50), random.Next(0, i * 10 + 50),
                    random.Next(0, i * 10 + 50)));
            }

            var (min, path) = MyDTW.DTW<Vector3>(left, right, Distance);

            Vector3 average = Vector3.Zero;

            foreach (var (l, r) in path)
            {
                average += right[r] - left[l];
            }

            average /= path.Count;

            for (int i = 0; i < right.Count; i++)
            {
                right[i] -= average;
            }

            var (min2, path2) = MyDTW.DTW<Vector3>(left, right, Distance);

            var pathString = path.Select((l, r) => $"({l}, {r})").Aggregate((a, b) => $"{a}, {b}");
            var path2String = path2.Select((l, r) => $"({l}, {r})").Aggregate((a, b) => $"{a}, {b}");

            Assert.AreEqual(path, path2, $"Min: {min} {min2}, Path: [{pathString}], Path2: [{path2String}]");
            return;

            float Distance(Vector3 a, Vector3 b) => Vector3.DistanceSquared(a, b);
        }
    }
}