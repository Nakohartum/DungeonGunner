using System;
using UnityEngine;

namespace AStar
{
    public class Node : IComparable<Node>
    {
        public Vector2Int gridPosition;
        public int gCost = 0;
        public int hCost = 0;
        public Node parentNode;

        public int FCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        public Node(Vector2Int gridPosition)
        {
            this.gridPosition = gridPosition;
            parentNode = null;
        }
        
        public int CompareTo(Node nodeToCompare)
        {
            int compare = FCost.CompareTo(nodeToCompare.FCost);
            if (compare == 0)
            {
                compare = hCost.CompareTo(nodeToCompare.hCost);
            }
            return compare;
        }
    }
}