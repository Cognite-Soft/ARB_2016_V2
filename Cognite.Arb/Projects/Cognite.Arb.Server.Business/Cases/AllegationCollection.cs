namespace Cognite.Arb.Server.Business.Cases
{
    public class AllegationCollection
    {
        public Allegation[] Items { get; set; } // can only add items
        public bool CanAddAllegation { get; set; }
    }
}