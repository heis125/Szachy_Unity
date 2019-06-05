using UnityEngine;
using UnityEditor;

public class PossibleMove
{
    public int fromX;
    public int fromY;
    public int toX;
    public int toY;

    public PossibleMove(int fromX, int fromY, int toX, int toY)
    {
        this.fromX = fromX;
        this.fromY = fromY;
        this.toX = toX;
        this.toY = toY;
    }
}