using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Mapping
{
    public class EdgePosition
    {
        private static readonly ConditionalWeakTable<Edge, EdgePosition> Instances =
            new ConditionalWeakTable<Edge, EdgePosition>();

        private readonly Edge edge;
        private readonly GameObject gameObject;

        private EdgePosition(Edge edge, GameObject gameObject)
        {
            this.edge = edge;
            this.gameObject = gameObject;
        }

        public void UpdatePosition()
        {
            gameObject.transform.position = (edge.From.Position + edge.To.Position) / 2 + DroneView.DroneView.Offset;
            gameObject.transform.rotation = Quaternion.LookRotation(edge.To.Position - edge.From.Position);
            gameObject.transform.localScale = new Vector3(1, 1, Vector3.Distance(edge.From.Position, edge.To.Position));
        }

        public void SetColor(Color color)
        {
            gameObject.GetComponentInChildren<MeshRenderer>().material.color = color;
        }

        public static EdgePosition Of(Edge edge)
        {
            if (Instances.TryGetValue(edge, out var instance))
            {
                return instance;
            }

            return null;
        }

        public static void Create(Edge edge, GameObject gameObject)
        {
            var edgePosition = new EdgePosition(edge, gameObject);
            Instances.Add(edge, edgePosition);
            edgePosition.UpdatePosition();
        }
    }
}