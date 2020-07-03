using System;

namespace Quick_Point.co.uk.ViewModels
{
    public class RequestArticle
    {
        public string ArticleID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string BaseUrl { get; set; }

        public string TaxOrLegal { get; set; }

        public string AgreedTC { get; set; }

        public DateTime? AgreedDate { get; set; }

    }
}