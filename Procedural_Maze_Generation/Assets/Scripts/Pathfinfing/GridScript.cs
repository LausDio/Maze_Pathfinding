using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour
{
    public LayerMask _obstacle;
    public Vector2 grid_Size;
    public float node_radius;
    public Transform player;
    public GameObject effect;
    public List<Node> path;

    public Node[,] grid;
    private float node_diameter;
    private int grid_size_x;
    private int grid_size_y;
    public List<Node> pathToPlayer;
   

    [SerializeField]

    public EnemyPathFind enemy;
    public Canvas canvas;


    public float UpdatePathSpeed = .01f;
    public float UpdateEnemyPositionSpeed = .2f;




    private void Start()
    {
       
        canvas.enabled = false;
        node_diameter = node_radius * 2;
        grid_size_x = Mathf.RoundToInt(grid_Size.x / node_diameter);
        grid_size_y = Mathf.RoundToInt(grid_Size.y / node_diameter);
        CreateGrid();
        InvokeRepeating("UpdatePath", 0f, UpdatePathSpeed);
        InvokeRepeating("GetToDestination", 0f, UpdateEnemyPositionSpeed);

    }

   
    void CreateGrid()
    {
        grid = new Node[grid_size_x, grid_size_y];
        Vector3 grid_bottom_left = transform.position - Vector3.right * grid_Size.x / 2 - Vector3.forward * grid_Size.y / 2;

        for (int x = 0; x < grid_size_x; x++)
        {
            for (int y = 0; y < grid_size_y; y++)
            {
                Vector3 world_point = grid_bottom_left + Vector3.right * (x * node_diameter + node_radius) + Vector3.forward * (y * node_diameter + node_radius);
                bool walkable = !(Physics.CheckSphere(world_point, node_radius, _obstacle));
                grid[x, y] = new Node(walkable, world_point, x,y,0);
            }
        }
    }
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                int check_x = node.gridX + x;
                int check_y = node.gridY + y;

                if(check_x >= 0 && check_x < grid_size_x && check_y >= 0 && check_y < grid_size_y)
                {
                    neighbours.Add(grid[check_x, check_y]); 
                }
            }
        }
        return neighbours;
    }
    public Node NodeFromWoldPoint(Vector3 world_pos)
    {
        float percentageX = (world_pos.x + grid_Size.x / 2) / grid_Size.x;
        float percentageY = (world_pos.z + grid_Size.y / 2) / grid_Size.y;
        percentageX = Mathf.Clamp01(percentageX);
        percentageY = Mathf.Clamp01(percentageY);

        int x = Mathf.RoundToInt((grid_size_x - 1) * percentageX);
        int y = Mathf.RoundToInt((grid_size_y - 1) * percentageY);

        return grid[x, y]; 
        
    }
    private void Update()
    {
 
           // GetToDestination();        

    }
    public int MaxSize
    {
        get
        {
            return grid_size_x * grid_size_y;
        }
    }


    void GetToDestination()
    {
        enemy.GetToDestination();
        /*
        Node p_node; 
        if (grid != null)
        {
            Node player_node = NodeFromWoldPoint(player.position);
            foreach (Node n in grid)
            {


                if (player_node == n)
                {
                    p_node = n;
                }
                if (pathToPlayer != null)
                {
                    if (pathToPlayer.Contains(n))
                    {

                        GameObject.FindGameObjectWithTag("Enemy").transform.position = Vector3.Lerp(GameObject.FindGameObjectWithTag("Enemy").transform.position, new Vector3(n.world_position.x, GameObject.FindGameObjectWithTag("Enemy").transform.position.y, n.world_position.z),  pl_speed * Time.deltaTime);
                    }
                }

            }
        }*/
    }

    private void UpdatePath()
    {
        enemy.UpdatePath();
    }

  
}
