namespace TangerineAuction.Core.Models;

/// <summary>
/// Base entity with ID
/// </summary>
public abstract class BaseEntity
{

    /// <summary>
    /// Entity ID
    /// </summary>
    public Guid Id { get; private set; } = Guid.Empty;

    public override string ToString()
    {
        return $"Id: {Id}";
    }

    public bool Equals(BaseEntity other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is BaseEntity entity && Equals(entity);
    }

    public override int GetHashCode() => Id.GetHashCode();

}