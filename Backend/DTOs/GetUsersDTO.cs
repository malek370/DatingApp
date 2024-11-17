using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class GetUsersDTO
{

    public required string username { get; set; }
    public required int id { get; set; }
}