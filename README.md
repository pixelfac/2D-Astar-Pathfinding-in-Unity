# 2D-Astar-Pathfinding-in-Unity
This project is an adaptation of Sebastion Lague's A* Pathfinding code from his [youtube series](https://youtu.be/-L-WgKMFuhE) into 2D. Specifically, this project uses Unity [Tilemaps](https://docs.unity3d.com/Manual/class-Tilemap.html) to detect obstacles, rather than using collision boxes as in Lague's version. Since Tilemaps are a very powerful tool in designing settings in 2D games, adding and manipulating the environment that your entities need to path through is very fast and straightforward.

__DISCLAIMER__: My implementation of this code into my own game locks the characters to the Tilemap grid. Theoretically, this code should still function properly in an environment where the characters can move freely on all axes, maybe with minor adjustments. Additionally, I haven't tested whether this code can support multiple entities pathfinding in real time simultaneously, since each instance of Pathfinding2D calls back to the same Grid2D instance. If this poses an issue in your implementation of this code, try making multiple Grid2D instances for each instance of Pathfinding2D or do what you want with the `path` variable immediately after that object calls the `FindPath()` function, to avoid that object using the path from another objects `FindPath()`.


## How Does It Work?
There are 3 classes: Node2D, Grid2D, and Pathfinding2D.
- Node2D: A non-physical object that acts as a marker for where each tile is within the Grid and stores the cost of that tile and whether it is an obstacle or passable.
- Grid2D: Creates a 2D array of nodes within a certain bound, gridWorldSize, and facilitates Pathfinding2D to iterate through the Grid. Also renders a visual description of the Grid in the `onDrawGizmos()` function.
- Pathfinding2D: Where the calculation of the appropriate path occurs. Given 2 transforms, seeker and target, and an instance of Grid2D, will find the shortest possible path between them.

The `FindPath()` function, using the copy of the grid that it made in `Start()` creates the Node2D List that contains all of the Nodes forming the path from startPos to targetPos, starting at the Node closest to startPos and ending with the Node at targetPos's location. As long as startPos and targetPos do not occupy the same Node, the list will be at least one Node large. This List is passed to the "path" variable in your gridowner object. You can then call 
```
"object_name".GetComponent<Pathfinding2D>()."gridowner_name".GetComponent<Grid2D>().path[0].worldPosition;
```
to get the position vector of the first Node in the List. For my game, I simply set the position to the first Node in the List during that characters turn:
```
seeker.transform.position = seeker.GetComponent<Pathfinding2D>().GridOwner.GetComponent<Grid2D>().path[0].worldPosition;
```


## Setup
1. Copy these .cs files into your Unity Assets folder or wherever you store your scripts in your project.
2. Create a new Empty GameObject (I named mine GridOwner) and attach the Grid2D script to it.
3. Create a new Tilemap and name it something like "obstaclemap".
4. Attach the Pathfinding2D script to whichever object(s) will be doing the pathfinding.
5. In the Grid2D Component, Set Node Radius to half of what your Tilemap scale is. Tilemap Tiles are default 1x1 so Node Radius should be set to 0.5
6. In the Grid2D Component, change the x and y values of Grid World Size to encapsulate the region that is 'pathfindable'. In Scene view, there should be a WireFrame box reflecting the size of Grid World Size. Use that to correctly block out how much space you want.
   - Both the x and y should be a multiple of 2\*Node Radius to avoid misalignment with Tilemap.
7. Drag your "obstaclemap" Tilemap over the Obstaclemap variable in the Grid2D Component to set the reference.
8. In the Grid2D Component, leave Grid Size X and Grid Size Y as 0. They are set in the `Awake()` function.
9. In the Pathfinding2D Component, set the Seeker, Target, and GridOwner variables the same way you set Obstaclemap. Seeker should be the same object that contains the Pathfinding2D Component, Target is the Object that Seeker is pathfinding to, and GridOwner is the object the object that contains Grid2D.
10. Populate the "obstaclemap" Tilemap with tiles that your character should not be able to pass through.
11. Voila! Your character(s) should now path to their target, going around all tiles in obstaclemap.


## Tips
- To help keep your Tilemap and Grid2D grid aligned, make your "GridOwner" object a child of the Grid object that contains your "obstaclemap" Tilemap and reset it's transform.
- After `FindPath()` has been run once, Scene View should show Cube Gizmos of different colors. White/Grey are passable Nodes, Red are impassable, and Black is the path the seeker will take to reach the target, assuming a path exists.
- The default setting is to only allow movement in the 4 cardinal directions. If you would like to enable diagonal motion as well, simply uncomment out that portion of text in Grid2D.cs.
