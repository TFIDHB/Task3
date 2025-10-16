using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task3.Core;
using System.Data.SqlClient;

namespace Task3.Core
{
    public interface IRepository
    {
        Task<IEnumerable<TaskClass>> GetTaskListAsync();
        Task<TaskClass> GetTaskAsync(int id);
        Task CreateAsync(TaskClass task);
        Task UpdateAsync(TaskClass task);
        Task DeleteAsync(int id);

    }
}
