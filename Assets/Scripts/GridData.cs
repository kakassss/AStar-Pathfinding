using UnityEngine;

public struct GridData
{
    public string Name;
    
    public Vector2 Position;
    public int X;
    public int Y;

    public Node GridNode;

    public GridData(int x, int y,Node node,Vector2 position, string name)
    {
        Position = position;
        X = x;
        Y = y;
        GridNode = node;
        Name = name;
    }
    
    public static bool operator ==(GridData op1,  GridData op2) 
    {
        return op1.Equals(op2);
    }

    public static bool operator !=(GridData op1,  GridData op2) 
    {
        return !op1.Equals(op2);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
}