namespace universe_lab.DAL.Models;

public class QueryOrdersDalModel
{
    public long[] Ids { get; set; }

    public long[] CustomerIds { get; set; }

    public int Limit { get; set; }

    public int Offset { get; set; }
}