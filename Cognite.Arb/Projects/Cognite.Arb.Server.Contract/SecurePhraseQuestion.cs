﻿using System.Net;

namespace Cognite.Arb.Server.Contract
{
    public class SecurePhraseQuestion
    {
        public static HttpStatusCode HttpStatusCode = HttpStatusCode.OK;

        public string SecurityToken { get; set; }
        public int FirstCharacterIndex { get; set; }
        public int SecondCharacterIndex { get; set; }
    }
}
