using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;


public class pathfinding : MonoBehaviour
{

    
    private grid Grid;
    public List<Node1> discovered;
    public Queue<Node1> sequence;
    private Dictionary<Node1, Node1> prev;
    private PriorityQueue priorityQueue;

  


    //Octile distance/Euclidean Distance
    //double hVDistance = 1.0;
    //double dDistance = 1.4;

    /* for Manhattan Distances,
    double horizontalVerticalDistance = 1.0;
    double diagonalDistance = 2.0;

    for Chebyshev Distances,
    double horizontalVerticalDistance = 1.0;
    double diagonalDistance = 1.0; */




    public float vectorDistance(Vector3 v1, Vector3 v2)
    {
        
        float x_2 = v2.x;
        float x_1 = v1.x;
        float y_2 = v2.z;
        float y_1 = v1.z;
       
       
        return Mathf.Sqrt(Mathf.Pow((x_2 - x_1), 2) + Mathf.Pow((y_2 - y_1), 2)); ;
    }
    private Queue<Node1> reverseQueue(Queue<Node1> getReversed) 
    {
        Stack<Node1> convertQueue = new Stack<Node1>();
        while (getReversed.Count != 0) 
        {
            convertQueue.Push(getReversed.Dequeue());
        }
        while (convertQueue.Count != 0) 
        {
            getReversed.Enqueue(convertQueue.Pop());
        }
        return getReversed;
    }
    private Node1 reconctruct_path(Dictionary<Node1, Node1> cameFrom, Node1 current, Node1 start)
    {

        sequence = new Queue<Node1>();

        while (!sequence.Contains(start))
        {

            sequence.Enqueue(current);
            current = cameFrom[current];
        }
        sequence = reverseQueue(sequence);
        
        return current;
    }
    private float Heuristics(Node1 current, Node1 finish, Node1 Neighbour)
    {
        float dx = Mathf.Abs(Neighbour.pos.x - finish.pos.x);
        float dy = Mathf.Abs(Neighbour.pos.z - finish.pos.z);
        float D = vectorDistance(current.pos, Neighbour.pos);

        float OctileDistance = D * Mathf.Min(dx, dy) + Mathf.Abs(dx - dy);

        return OctileDistance;
    }



    #region pathfinding algorithms

    protected Node1 Greedy(Vector3 StartingPos, Vector3 FinishPos)
    {
        priorityQueue = new PriorityQueue();
        prev = new Dictionary<Node1, Node1>();
        discovered = new List<Node1>();
        Grid = GetComponent<grid>();
        Node1 start = Grid.getNodeposition(StartingPos);
        Node1 finish = Grid.getNodeposition(FinishPos);
        discovered.Add(start);
        prev.Add(start, null);
        if (!start.unwalkable)
            priorityQueue.add(new Node1(start.pos, false));




        while (!priorityQueue.isEmpty())
        {
            Node1 current = priorityQueue.poll();
            discovered.Add(current);

            if (current.Equals(finish))
                return reconctruct_path(prev, current, start);


            List<Node1> neighbours = Grid.Neighbours(current);

            foreach (var next in neighbours)
            {

                if (next.unwalkable || discovered.Contains(next)) continue;

                next.H = Heuristics(current, finish, next);

                priorityQueue.add(next);
                if (!prev.ContainsKey(next))
                {
                    prev.Add(next, current);
                }
            }






        }

        return finish;
    }




