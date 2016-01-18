namespace Cognite.Arb.Web.Models.Additional
{
    public class IdValueModel<Tid, Tvalue>
    {
        public Tid Id { get; set; }
        public Tvalue Value { get; set; }
    }
}