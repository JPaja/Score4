namespace Score4.Core;

public static class TableUtils
{
    static TableUtils()
    {
        WinningPositions = GenerateVerticalWinningPositions()
            .Concat(GenerateHorizontalXWinningPositions())
            .Concat(GenerateHorizontalYWinningPositions())
            .Concat(GenerateDiagonalWinningPositions())
            .ToArray();
        var test = WinningPositions.Distinct().ToArray();
        int a = 0;
    }
    
    public static ulong[] WinningPositions;


    private static IEnumerable<ulong> GenerateVerticalWinningPositions()
    {
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                var table = new Table();
                for (int z = 0; z < 4; z++)
                {
                    table.SetBoard(x, y, z);
                }

                yield return table.Board;
            }
        }
    }


    private static IEnumerable<ulong> GenerateHorizontalYWinningPositions()
    {
        for (int z = 0; z < 4; z++)
        {
            for (int y = 0; y < 4; y++)
            {
                var table = new Table();
                for (int x = 0; x < 4; x++)
                {
                    table.SetBoard(y, x, z);
                }
                yield return table.Board;
            }
        }
    }
    
    private static IEnumerable<ulong> GenerateHorizontalXWinningPositions()
    {
        for (int z = 0; z < 4; z++)
        {
            for (int x = 0; x < 4; x++)
            {
                var table = new Table();
                for (int y = 0; y < 4; y++)
                {
                    table.SetBoard(y, x, z);
                }
                yield return table.Board;
            }
        }
    }

    private static IEnumerable<ulong> GenerateDiagonalWinningPositions()
    {
        var table1 = new Table();
        var table2 = new Table();
        var table3 = new Table();
        var table4 = new Table();
        for (int i = 0; i < 4; i++)
        {
            table1.SetBoard(i, i, i);
            table2.SetBoard(i, i, 3 - i);
            table3.SetBoard(3 - i, i, i);
            table4.SetBoard(3 - i, i, 3 - i);
        }
        yield return table1.Board;
        yield return table2.Board;
        yield return table3.Board;
        yield return table4.Board;
    }

}