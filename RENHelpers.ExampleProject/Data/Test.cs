using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RENHelpers.ExampleProject.Data;

public class Test
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string TestName { get; set; }
    public Side Side { get; set; }
    public int SideId { get; set; }

    public List<TestDescription> TestDescriptions { get; set; }
}