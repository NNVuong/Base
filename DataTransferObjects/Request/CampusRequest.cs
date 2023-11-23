using System.ComponentModel;
using DataTransferObjects.Base;
using Utilities.Helper;

namespace DataTransferObjects.Request;

public class CampusRequest : BaseRequest
{
    [DisplayName("Tên campus")] public string Name { get; set; } = "";

    [DisplayName("Mã campus")] public string Code => StringUtilities.ToUnicode(Name).ToUpper();
}