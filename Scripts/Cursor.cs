using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class Cursor : Node2D
{
    private int posX = 0;
    private int posY = 1;

    public void shiftGFXUp()
    {
        //TODO die 0.5 muessen variablen sein. gefaellt mir nicht genau wie die 2 im Field
        this.Position = new Vector2(posX + Block.BLOCK_WIDTH/2, this.Position.Y - 1);
    }

    public void setX(int newX)
    {
        if (newX < 0)
        {
            return;
        }
        this.posX = newX;
    }

    public void setY(int newY)
    {
        if (newY < 0)
        {
            return;
        }
        this.posY = newY;
    }

    public float getX()
    {
        return posX;
    }

    public float getY()
    {
        return posY;
    }
}
