using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Astar : MonoBehaviour {


//_______________path finding per l'assassino___________________________________
	public void FindPath(int startx, int starty, int targetx, int targety, Gamemanager board)
	{
		Cells startCell = board.mAllCells[startx, starty];
		Cells targetCell = board.mAllCells[targetx,targety];

		List<Cells> openSet = new List<Cells>();
		HashSet<Cells> closedSet = new HashSet<Cells>();
		openSet.Add(startCell);

		while (openSet.Count > 0)
		{
    			Cells cell = openSet[0];
    			for (int i = 1; i < openSet.Count; i ++)
					{
    				if (openSet[i].fCost < cell.fCost || openSet[i].fCost == cell.fCost)
						{
    					if (openSet[i].hCost < cell.hCost)
    						cell = openSet[i];
    				}
    			}

    			openSet.Remove(cell);
    			closedSet.Add(cell);

    			if (cell == targetCell)
					{
    				RetracePath(startCell,targetCell, board.killer);
    				return;
    			}

    			foreach (Cells neighbour in board.GetNeighbours(cell))
					{
    				if (neighbour.tipocella != "" || closedSet.Contains(neighbour))
						{
    					continue;
    				}

    				int newCostToNeighbour = cell.gCost + GetDistance(cell, neighbour);
    				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
						{
    					neighbour.gCost = newCostToNeighbour;
    					neighbour.hCost = GetDistance(neighbour, targetCell);
    					neighbour.parent = cell;

    					if (!openSet.Contains(neighbour))
    						openSet.Add(neighbour);
    				}
    			}
		}
	}

	private void RetracePath(Cells startCell, Cells endCell, Killer killer)
	{
		List<Cells> path = new List<Cells>();
		Cells currentCell = endCell;

		while (currentCell != startCell)
		{
			//currentCell.Stampa();
			path.Add(currentCell);
			currentCell = currentCell.parent;
		}
		path.Reverse();

		killer.path = path;
	}

//_______________path finding per la solvibilità________________________________

	public void FindPathInterno(int startx, int starty, int targetx, int targety, Room room)
	{

		Cells startCell = room.roomCells[startx, starty];
		Cells targetCell = room.roomCells[targetx,targety];

		List<Cells> openSet = new List<Cells>();
		HashSet<Cells> closedSet = new HashSet<Cells>();
		openSet.Add(startCell);

		while (openSet.Count > 0)
		{
					//Debug.Log(openSet.Count);
    			Cells cell = openSet[0];
    			for (int i = 1; i < openSet.Count; i ++)
					{
    				if (openSet[i].fCost < cell.fCost || openSet[i].fCost == cell.fCost)
						{
    					if (openSet[i].hCost < cell.hCost)
    						cell = openSet[i];
    				}
    			}

    			openSet.Remove(cell);
    			closedSet.Add(cell);

    			if (cell == targetCell)
					{
    				RetracePath(startCell,targetCell, room);
    				return;
    			}

    			foreach (Cells neighbour in room.GetNeighboursRoom(cell))
					{
    				if (neighbour.tipocella != "" || closedSet.Contains(neighbour))
						{
    					continue;
    				}

    				int newCostToNeighbour = cell.gCost + GetDistance(cell, neighbour);
    				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
						{
    					neighbour.gCost = newCostToNeighbour;
    					neighbour.hCost = GetDistance(neighbour, targetCell);
    					neighbour.parent = cell;

    					if (!openSet.Contains(neighbour))
    						openSet.Add(neighbour);
    				}
    			}
		}
	}

	private void RetracePath(Cells startCell, Cells endCell, Room room)
	{
		List<Cells> path = new List<Cells>();
		Cells currentCell = endCell;

		while (currentCell != startCell)
		{
			//currentCell.Stampa();
			path.Add(currentCell);
			currentCell = currentCell.parent;
		}
		path.Add(startCell);
		path.Reverse();

		room.path.Clear();
		room.path = path;
	}

//______________________________________________________________________________

	private int GetDistance(Cells cellA, Cells cellB)
	{
		int dstX = Mathf.Abs(cellA.mRoomPosition.x - cellB.mRoomPosition.x);
		int dstY = Mathf.Abs(cellA.mRoomPosition.y - cellB.mRoomPosition.y);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}
}
