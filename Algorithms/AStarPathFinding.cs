using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinding : MonoBehaviour
{
    private WaitForSeconds waitFor50ms = new WaitForSeconds(0.05f);
    private WaitForSeconds waitFor20ms = new WaitForSeconds(0.02f);
    private bool finding = false;

    public bool Finding { get => finding; set => finding = value; }

    public class Node
    {
        private TileMap.Node graphNode;
        private TileMap.Node graphNodeParent;
        private int totalCost;
        private int evaluationVal;

        public Node(TileMap.Node graphNode)
        {
            this.GraphNode = graphNode;
        }

        public TileMap.Node GraphNode { get => graphNode; set => graphNode = value; }
        public TileMap.Node GraphNodeParent { get => graphNodeParent; set => graphNodeParent = value; }
        public int TotalCost { get => totalCost; set => totalCost = value; }
        public int EvaluationVal { get => evaluationVal; set => evaluationVal = value; }
    }
    class PriorityQueue
    {
        private List<Node> nodeList;
        public List<Node> NodeList { get => nodeList; set => nodeList = value; }

        public PriorityQueue()
        {
            nodeList = new List<Node>();
            TileMap.Node buffer = new TileMap.Node(0, 0, 0);
            buffer.SearchNode = new Node(buffer) { GraphNode = buffer };
            nodeList.Add(buffer.SearchNode);
        }
        public int Size()
        {
            return nodeList.Count - 1;
        }
        private void AddNode(TileMap.Node graphNode, int totalCost, int evaluationVal)
        {
            Node node = new Node(graphNode)
            {
                TotalCost = totalCost,
                EvaluationVal = evaluationVal
            };
            graphNode.SearchNode = node;
            nodeList.Add(node);
        }
        private int NodeValue(int index)
        {
            return nodeList[index].EvaluationVal;
        }
        private void SwapNode(int i, int j)
        {
            Node tmp = nodeList[i];
            nodeList[i] = nodeList[j];
            nodeList[j] = tmp;
        }
        public void Push(TileMap.Node graphNode, int totalCost, int evaluationVal)
        {
            AddNode(graphNode, totalCost, evaluationVal);
            int i = Size();
            while (i > 1 && NodeValue(i / 2) > NodeValue(i))
            {
                SwapNode(i, i / 2);
                i /= 2;
            }
        }
        private void MinHeap(int i)
        {
            int smallest = i;
            int left = 2 * i;
            int right = 2 * i + 1;

            if (left <= Size() && NodeValue(left) < NodeValue(smallest))
                smallest = left;

            if (right <= Size() && NodeValue(right) < NodeValue(smallest))
                smallest = right;

            if (smallest != i)
            {
                SwapNode(i, smallest);
                MinHeap(smallest);
            }
        }
        public Node Pop()
        {
            if (Size() == 0)
            {
                Debug.Log("Priority Queue empty!");
                return nodeList[0];
            }
            Node node = nodeList[1];
            SwapNode(1, Size());
            nodeList.RemoveAt(Size());
            MinHeap(1);
            return node;
        }

    }

    private int Heuristic(TileBlock startBlock, TileBlock endBlock)
    {
        return startBlock.Node.CostTo[endBlock.Node.X - endBlock.Tilemap.MapRootX
            , endBlock.Node.Y - endBlock.Tilemap.MapRootY];
    }
    private IEnumerator PathColoring(TileBlock endBlock)
    {
        Node node = endBlock.Node.SearchNode.GraphNodeParent.SearchNode;
        Stack<Node> stack = new Stack<Node>();
        while (node.GraphNodeParent != null)
        {
            stack.Push(node);
            node = node.GraphNodeParent.SearchNode;
        }
        while (stack.Count > 0)
        {
            node = stack.Pop();
            node.GraphNode.TileBlock.SetStatus(6);
            endBlock.Tilemap.Player.GetComponent<CharacterActions>().JumpTo(node.GraphNode.X, node.GraphNode.Y);
            yield return waitFor50ms;
        }
        endBlock.Tilemap.Player.GetComponent<CharacterActions>().JumpTo(endBlock.Node.X, endBlock.Node.Y);
    }
    public IEnumerator FindPath(GameObject gameObj, TileBlock startBlock, TileBlock endBlock)
    {
        PriorityQueue open = new PriorityQueue();
        open.Push(startBlock.Node, 0, Heuristic(startBlock, endBlock));
        while (open.Size() > 0)
        {
            Node node = open.Pop();
            if (node.GraphNode.TileBlock.Status == 4)
            {
                break;
            }
            if (node.GraphNode.TileBlock.Holding != null 
                && !node.GraphNode.TileBlock.Holding.CompareTag(gameObj.tag))
            {
                endBlock = node.GraphNode.TileBlock;
                break;
            }
            foreach (TileMap.Node neighbour in node.GraphNode.Neighbours)
            {
                if (neighbour == node.GraphNodeParent)
                    continue;
                int curStatus = neighbour.TileBlock.Status;
                int tmpCost = node.TotalCost + neighbour.Cost;
                int predict = tmpCost + Heuristic(neighbour.TileBlock, endBlock);
                if (curStatus == 3)
                {
                    continue;
                }
                if (curStatus == 5)
                {
                    if (neighbour.SearchNode.EvaluationVal > predict)
                    {
                        neighbour.SearchNode.GraphNodeParent = node.GraphNode;
                        neighbour.SearchNode.TotalCost = tmpCost;
                        neighbour.SearchNode.EvaluationVal = predict;
                    }
                }
                else if (curStatus == 7)
                {
                    if (neighbour.SearchNode.EvaluationVal > predict)
                    {
                        open.Push(neighbour, tmpCost, predict);
                        neighbour.TileBlock.SetStatus(5);
                    }
                }
                else
                {
                    open.Push(neighbour, tmpCost, predict);
                    neighbour.SearchNode.GraphNodeParent = node.GraphNode;
                    if (curStatus != 4)
                        neighbour.TileBlock.SetStatus(5);
                }
                yield return waitFor20ms;
            }
            if (node.GraphNode.TileBlock.Status == 1 
                || node.GraphNode.TileBlock.Status == 2 
                || node.GraphNode.TileBlock.Status == 5)
            {
                node.GraphNode.TileBlock.SetStatus(7);
            }
            yield return waitFor20ms;
        }
        StartCoroutine(PathColoring(endBlock));
    }
}
