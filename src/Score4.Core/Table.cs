﻿using System.Runtime.CompilerServices;

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
        if (dataCount is -1 or >= 4)
            return this;
        
        boardData = SetDataBit(boardData, dataCount, true);
        playerData = SetDataBit(playerData, dataCount, player);
        
        return this with
        {
            Board = SetPropertyData(Board, position, boardData), 
            Player = SetPropertyData(Player, position, playerData)
        };
    }
    public bool CanPlay(int x, int y) 
        => TryPlay(x,y,false, out _);

    public bool TryPlay(int x, int y, bool player, out Table table)
    {
        table = Play(x, y, player);
        return table != this;
    }
    
    public bool IsValid()
    {
        for (int position = 0; position < 16; position++)
        {
            var boardData = GetPositionBoardData(position);
            var playerData = GetPositionPlayerData(position);
            var dataCount = GetDataCount(boardData);
            if (dataCount is -1 or >= 4)
                return false;
        }

        return true;
    }
    
    public (uint Player1, uint Player2) CountPoints()
    {
        var points = (Player1: 0U, Player2: 0U);
        foreach (var winningMask in TableUtils.WinningBoardMasks)
        {
            if ((Board & winningMask) != winningMask)
                continue;
            var playerMask = (Player & winningMask);
            if (playerMask == 0)
                points.Player1++;
            else if (playerMask == winningMask)
                points.Player2++;
        }

        return points;
    }

    public (bool Populated, bool Player)[,,] GetMatrix()
    {
        var result = new (bool, bool)[4, 4, 4];
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                var position = GetPosition(x, y);
                var boardData = GetPositionBoardData(position);
                var playerData = GetPositionPlayerData(position);
                for (int z = 0; z < 4; z++)
                    result[x, y, z] = (GetDataBit(z, boardData), GetDataBit(z, playerData));
            }
        }

        return result;
    }
    
    
    internal Table SetBoard(int x, int y, int z)
    {
        if (x >= 4 || y >= 4 || z >= 4)
            return this;
        var position = GetPosition(x, y);
        var boardData = GetPositionBoardData(position);
        boardData = SetDataBit(boardData, z, true);
        return this with
        {
            Board = SetPropertyData(Board, position, boardData)
        };
    }

    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte GetPositionBoardData(int position) 
        => GetPropertyData(Board, position);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte GetPositionPlayerData(int position)
        => GetPropertyData(Player, position);

    private static int GetPosition(int x, int y)
        => ((y & 0b11) << 2) | (x & 0b11);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte GetPropertyData(ulong property, int position)
        => (byte) ((property >> (position << 2)) & 0b1111);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong SetPropertyData(ulong property, int position, byte data)
    {
        var shift = position << 2;
        property &= ~(0b1111UL << shift);
        property |= (ulong)data << shift;
        return property;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool GetDataBit(int position, byte data)
    {
        var mask = 1 << position;
        return (data & mask) == mask;
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