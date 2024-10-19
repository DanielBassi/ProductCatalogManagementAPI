namespace Domain.Dto;

public record ProductFilterDto(Guid? Id = null, string? Name = null, decimal? MinPrice = null, decimal? MaxPrice = null);
