using Edubase.Data.Entity.Lookups;
using Edubase.Services.Enums;
using Edubase.Services.Security;
using Edubase.Services.Security.Permissions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.StubUserBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new Config();

            config.UserList.Add(new User
            {
                DisplayName = "Estab user (urn:100053)",
                Description = "Establishment (excludes children's centres)",
                Assertion = new Assertion
                {
                    NameId = "estab.user",
                    AttributeStatements = new List<AttributeStatement>
                    {
                        new AttributeStatement
                        {
                            Type = ClaimTypes.Role,
                            Value = EdubaseRoles.Establishment
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.EditEstablishment,
                            ValueObject = new EditEstablishmentPermissions
                            {
                                Urns = new[] { 100053 }
                            }
                        }
                    }
                }
            });



            config.UserList.Add(new User
            {
                DisplayName = "MAT user (uid:1578)",
                Description = "Updating own governance information and details for academies belonging to the trust",
                Assertion = new Assertion
                {
                    NameId = "mat.user",
                    AttributeStatements = new List<AttributeStatement>
                    {
                        new AttributeStatement
                        {
                            Type = ClaimTypes.Role,
                            Value = EdubaseRoles.MAT
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.EditEstablishment,
                            ValueObject = new EditEstablishmentPermissions
                            {
                                AllUrns = true,
                                GroupIds = new[] { 1578 }
                            }
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.EditGroup,
                            ValueObject = new EditGroupPermissions
                            {
                                GroupIds = new[] { 1578 }
                            }
                        }
                    }
                }
            });


            config.UserList.Add(new User
            {
                DisplayName = "Local authority (Westminster) (code:213)",
                Description = "Updating LA maintained school records within own authority.",
                Assertion = new Assertion
                {
                    NameId = "la.user",
                    AttributeStatements = new List<AttributeStatement>
                    {
                        new AttributeStatement
                        {
                            Type = ClaimTypes.Role,
                            Value = EdubaseRoles.LA
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.EditEstablishment,
                            ValueObject = new EditEstablishmentPermissions
                            {
                                AllUrns = true,
                                LocalAuthorities = new[] { eLocalAuthority.Westminster },
                                EstablishmentTypeGroups = new [] { eLookupEstablishmentTypeGroup.LAMaintainedSchools }
                            }
                        }
                    }
                }
            });

            config.UserList.Add(new User
            {
                DisplayName = "Local authority CC (Kent) (code:886) ",
                Description = "Creating and updating children's centre records within own authority.",
                Assertion = new Assertion
                {
                    NameId = "la.cc.user",
                    AttributeStatements = new List<AttributeStatement>
                    {
                        new AttributeStatement
                        {
                            Type = ClaimTypes.Role,
                            Value = EdubaseRoles.LAChildrensCentre
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.EditEstablishment,
                            ValueObject = new EditEstablishmentPermissions
                            {
                                AllUrns = true,
                                LocalAuthorities = new[] { eLocalAuthority.Kent },
                                EstablishmentTypeGroups = new [] { eLookupEstablishmentTypeGroup.ChildrensCentres }
                            }
                        }
                    }
                }
            });


            config.UserList.Add(new User
            {
                DisplayName = "Education Funding Agency",
                Description = "Creating and update academy records.",
                Assertion = new Assertion
                {
                    NameId = "efa.user",
                    AttributeStatements = new List<AttributeStatement>
                    {
                        new AttributeStatement(ClaimTypes.Role, EdubaseRoles.EFA),
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.EditEstablishment,
                            ValueObject = new EditEstablishmentPermissions
                            {
                                AllUrns = true,
                                EstablishmentTypeGroups = new [] { eLookupEstablishmentTypeGroup.Academies, eLookupEstablishmentTypeGroup.FreeSchools }
                            }
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.EditGroup,
                            ValueObject = new EditGroupPermissions
                            {
                                AllGroups = true,
                                Types = new[] { eLookupGroupType.MultiacademyTrust, eLookupGroupType.SingleacademyTrust, eLookupGroupType.SchoolSponsor }
                            }
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.CreateGroup,
                            ValueObject = new CreateGroupPermissions
                            {
                                Types = new[] { eLookupGroupType.MultiacademyTrust, eLookupGroupType.SingleacademyTrust, eLookupGroupType.SchoolSponsor }
                            }
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.CreateEstablishment,
                            ValueObject = new CreateEstablishmentPermissions
                            {
                                EstablishmentTypeGroups = new [] { eLookupEstablishmentTypeGroup.Academies, eLookupEstablishmentTypeGroup.FreeSchools }
                            }
                        }
                    }
                }
            });



            config.UserList.Add(new User
            {
                DisplayName = "AOS - Data owner",
                Description = "Creating new academy records, creating MAT, SAT and Sponsor records, assigning academies to MATs.",
                Assertion = new Assertion
                {
                    NameId = "aos.user",
                    AttributeStatements = new List<AttributeStatement>
                    {
                        new AttributeStatement(ClaimTypes.Role, EdubaseRoles.AOS),
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.EditEstablishment,
                            ValueObject = new EditEstablishmentPermissions
                            {
                                AllUrns = true,
                                EstablishmentTypeGroups = new [] { eLookupEstablishmentTypeGroup.Academies }
                            }
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.EditGroup,
                            ValueObject = new EditGroupPermissions
                            {
                                AllGroups = true,
                                Types = new[] { eLookupGroupType.MultiacademyTrust, eLookupGroupType.SingleacademyTrust, eLookupGroupType.SchoolSponsor }
                            }
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.CreateGroup,
                            ValueObject = new CreateGroupPermissions
                            {
                                Types = new[] { eLookupGroupType.MultiacademyTrust, eLookupGroupType.SingleacademyTrust, eLookupGroupType.SchoolSponsor }
                            }
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.CreateEstablishment,
                            ValueObject = new CreateEstablishmentPermissions
                            {
                                EstablishmentTypeGroups = new [] { eLookupEstablishmentTypeGroup.Academies }
                            }
                        }
                    }
                }
            });



            config.UserList.Add(new User
            {
                DisplayName = "Free Schools Group",
                Description = "Creating and updating new free school records.",
                Assertion = new Assertion
                {
                    NameId = "fsg.user",
                    AttributeStatements = new List<AttributeStatement>
                    {
                        new AttributeStatement(ClaimTypes.Role, EdubaseRoles.FSG),
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.EditEstablishment,
                            ValueObject = new EditEstablishmentPermissions
                            {
                                AllUrns = true,
                                EstablishmentTypeGroups = new [] { eLookupEstablishmentTypeGroup.FreeSchools }
                            }
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.CreateEstablishment,
                            ValueObject = new CreateEstablishmentPermissions
                            {
                                EstablishmentTypeGroups = new [] { eLookupEstablishmentTypeGroup.FreeSchools }
                            }
                        }
                    }
                }
            });


            config.UserList.Add(new User
            {
                DisplayName = "Independent Education and Boarding Schools Team user",
                Description = "Creating and updating independent school, non-maintained special school and British Schools Overseas records.",
                Assertion = new Assertion
                {
                    NameId = "iebt.user",
                    AttributeStatements = new List<AttributeStatement>
                    {
                        new AttributeStatement(ClaimTypes.Role, EdubaseRoles.IEBT),
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.EditEstablishment,
                            ValueObject = new EditEstablishmentPermissions
                            {
                                AllUrns = true,
                                EstablishmentTypeGroups = new [] { eLookupEstablishmentTypeGroup.IndependentSchools }, 
                                EstablishmentTypes = new [] { eLookupEstablishmentType.BritishSchoolsOverseas, eLookupEstablishmentType.NonmaintainedSpecialSchool }
                            }
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.CreateEstablishment,
                            ValueObject = new CreateEstablishmentPermissions // todo: do permissions for IEBT
                            {
                                EstablishmentTypeGroups = new [] { eLookupEstablishmentTypeGroup.IndependentSchools },
                                EstablishmentTypes = new [] { eLookupEstablishmentType.BritishSchoolsOverseas, eLookupEstablishmentType.NonmaintainedSpecialSchool }
                            }
                        }
                    }
                }
            });

            config.UserList.Add(new User
            {
                DisplayName = "School Organisation",
                Description = "Creating and updating la maintained school records.",
                Assertion = new Assertion
                {
                    NameId = "school.user",
                    AttributeStatements = new List<AttributeStatement>
                    {
                        new AttributeStatement(ClaimTypes.Role, EdubaseRoles.School),
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.EditEstablishment,
                            ValueObject = new EditEstablishmentPermissions
                            {
                                AllUrns = true,
                                EstablishmentTypeGroups = new [] { eLookupEstablishmentTypeGroup.LAMaintainedSchools }
                            }
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.CreateEstablishment,
                            ValueObject = new CreateEstablishmentPermissions
                            {
                                EstablishmentTypeGroups = new [] { eLookupEstablishmentTypeGroup.LAMaintainedSchools }
                            }
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.EditGroup,
                            ValueObject = new EditGroupPermissions
                            {
                                AllGroups = true,
                                Types = new[] { eLookupGroupType.Federation, eLookupGroupType.Trust }
                            }
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.CreateGroup,
                            ValueObject = new CreateGroupPermissions
                            {
                                Types = new[] { eLookupGroupType.Federation, eLookupGroupType.Trust }
                            }
                        }
                    }
                }
            });



            config.UserList.Add(new User
            {
                DisplayName = "Pupil Referral Unit (PRU)/Alternative Provision (AP) Team",
                Description = "Creating and updating PRU records.",
                Assertion = new Assertion
                {
                    NameId = "pru.user",
                    AttributeStatements = new List<AttributeStatement>
                    {
                        new AttributeStatement(ClaimTypes.Role, EdubaseRoles.PRU),
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.EditEstablishment,
                            ValueObject = new EditEstablishmentPermissions
                            {
                                AllUrns = true,
                                EstablishmentTypes = new [] { eLookupEstablishmentType.PupilReferralUnit }
                            }
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.CreateEstablishment,
                            ValueObject = new CreateEstablishmentPermissions
                            {
                                EstablishmentTypes = new [] { eLookupEstablishmentType.PupilReferralUnit }
                            }
                        }
                    }
                }
            });

            config.UserList.Add(new User
            {
                DisplayName = "Children's centre",
                Description = "Creating and updating all children's centre and children's centre group records.",
                Assertion = new Assertion
                {
                    NameId = "cc.user",
                    AttributeStatements = new List<AttributeStatement>
                    {
                        new AttributeStatement(ClaimTypes.Role, EdubaseRoles.ChildrensCentre),
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.EditEstablishment,
                            ValueObject = new EditEstablishmentPermissions
                            {
                                AllUrns = true,
                                EstablishmentTypeGroups = new [] { eLookupEstablishmentTypeGroup.ChildrensCentres }
                            }
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.CreateEstablishment,
                            ValueObject = new CreateEstablishmentPermissions
                            {
                                EstablishmentTypeGroups = new [] { eLookupEstablishmentTypeGroup.ChildrensCentres }
                            }
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.EditGroup,
                            ValueObject = new EditGroupPermissions
                            {
                                AllGroups = true,
                                Types = new[] { eLookupGroupType.ChildrensCentresGroup }
                            }
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.CreateGroup,
                            ValueObject = new CreateGroupPermissions
                            {
                                Types = new[] { eLookupGroupType.ChildrensCentresGroup }
                            }
                        }
                    }
                }
            });


            config.UserList.Add(new User
            {
                DisplayName = "Backoffice",
                Description = "Administrator",
                Assertion = new Assertion
                {
                    NameId = "administrator.user",
                    AttributeStatements = new List<AttributeStatement>
                    {
                        new AttributeStatement(ClaimTypes.Role, EdubaseRoles.Admin),
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.EditEstablishment,
                            ValueObject = new EditEstablishmentPermissions
                            {
                                AllUrns = true
                            }
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.CreateEstablishment,
                            ValueObject = new CreateEstablishmentPermissions()
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.EditGroup,
                            ValueObject = new EditGroupPermissions
                            {
                                AllGroups = true
                            }
                        },
                        new AttributeStatement
                        {
                            Type = EduClaimTypes.CreateGroup,
                            ValueObject = new CreateGroupPermissions()
                        }
                    }
                }
            });


            var json = Newtonsoft.Json.JsonConvert.SerializeObject(config, new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented
            });

            Console.Write(json);

            File.WriteAllText("stubidp_config.json", json);

            Console.ReadKey();
        }

    }
}
