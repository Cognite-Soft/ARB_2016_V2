using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cognite.Arb.Web.Core
{
    public static class Extensions
    {
        public static string GetBooleanYesNoText(this bool value)
        {
            return value ? GlobalStrings.BooleanTrueText : GlobalStrings.BooleanFalseText;
        }
    }
}