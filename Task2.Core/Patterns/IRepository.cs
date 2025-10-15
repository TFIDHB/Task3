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
        IEnumerable<TaskClass> GetTaskList();
        TaskClass GetTask(int id);
        void Create(TaskClass task);
        void Update(TaskClass task);
        void Delete(int id);

    }
}
