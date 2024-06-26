using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class BlockRow : Node2D
{
    //TODO Godot Timer einbauen für die Swap animation
    private const float ANIMATION_DELTA = 0.02f;
    private const float ANIMATION_OFFSET_X = 0.25f;
    private const int ANIMATION_STEPS = 4;

    private Field containingField;
    private Block[] data;
    private int posY = 0;
    private int width = 0;
    private bool isSwapInProgress;
    private Block leftSwappingBlock, rightSwappingBlock;
    private int animationCounter = 0; //TODO Geht in Godot bestimmt besser
    private float lastUpdate = 0; //TODO Geht in Godot bestimmt besser

    public BlockRow init(Field field, int posY, int width)
    {
        this.containingField = field;
        this.posY = posY;
        this.width = width;
        this.data = new Block[width];
        return this;
    }

    public Block get(int index)
    {
        return data[index];
    }

    public void set(int index, Block newBlock)
    {
        data[index] = newBlock;
    }

    public int getSize()
    {
        return data.Length;
    }

    public void updateBlockPositionValues()
    {
        for (int i = 0; i < data.Length; i++)
        {
            Block block = data[i];
            block.setPosition(i, this.posY);
        }
    }
    public void activate()
    {
        for (int i = 0; i < data.Length; i++)
        {
            Block block = data[i];
            block.removeGreyOut();
            block.enable();
        }
    }

    public void setPosY(int newY)
    {
        this.posY = newY;
        updateBlockPositionValues();
    }

    public int getPosY()
    {
        return this.posY;
    }

    public int getWidth()
    {
        return this.width;
    }
}
