using System;

namespace Cognite.Arb.Server.Business
{
    public interface IConfiguration
    {
        TimeSpan ResetPasswordTokenLifespan { get; }
        TimeSpan ResetSecurePhraseTokenLifespan { get; set; }
        TimeSpan ResetUserTokenLifespan { get; set; }
        byte[] SecurePhraseEncryptionKey { get; }
        byte[] SecurePhraseIv { get; }
    }
}
