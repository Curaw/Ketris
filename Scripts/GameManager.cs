using Godot;
using System;

public partial class GameManager : Node2D
{
    [Export] private Field playingField;
    [Export] private double shiftSpeed = 0.5f;
    private int pixelsShifted = 0;

    private double shiftTime = 0; //TODO geht in Godot bestimmt besser mit Timern

    public override void _Process(double delta)
    {
        double dt = delta;
        shiftTime += dt;

        if (shiftTime > shiftSpeed)
        {
            shiftTime = 0;
            pixelsShifted += 1;
            if (pixelsShifted == 15)
            {
                pixelsShifted = 0;
                activateLastRow();
                addRandomBlockRow();
            }
            else
            {
                playingField.shiftEverythingUp();
            }
        }
    }

    private void activateLastRow()
    {
        playingField.activateLastRow();
    }

    private void addRandomBlockRow()
    {
        playingField.addRandomBlockRowToBottom();
    }
}
