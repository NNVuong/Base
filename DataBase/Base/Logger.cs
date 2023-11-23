using System;
using System.ComponentModel;

namespace DataBase.Base;

public class Logger
{
    [DisplayName("Thời gian")] public DateTime Time { get; set; }

    [DisplayName("Nội dung")] public required string Content { get; set; }
}