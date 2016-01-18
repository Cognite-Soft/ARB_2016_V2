using System;

namespace Cognite.Arb.Server.Business
{
    public interface IDocumentStore
    {
        void Upload(Guid documentId, int caseId, DocumentStoreItem item);
        void Update(Guid documentId, int caseId, DocumentStoreItem item);
        DocumentStoreItem Download(Guid documentId);
        void Delete(Guid documentId);
    }

    public class DocumentStoreItem
    {
        public string Name { get; set; }
        public byte[] Content { get; set; }
    }
}
