namespace Cognite.Arb.WebApi.Resource.Documents
{
    public class Constant
    {
        #region //Public Properties

        public static string GetDocumentsQuery =
            "<View><Query><Where><Eq><FieldRef Name='AssociationId'/><Value Type='Text'>{0}</Value></Eq></Where></Query></View>";

        public static string FileName = "FileLeafRef";
        public static string DocumentId = "ID";

        public static string LibraryNameKey = "LibraryName";
        public static string KeyColoumn = "PrimaryKey";
        public static string CaseIdColoumn = "CaseIdKey";
        public static string SharepointUrl = "ServerUrl";

        #endregion
    }
}
