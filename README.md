# 2D TIlemap-based Astar Pathfinding in Unity [Improved]
This project is adapted from https://github.com/pixelfac/2D-Astar-Pathfinding-in-Unity , since I have found some of the QoL features lacking.
Still, for the core implemetation, big thanks to @pixelfac.

## Contents:
There are 3 classes: Node2D, Grid2D, and Pathfinding2D.
- Node2D: A non-physical object that acts as a marker for where each tile is within the Grid and stores the cost of that tile and whether it is an obstacle or passable.
- Grid2D: Creates a 2D array of nodes within a certain bound, gridWorldSize, and facilitates Pathfinding2D to iterate through the Grid. Also renders a visual description of the Grid in the `onDrawGizmos()` function.
- Pathfinding2D: Where the calculation of the appropriate path occurs. Given 2 transforms, seeker and target, and an instance of Grid2D, will find the shortest possible path between them.

Grid2D and Pathfinding2D have `[ExecuteInEditMode]` attribute, so you can see all the changes in editor. Don't be scared if you get a few nullrefs when you import the scipts. Once you hook everything up, they will be gone.

## Functionality:
- **Feature #1** - Obstacle pathing. Core functionality of Astar pathfinding remains as one would expect - we pass a Vector3 coordinates, and algorithm calculates the shortest path through the nodes, avoiding any obstacles. However, you are now able to click an obstacle itself and Astar will return you a path to the closest walkable node, instead of throwing an error (since the obstacle node isn't technically traversible).

![Alt Text](https://github.com/bonanzaa/gif_dump/blob/main/astar_basic_moving.gif)

- **Feature #2** - OffTile Colliders. Algorithm can account for any non-tilemap colliders in the scene and adjust its pathing accordingly. You can specify a LayerMask for these objects.

![Alt Text](https://github.com/bonanzaa/gif_dump/blob/main/astar_offtile_colliders.gif)

- **Feature #3** - Dynamic Grid. With a toggle option, you can move and resize the grid at runtime, saving all the node states.

![Alt Text](https://github.com/bonanzaa/gif_dump/blob/main/astar_dynamic_grid.gif)

- **Feature #4** - Integer snapping. You can lock the grid to integer values for its position, or keep it freeform with floats. Generally, keeping int snapping means higher node-to-tile precision.

![Alt Text](https://github.com/bonanzaa/gif_dump/blob/main/astar_int_snapping_reworked.gif)

- **Feature #5** - Grid shape. Not You can set it to be a perfect square or a custom-sized rectangle.

![Alt Text](https://github.com/bonanzaa/gif_dump/blob/main/grid_size.png)

## Setup
1. Copy these .cs files into your Unity Assets folder or wherever you store your scripts in your project.
2. Create a new Empty GameObject and attach the Grid2D script to it.
3. Create a new Tilemap with obstacles on it.
4. Attach the Pathfinding2D script to whichever object(s) will be doing the pathfinding.
5. Adjust Grid2D parameters as you see fit.
6. Reference the obstacle Tilemap in the Grid2D Component.
7. Set the seeking Target / Position in the Pathfinding2D Component.
8. Done!

## How to retrieve the path.
Pathfinding2D script has methods to get the path as a List<Vector3>.

```
public List<Vector3> GetPath(Transform targetT)
```
and
```
public List<Vector3> GetPath(Vector3 targetPos)
```
Use / Modify them as you see fit.

## Additional info
- You can enable additional diagonal movement in Grid2D parameters.
- For bigger grids, you might want to consider reworking the Grid2D methods, since with "Dynamic Grid" enabled, it redraws the grid every single frame. You can set the drawing intervals manually using
```
InvokeRepeating("your method",0,t);
```
or do something sompletely different ;)
Current setup isn't too optimized, and is mainly targeted for smaller-scale grids.
