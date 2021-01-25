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
        public byte[] CreateRetToWorkDocument(FormCollection _form)
        {
            //declare content
            string to_para = _form["RecipientName"].ToString() + "\n" + _form["RecipientAddress1"].ToString() + "\n" + _form["RecipientAddress2"].ToString() + "\n" + _form["RecipientAddress3"].ToString() + "\n" + _form["RecipientPostCode"].ToString() + "\n";
            string from_para = _form["SenderName"].ToString() + "\n" + _form["SenderName"].ToString() + "\n" + _form["SenderAddress1"].ToString() + "\n" + _form["SenderAddress2"].ToString() + "\n" + _form["SenderAddress3"].ToString() + "\n" + _form["SenderPostCode"].ToString() + "\n" + _form["TodaysDate"].ToString() + "\n";
            string dear = "Dear " + _form["RecipientName"].ToString() + "\n \n";
            string title = "FURLOUGH LEAVE – RETURNING TO WORK \n";
            string body = "We would like to express our gratitude to you for previously agreeing to being placed on furlough leave. \n\n" +
                "We are writing to inform you that we are happy to be able to end your period of furlough. The end of your furlough is on "+_form["EndDate"].ToString()+ " and you should return to work on " + _form["ReturnDate"].ToString() + ". " +
                "\n\n Please note that from the date your furlough leave ends, you are required to return to your usual work duties, on your normal work hour although you may be required to work from home. You will receive full pay from the date your return to work. Please let us know promptly if you are unable to return to work on that date. \n\n" +
                "Please be advised that depending on the COVID-19 situation, you might be required to go back on furlough leave later. \n\n" +
                "If you have any queries about returning to work, please contact " + _form["QueryContact"].ToString() + ". \n\n" +
                "To confirm you understand and agree to return to work, please respond to "+ _form["ReturnEmail"].ToString() + " with a signed copy of this letter. \n";

            string bottom = "\n Yours sincerely," +
                    "\n " + _form["SenderFirstName"].ToString() + "\n"
                    + _form["SenderPosition"].ToString() + "\n"
                    + _form["SenderName"].ToString() + "\n \n " +
                    "--------------------------------------------------------------------------------------------------------------------" +
                    "\nI confirm that I understand that I will no longer be a furloughed worker from " + _form["EndDate"].ToString() + " and I am able to return to work on my usual terms of employment on " + _form["ReturnDate"].ToString() + ". " +
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
            builder.Write(bottom);



            var ms = new MemoryStream();
            doc.Save(ms, SaveFormat.Pdf);
            byte[] data = ms.ToArray();
            return data;
        }
        public byte[] CreateFurlLeaveDocument(FormCollection _form)
        {
            //declare content
            string to_para = _form["RecipientName"].ToString() + "\n" + _form["RecipientAddress1"].ToString() + "\n" + _form["RecipientAddress2"].ToString() + "\n" + _form["RecipientAddress3"].ToString() + "\n" + _form["RecipientPostCode"].ToString() + "\n";
            string from_para = _form["SenderName"].ToString() + "\n" + _form["SenderName"].ToString() + "\n" + _form["SenderAddress1"].ToString() + "\n" + _form["SenderAddress2"].ToString() + "\n" + _form["SenderAddress3"].ToString() + "\n" + _form["SenderPostCode"].ToString() + "\n";
            string dear = "Dear " + _form["RecipientName"].ToString() + "\n \n";
            string title = "FURLOUGH LEAVE AGREEMENT LETTER";
            string body = "Due to the fact that COVID-19 pandemic has adversely affected operations of " + _form["SenderName"].ToString() + ", we have come to a decision to apply to the Coronavirus Job Retention Scheme. This is a governmental scheme which allows employers to place staff on ‘furlough leave’ and claim a grant of 80% of furloughed workers’ pay (up to a maximum of £2,500 per month). \n \n" +
                "Following our discussion, I am writing to officially ask your consent to being placed on furlough leave. \n \n" +
                "Although it has not been an easy decision for " + _form["SenderName"].ToString() + ", we decided that this would be the most optimal response for " + _form["SenderName"].ToString() + " to take in light of the pandemic. In this way, we will be able to avoid redundancies and keep our business going. \n \n" +
                "We propose to place you on furlough leave from " + _form["StartDate"].ToString() + " and currently intend that you will remain on furlough leave until " + _form["ReturnDate"].ToString() + ".  This period may need to be extended and, if so, we will discuss this with you. We will give you reasonable notice should we require you to end your leave and resume work and will keep you updated on the situation. \n \n " +
                "In order to place you on furlough leave, it is necessary to adjust the terms of your contract with " + _form["SenderName"].ToString() + ". The proposed changes are as follows: \n \n" +
                " 1.	For the time of your furlough leave, you are not permitted to carry out any work for " + _form["SenderName"].ToString() + ". \n" +
                "2.	During your period of furlough leave, you will be paid a gross sum of " + _form["GrossPay"].ToString() + ", which is the sum the business is able to reclaim for your pay under the Coronavirus Job Retention Scheme. This sum will be subject to any applicable deductions for tax, employee national insurance contributions and employee pension contributions. If it is feasible, payments will be made on your regular paydays. \n \n" +
                "Unfortunately, we are not in the position to top up your wage to the full amount but you will be informed if it changes. \n\n" +
                "Aside from the adjustments above, the contract remains unchanged.  \n\n" +
                "Your furlough leave will have the following conditions: \n\n" +
                "1.	You should remain available to return to work on short notice should we require so. \n\n" +
                "2.	During your leave, you may undertake volunteer work and take training that is directly relevant to your job.\n\n" +
                "3.	Under the Scheme, furloughed staff are entitled to take annual leave during furlough leave, to be paid at their usual rate of pay. \n\n";

            string bottom = "If you agree to the terms set out in the present letter, please respond to " + _form["SenderEmail"].ToString() + " with a signed copy of this letter. " +
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
            builder.Write(bottom);



            var ms = new MemoryStream();
            doc.Save(ms, SaveFormat.Pdf);
            byte[] data = ms.ToArray();
            return data;
        }

        public byte[] CreateFlexFurlDocument(FormCollection _form)
        {
            //declare content
            string workingdays = GetWorkingDays(_form);
            string to_para = _form["RecipientName"].ToString() + "\n" + _form["RecipientAddress1"].ToString() + "\n" + _form["RecipientAddress2"].ToString() + "\n" + _form["RecipientAddress3"].ToString() + "\n" + _form["RecipientPostCode"].ToString() + "\n";
            string from_para = _form["SenderName"].ToString() + "\n" + _form["SenderName"].ToString() + "\n" + _form["SenderAddress1"].ToString() + "\n" + _form["SenderAddress2"].ToString() + "\n" + _form["SenderAddress3"].ToString() + "\n" + _form["SenderPostCode"].ToString() + "\n";
            string dear = "Dear " + _form["RecipientName"].ToString() + "\n \n";
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
        public byte[] CreateReFurlDocument(FormCollection _form)
        {
            //declare content
            string to_para = _form["RecipientName"].ToString() + "\n" + _form["RecipientAddress1"].ToString() + "\n" + _form["RecipientAddress2"].ToString() + "\n" + _form["RecipientAddress3"].ToString() + "\n" + _form["RecipientPostCode"].ToString() + "\n";
            string from_para = _form["SenderName"].ToString() + "\n" + _form["SenderName"].ToString() + "\n" + _form["SenderAddress1"].ToString() + "\n" + _form["SenderAddress2"].ToString() + "\n" + _form["SenderAddress3"].ToString() + "\n" + _form["SenderPostCode"].ToString() + "\n";
            string dear = "Dear " + _form["RecipientName"].ToString() + "\n \n";
            string title = "FURLOUGH LEAVE AGREEMENT LETTER\n\n";
            string body = "Due to the fact that COVID-19 pandemic has adversely affected operations of " + _form["StartDate"].ToString() + ", we have come to a decision to apply to the Coronavirus Job Retention Scheme. This is a governmental scheme which allows employers to place staff on ‘furlough leave’ and claim a grant of 80% of furloughed workers’ pay (up to a maximum of £2,500 per month).  " +
                "\n\nYou have already been placed to the furlough leave which ended " + _form["LReturnDate"].ToString() + " and you resumed work on " + _form["LStartDate"].ToString() + ". As we have previously discussed , you will be placed on furlough again. " +
                "\n\nFollowing our discussion, I am writing to officially ask your consent to being placed on furlough leave." +
                "\n\nAlthough it has not been an easy decision for " + _form["SenderName"].ToString() + ", we decided that this would be the most optimal response for " + _form["SenderName"].ToString() + " to take in light of the pandemic. In this way, we will be able to avoid redundancies and keep our business going. " +
                "\n\n We propose to place you on furlough leave from " + _form["StartDate"].ToString() + " and currently intend that you will remain on furlough leave until " + _form["ReturnDate"].ToString() + ".  This period may need to be extended and, if so, we will discuss this with you. We will give you reasonable notice should we require you to end your leave and resume work and will keep you updated on the situation. " +
                "In order to place you on furlough leave, it is necessary to adjust the terms of your contract with " + _form["SenderName"].ToString() + ". The proposed changes are as follows:" +
                "\n\n 1.	For the time of your furlough leave, you are not permitted to carry out any work for " + _form["SenderName"].ToString() + ". " +
                "\n\n " +
                "2.	During your period of furlough leave, you will be paid a gross sum of " + _form["GrossPay"].ToString() + ", which is the sum the business is able to reclaim for your pay under the Coronavirus Job Retention Scheme. This sum will be subject to any applicable deductions for tax, employee national insurance contributions and employee pension contributions. If it is feasible, payments will be made on your regular paydays. " +
                "\n\n Unfortunately, we are not in the position to top up your wage to the full amount but you will be informed if it changes." +
                "\n\n Aside from the adjustments above, the contract remains unchanged. " +
                "\n\n Your furlough leave will have the following conditions: " +
                "\n\n 1.	You should remain available to return to work on short notice should we require so. " +
                "\n\n 2.	During your leave, you may undertake volunteer work and take training that is directly relevant to your job." +
                "\n\n 3.	Under the Scheme, furloughed staff are entitled to take annual leave during furlough leave, to be paid at their usual rate of pay. " +
                "\n\n If you agree to the terms set out in the present letter, please respond to " + _form["ReturnEmail"].ToString() + " with a signed copy of this letter. " +
                    "\n\nOnce signed, the changes set out in this letter will be effective from " + _form["StartDate"].ToString() + "." +
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
        public byte[] CreateTempFurlDocument(FormCollection _form)
        {
            string benefits;
            try {
                if (_form["Benefits"].ToString() != null || _form["Benefits"].ToString() == "")
                {
                    benefits = "plus the following additional benefits, " + _form["Benefits"].ToString();
                }
                else
                {
                    benefits = ".";
                }
            }
            catch
            {
                benefits = " ";
            }


            //declare content
            string title = "Letter confirming an agreement to temporary furlough";
            string body = "Dear " + _form["RecipientName"].ToString() + ", \n\n" +
                    "As discussed with " + _form["Manager"].ToString() + " on " + _form["DiscussionDate"].ToString() + ", we will place you ‘on furlough’.  \n\n" +
                    "This means that you will still be employed by us (although at a lower rate of pay).  You will not do any work for us during the furlough period. We can then use the Government's Coronavirus Job Retention Scheme, which covers 80% of your normal pay, up to a maximum of £2,500 per month.\n\n" +
                    "In this way, we hope to keep the business going and avoid redundancies if possible until matters get back to normal.  \n\n" +
                    "If you agree to be placed on furlough, your contract of employment will be temporarily varied. You will need to sign to confirm your agreement to the variation in the section at the end of this letter headed “confirmation of agreement” and return a copy to us. We are sending two copies of this letter so that you can keep one for your records. Unless we agree otherwise and unless your contract of employment is terminated by you or by us before that date, the temporary variation will come to an end on the date when you return to normal work.\n\n" +
                    "Your period of furlough will begin on " + _form["StartDate"].ToString() + ". It will last for at least three weeks and may last up to three months. After three weeks, we will keep the situation under review. The three months may need to be extended and, if so, we will discuss this with you. As soon as we think we can get you back to work as normal, we will give you notice and will expect you to return to work immediately unless agreed otherwise. \n\n" +
                    "Please confirm your contact details in the section at the bottom of this letter so that we can keep in touch. \n\n" +
                    "To summarise, this is how furlough will work: \n \n" +
                    "1.	Based on your weekly wage/salary, while on furlough, we will pay you " + _form["WeeklyPay"].ToString() + ". This amounts to 80% of your wage/salary, or £2,500 (maximum). This amount is subject to deductions for tax and national insurance in the usual way." +
                    "\n\n 2.	In addition to that wage/salary, we will pay employer national insurance contributions and minimum automatic enrolment employer pension contributions on that wage/salary." +
                    "\n\n 3.	Your contract of employment will continue with " + _form["SenderName"].ToString() + ", but the terms of the Job Retention Scheme require that you do not do any work for us during the furlough period.  " +
                    "\n\n 4.	While your statutory rights are unaffected by this variation to your contract of employment, your contractual entitlements to pay and other financial benefits during the furlough period are limited to those in points 1 and 2 " + benefits + "." +
                    "\n\n If you agree to this temporary variation, please sign and date below and return a signed copy of the letter to " + _form["Manager"].ToString() + " by " + _form["ReturnDeadline"].ToString() + ".  " +
                    "\n\n If you have any questions about your entitlement to annual leave or any other of your rights or entitlements during the period of furlough, please direct those questions to " + _form["Manager"].ToString() + "." +
                    "\n\n Yours sincerely \n\n" +
                    "" + _form["SenderName"].ToString() + "\n\n" +
                    "-------------------------------------------------------------------------------------------------------------------- \n\n" +
                    "Confirmation of agreement \n\n" +
                    "We agree that the contract of employment between " + _form["RecipientName"].ToString() + " and " + _form["SenderName"].ToString() + " will be temporarily varied and that " + _form["RecipientName"].ToString() + " will be placed on furlough on the terms set out in this letter.  " +
                    "\n \n Employee/Worker" +
                    " \nSigned _______________________________" +
                    "\n \n Name _________________________________" +
                    "\n \n Date _________________________________" +
                     "\n \n Employer" +
                    "\n Signed _______________________________" +
                    "\n \n Name _________________________________" +
                    "\n \n Date _________________________________"
                    + "\n \n Employee Contact Details" +
                    "\n\n Tel __________________" +
                    "\n\n Email ________________" +
                    "\n\n Address ___________________";



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
