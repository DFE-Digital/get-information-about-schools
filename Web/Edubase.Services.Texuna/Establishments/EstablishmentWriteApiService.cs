using Edubase.Services.Establishments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Establishments.Models;
using System.Security.Principal;

namespace Edubase.Services.Texuna.Establishments
{
    public class EstablishmentWriteApiService : IEstablishmentWriteService
    {
        public Task SaveAsync(EstablishmentModel model, IPrincipal principal)
        {
            throw new NotImplementedException();
        }
    }
}
