using System.Linq;
using Cognite.Arb.Integration.Business;

namespace Cognite.Arb.Integration.Resource.ExternalDatabase
{
    public class Source : Integrator.ISource
    {
        public int[] GetNewCases()
        {
            using (var entities = new ExternalDatabaseEntities())
            {
                var result = entities.NewCases.Select(item => item.CaseNumber).Cast<int>().ToArray();
                return result;
            }
        }

        public Integrator.CaseData GetCase(int caseId)
        {
            using (var entities = new ExternalDatabaseEntities())
            {
                var query = entities.Cases
                    .Where(@case => @case.CaseNumber == caseId)
                    .Select(@case =>
                        new Integrator.CaseData
                        {
                            Id = @case.CaseNumber,
                            ClaimantContact = @case.CasePersons
                                .Where(casePerson => casePerson.RoleInCase == "Complainant")
                                .SelectMany(casePerson => casePerson.RegulationExecOrgPersons_Seed.RegulationExecutivePersons)
                                .Select(person =>
                                    new Integrator.CaseData.ContactData
                                    {
                                        Name = person.InvertedDisplayName,
                                        EMail = person.EmailAddress,
                                        Address = person.Address1 + "\n" + person.Address2 + "\n" + person.Address3 + "\n" +
                                                  person.Address4 + "\n" + person.Address5 + "\n" + person.Address6 + "\n" +
                                                  person.AddressPostcode,
                                        Phone = person.Telephone,
                                        // TODO: start date
                                    }).FirstOrDefault(),
                            CaseManagerEmail = @case.RegulationExecutiveUser.EmailAddress,
                        });

                var result = query.FirstOrDefault();

                return result;
            }
        }

        public void MarkCaseAsOld(int caseId)
        {
            using (var entities = new ExternalDatabaseEntities())
            {
                var entity = entities.NewCases.FirstOrDefault(item => item.CaseNumber == caseId);
                if (entity == null) return;
                entities.NewCases.Remove(entity);
                entities.SaveChanges();
            }
        }
    }
}