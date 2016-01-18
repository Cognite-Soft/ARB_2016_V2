using System;
using System.Linq;
using Cognite.Arb.Server.Business;
using System.IO;

namespace Cognite.Arb.WebApi.Resource.Documents
{
    public class SharePointDocumentStore : IDocumentStore
    {
        public void Upload(Guid documentId, int caseId, DocumentStoreItem item)
        {
            var docRepos = new DocumentRepository();

            using(var memoryStream = new MemoryStream(item.Content))
	        {
                docRepos.CreateDocument(memoryStream, caseId.ToString(), documentId.ToString(), item.Name);
	        }
        }

        public void Update(Guid documentId, int caseId, DocumentStoreItem item)
        {
            var docRepos = new DocumentRepository();

            using (var memoryStream = new MemoryStream(item.Content))
            {
                docRepos.UpdateDocument(memoryStream, caseId.ToString(), documentId.ToString(), item.Name);
            }
        }

        public DocumentStoreItem Download(Guid documentId)
        {
            var docRepos = new DocumentRepository();
            var documents = docRepos.GetDocuments(documentId.ToString());
            var document = documents.FirstOrDefault();
            
            if (document == null)
                return null;

            var result = new DocumentStoreItem();
            result.Name = document.DocumentName;
            result.Content = document.Content;

            return result;
        }

        public void Delete(Guid documentId)
        {
        }
    }
}