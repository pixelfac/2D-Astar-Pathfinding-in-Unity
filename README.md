# 2D-Astar-Pathfinding-in-Unity
This project is an adaptation of Sebation Lague's A* Pathfinding code from his [youtube series](https://youtu.be/-L-WgKMFuhE) into 2D. Specifically, this project uses Unity [Tilemaps](https://docs.unity3d.com/Manual/class-Tilemap.html) to detect obstacles, rather than using collision boxes as in Lague's version. Since Tilemaps are a very powerful tool in designing settings in 2D games, adding and manipulating the environment that your entities need to path through is very fast and straightforward.

__DISCLAIMER__: My implementation of this code into my own game locks the characters to the Tilemap grid. Theoretically, this code should still function properly in an environment where the characters can move freely on all axes, maybe with minor adjustments.

## How Does It Work?
There are 3 classes: Node2D, Grid2D, and Pathfinding2D.
- Node2D: An non-physical object that acts as a marker for where each tile is within the Grid and stores the cost of that tile and whether it is an obstacle or passable.
- Grid2D: Creates a 2D array of nodes within a certain bound, gridWorldSize, and facilitates Pathfinding2D to iterate through the Grid. Also renders a visual description of the Grid in the `onDrawGizmos()` function.
- Pathfinding2D: Where the calculation of the appropriate path occurs. Given 2 transforms, seeker and target, and an instance of Grid2D, will find the shortest possible path between them.

## Setup

### Example
