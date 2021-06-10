using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarServiceProg.EF;

namespace CarServiceProg
{
    public class DB
    {
        public static Context Context { get; set; } = new Context();
    }
}
