using System;
using System.Configuration;
using System.Net;
using Microsoft.SharePoint.Client;

namespace Cognite.Arb.WebApi.Resource.Documents
{
    public abstract class BaseRepository
    {
        #region //Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository"/> class.
        /// </summary>
        protected BaseRepository()
        {
            LibraryName = ConfigurationManager.AppSettings[Constant.LibraryNameKey];
            if (LibraryName == string.Empty)
            {
                throw new Exception("Library Name doesn't Exsists in config");
            }
            
            PrimaryKey = ConfigurationManager.AppSettings[Constant.KeyColoumn];
            if (PrimaryKey == string.Empty)
            {
                throw new Exception("Primary Key doesn't Exsists in config");
            }

            CaseIdKey = ConfigurationManager.AppSettings[Constant.CaseIdColoumn];
            if (CaseIdKey == string.Empty)
            {
                throw new Exception("CaseId Key doesn't Exsists in config");
            }
            
            Url = ConfigurationManager.AppSettings[Constant.SharepointUrl];
            if (Url == string.Empty)
            {
                throw new Exception("URL doesn't Exsists in config");
            }
            else
            {
                ConnectionOpen(Url);
            }

        }
        #endregion

        #region //protected  Properties
        /// <summary>
        /// Context
        /// </summary>
        protected ClientContext _ctx { get; set; }
        /// <summary>
        /// Library name
        /// </summary>
        protected string LibraryName { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        protected string Url { get; set; }

        /// <summary>
        /// Gets or sets the primary key.
        /// </summary>
        /// <value>
        /// The primary key.
        /// </value>
        protected string PrimaryKey { get; set; }

        protected string CaseIdKey { get; set; }

        #endregion

        #region //Private Methods
        /// <summary>
        /// Connections the open.
        /// </summary>
        /// <param name="url">The URL.</param>
        private void ConnectionOpen(string url)
        {
           // return;
            try
            {
                using (_ctx = new ClientContext(url))
                {
                    _ctx.Credentials = new NetworkCredential("CogniteApp", "Runtime1","HALLAMSTREET");
                    Web web = _ctx.Web;
                    //_ctx.Load(web, w => w.ServerRelativeUrl);
                    //_ctx.Load(web, w => w.Created);
                    _ctx.Load(web);
                    _ctx.ExecuteQuery();
                }
            }
            catch (Exception)
            {
                throw new Exception("Login Failed");
            }
        }

        #endregion

        #region //protected methods
        /// <summary>
        /// Connections the close.
        /// </summary>
        protected void ConnectionClose()
        {
           // return;
            try
            {
                _ctx.Dispose();
            }
            catch (Exception)
            {
                throw new Exception("Context Doesn't Exsist");
            }
        }

        #endregion
    }
}
