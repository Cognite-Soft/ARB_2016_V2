using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SharePoint.Client;

namespace Cognite.Arb.WebApi.Resource.Documents
{
    public class DocumentRepository : BaseRepository
    {
        #region //Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentRepository"/> class.
        /// </summary>
        public DocumentRepository()
            : base()
        {
        }
        #endregion

        #region //Public Methods

        /// <summary>
        /// Gets the documents.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public List<ClaimDocument> GetDocuments(string id)
        {
           // return new List<ClaimDocument>();

            try
            {
                if (_ctx != null)
                {
                    List<ClaimDocument> docs = new List<ClaimDocument>();
                    List list = _ctx.Web.Lists.GetByTitle(LibraryName);
                    FieldCollection fields = list.Fields;
                    _ctx.Load(fields);
                    _ctx.ExecuteQuery();

                    CamlQuery camlQuery = new CamlQuery();
                    camlQuery.ViewXml = string.Format(Constant.GetDocumentsQuery, id);

                    ListItemCollection listItems = list.GetItems(camlQuery);
                    _ctx.Load(listItems);
                    _ctx.ExecuteQuery();


                    foreach (var item in listItems)
                    {
                        if (item.File != null)
                        {
                            ClaimDocument obj = new ClaimDocument();
                            string fileName = item[Constant.FileName] as string;
                            var fInfo = new FileInfo(fileName);
                            var source = string.Format("{0}/{1}/{2}", Url,
                                                       LibraryName.Replace(" ", ""), fInfo.Name);
                            obj.DocumentName = fInfo.Name;
                            obj.DocumentUrl = source;
                            obj.DocumentId = item[Constant.DocumentId].ToString();
                            obj.Content = GetFileContent(item.File);
                            docs.Add(obj);
                        }
                    }
                    return docs;
                }
                else
                {
                    throw new Exception("Context Doesn't Exsist");
                }
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                ConnectionClose();
            }
        }

        private byte[] GetFileContent(Microsoft.SharePoint.Client.File file)
        {
          //  return new byte[0];
            try
            {
                _ctx.Load(file);
                _ctx.ExecuteQuery();

                FileInformation spFileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(_ctx, file.ServerRelativeUrl);
                using (var memoryStream = new MemoryStream())
                {
                    spFileInfo.Stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch
            {
                return new byte[0];
            }
        }


        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <param name="memoryStream">The memory stream.</param>
        /// <param name="claimId">The claim id.</param>
        public void CreateDocument(MemoryStream memoryStream, string caseId, string claimId, string filename)
        {
            //return;
            try
            {
                if (_ctx != null)
                {
                    var list = _ctx.Web.Lists.GetByTitle(LibraryName);
                    _ctx.Load(list);
                    string file = string.Format("{1:yyyy-MM-dd_hh-mm-ss-tt}_{0}", filename, DateTime.Now);
                    string uploadLocation = string.Format("{0}/{1}/{2}", "http://dev8spt", LibraryName.Replace(" ", ""), file);

                    FileCreationInformation fileCreationInformation = new FileCreationInformation();
                    fileCreationInformation.Content = memoryStream.ToArray();
                    fileCreationInformation.Overwrite = true;
                    fileCreationInformation.Url = uploadLocation;

                    FileCollection documentFiles = list.RootFolder.Files;
                    _ctx.Load(documentFiles);

                    Microsoft.SharePoint.Client.File newFile = documentFiles.Add(fileCreationInformation);
                    _ctx.Load(newFile);

                    ListItem docitem = newFile.ListItemAllFields;
                    _ctx.Load(docitem);

                    _ctx.ExecuteQuery();

                    docitem[CaseIdKey] = caseId;
                    docitem[PrimaryKey] = claimId;
                    docitem.Update();

                    _ctx.ExecuteQuery();
                }
                else
                {
                    throw new Exception("Context Doesn't Exsist");
                }
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                ConnectionClose();
            }
        }

        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <param name="memoryStream">The memory stream.</param>
        /// <param name="claimId">The claim id.</param>
        public void UpdateDocument(MemoryStream memoryStream, string caseId, string claimId, string filename)
        {
            //return;
            try
            {
                if (_ctx != null)
                {
                    var oldDocument = GetDocuments(claimId);
                    if (oldDocument == null || oldDocument.Count == 0)
                        return;

                    var list = _ctx.Web.Lists.GetByTitle(LibraryName);
                    _ctx.Load(list);
                    string sharepointFileName = oldDocument[0].DocumentName;//string.Format("{1:yyyy-MM-dd_hh-mm-ss-tt}_{0}", filename, DateTime.Now);
                    string uploadLocation = string.Format("{0}/{1}/{2}", "http://dev8spt", LibraryName.Replace(" ", ""), sharepointFileName);

                    FileCreationInformation fileCreationInformation = new FileCreationInformation();
                    fileCreationInformation.Content = memoryStream.ToArray();
                    fileCreationInformation.Overwrite = true;
                    fileCreationInformation.Url = uploadLocation;

                    FileCollection documentFiles = list.RootFolder.Files;
                    _ctx.Load(documentFiles);

                    Microsoft.SharePoint.Client.File newFile = documentFiles.Add(fileCreationInformation);
                    _ctx.Load(newFile);

                    ListItem docitem = newFile.ListItemAllFields;
                    _ctx.Load(docitem);

                    _ctx.ExecuteQuery();

                    docitem[CaseIdKey] = caseId;
                    docitem[PrimaryKey] = claimId;
                    docitem.Update();

                    _ctx.ExecuteQuery();
                }
                else
                {
                    throw new Exception("Context Doesn't Exsist");
                }
            }
            catch (Exception ee)
            {
                throw ee;
            }
            finally
            {
                ConnectionClose();
            }
        }

        #endregion
    }
}
