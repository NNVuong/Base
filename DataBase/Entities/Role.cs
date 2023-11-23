using System.Collections.Generic;
using System.Text.Json.Serialization;
using DataBase.Base;

namespace DataBase.Entities;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Role : BaseEntity
{
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    /* Tham chiếu */
    [JsonIgnore] public virtual ICollection<UserRole>? UserRole { get; set; }
}