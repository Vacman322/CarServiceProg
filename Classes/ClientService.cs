using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarServiceProg.EF
{
    public partial class ClientService
    {
        public int CountOfDocs { get => DocumentByService.Count; }
    }
}
