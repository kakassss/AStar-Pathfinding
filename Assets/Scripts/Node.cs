using System.Collections.Generic;
public class Node
{
    public int GCost;
    public int FCost;
    public int HCost;
    
    public List<GridData> NeighbourNodes = new List<GridData>();
}
 