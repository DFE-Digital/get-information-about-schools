using Edubase.Services.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Groups.Models;
using System.Security.Principal;

namespace Edubase.Services.Texuna.Groups
{
    public class GroupsWriteApiService : IGroupsWriteService
    {
        public Task<int> SaveAsync(SaveGroupDto dto, IPrincipal principal)
        {
            throw new NotImplementedException($"{nameof(GroupsWriteApiService)}::{nameof(SaveAsync)}");
        }
    }
}
