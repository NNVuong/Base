namespace DataTransferObjects.Base;

public abstract class BaseRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public bool IsDeleted { get; set; } = false;
}