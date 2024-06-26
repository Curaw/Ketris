using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class Field : Node2D
{
    [Export] private int height;
    [Export] private int width;
    [Export] private PackedScene cursorPrefab;
    [Export] private PackedScene blockRowPrefab;

    [Export] private PackedScene emptyBlockPrefab;
    [Export] private PackedScene redBlockPrefab;
    [Export] private PackedScene blueBlockPrefab;
    [Export] private PackedScene greenBlockPrefab;
    [Export] private PackedScene yellowBlockPrefab;
    [Export] private PackedScene purpleBlockPrefab;

    private Block coyoteBlock;

    private ShiftingArray<BlockRow> blockRows;
    private Cursor cursor;
    private BlockRow lastCreatedRow;
    RandomNumberGenerator rng;

    private List<Block> solveCandidates;
    private List<Block> solvedBlocks;
    private List<Block> fallingBlocks;
    private int fieldComboCounter = 1;

    private const float ONE_PIXEL_UNIT = 0.0625f; //TODO: Fuer Godot nochmal checken
    private const float FALLDOWN_DELTA = 0.02f; //TODO: Godot Timer
    private const float COYOTE_TIME = 0.18f;    //TODO: Coyote Time auch fuer Bloecke denen der Boden weggezogen wird

    public override void _Ready()
    {
        this.cursor = cursorPrefab.Instantiate<Cursor>();
        AddChild(cursor);
        this.blockRows = new ShiftingArray<BlockRow>(height);
        this.solveCandidates = new List<Block>();
        this.solvedBlocks = new List<Block>();
        this.fallingBlocks = new List<Block>();
        this.rng = new RandomNumberGenerator();
        rng.Randomize();
    }

    public int getWidth()
    {
        return this.width;
    }
    
    private BlockRow createNewBlockRow(int yPos)
    {
        BlockRow newRow = blockRowPrefab.Instantiate<BlockRow>().init(this, yPos, width);
        AddChild(newRow);
        return newRow;
    }

    public void activateLastRow()
    {
        if (blockRows.get(0) == null)
        {
            return;
        }
        blockRows.get(0).activate();
        //handleBlockSolvingforRow(0); //TODO Later, wenn Blocksolving implementiert ist
    }

    public void shiftEverythingUp()
    {
        shiftControllerUp();
        shiftBlocksUp();
    }

    private void shiftControllerUp()
    {
        if (isCursorAtTop())
        {
            return;
        }
        cursor.shiftGFXUp();
    }

    private bool isCursorAtTop()
    {
        //TODO Die 2 gefaellt mir hier gar nicht. vielleicht irgendwie einheitlicher machen mit den Blockgrafiken
        if (cursor.getY() >= height - 2)
        {
            GD.Print("Cursor ist ganz oben");
            return true;
        }
        return false;
    }

    private void shiftBlocksUp()
    {
        BlockRow blockRow;
        for (int i = 0; i < blockRows.getSize(); i++)
        {
            if (blockRows.get(i) != null)
            {
                blockRow = blockRows.get(i);
                blockRow.Position = new Vector2(blockRow.Position.X, blockRow.Position.Y - 1);
            }
        }
    }

    public void addRandomBlockRowToBottom()
    {
        removeTopBlockRow();
        BlockRow newBlockRow = createNewBlockRow(-1);   //-1 because increaseBlockPositions sets the new row to 0. TODO Fix ich das noch? 
        fillBlockRowWithRandomBlocks(newBlockRow);
        blockRows.addToBot(newBlockRow);
        increaseBlockPositions();
        shiftControllerUp();
    }

    private void removeTopBlockRow()
    {
        BlockRow rowToFree = blockRows.get(blockRows.getSize() - 1);
        if(rowToFree != null)
        {
            rowToFree.Free();
        }
    }

    //Spaeter nur vom Host benutzt (if IsMultiplayer() == true, Alex das Stueck Muell 2024)
    private void fillBlockRowWithRandomBlocks(BlockRow row)
    {
        for (int i = 0; i < this.width; i++)
        {
            Block newBlock;
            if (i == 0)
            {
                //Der erste Block darf nur nicht die gleiche farbe haben, wie der ueber sich
                BlockColor rowColorAbove = getColorFromLastRowAtIndex(i);
                newBlock = createRandomBlockWithException(i, rowColorAbove);
            }
            else
            {
                //Alle folgenden Bloecke duerfen zusaetzlich nicht die Farbe von ihrem linken Nachbarn haben
                BlockColor lastBlockColor = row.get(i - 1).getBlockColor();
                BlockColor rowColorAbove = getColorFromLastRowAtIndex(i);
                newBlock = createRandomBlockWithTwoExceptions(i, lastBlockColor, rowColorAbove);
                newBlock.setPosition(i, 0);
            }
            row.AddChild(newBlock);
            row.set(i, newBlock);
        }
        this.lastCreatedRow = row;
    }

    private BlockColor getColorFromLastRowAtIndex(int index)
    {
        if (lastCreatedRow != null)
        {
            return lastCreatedRow.get(index).getBlockColor();
        }
        return BlockColor.Empty;
    }

    //Spaeter nur vom Host benutzt
    private Block createRandomBlockWithException(int posX, BlockColor exceptionColor)
    {
        //Die Funktion stellt in O(1) sicher, dass nicht die Farbe exceptionColor generiert wird
        BlockColor randomType = (BlockColor)rng.RandiRange(2, 5); //TODO 5 ersetzen durch die Anzahl der Farben - 1
        if (randomType >= exceptionColor)
        {
            randomType = randomType + 1;
        }
        return createBlock(randomType, posX);
    }

    //Der hier dann vom Client?
    // TODO Das hier vielleicht in den Blockrow schieben?
    private Block createBlock(BlockColor color, int posX)
    {
        switch (color)
        {
            case BlockColor.Red:
                return redBlockPrefab.Instantiate<Block>().init(BlockColor.Red, posX); //TODO Position noch setzen? Godot
            case BlockColor.Blue:
                return blueBlockPrefab.Instantiate<Block>().init(BlockColor.Blue, posX);
            case BlockColor.Green:
                return greenBlockPrefab.Instantiate<Block>().init(BlockColor.Green, posX);
            case BlockColor.Yellow:
                return yellowBlockPrefab.Instantiate<Block>().init(BlockColor.Yellow, posX);
            case BlockColor.Purple:
                return purpleBlockPrefab.Instantiate<Block>().init(BlockColor.Purple, posX);
            default:
                return emptyBlockPrefab.Instantiate<Block>().init(BlockColor.Empty, posX);
        }
    }

    //Spaeter nur vom Host benutzt
    //Die Funktion stellt in O(1) sicher, dass nicht die Farbe exceptionColor1 und exceptionColor2 generiert wird
    private Block createRandomBlockWithTwoExceptions(int posX, BlockColor exceptionColor1, BlockColor exceptionColor2)
    {
        //Sind die Farben gleich, dann gehen wir den einfachen Weg
        if (exceptionColor1 == exceptionColor2)
        {
            return createRandomBlockWithException(posX, exceptionColor1);
        }

        //Erst Farben sortieren
        if (exceptionColor1 > exceptionColor2)
        {
            BlockColor temp = exceptionColor1;
            exceptionColor1 = exceptionColor2;
            exceptionColor2 = temp;
        }

        //Zufällige Farbe ziehen
        BlockColor randomType = (BlockColor)rng.RandiRange(2, 4); //TODO 4 ersetzen durch die Anzahl der Farben - 2

        //Farbe anpassen wenn eine der beiden Exceptions getroffen wurde
        if (randomType >= exceptionColor1)
        {
            randomType = randomType + 1;
        }
        if (randomType >= exceptionColor2)
        {
            randomType = randomType + 1;
        }
        return createBlock(randomType, posX);
    }

    private void increaseBlockPositions()
    {
        BlockRow blockRow;
        for (int i = 0; i < blockRows.getSize(); i++)
        {
            if (blockRows.get(i) != null)
            {
                blockRow = blockRows.get(i);
                blockRow.Position = new Vector2(0, -blockRow.getPosY() * Block.BLOCK_HEIGHT);
                blockRow.setPosY(blockRow.getPosY() + 1);
            }
        }
    }

    //Spaeter nur vom Host benutzt, unused
    private Block createRandomBlock(int posX)
    {
        BlockColor randomType = (BlockColor) rng.RandiRange(2, 6); //TODO 6 ersetzen durch die Anzahl der Farben
        return createBlock(randomType, posX);
    }

    public override void _Process(double delta)
    {
        //updateCoyoteBlock(); //TODO later
        //updateFallingBlocks();

        //TODO ranges festlegen. koennte das field hier selbst entscheiden und die pos nur aendern, wenn es noch im feld ist
        /*if (Input.GetKeyDown("w"))
        {
            moveCursorUp();
        }
        if (Input.GetKeyDown("a"))
        {
            moveCursorLeft();
        }
        if (Input.GetKeyDown("d"))
        {
            moveCursorRight();
        }
        if (Input.GetKeyDown("s"))
        {
            moveCursorDown();
        }
        if (Input.GetKeyDown("k"))
        {
            swapBlocksAtCursorPosition();
        }*/

        //50 FPS
        //1 Frame = 0,02 Sekunden
        //4 Frames zum Swappen
        //13 Frames bis zum Runterfallen nach dem Swap (9 Coyote Frames)
        //Beim Fallen: 1 Frame --> 1 Reihe tiefer
    }
}

