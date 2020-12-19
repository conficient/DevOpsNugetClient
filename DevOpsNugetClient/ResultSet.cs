using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOpsNugetClient
{
    /// <summary>
    /// Generic resultset returned from devops feed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultSet<T>
    {
        public int Count { get; set; }

        public T Value { get; set; }
    }
}
