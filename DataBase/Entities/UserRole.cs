using System;
using System.Text.Json.Serialization;
using DataBase.Base;

namespace DataBase.Entities;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class UserRole : BaseEntity
{
    /* Khóa ngoại */
    public Guid UserId { get; set; } = Guid.Empty;

    public Guid RoleId { get; set; } = Guid.Empty;

    /* Tham chiếu */
    [JsonIgnore] public virtual User User { get; set; } = null!;

    [JsonIgnore] public virtual Role Role { get; set; } = null!;
}