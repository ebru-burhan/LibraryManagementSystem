namespace Library.Model.Dtos;

public abstract class BaseExternalDto
{
    // Dış dünya id olarak bilecek, ama  ExternalId arkada
    public Guid Id { get; set; }
}