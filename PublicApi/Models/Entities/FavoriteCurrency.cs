using System.ComponentModel.DataAnnotations;
using Fuse8.BackendInternship.PublicApi.Models.Types;

namespace Fuse8.BackendInternship.PublicApi.Models.Entities;

public class FavoriteCurrency
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public CurrencyCode Currency { get; set; }

    [Required]
    public CurrencyCode BaseCurrency { get; set; }
}