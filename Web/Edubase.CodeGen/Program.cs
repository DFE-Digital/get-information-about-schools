using Edubase.Data;
using Edubase.Data.Entity;
using System.IO;

namespace Edubase.CodeGen
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataTables = DbUtil.GenerateDataTables(new ApplicationDbContext());
            var dataTable = dataTables.Get<Establishment>();
            var code = new AZSIndexConfigGenerator().Generate(dataTable, "Edubase.Services.Establishments.Search", "EstablishmentsSearchIndex", "establishments", "establishments");
            File.WriteAllText("EstablishmentsSearchIndex.cs", code);

            var governorDataTable = dataTables.Get<Governor>();
            code = new AZSIndexConfigGenerator().Generate(governorDataTable, "Edubase.Services.Governors.Search", "GovernorsSearchIndex", "governors", "governors");
            File.WriteAllText("GovernorsSearchIndex.cs", code);

            var groupsDataTable = dataTables.Get<GroupCollection>();
            code = new AZSIndexConfigGenerator().Generate(groupsDataTable, "Edubase.Services.Groups.Search", "GroupsSearchIndex", "groups", "groups");
            File.WriteAllText("GroupsSearchIndex.cs", code);
        }
    }
}
