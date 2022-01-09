using System.Runtime.CompilerServices;

namespace Score4.Core;

public readonly record struct Table(ulong Board = 0UL, ulong Player = 0UL)
{
    public bool IsFull => Board == ulong.MaxValue;

    public Table Play(int x, int y, bool player)
    {
        if (x >= 4 || y >= 4)
            return this;
        var position = GetPosition(x, y);
        var boardData = GetPositionBoardData(position);
        var playerData = GetPositionPlayerData(position);
        var dataCount = GetDataCount(boardData);
        var playerCount = GetDataCount(playerData);
        if (dataCount is -1 or >= 4 || dataCount != playerCount)
            return this;
        
        boardData = SetDataBit(boardData, dataCount + 1, true);
        playerData = SetDataBit(playerData, dataCount + 1, player);
        var newBoard = SetPropertyData(Board, position, boardData);
        var newPlayer = SetPropertyData(Player, position, playerData);
        
        return this with
        {
            Board = newBoard, 
            Player = newPlayer
        };
    }

    internal ulong SetBoard(int x, int y, int z)
    {
        if (x >= 4 || y >= 4 || z >= 4)
            return Board;
        var position = GetPosition(x, y);
        var boardData = GetPositionBoardData(position);
        boardData = SetDataBit(boardData, z, true);
        return SetPropertyData(Board, position, boardData);
    }

    public bool CanPlay(int x, int y) 
        => Play(x,y, false) != this;


    public (uint Player1, uint Player2) CountPoints()
    {
        return (0, 0);
    }
    

    private static int GetPosition(int x, int y)
        => ((y & 0b11) << 2) & (x & 0b11);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte GetPositionBoardData(int position) 
        => (byte) ((Board >> (position << 2)) & 0b1111);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte GetPositionPlayerData(int position)
        => (byte) ((Player >> (position << 2)) & 0b1111);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong SetPropertyData(ulong property, int position, byte data)
    {
        var shift = position << 2;
        property &= ~(0b1111UL << shift);
        property |= (data & 0b1111UL) << shift;
        return property;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetDataCount(byte data) => data switch {
        0b1111 => 4,
        0b0111 => 3,
        0b0011 => 2,
        0b0001 => 1,
        0b0000 => 0,
        _ => -1 // invalid
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte SetDataBit(byte data, int position, bool value)
    {
        byte mask = (byte)(1 << (position & 0b11));
        data &= (byte)~mask;
        if (value)
            data |= mask;
        return data;
    }
}