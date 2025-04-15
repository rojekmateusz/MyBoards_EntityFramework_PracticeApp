﻿namespace MyBoards.Entities;

public class Address
{
    public string Country { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string PostalCode { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } 
}
