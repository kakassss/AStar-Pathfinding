using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static Action<List<GridData>> OnPathPositionCompleted;
    
    private const int STRAIGHTCOST = 10;
    private const int CROSSCOST = 14;

    [SerializeField] private int _height;
    [SerializeField] private int _weight;

    private GridData[,] _gridDatas;

    private void Awake()
    {
        _gridDatas = new GridData[_height, _weight];
        
        CreateGridData();
    }

    private void Start()
    {
        FindPath();
        
        Vector2 vector1 = new Vector2(1, 2);
        Vector2 vector2 = new Vector2(2, 1);

        float distance = Vector2.Distance(vector1, vector2);
        Debug.Log("onur " + distance);

    }

    private void CreateGridData()
    {
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _weight; j++)
            {
                var newNode = new Node();
                _gridDatas[i, j] = new GridData(i, j,newNode,new Vector2(i, j),i+j.ToString());
            }
        }
    }
    
    private List<GridData> _path;
    private void FindPath()
    {
        _path = new List<GridData>();
        
        
        GridData startGridData;
        GridData endgridData;
        
        //Example path
        startGridData = _gridDatas[1, 1];
        endgridData = _gridDatas[3, 2];
        
        Debug.Log("startGridData " + startGridData.Position);
        
        while (startGridData != endgridData)
        {
            FindGCost(startGridData);
            FindHCost(startGridData,endgridData);
            FindFCost(startGridData);
        
            FindNeighbours(startGridData);

            foreach (var gridNodeNeighbourNode in startGridData.GridNode.NeighbourNodes)
            {
                FindGCost(gridNodeNeighbourNode);
                FindHCost(gridNodeNeighbourNode,endgridData);
                FindFCost(gridNodeNeighbourNode);
            }
            
            var nextGridData = SelectMinimumFCost(startGridData.GridNode.NeighbourNodes);
            
            nextGridData.GridNode.connectedGridData = startGridData;
            _path.Add(nextGridData);
            startGridData = nextGridData;
            
            Debug.Log("current grid " + startGridData.Position);
        }
        Debug.Log("end grid" + startGridData.Position);
        
        OnPathPositionCompleted?.Invoke(_path);
    }

    private GridData SelectMinimumFCost(List<GridData> nomineeGridDatas)
    {
        int minimumFCost = 0;
        int minimumHCost = 0;
        
        minimumFCost = nomineeGridDatas[0].GridNode.FCost;
        minimumHCost = nomineeGridDatas[0].GridNode.HCost;

        GridData selectedData = nomineeGridDatas[0];
        
        foreach (var nomineeGridData in nomineeGridDatas)
        {
            if (nomineeGridData.GridNode.FCost < minimumFCost)
            {
                minimumFCost = nomineeGridData.GridNode.FCost;
                minimumHCost = nomineeGridData.GridNode.HCost;

                selectedData = nomineeGridData;
            }

            if (nomineeGridData.GridNode.FCost == minimumFCost)
            {
                if (nomineeGridData.GridNode.HCost < minimumHCost)
                {
                    minimumFCost = nomineeGridData.GridNode.FCost;
                    minimumHCost = nomineeGridData.GridNode.HCost;

                    selectedData = nomineeGridData;
                }
            }
        }

        return selectedData;
    }

    private List<Vector2> _potentialGridNeigbourList;
    private void FindNeighbours(GridData activeGrid)
    {
        _potentialGridNeigbourList = new List<Vector2>();
        
        foreach (var grid in _gridDatas)
        {
            if (Vector2.Distance(grid.Position, activeGrid.Position) <= 1.5f)
            {
                _potentialGridNeigbourList.Add(grid.Position);
            }
        }
        
        
        // var neighbourGrid1 = activeGrid.Position + Vector2.up;    // Up Neighbour
        // var neighbourGrid2 = activeGrid.Position + Vector2.down;  // Down Neighbour
        // var neighbourGrid3 = activeGrid.Position + Vector2.left;  // Left Neighbour
        // var neighbourGrid4 = activeGrid.Position + Vector2.right; // Right Neighbour
        //
        // var neighbourGrid5 = activeGrid.Position + Vector2.up + Vector2.left;    // Up-Left Neighbour  
        // var neighbourGrid6 = activeGrid.Position + Vector2.up + Vector2.right;   // Up-Right Neighbour
        // var neighbourGrid7 = activeGrid.Position + Vector2.down + Vector2.left;  // Down-Left Neighbour
        // var neighbourGrid8 = activeGrid.Position + Vector2.down + Vector2.right; // Down-Right Neighbour
        //
        // _potentialGridNeigbourList.Add(neighbourGrid1);
        // _potentialGridNeigbourList.Add(neighbourGrid2);
        // _potentialGridNeigbourList.Add(neighbourGrid3);
        // _potentialGridNeigbourList.Add(neighbourGrid4);
        // _potentialGridNeigbourList.Add(neighbourGrid5);
        // _potentialGridNeigbourList.Add(neighbourGrid6);
        // _potentialGridNeigbourList.Add(neighbourGrid7);
        // _potentialGridNeigbourList.Add(neighbourGrid8);


        foreach (var potGrid in _potentialGridNeigbourList.Where(IsInGrid))
        {
            activeGrid.GridNode.NeighbourNodes.Add(GetGridDataFromPosition(potGrid));
        }

        foreach (var grid in activeGrid.GridNode.NeighbourNodes)
        {
            //Debug.Log("neigbour gridsss " + grid.Position);
        }
    }

    private Vector2 GetPositionFromGridData(GridData gridData)
    {
        if (_gridDatas.Cast<GridData>().Any(grid => grid.Position == gridData.Position))
        {
            return gridData.Position;
        }

        throw new InvalidOperationException("The position could not be found in the grid data.");
    }
    
    private GridData GetGridDataFromPosition(Vector2 position)
    {
        return (from GridData gridData in _gridDatas where 
            gridData.Position == position select gridData).FirstOrDefault();
    }

    private bool IsInGrid(Vector2 gridPos)
    {
        return _gridDatas.Cast<GridData>().Any(gridData => gridPos == gridData.Position);
    }

    private void FindFCost(GridData activeGrid)
    {
        activeGrid.GridNode.FCost = activeGrid.GridNode.HCost + activeGrid.GridNode.GCost;
    }

    private void FindGCost(GridData startGrid)
    {
        var gridPosDifference = startGrid.Position;
        var absGridsPosX = Mathf.Abs(gridPosDifference.x);
        var absGridsPosY = Mathf.Abs(gridPosDifference.y);
        var absGridPos = new Vector2(absGridsPosX, absGridsPosY);

        if (absGridPos.x != 0 || absGridPos.y != 0)
        {
            float minGridValue = absGridsPosX <= absGridsPosY ? absGridsPosX : absGridsPosY;
            float maxGridValue = absGridsPosX >= absGridsPosY ? absGridsPosX : absGridsPosY;
            int crossGridCounter = 0;

            for (int i = 0; i < minGridValue; i++)
            {
                absGridPos -= Vector2.one;
                crossGridCounter++;
            }

            maxGridValue -= crossGridCounter;
            
            startGrid.GridNode.GCost += crossGridCounter * CROSSCOST;
            startGrid.GridNode.GCost += Mathf.FloorToInt(maxGridValue * STRAIGHTCOST);
        }
        else
        {
            startGrid.GridNode.GCost = 0;
        }
    }
    
    private void FindHCost(GridData activeGrid,GridData targetGrid)
    {
        var gridPosDifference = activeGrid.Position - targetGrid.Position;
        var absGridsPosX = Mathf.Abs(gridPosDifference.x);
        var absGridsPosY = Mathf.Abs(gridPosDifference.y);
        var absGridPos = new Vector2(absGridsPosX, absGridsPosY);
        
        if (absGridPos.x != 0 || absGridPos.y != 0)
        {
            float minGridValue = absGridsPosX <= absGridsPosY ? absGridsPosX : absGridsPosY;
            float maxGridValue = absGridsPosX >= absGridsPosY ? absGridsPosX : absGridsPosY;
            int crossGridCounter = 0;
            
            for (int i = 0; i < minGridValue; i++)
            {
                absGridPos -= Vector2.one;
                crossGridCounter++;
            }
            maxGridValue -= crossGridCounter;

            
            activeGrid.GridNode.HCost += crossGridCounter * CROSSCOST;
            activeGrid.GridNode.HCost += Mathf.FloorToInt(maxGridValue * STRAIGHTCOST);
        }
        
    }
}


// else
// {
//     if (absGridPos.x == 0)
//     {
//         if (GCost)
//         {
//             var distanceX = Mathf.Abs(targetGrid.Position.x);
//             activeGrid.GridNode.GCost += Mathf.FloorToInt(distanceX * STRAIGHTCOST);
//         }
//         if (HCost)
//         {
//             var distanceX = Mathf.Abs(targetGrid.Position.x);
//             activeGrid.GridNode.HCost += Mathf.FloorToInt(distanceX * STRAIGHTCOST);
//         }
//                 
//     }
//         
//     if (absGridPos.y == 0)
//     {
//         if (GCost)
//         {
//             var distanceY = Mathf.Abs(targetGrid.Position.y);
//             activeGrid.GridNode.GCost += Mathf.FloorToInt(distanceY * STRAIGHTCOST);
//                     
//         }
//         if (HCost)
//         {
//             var distanceY = Mathf.Abs(targetGrid.Position.y);
//             activeGrid.GridNode.HCost += Mathf.FloorToInt(distanceY * STRAIGHTCOST);
//         }
//     }
// }