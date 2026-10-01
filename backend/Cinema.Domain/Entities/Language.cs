namespace Cinema.Domain.Entities;

public class Language
{
    public int LanguageId { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<Movie> Movies { get; set; } = [];
}
