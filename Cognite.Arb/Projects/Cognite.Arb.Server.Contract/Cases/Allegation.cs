using System;

namespace Cognite.Arb.Server.Contract.Cases
{
    public class Allegation
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public Document[] Documents { get; set; }
        public bool CanBeDeleted { get; set; }
        public MyAllegationComment MyComment { get; set; }
    }
}