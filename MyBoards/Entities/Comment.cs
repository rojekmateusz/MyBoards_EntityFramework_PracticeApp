﻿namespace MyBoards.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Message { get; set; }
    public virtual User Author { get; set; }
    public Guid AuthorId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public int WorkItemId { get; set; }
    public WorkItem WorkItem { get; set; }
}
