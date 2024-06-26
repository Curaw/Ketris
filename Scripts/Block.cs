using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class Block : Node2D
{
    public const int BLOCK_WIDTH = 16;
    public const int BLOCK_HEIGHT = 16;

    [Export] private BlockColor color;

    private float fallDownTimer;

    private bool disabled;
    private bool swapping;
    private bool levitating;
    private bool falling;

    private int length;
    private int height;
    private int posX;
    private int posY;

    public Block init(BlockColor color, int posX)
    {
        this.color = color;
        length = 1;
        height = 1;
        this.Position = new Vector2(posX * BLOCK_WIDTH, 0);
        return this;
    }

    public bool isDisabled()
    {
        return disabled;
    }
    public bool isFalling()
    {
        return falling;
    }

    public bool isSwapping()
    {
        return swapping;
    }

    public bool isLevitating()
    {
        return levitating;
    }

    public void disable()
    {
        this.disabled = true;
    }

    public void setFalling(bool newVal)
    {
        this.falling = newVal;
    }

    public void setSwapping(bool newVal)
    {
        this.swapping = newVal;
    }

    public void setLevitating(bool newVal)
    {
        this.levitating = newVal;
    }

    public void enable()
    {
        this.disabled = false;
    }

    public int getX()
    {
        return this.posX;
    }

    public int getY()
    {
        return this.posY;
    }
    public float getFallDownTimer()
    {
        return this.fallDownTimer;
    }

    public void setPosition(int x, int y)
    {
        this.posX = x;
        this.posY = y;
        this.Position = new Vector2(x * BLOCK_WIDTH, y);
    }

    public void setX(int newX)
    {
        this.posX = newX;
    }

    public void setY(int newY)
    {
        this.posY = newY;
    }

    public void setFallDownTimer(float newVal)
    {
        this.fallDownTimer = newVal;
    }

    public BlockColor getBlockColor()
    {
        return this.color;
    }

    public void setBlockColor(BlockColor newColor)
    {
        this.color = newColor;
        //TODO hier die Grafik updaten im Godot Stil
        //this.renderer.material.color = new Color(1, 1, 1, 0f);
    }

    public void greyOut()   //TODO: Spaeter wieder auf private
    {
        // TODO: Auf Godot Stil (Modulate angeblich laut Marius dem Lappen)
        //this.renderer.material.color = new Color(1, 1, 1, 0.3f);
    }

    public void removeGreyOut()
    {
        // TODO: Auf Godot Stil (Modulate angeblich laut Marius dem Lappen)
        //this.renderer.material.color = new Color(1, 1, 1, 1);
    }

    // Start is called before the first frame update
    void Start()
    {
        //this.renderer = GetComponent<Renderer>();
        this.greyOut();
        this.disable();
    }
}
