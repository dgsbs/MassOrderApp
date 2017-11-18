using System;

public class OrderGroup {
    public int OrderGroupId {get;set;}

    public int OrderId {get;set;}    
    public StackExchange.Redis.Order Order { get;set;}
    public Status Status {get;set;}
    public int StatusId {get;set;}
    public string Name { get; set; }
    public DateTime Date {get;set;}
    
}