using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Mapping.Data;
using UnityEngine;

namespace Mapping
{
    public class NodePosition
    {
        private static readonly ConditionalWeakTable<Node, NodePosition> Instances =
            new ConditionalWeakTable<Node, NodePosition>();

        private readonly Node node;
        private readonly GameObject gameObject;

        private NodePosition(Node node, GameObject gameObject)
        {
            this.node = node;
            this.gameObject = gameObject;
        }

        public void UpdatePosition()
        {
            gameObject.transform.position = node.Position + DroneView.DroneView.Offset;
        }

        public static NodePosition Of(Node node)
        {
            if (Instances.TryGetValue(node, out var instance))
            {
                return instance;
            }

            return null;
        }

        public static void Create(Node node, GameObject gameObject)
        {
            var nodePosition = new NodePosition(node, gameObject);
            Instances.Add(node, nodePosition);
            nodePosition.UpdatePosition();
        }
    }
}