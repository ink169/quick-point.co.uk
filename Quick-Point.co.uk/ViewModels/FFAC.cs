using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quick_Point.co.uk.ViewModels
{
    public class FFAC
    {
        public BusinessType Business { get; set; }

        public static IEnumerable<SelectListItem> GetSelectItems()
        {
            yield return new SelectListItem { Text = "LTD", Value = "LTD" };
            yield return new SelectListItem { Text = "Sole Trader", Value = "Sole Trader" };
            yield return new SelectListItem { Text = "LLP", Value = "LLP" };
        }
       
        public bool Bookkeeping { get; set; }
        public bool Payroll { get; set; }
        public bool CompaniesHouseReturns { get; set; }
        public bool SelfAssessment { get; set; }
        public bool VATReturns { get; set; }
        public bool AccountsManagement { get; set; }
        public bool BusinessConsultation { get; set; }
        public bool TaxationAdvice { get; set; }






    }

    public enum BusinessType
    {
        SoleTrader,
        LTD,
        LLP
    }

}