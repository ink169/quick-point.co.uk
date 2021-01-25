using Aspose.Words;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DocumentGeneratorLibrary
{
    public class DocGen
    {
        public byte[] CreateDocument(FormCollection _form)
        {
            //declare content
            string workingdays = GetWorkingDays(_form);
            string to_para = _form["RecipientName"].ToString() + "\n" + _form["RecipientAddress1"].ToString() + "\n" + _form["RecipientAddress2"].ToString() + "\n" + _form["RecipientAddress3"].ToString() + "\n" + _form["RecipientPostCode"].ToString() + "\n";
            string from_para = _form["SenderName"].ToString() + "\n" + _form["SenderName"].ToString() + "\n" + _form["SenderAddress1"].ToString() + "\n" + _form["SenderAddress2"].ToString() + "\n" + _form["SenderAddress3"].ToString() + "\n" + _form["SenderPostCode"].ToString() + "\n";
            string dear = "Dear " + _form["RecipientName"].ToString() +"\n \n";
            string title = "FLEXIBLE FURLOUGH AGREEMENT LETTER";
            string body = "The COVID-19 pandemic has adversely affected operations of " + _form["SenderName"].ToString() + "," +
                    " and a lot of employees (including you) were placed on furlough leave, which allows employers to claim a grant of 80%" +
                    " of furloughed workers’ pay for the time they are on furlough leave." + "\n" + "\n" +
                    "As we have previously discussed, you will be placed on flexible furlough leave. I am writing to you to officially request your consent." + "\n" + "\n" +
                    "Although it has not been an easy decision for " + _form["SenderName"].ToString() + ", we decided that this would be the most optimal response for " + _form["SenderName"].ToString() + " to take in light of the pandemic." +
                    " In this way, we will be able to avoid redundancies and keep our business going. " + "\n" + "\n" +
                    "We propose following changes: " + "\n" + "\n" +
                    "1. You will be placed on flexible furlough leave from " + _form["StartDate"].ToString() + ". \n" +
                    "2.You will be working " + _form["WorkingDays"].ToString() + " days per week and will be on furlough leave " + _form["FurloughDays"].ToString() + ". \n" +
                    "3. You working days will be " + workingdays + ". \n" +
                    "4. On your working days you will be requested to work your normal hours: " + _form["WorkingHours"].ToString() + ". \n \n " +
                    "We currently intend that you will remain on flexible furlough leave until " + _form["ReturnDate"].ToString() + ".  This period may need to be extended and, if so, we will discuss this with you. We will give you reasonable notice should we require you to end your leave and resume full work and will keep you updated on the situation. " +
                    "\n \n " +
                    "In order to place you on flexible furlough leave, it is necessary to adjust the terms of your contract with " + _form["SenderName"].ToString() + " . The proposed changes are as follows:" +
                    "\n 1.	For the time of your furlough leave, you are not permitted to carry out any work for " + _form["SenderName"].ToString() + ". \n " +
                    "2.	During your period of flexible furlough leave, you will be paid a gross sum of  " + _form["GrossPay"].ToString() + ", which is the sum the business is able to reclaim for your pay under the Coronavirus Job Retention Scheme. This sum will be subject to any applicable deductions for tax, employee national insurance contributions and employee pension contributions. If it is feasible, payments will be made on your regular paydays. " +
                    "\n \n" +
                    "If you agree to the terms set out in the present letter, please respond to " + _form["SenderEmail"].ToString() + " with a signed copy of this letter. " +
                    "Once signed, the changes set out in this letter will be effective from " + _form["StartDate"].ToString() + "." +
                    "\n \n We understand that the news may be rather unfortunate, we ask you to remember that this is the best response for our company in light of the crisis. Do not hesitate to contact me should you have any questions or issues. " +
                    "\n Yours sincerely," +
                    "\n " + _form["SenderFirstName"].ToString() + "\n"
                    + _form["SenderPosition"].ToString() + "\n"
                    + _form["SenderName"].ToString() + "\n \n " +
                    "--------------------------------------------------------------------------------------------------------------------" +
                    "\n I agree to the adjustment of my contract for the period of my furlough leave, which I acknowledge began on " + _form["StartDate"].ToString() + " and will continue until I am notified of an end date. I understand that I cannot undertake any work for " + _form["SenderName"].ToString() + ". " +
                    "\n \n Signed _______________________________" +
                    "\n \n Name _________________________________" +
                    "\n \n Date _________________________________";

            Aspose.Words.License license = new Aspose.Words.License();
            try
            {
                license.SetLicense("../assets/Aspose.Words.NET.lic");
                Console.WriteLine("License set successfully.");
            }
            catch (Exception e)
            {
                // We do not ship any license with this example, visit the Aspose site to obtain either a temporary or permanent license. 
                Console.WriteLine("\nThere was an error setting the license: " + e.Message);
            }

            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Specify font formatting
            Font font = builder.Font;
            font.Size = 12;
            font.Bold = false;
            font.Name = "Arial";

            Style alignright = builder.Document.Styles.Add(StyleType.Paragraph, "alignright");
            alignright.ParagraphFormat.Alignment = ParagraphAlignment.Right;

            builder.ParagraphFormat.StyleName = alignright.Name;
            builder.Write(to_para);
            builder.InsertStyleSeparator();

            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.BodyText;
            builder.Write(from_para);
            builder.Write(dear);
            builder.InsertStyleSeparator();

            Style titlep = builder.Document.Styles.Add(StyleType.Paragraph, "titlep");
            titlep.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            titlep.Font.Bold = true;

            builder.ParagraphFormat.StyleName = titlep.Name;
            builder.Write(title + "\n \n");
            builder.InsertStyleSeparator();
            
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.BodyText;
            builder.Write(body);

            var ms = new MemoryStream();
            doc.Save(ms, SaveFormat.Pdf);
            byte[] data = ms.ToArray();
            return data;
        }

        public string GetWorkingDays(FormCollection _form)
        {
            var list = new List<string>();
            if (Convert.ToBoolean(_form["Monday"].Split(',')[0])) { list.Add("Monday"); }
            if (Convert.ToBoolean(_form["Tuesday"].Split(',')[0])) { list.Add("Tuesday"); }
            if (Convert.ToBoolean(_form["Wednesday"].Split(',')[0])) { list.Add("Wednesday"); }
            if (Convert.ToBoolean(_form["Thursday"].Split(',')[0])) { list.Add("Thursday"); }
            if (Convert.ToBoolean(_form["Friday"].Split(',')[0])) { list.Add("Friday"); }
            if (Convert.ToBoolean(_form["Saturday"].Split(',')[0])) { list.Add("Saturday"); }
            if (Convert.ToBoolean(_form["SUnday"].Split(',')[0])) { list.Add("Sunday"); }

            string workingdays = "";
            foreach (string day in list) { workingdays = workingdays + day.ToString() + ", "; }
            return workingdays;
        }

    }
}