    protected Node1 Dijkstra(Vector3 StartingPos, Vector3 FinishPos)
    {

        priorityQueue = new PriorityQueue();
        prev = new Dictionary<Node1, Node1>();
        discovered = new List<Node1>();
        Grid = GetComponent<grid>();
        Node1 start = Grid.getNodeposition(StartingPos);
        Node1 finish = Grid.getNodeposition(FinishPos);
        discovered.Add(start);
        prev.Add(start, null);
        Dictionary<Node1, float> dist = new Dictionary<Node1, float>();
        if (!start.unwalkable)
            priorityQueue.add(new Node1(start.pos, false));


        foreach (var Graph in Grid.Grid)
        {

            dist.Add(Graph, float.PositiveInfinity);
        }
  
        
        while (!priorityQueue.isEmpty())
        {
            Node1 current = priorityQueue.poll();
            discovered.Add(current);


            if (current.Equals(finish))
                return reconctruct_path(prev, current, start);
         

            List<Node1> neighbour = Grid.Neighbours(current);

          


            foreach (var next in neighbour)
            {

                if (next.unwalkable || discovered.Contains(next)) continue;

                float newGcost = current.G + 1f;
                
               
                if (newGcost < dist[next] )
                {

                    dist[next] = newGcost;
                    next.G = dist[next];
                    priorityQueue.add(next);

                    if (!prev.ContainsKey(next))
                        prev.Add(next, current);


                }

            }
          



        }
        return finish;
    }


    protected Node1 A_Star(Vector3 StartingPos, Vector3 FinishPos)
    {

        priorityQueue = new PriorityQueue();
        prev = new Dictionary<Node1, Node1>();
        discovered = new List<Node1>();
        Grid = GetComponent<grid>();
        Node1 start = Grid.getNodeposition(StartingPos);
        Node1 finish = Grid.getNodeposition(FinishPos);
        discovered.Add(start);
        prev.Add(start, null);
        Dictionary<Node1, float> gScore = new Dictionary<Node1, float>();  
       if(!start.unwalkable)
            priorityQueue.add(new Node1(start.pos, false));

  
        foreach (var gridPos in Grid.Grid)
        {
            gScore.Add(gridPos, float.PositiveInfinity);
        }


        while (!priorityQueue.isEmpty())
        {
            Node1 current = priorityQueue.poll();
            discovered.Add(current);


            List<Node1> neighbours = Grid.Neighbours(current);

            if (current.Equals(finish))
                return reconctruct_path(prev, current, start);
       
            

            foreach (var N in neighbours)
            {
                Vector3 u = current.pos;
                Vector3 n = N.pos;

                if (N.unwalkable || discovered.Contains(N)) continue;


                float newGcost = current.G + vectorDistance(u, n);



                if (newGcost < gScore[N])
                {


                    //g-> is the cost so far that was calculated from starting node to current node
                    //h-> is heuristics which is responsible for estimated distance between current and finish node
                    gScore[N] = newGcost;
                    N.G = gScore[N];
                    N.H = Heuristics(current, finish, N);
                    //f(n)=g(n)+h(n)
                    N.F = N.G + N.H;
                    priorityQueue.add(N);

                    if (!prev.ContainsKey(N))
                        prev.Add(N, current);



                }
            }


        }
      

    

        return finish;
    }

    
 

    protected Node1 BreadthFirstSearch(Vector3 StartingPos, Vector3 FinishPos)
    {
        Grid = GetComponent<grid>();
        Node1 start = Grid.getNodeposition(StartingPos);
        Node1 finish = Grid.getNodeposition(FinishPos);
        Queue<Node1> queueOfNodes = new Queue<Node1>();
        prev = new Dictionary<Node1, Node1>();
        discovered = new List<Node1>();
        queueOfNodes.Enqueue(start);
        discovered.Add(start);
        prev.Add(start, null);


       


        while (queueOfNodes.Count != 0)
        {

            Node1 current = queueOfNodes.Dequeue();
            discovered.Add(current);


            if (current.Equals(finish))
                return reconctruct_path(prev, current, start);

            List<Node1> w = Grid.Neighbours(current);




            foreach (var next in w)
            {
                if (next.unwalkable || discovered.Contains(next)) continue;
                if (!prev.ContainsKey(next))
                {
                    queueOfNodes.Enqueue(next);
                    prev.Add(next, current);
                    

                }

            }

        }
        
        return finish;
    }


    #endregion


}


    



