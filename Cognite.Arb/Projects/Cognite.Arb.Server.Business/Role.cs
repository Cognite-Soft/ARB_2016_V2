namespace Cognite.Arb.Server.Business.Database
{
    public enum Role
    {
        None = 0,
        System = 1,
        Admin = 2,
        CaseWorker = 4,
        PanelMember = 8,
        Inquirer = 16,
        Solicitor = 32,
        ThirdPartyReviewer = 64,
    }
}
