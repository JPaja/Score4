namespace Score4.Core;

internal static class TableUtils
{
     static TableUtils()
    {
        WinningBoardMasks = GenerateVerticalWinningMasks()
            .Concat(GenerateHorizontalWinningMasks())
            .Concat(GenerateVerticalDiagonalWinningMasks())
            .Concat(GenerateHorizontalDiagonalWinningMasks())
            .Concat(Generate3DDiagonalWinningMasks())
            .ToArray();
    }

    public static ulong[] WinningBoardMasks;


    private static IEnumerable<ulong> GenerateVerticalWinningMasks()
    {
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                var table = new Table();
                for (int z = 0; z < 4; z++)
                {
                    table = table.SetBoard(x, y, z);
                }

                yield return table.Board;
            }
        }
    }


    private static IEnumerable<ulong> GenerateHorizontalWinningMasks()
    {
        for (int z = 0; z < 4; z++)
        {
            for (int i = 0; i < 4; i++)
            {
                var table1 = new Table();
                var table2 = new Table();
                for (int j = 0; j < 4; j++)
                {
                    table1 = table1.SetBoard(i, j, z);
                    table2 = table2.SetBoard(j, i, z);
                }

                yield return table1.Board;
                yield return table2.Board;
            }
        }
    }

    private static IEnumerable<ulong> GenerateVerticalDiagonalWinningMasks()
    {

        for (int i = 0; i < 4; i++)
        {
            var table1 = new Table();
            var table2 = new Table();
            var table3 = new Table();
            var table4 = new Table();
            for (int j = 0; j < 4; j++)
            {
                table1 = table1.SetBoard(i, j, j);
                table3 = table3.SetBoard(i, j, 3- j);
                table2 = table2.SetBoard(j, i, j);
                table4 = table4.SetBoard(j, i, 3- j);
            }
            yield return table1.Board;
            yield return table2.Board;
            yield return table3.Board;
            yield return table4.Board;

        }
    }

    private static IEnumerable<ulong> GenerateHorizontalDiagonalWinningMasks()
    {
        for (int z = 0; z < 4; z++)
        {
            var table1 = new Table();
            var table2 = new Table();
            for (int i = 0; i < 4; i++)
            {
                table1 = table1.SetBoard(i, i, z);
                table2 = table2.SetBoard(3-i, i, z);
            }

            yield return table1.Board;
            yield return table2.Board;
        }
    }

    private static IEnumerable<ulong> Generate3DDiagonalWinningMasks()
    {
        var table1 = new Table();
        var table2 = new Table();
        var table3 = new Table();
        var table4 = new Table();
        for (int i = 0; i < 4; i++)
        {
            table1 = table1.SetBoard(i, i, i);
            table2 = table2.SetBoard(i, i, 3 - i);
            table3 = table3.SetBoard(3 - i, i, i);
            table4 = table4.SetBoard(3 - i, i, 3 - i);
        }

        yield return table1.Board;
        yield return table2.Board;
        yield return table3.Board;
        yield return table4.Board;
    }
}