using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace _4PsPH.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetCityId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("CityId");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetCityName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("CityName");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetFullName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("FullName");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}