using Edubase.Data;
using Edubase.Data.Entity;
using System.IO;
using System.Linq;
using MoreLinq;
using System.Data;
using System.Text;

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
            code = new AZSSearchResultGenerator().Generate(dataTable, "Edubase.Services.Establishments.Search", "SearchEstablishmentDocument");
            File.WriteAllText("SearchEstablishmentDocument.cs", code);
            code = new AZSSearchCriteriaGenerator().Generate(dataTable, "Edubase.Services.Establishments.Search", "EstablishmentSearchCriteria");
            File.WriteAllText("EstablishmentSearchCriteria.cs", code);


            var governorDataTable = dataTables.Get<Governor>();
            code = new AZSIndexConfigGenerator().Generate(governorDataTable, "Edubase.Services.Governors.Search", "GovernorsSearchIndex", "governors", "governors");
            File.WriteAllText("GovernorsSearchIndex.cs", code);
            code = new AZSSearchResultGenerator().Generate(dataTable, "Edubase.Services.Governors.Search", "SearchGovernorDocument");
            File.WriteAllText("SearchGovernorDocument.cs", code);

            var groupsDataTable = dataTables.Get<GroupCollection>();
            code = new AZSIndexConfigGenerator().Generate(groupsDataTable, "Edubase.Services.Groups.Search", "GroupsSearchIndex", "groups", "groups");
            File.WriteAllText("GroupsSearchIndex.cs", code);
            code = new AZSSearchResultGenerator().Generate(dataTable, "Edubase.Services.Groups.Search", "SearchGroupDocument");
            File.WriteAllText("SearchGroupDocument.cs", code);
        }
    }
}
