namespace Domain.Primitivies;

public abstract class ValueObject : IEquatable<ValueObject>
{
    public abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
        => obj is ValueObject other
           && GetType() == other.GetType()     
           && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

    public bool Equals(ValueObject? other)
        => other is not null
           && GetType() == other.GetType()       
           && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var component in GetEqualityComponents())
            hash.Add(component);
        return hash.ToHashCode();
    }
}