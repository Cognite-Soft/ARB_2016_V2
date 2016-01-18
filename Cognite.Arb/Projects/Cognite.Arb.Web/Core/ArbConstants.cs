using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cognite.Arb.Web.Core
{
    public static class ArbConstants
    {
        public static string SessionPasswordForBothReset = "cognite.arb.session.pwd";
        public static string RedirectCookieName = "cognite.arb.cookies.redirect";
        public static string AuthenticationStepCookieName = "cognite.arb.cookies.auth.step";

        public static int ScheduleRowsCount = 10;
        public static int ScheduleRowCapacity = 3;

        public static string AssignCaseErrorTempDataKey = "AssignCaseError";
        public static string SecurePhraseQuestionTempDataKey = "SequrePhraseQuestion";
        public static string ComplaintModelTempDataKey = "ComplaintModel";

        public static string DatesAndDetailsSessionKey = "DatesAndDetails";
        public static string AllegationSessionKey = "Allegations";
        public static string AllegationsToDeleteSessionKey = "AllegationsDelete";
        public static string DateAndDetailsToDeleteSessionKey = "DateAndDetailsDelete";
        public static string SessionSynchronizationId = "ArbSessionSynchronization";
        public static string SessionCurrentCaseKey = "CurrentCaseId";

        public static string WebApiServiceAddress = "webapiaddress";
        public static string TempDataErrorKey = "TempDataError";

    }
}