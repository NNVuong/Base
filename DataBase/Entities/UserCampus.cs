using System;
using System.Text.Json.Serialization;
using DataBase.Base;

namespace DataBase.Entities;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class UserCampus : BaseEntity
{
    /* Khóa ngoại */
    public Guid UserId { get; set; } = Guid.Empty;

    public Guid CampusId { get; set; } = Guid.Empty;

    /* Tham chiếu */
    [JsonIgnore] public virtual User User { get; set; } = null!;

    [JsonIgnore] public virtual Campus Campus { get; set; } = null!;
}