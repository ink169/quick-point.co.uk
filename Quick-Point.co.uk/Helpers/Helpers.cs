using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Quick_Point.co.uk.Helpers
{
    public static class ArticleHelpers
    {
        private static Dictionary<string, string> _articleDictionary;

        static ArticleHelpers() {
            _articleDictionary = new Dictionary<string, string>();

            _articleDictionary.Add("TaxAccArticle1", "COVID-19 and claiming benefits");
            _articleDictionary.Add("TaxAccArticle2", "COVID-19: Support for Businesses");
            _articleDictionary.Add("TaxAccArticle3", "Companies House deadlines and Coronavirus");
            _articleDictionary.Add("TaxAccArticle4", "What is the Coronavirus Job Retention Scheme?");
            _articleDictionary.Add("TaxAccArticle5", "Deferral of VAT payments update");
            _articleDictionary.Add("TaxAccArticle6", "Self-employed Income Support Scheme");
            _articleDictionary.Add("TaxAccArticle7", "Profit Extraction Strategies & Tax Efficiency");
            _articleDictionary.Add("TaxAccArticle8", "What expenses can I claim through my limited company?");
            _articleDictionary.Add("TaxAccArticle9", "Growing your SME business during a global pandemic");
            _articleDictionary.Add("TaxAccArticle10", "Starting a business during Coronavirus");
            _articleDictionary.Add("TaxAccArticle11", "How to effectively manage your staff remotely");
            _articleDictionary.Add("TaxAccArticle12", "The Key to a Successful Zoom Meeting");
        }

        public static string GetTitle(string key)
        {
            string value;

            if (_articleDictionary.TryGetValue(key, out value))
                return value;
            else
                return String.Empty;

        }

    }
}