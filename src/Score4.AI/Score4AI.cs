using Score4.Core;

namespace Score4.AI;

public static class Score4AI
{
    public static int Depth = 7;
    public static Table Predict(Table table, bool player)
    {
        var (_, predictedTable) = Predict(table, Depth, player);
        return predictedTable;
    } 
    private static (int Weight, Table Table) Predict(Table table, int depth, bool player, int alpha = int.MinValue, int beta = int.MaxValue)
    {
        if (depth == 0 || table.IsFull)
        {
            var (player1, player2) = table.CountPoints();
            return ((int)player1 - (int)player2,table);
        }
        
        if (!player)
        {
            var max = int.MinValue;
            Table? maxTable = null;
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if(!table.TryPlay(x, y, player, out var newTable))
                        continue;
                    
                    var (weight, _) = Predict(newTable, depth -1, !player, alpha, beta);
                    if (weight >= max)
                    {
                        maxTable = newTable;
                        max = weight;
                    }
                    alpha = Math.Max(alpha, weight);
                    if (beta <= alpha)
                        return (max, maxTable!.Value);
                }
            }
            return (max, maxTable!.Value);
        }
        else
        {
            var min = int.MaxValue;
            Table? minTable = null;
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if(!table.TryPlay(x, y, player, out var newTable))
                        continue;
                    
                    var (weight, _) = Predict(newTable, depth -1, !player, alpha, beta);
                    if (weight <= min)
                    {
                        minTable = newTable;
                        min = weight;
                    }
                    alpha = Math.Min(alpha, weight);
                    if (beta <= alpha)
                        return (min, minTable!.Value);
                }
            }
            return (min, minTable!.Value);
        }
    }
    
    /*private static void Minimax(Table table, int generacija, bool maximize, sbyte alpha = sbyte.MinValue, sbyte beta = sbyte.MaxValue)
    {
        if (generacija == 0 || table.Tabla.Podaci == unchecked((ulong)-1))
        {
            var (poeni1, poeni2) = table.Tabla.IzracunajPoene();
            table.Vrednost = (sbyte)(poeni1 - poeni2);
            return;
        }

        if (maximize)
        {
            var max = sbyte.MinValue;
            foreach (var potez in table.Potezi)
            {
                if (potez == null)
                    continue;
                Minimax(potez, generacija - 1, false, alpha, beta);
                var vrednost = potez.Vrednost.Value;
                max = Math.Max(max, vrednost);
                alpha = Math.Max(alpha, vrednost);
                if (beta <= alpha)
                    break;
            }
            table.Vrednost = max;
        }
        else
        {
            var min = sbyte.MaxValue;
            foreach (var potez in table.Potezi)
            {
                if (potez == null)
                    continue;
                Minimax(potez, generacija - 1, true, alpha, beta);
                var vrednost = potez.Vrednost.Value;
                min = Math.Min(min, vrednost);
                beta = Math.Min(beta, vrednost);
                if (beta <= alpha)
                    break;
            }
            table.Vrednost = min;
        }

    }*/

}