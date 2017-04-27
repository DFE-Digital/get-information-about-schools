using Edubase.Common;
using Edubase.Data.DbContext;
using Edubase.Data.Entity;
using Edubase.Services.Exceptions;
using Edubase.Services.Governors.Models;
using Edubase.Services.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Governors
{
    public class GovernorsWriteService : IGovernorsWriteService
    {
        private readonly IApplicationDbContextFactory _dbContextFactory;
        private readonly ISecurityService _securityService;
        
        public GovernorsWriteService(IApplicationDbContextFactory dbContextFactory, ISecurityService securityService)
        {
            _dbContextFactory = dbContextFactory;
            _securityService = securityService;
        }

        public async Task<int> SaveAsync(GovernorModelBase model, IPrincipal principal)
        {
            Guard.IsNotNull(model.GroupUID ?? model.EstablishmentUrn, () => new EdubaseException("GroupUID and URN cannot both be null"));

            var dc = _dbContextFactory.Obtain();
            var dataModel = model.IsNewEntity ? new Governor() : dc.Governors.SingleOrThrow(x => x.Id == model.Id);

            dataModel.AppointingBodyId = model.AppointingBodyId;
            dataModel.AppointmentEndDate = model.AppointmentEndDate;
            dataModel.AppointmentStartDate = model.AppointmentStartDate;
            dataModel.DOB = model.DOB;
            dataModel.EmailAddress = model.EmailAddress;
            dataModel.EstablishmentUrn = model.EstablishmentUrn;
            dataModel.GroupUID = model.GroupUID;
            dataModel.Nationality = model.Nationality;

            dataModel.Person.Title = model.Person_Title;
            dataModel.Person.FirstName = model.Person_FirstName;
            dataModel.Person.MiddleName = model.Person_MiddleName;
            dataModel.Person.LastName = model.Person_LastName;

            dataModel.PreviousPerson.Title = model.PreviousPerson_Title;
            dataModel.PreviousPerson.FirstName = model.PreviousPerson_FirstName;
            dataModel.PreviousPerson.MiddleName = model.PreviousPerson_MiddleName;
            dataModel.PreviousPerson.LastName = model.PreviousPerson_LastName;
            
            dataModel.RoleId = model.RoleId;
            dataModel.PostCode = model.PostCode;
            dataModel.TelephoneNumber = model.TelephoneNumber;

            if (model.IsNewEntity) dc.Governors.Add(dataModel);

            await dc.SaveChangesAsync();

            model.Id = dataModel.Id;
            return dataModel.Id;
        }

        public async Task DeleteAsync(int id, IPrincipal principal)
        {
            var dc = _dbContextFactory.Obtain();
            var dataModel = dc.Governors.SingleOrThrow(x => x.Id == id);
            dc.Governors.Remove(dataModel);
            await dc.SaveChangesAsync();
        }

        //TODO: TEXCHANGE - implement ability to add an establishment to a shared governor
        public async Task AddUpdateEstablishmentToSharedGovernor(int governorId, int establishmentUrn, DateTime appointmentStartDate, DateTime appointmentEndDate)
        {
            var context = _dbContextFactory.Obtain();
            if (! await context.EstablishmentGovernors.AnyAsync(e => e.GovernorId == governorId &&
                                                         e.EstabishmentUrn == establishmentUrn))
            {
                context.EstablishmentGovernors.Add(new EstablishmentGovernor
                {
                    GovernorId = governorId,
                    EstabishmentUrn = establishmentUrn,
                    AppointmentStartDate = appointmentStartDate,
                    AppointmentEndDate = appointmentEndDate
                });
            }
            else
            {
                var governor = context.EstablishmentGovernors.SingleOrThrow(
                        e => e.GovernorId == governorId && e.EstabishmentUrn == establishmentUrn);
                governor.AppointmentStartDate = appointmentStartDate;
                governor.AppointmentEndDate = appointmentEndDate;
            }

            await context.SaveChangesAsync();
        }

        public async Task DeleteSharedGovernorEstablishment(int governorId, int establishmentUrn)
        {
            var context = _dbContextFactory.Obtain();
            var entity = context.EstablishmentGovernors.SingleOrThrow(e => e.GovernorId == governorId && e.EstabishmentUrn == establishmentUrn);
            context.EstablishmentGovernors.Remove(entity);
            await context.SaveChangesAsync();
        }
    }
}
