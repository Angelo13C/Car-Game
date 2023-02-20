using System;
using System.Text;

public struct Cell : IEquatable<Cell>
{
    public uint SuperPosition;

    public bool IsCollapsed => (SuperPosition & (SuperPosition -1)) == 0;

    public static readonly Cell EMPTY = new Cell { SuperPosition = 0 };

    public Cell Overlap(Cell other) => new Cell { SuperPosition = this.SuperPosition & other.SuperPosition };
    public Cell Union(Cell other) => new Cell { SuperPosition = this.SuperPosition | other.SuperPosition };

    public byte GetEntropy()
    {
        //Just count the number of bits set
        uint superPosition = SuperPosition;
        byte entropy = 0;
        while(superPosition != 0)
        {
            superPosition &= (superPosition - 1);
            entropy++;
        }
        return entropy;
    }

    public bool Equals(Cell other) => SuperPosition == other.SuperPosition;
    public static bool operator == (Cell cell1, Cell cell2) => cell1.Equals(cell2);
    public static bool operator != (Cell cell1, Cell cell2) => !cell1.Equals(cell2);
    public override bool Equals(Object obj) => obj is Cell c && this == c;
    public override int GetHashCode() => SuperPosition.GetHashCode();
    
    public override string ToString()
    {
        var result = new StringBuilder();
        uint superPosition = SuperPosition;
        var currentBit = 1;
        while(superPosition != 0)
        {
            if((superPosition & 0x01) == 1)
                result.Append("|" + currentBit);
            superPosition = superPosition >> 1;
            currentBit *= 2;
        }
        if(result.Length != 0)
            result.Append('|');

        return result.ToString();
    }
}