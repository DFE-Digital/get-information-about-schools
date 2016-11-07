//using AutoMapper;

//namespace Edubase.Import.Mapping
//{
//    using Data.Entity.Lookups;
//    using Helpers;

//    public class AutoMapperConfiguration
//    {
//        public static void Configure()
//        {
//            Mapper.Initialize(cfg =>
//            {
//                cfg.CreateMap<dynamic, LookupGovernorAppointingBody>()
//                    .ForAllMembers(x => x.ResolveUsing(new ValueResolver()));
                

//            });
//        }

//        public class ValueResolver : IValueResolver<object, LookupGovernorAppointingBody, object>
//        {
//            public ValueResolver()
//            {

//            }

//            public object Resolve(dynamic source, LookupGovernorAppointingBody destination, object destMember, ResolutionContext context)
//            {
//                destination.Code = Helper.GetPropertyValue(source, COL_CODE);
//                destination.Name = Helper.GetPropertyValue(source, COL_NAME);
//                destination.DisplayOrder = Helper.ToShort(Helper.GetPropertyValue(source, COL_ORDER));
//                return null;
//            }
//        }
//    }
//}
