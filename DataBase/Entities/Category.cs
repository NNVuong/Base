using System.Collections.Generic;
using System.Text.Json.Serialization;
using DataBase.Base;

namespace DataBase.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = "";

    /* Tham chiếu */
    [JsonIgnore] public virtual ICollection<CategoryDetails> CategoryDetails { get; set; } = null!;
}