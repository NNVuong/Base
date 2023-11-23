using System.Collections.Generic;
using System.Text.Json.Serialization;
using DataBase.Base;

namespace DataBase.Entities;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class User : BaseEntity
{
    public string Name { get; set; } = "";

    public string Email { get; set; } = "";

    /* Tham chiếu */
    [JsonIgnore] public virtual ICollection<UserRole>? UserRole { get; set; }

    [JsonIgnore] public virtual ICollection<UserCampus>? UserCampus { get; set; }
}