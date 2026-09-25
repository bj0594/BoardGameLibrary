using System.ComponentModel.DataAnnotations;

namespace BoardGameLibrary.Api.Models;

public class CreateBoardGameRequest
{
    [Range(1, int.MaxValue)]
    public int BggId { get; set; }
}
