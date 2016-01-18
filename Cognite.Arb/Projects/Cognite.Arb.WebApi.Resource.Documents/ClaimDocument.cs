namespace Cognite.Arb.WebApi.Resource.Documents
{
    public class ClaimDocument
    {
        /// <summary>
        /// Gets or sets the name of the document.
        /// </summary>
        /// <value>
        /// The name of the document.
        /// </value>
        public string DocumentName { get; set; }
        /// <summary>
        /// Gets or sets the document URL.
        /// </summary>
        /// <value>
        /// The document URL.
        /// </value>
        public string DocumentUrl { get; set; }

        /// <summary>
        /// Gets or sets the document id.
        /// </summary>
        /// <value>
        /// The document id.
        /// </value>
        public string DocumentId { get; set; }

        public byte[] Content { get; set; }
    }
}
