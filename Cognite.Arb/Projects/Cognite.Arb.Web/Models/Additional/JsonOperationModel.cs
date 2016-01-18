namespace Cognite.Arb.Web.Models.Additional
{
    public class JsonOperationModel
    {
        public bool IsSucceeded { get; set; }
        public object Result { get; set; }

        public JsonOperationModel(bool isSucceeded, object result)
        {
            this.IsSucceeded = isSucceeded;
            this.Result = result;
        }
    }
}