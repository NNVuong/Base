using System;

namespace DataBase.Base;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CreatedBy { get; set; } = Guid.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Guid ModifiedBy { get; set; } = Guid.Empty;

    public DateTime ModifiedAt { get; set; } = DateTime.Now;

    public bool IsDeleted { get; set; } = false;
}