using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;

namespace Mapping.Algorithms
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

            var offset = new Vector3(random.Next(0, 10), random.Next(0, 10), random.Next(0, 10));

            var right = new List<Vector3>();
            for (int i = 0; i < 20; i++)
            {
                // right.Add(new Vector3(random.Next(0, i * 10 + 50), random.Next(0, i * 10 + 50),
                // random.Next(0, i * 10 + 50)));
                if (random.Next(0, 5) == 0)
                    continue;
                right.Add(
                    left[i / 2] + offset + new Vector3(random.Next(-2, 2), random.Next(-2, 2), random.Next(-2, 2)));
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

            Assert.AreEqual(offset, average, $"Min: {min} {min2}, \nPath: [{pathString}], \nPath2: [{path2String}]");
            return;

            float Distance(Vector3 a, Vector3 b) => Vector3.DistanceSquared(a, b);
        }
    }
}