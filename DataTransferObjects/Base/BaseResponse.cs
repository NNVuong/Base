using System.ComponentModel;

namespace DataTransferObjects.Base;

public abstract class BaseResponse
{
    [DisplayName("Khóa chính")] public Guid Id { get; set; } = Guid.NewGuid();

    [DisplayName("Đã xóa")] public bool IsDeleted { get; set; } = false;
}