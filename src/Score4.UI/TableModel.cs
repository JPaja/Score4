using Score4.Core;

namespace Score4.UI;

public class TableModel
{
    private Table _table = new Table();

    public uint Player1 { get; private set; } = 0; 
    public uint Player2 { get; private set; } = 0; 
    
    public (bool Populated, bool Player)[,,] TableMatrix { get; private set; } 

    public void Reset()
    {
        _table = new Table();
        LoadState();
    }

    private void LoadState()
    {
        (Player1, Player1) = _table.CountPoints();
        TableMatrix = _table.GetMatrix();
    }

    public void Move(int x, int y)
    {
        
    }

    public void Predict()
    {
        
    }
}