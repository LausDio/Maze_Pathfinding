using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algorithm : MonoBehaviour
{
    GridScript grid;
    public Transform debut;
    public Transform fin;
    private void Awake()
    {
        grid = GetComponent<GridScript>();
    }
    private void Update()
    {
        FindPath(debut.position, fin.position);
    }
    void FindPath(Vector3 start_position, Vector3 target_position)
    {
        Node start_node = grid.NodeFromWoldPoint(start_position);
        Node target_node = grid.NodeFromWoldPoint(target_position);

        List<Node> open_list = new List<Node>();
        HashSet<Node> closed_list = new HashSet<Node>();

        open_list.Add(start_node);

        while (open_list.Count > 0)
        {
            Node current_node = open_list[0];

            for (int i = 1; i < open_list.Count; i++)
            {
                if(open_list[i].fCost < current_node.fCost || open_list[i].fCost == current_node.fCost && open_list[i].hCost < current_node.hCost)
                {
                    current_node = open_list[i];
                }
            }
            
            open_list.Remove(current_node);
            closed_list.Add(current_node);

            if (current_node == target_node)
            {
                ShowPath(start_node, target_node);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(current_node))
            {
                if(!neighbour.walkable || closed_list.Contains(neighbour))
                {
                    
                    continue;
                }

                int distance_to_neighbor = current_node.gCost + GetDistance(current_node, neighbour);
                if(distance_to_neighbor < neighbour.gCost || !open_list.Contains(neighbour))
                {
                    neighbour.gCost = distance_to_neighbor;
                    neighbour.hCost = GetDistance(neighbour, target_node);
                    neighbour.parent = current_node;

                    if (!open_list.Contains(neighbour))
                        open_list.Add(neighbour);
                }
            }

        }

        int GetDistance(Node node1, Node node2)
        {
            int distance_x = Mathf.Abs(node1.gridX - node2.gridX);
            int distance_y = Mathf.Abs(node1.gridY - node2.gridY);

            if(distance_x > distance_y)
            {
                return 14 * distance_y + 10 * (distance_x - distance_y);
            }
            return 14 * distance_x + 10 * (distance_y - distance_x);
        }
    }

    void ShowPath(Node start_node , Node destination_node)
    {
        List<Node> path = new List<Node>();
        Node current_node = destination_node;

        while(current_node != start_node)
        {
            path.Add(current_node);
            current_node = current_node.parent;
        }
        path.Reverse();

        grid.path = path;
    }
   
}
