using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RENHelpers.ExampleProject.Data;

public class TestDescription
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Description { get; set; }

    public Test Test { get; set; }
    public int TestId { get; set; }

    public DateTime Date { get; set; }
}