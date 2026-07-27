namespace Atlas.Itam.Domain.ValueObjects;

public sealed class SerialNumber : IEquatable<SerialNumber>
{
    public string Value { get; }

    private SerialNumber(string value)
    {
        Value = value;
    }

    public static SerialNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Serial number cannot be empty.", nameof(value));

        if (value.Length > 100)
            throw new ArgumentException("Serial number cannot exceed 100 characters.", nameof(value));

        return new SerialNumber(value.Trim());
    }

    public bool Equals(SerialNumber? other)
    {
        if (other is null) return false;
        return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => Equals(obj as SerialNumber);
    public override int GetHashCode() => Value.ToUpperInvariant().GetHashCode();
    public override string ToString() => Value;

    public static bool operator ==(SerialNumber? left, SerialNumber? right) => Equals(left, right);
    public static bool operator !=(SerialNumber? left, SerialNumber? right) => !Equals(left, right);
}
