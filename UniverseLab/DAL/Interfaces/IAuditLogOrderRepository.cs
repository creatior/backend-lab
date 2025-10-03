using universe_lab.DAL.Models;

namespace universe_lab.DAL.Interfaces;

public interface IAuditLogOrderRepository
{
    Task<V1AuditLogOrderDal[]> BulkInsert(V1AuditLogOrderDal[] logs, CancellationToken token);
}