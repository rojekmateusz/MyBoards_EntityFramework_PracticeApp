﻿namespace MyBoards.Entities;

public abstract class WorkItem
{
    public int Id { get; set; }
    public string Area { get; set; }
    public string IterationPath { get; set; }
    public int Priority { get; set; }
    
    public List<Comment> Comments { get; set; } = [];
    
    public User Author { get; set; }
    public Guid AuthorId { get; set; }

    public int? TagId { get; set; }
    public List<Tag> Tags { get; set; }

    public int StateId { get; set; }
    public WorkItemState WorkItemstate { get; set; }
}

public class Epic : WorkItem
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class Issue : WorkItem
{
    public decimal Efford { get; set; }
}

public class Task : WorkItem
{
    public string Activity { get; set; }
    public decimal RemaningWork { get; set; }
}