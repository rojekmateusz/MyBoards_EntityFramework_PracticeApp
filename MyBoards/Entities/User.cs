﻿namespace MyBoards.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FirtsName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public Address Address { get; set; }
    public List<WorkItem> WorkItems { get; set; } = [];
    
}
