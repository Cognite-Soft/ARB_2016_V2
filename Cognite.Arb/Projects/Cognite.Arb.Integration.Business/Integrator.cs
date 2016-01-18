using System;
using System.Collections.Generic;

namespace Cognite.Arb.Integration.Business
{
    public class Integrator
    {
        public interface ISource
        {
            int[] GetNewCases();
            CaseData GetCase(int caseId);
            void MarkCaseAsOld(int caseId);
        }

        public interface IDestination
        {
            void CreateCase(CaseData caseData);
        }

        public interface ILog
        {
            void LogException(Exception exception);
        }

        public class CaseData
        {
            public class ContactData
            {
                public string Name { get; set; } // InvertedDisplayName
                public string EMail { get; set; }
                public string Address { get; set; }
                public string Phone { get; set; } // phone, business, fax
            }

            public int Id { get; set; }
            public ContactData ClaimantContact { get; set; }
            public string CaseManagerEmail { get; set; }
            public DateTime StartDate { get; set; }
        }

        private readonly ISource _source;
        private readonly IDestination _destination;
        private readonly ILog _log;

        public Integrator(ISource source, IDestination destination, ILog log)
        {
            _source = source;
            _destination = destination;
            _log = log;
        }

        public void Integrate()
        {
            var newCases = GetNewCases();
            IntegrateNewCases(newCases);
        }

        private IEnumerable<int> GetNewCases()
        {
            try
            {
                var cases = _source.GetNewCases();
                return cases;
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
                return new int[0];
            }
        }

        private void IntegrateNewCases(IEnumerable<int> newCases)
        {
            foreach (var caseId in newCases)
                IntegrateNewCase(caseId);
        }

        private void IntegrateNewCase(int caseId)
        {
            try
            {
                var caseData = _source.GetCase(caseId);
                _destination.CreateCase(caseData);
                _source.MarkCaseAsOld(caseId);
            }
            catch (Exception ex)
            {
                _log.LogException(ex);
            }
        }
    }
}
