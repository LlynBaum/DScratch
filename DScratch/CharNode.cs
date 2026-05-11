namespace DScratch;

public class CharNode(char value, CharNode? origin, CharNode? rightOrigin)
{
    public char Value { get; } = value;
    
    public CharNode? Origin { get; internal set; } = origin;
    
    public CharNode? RightOrigin { get; internal set; } = rightOrigin;

    public bool Equals(CharNode? other)
    {
        return other is not null 
               && Value == other.Value 
               && Equals(Origin, other.Origin)
               && Equals(RightOrigin, other.RightOrigin);
    }
}