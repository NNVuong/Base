using System.Collections.Generic;
using System.Text.Json.Serialization;
using DataBase.Base;

namespace DataBase.Entities;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Campus : BaseEntity
{
    public string Name { get; set; } = "";

    public string Code { get; set; } = "";

    /* Tham chiếu */
    [JsonIgnore] public virtual ICollection<UserCampus>? UserCampus { get; set; }
}