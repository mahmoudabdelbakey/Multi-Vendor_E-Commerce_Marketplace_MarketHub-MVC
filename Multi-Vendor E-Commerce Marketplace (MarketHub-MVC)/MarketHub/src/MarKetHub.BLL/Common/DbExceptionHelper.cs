using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MarketHub.BLL.Common;

public static class DbExceptionHelper
{
    /// <summary>SQL Server error numbers: 2601 = duplicate key in a unique index, 2627 = unique constraint violation.</summary>
    public static bool IsUniqueViolation(DbUpdateException ex) =>
        ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627);
}
