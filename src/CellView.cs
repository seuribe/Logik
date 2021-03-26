using Godot;
using Logik.Core;
using System;

public class CellView : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private Label valueLabel;
    private TextEdit nameText;
    private TextEdit formulaText;
    private Cell cell;

    private static CellIndex cellIndex = new CellIndex();

    public string formula;
    public string value {
        get { return formula; }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        valueLabel = (Label)GetNode("Panel/ValueLabel");
        nameText = (TextEdit)GetNode("Panel/NameText");
        formulaText = (TextEdit)GetNode("Panel/FormulaText");
        cell = cellIndex.CreateCell();
        cell.ValueChanged += (cell) => {
            valueLabel.Text = cell.Value;
        };
        nameText.Text = cell.Id;
    }
    
    public void onFormulaChanged() {
        formula = formulaText.Text;
        cell.Formula = formula;
        valueLabel.Text = cell.Value;
    }
    public string evalFormula(string formula) {
        return formula;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
