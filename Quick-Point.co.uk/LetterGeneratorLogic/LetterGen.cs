using Quick_Point.co.uk.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DocumentGeneratorLibrary;

namespace Quick_Point.co.uk.LetterGeneratorLogic
{
    public class LetterGen
    {
        private FormCollection _form;

        public byte[] Filestream { get; set; }
        public string LetterName { get; set; }

        public LetterGen(FormCollection form)
        {
            _form = form;
        }

        public void CreateDoc(string Type)
        {
            if(Type == "FlexibleFurlough") { FlexFurlGen(); LetterName = "Flexible Furlough Letter.docx"; };
            //...

            var mailMessage = new MailMessage();
            mailMessage.From = new
               MailAddress(Utils.GetConfigSetting("Fredemail"), "Quick Point Admin");
            mailMessage.To.Add(new
                 MailAddress(_form["Email"].ToString()));
            mailMessage.Subject = "Quick Point Generated Letter";
            mailMessage.Body = "Here is the letter you have just generated through www.quick-point.co.uk";
            mailMessage.IsBodyHtml = false;
            Attachment att = new Attachment(new MemoryStream(Filestream), LetterName);
            mailMessage.Attachments.Add(att);
            SmtpClient client = new SmtpClient();
            client.Credentials = new NetworkCredential(Utils.GetConfigSetting("Fredemail"), Utils.GetConfigSetting("fpw"));
            client.Port = 587;
            client.Host = "smtp.office365.com";
            client.EnableSsl = true;
            client.Send(mailMessage);
        }
        public void FlexFurlGen()
        {
            var docgen = new DocGen();
            Filestream = docgen.CreateDocument(_form);
        }

         /*public void CreateDocument(FormCollection _form)
            {
                string workingdays = GetWorkingDays(_form);
                //////
                ///

                Document document = new Document();
                Section section = document.AddSection();

                MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToFile(document);
                string to_para = _form["RecipientName"].ToString() + "\n" + _form["RecipientAddress1"].ToString() + "\n" + _form["RecipientAddress2"].ToString() + "\n" + _form["RecipientAddress3"].ToString() + "\n" + _form["RecipientPostCode"].ToString() + "\n";
                string from_para = _form["SenderName"].ToString() + "\n" + _form["SenderName"].ToString() + "\n" + _form["SenderAddress1"].ToString() + "\n" + _form["SenderAddress2"].ToString() + "\n" + _form["SenderAddress3"].ToString() + "\n" + _form["SenderPostCode"].ToString() + "\n";

                Paragraph to_paragraph = section.AddParagraph();
                to_paragraph.Format.Alignment = ParagraphAlignment.Right;
                FormattedText ft = to_paragraph.AddFormattedText(to_para);
                ft.Font.Size = 6;

                section.AddParagraph(from_para);

                PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(false);
                pdfRenderer.Document = document;
                pdfRenderer.RenderDocument();
                MemoryStream stream = new MemoryStream();
                pdfRenderer.PdfDocument.Save(stream, false);
                /////

                Filestream = stream;

                /*
                using (FileStream fs = File.OpenRead("FlexibleFurlough.docx"))
                {
                    var binaryReader = new BinaryReader(fs);
                    fileData = binaryReader.ReadBytes((int)fs.Length);
                }
                filebytes = fileData;

                object oMissing = System.Reflection.Missing.Value;
                object oEndOfDoc = "\\endofdoc"; /* \endofdoc is a predefined bookmark 

                //Start Word and create a new document.
                Microsoft.Office.Interop.Word._Application oWord;
                Microsoft.Office.Interop.Word._Document oDoc;
                oWord = new Microsoft.Office.Interop.Word.Application();
                oWord.Visible = true;
                oDoc = oWord.Documents.Add(ref oMissing, ref oMissing,
                ref oMissing, ref oMissing);

                //Insert a paragraph at the beginning of the document.
                Microsoft.Office.Interop.Word.Paragraph to_section;
                to_section = oDoc.Content.Paragraphs.Add(ref oMissing);
                to_section.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
                to_section.Range.Font.Bold = 0;
                to_section.Format.SpaceAfter = 12;    //24 pt spacing after paragraph.
                to_section.Range.Text = _form["RecipientName"].ToString() + "\n" + _form["RecipientAddress1"].ToString() + "\n" + _form["RecipientAddress2"].ToString() + "\n" + _form["RecipientAddress3"].ToString() + "\n" + _form["RecipientPostCode"].ToString() + "\n";
                to_section.Range.InsertParagraph();

                //Insert a paragraph at the beginning of the document.
                Microsoft.Office.Interop.Word.Paragraph from_section;
                from_section = oDoc.Content.Paragraphs.Add(ref oMissing);
                from_section.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                from_section.Range.Font.Bold = 0;
                from_section.Format.SpaceAfter = 12;    //24 pt spacing after paragraph.
                from_section.Range.Text = _form["SenderName"].ToString() + "\n" + _form["SenderName"].ToString() + "\n" + _form["SenderAddress1"].ToString() + "\n" + _form["SenderAddress2"].ToString() + "\n" + _form["SenderAddress3"].ToString() + "\n" + _form["SenderPostCode"].ToString() + "\n";
                from_section.Range.InsertParagraphAfter();

                //Insert a paragraph at the beginning of the document.
                Microsoft.Office.Interop.Word.Paragraph dear;
                dear = oDoc.Content.Paragraphs.Add(ref oMissing);
                dear.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                dear.Range.Font.Bold = 0;
                dear.Format.SpaceAfter = 12;    //24 pt spacing after paragraph.
                dear.Range.Text = "Dear " + _form["RecipientName"].ToString();
                dear.Range.InsertParagraphAfter();

                //Insert a paragraph at the beginning of the document.
                Microsoft.Office.Interop.Word.Paragraph title;
                title = oDoc.Content.Paragraphs.Add(ref oMissing);
                title.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                title.Range.Font.Bold = 1;
                title.Format.SpaceAfter = 12;    //24 pt spacing after paragraph.
                title.Range.Text = "FLEXIBLE FURLOUGH AGREEMENT LETTER";
                title.Range.InsertParagraphAfter();

                //Insert a paragraph at the beginning of the document.
                Microsoft.Office.Interop.Word.Paragraph content;
                content = oDoc.Content.Paragraphs.Add(ref oMissing);
                content.Range.Font.Bold = 0;
                content.Format.SpaceAfter = 12;    //24 pt spacing after paragraph.
                content.Range.Text = "The COVID-19 pandemic has adversely affected operations of " + _form["SenderName"].ToString() + "," +
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
                    "-----------------------------------------------------------------------------------------------------------------------" +
                    "\n I agree to the adjustment of my contract for the period of my furlough leave, which I acknowledge began on " + _form["StartDate"].ToString() + " and will continue until I am notified of an end date. I understand that I cannot undertake any work for " + _form["SenderName"].ToString() + ". " +
                    "\n \n Signed _______________________________" +
                    "\n \n Name _________________________________" +
                    "\n \n Date _________________________________";
                content.Range.InsertParagraphAfter();

                oDoc.SaveAs2("FlexibleFurlough.docx");


            }*/

     

    }
}