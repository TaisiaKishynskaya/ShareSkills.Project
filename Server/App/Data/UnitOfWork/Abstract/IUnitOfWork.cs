using Microsoft.EntityFrameworkCore;

namespace App.Data.UnitOfWork.Abstract;

public interface IUnitOfWork : IDisposable
{
    DbContext Context { get; }
    public Task SaveChangesAsync();
}
