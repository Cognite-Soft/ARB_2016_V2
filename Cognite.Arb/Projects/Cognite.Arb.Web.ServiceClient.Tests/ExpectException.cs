using System;
using NUnit.Framework;

namespace Cognite.Arb.Web.ServiceClient.Tests
{
    public static class ExpectException
    {
        public static void From<T>(Action action)
        {
            try
            {
                action();
                Assert.Fail("Expected Exception " + typeof (T));
            }
            catch (Exception ex)
            {
                if (!(ex is T)) Assert.Fail("Expected Exception " + typeof(T) + " but was " + ex.GetType());
            }
        }
    }
}
