using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Neuraltech.SharedKernel.Infraestructure.Persistence.EFCore.Models;

public class BaseEFCoreModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required Guid Id { get; init; }
    
    [Required]
    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public BaseEFCoreModel() { }
}