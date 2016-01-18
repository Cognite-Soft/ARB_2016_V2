namespace Cognite.Arb.Server.Business.Mailing
{
    public interface IMailSender
    {
        bool SendMail(IndividualMail mail);
    }
}
