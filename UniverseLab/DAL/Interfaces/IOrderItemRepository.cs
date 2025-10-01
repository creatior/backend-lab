using universe_lab.DAL.Models;

namespace universe_lab.DAL.Interfaces;

public interface IOrderItemRepository
{
    Task<V1OrderItemDal[]> BulkInsert(V1OrderItemDal[] model, CancellationToken token);
    
    Task<V1OrderItemDal[]> Query(QueryOrderItemsDalModel model, CancellationToken token);
}