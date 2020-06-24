using Microsoft.SharePoint.BusinessData.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quick_Point.co.uk.ViewModels
{
    public class ResourceViewModel
    {
        public string BaseUrl { get; set; }
    }

    public class RequestArticleViewModel
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string BaseUrl { get; set; }

        public string TaxOrLegal { get; set; }

        public string AgreedTC { get; set; }

        public DateTime? AgreedDate { get; set; }

    }
}