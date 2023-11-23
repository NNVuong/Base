using System;
using System.Text.Json.Serialization;
using DataBase.Base;

namespace DataBase.Entities;

public class CategoryDetails : BaseEntity
{
    public string Name { get; set; } = "";

    /* Khóa ngoại */
    public Guid CategoryId { get; set; } = Guid.Empty;

    /* Tham chiếu */
    [JsonIgnore] public virtual Category Category { get; set; } = null!;
}