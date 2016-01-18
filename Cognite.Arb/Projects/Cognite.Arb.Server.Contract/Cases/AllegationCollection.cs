namespace Cognite.Arb.Server.Contract.Cases
{
    public class AllegationCollection
    {
        public Allegation[] Items { get; set; } // can only add items
        public bool CanAddAllegation { get; set; }
    }
}