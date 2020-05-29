using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JASShortCircuitExample
{
    public interface IDataAccessLayer
    {
        Task<bool> SqlPostAsync(string conString, string procName, string jsonBody);
    }
}
