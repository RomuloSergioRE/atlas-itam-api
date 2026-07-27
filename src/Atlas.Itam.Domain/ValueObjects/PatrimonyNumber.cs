namespace Atlas.Itam.Domain.ValueObjects;

public sealed class PatrimonyNumber : IEquatable<PatrimonyNumber>
{
    public string Value { get; }

    private PatrimonyNumber(string value)
    {
        Value = value;
    }

    public static PatrimonyNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Patrimony number cannot be empty.", nameof(value));

        if (value.Length > 50)
            throw new ArgumentException("Patrimony number cannot exceed 50 characters.", nameof(value));

        return new PatrimonyNumber(value.Trim());
    }

    public bool Equals(PatrimonyNumber? other)
    {
        if (other is null) return false;
        return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => Equals(obj as PatrimonyNumber);
    public override int GetHashCode() => Value.ToUpperInvariant().GetHashCode();
    public override string ToString() => Value;

    public static bool operator ==(PatrimonyNumber? left, PatrimonyNumber? right) => Equals(left, right);
    public static bool operator !=(PatrimonyNumber? left, PatrimonyNumber? right) => !Equals(left, right);
}
