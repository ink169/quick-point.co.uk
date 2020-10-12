// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Net.Http;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using AdaptiveCards;
using System.Net.Mail;
using System.Net;
using QnABot;

namespace Microsoft.BotBuilderSamples
{
    public class QnABot : ActivityHandler
    {

        private List<String> _ids;
        public QnABot(IConfiguration configuration, ILogger<QnABot> logger, IHttpClientFactory httpClientFactory, IBotTelemetryClient telemetryClient)
        {
             var TelemetryClient = telemetryClient;
            _configuration = configuration;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _ids = new List<String>();
        }

        public int i = 0;
        

        private Microsoft.Bot.Schema.Attachment wrongAnswerCard()
        {
            var card = new AdaptiveCard("1.0");
            card.Body.Add(new AdaptiveTextBlock() { Text = "If you would like your question answered personally, our team will be happy to reply your question via live consultation via email or call, please fill in your details and we will be in touch, otherwise continue to ask a question", Size = AdaptiveTextSize.Medium, Wrap = true });
            //card.Body.Add(new AdaptiveTextBlock() { Text = "If you would like a member of the team to answer your question personally, please enter your email and question", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder, Wrap = true });
            card.Body.Add(new AdaptiveTextBlock() { Text = "Your Email:", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveTextInput() { Style = AdaptiveTextInputStyle.Text, Id = "Email" });
            card.Body.Add(new AdaptiveTextBlock() { Text = "Your Name:", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveTextInput() { Style = AdaptiveTextInputStyle.Text, Id = "Name" });
            card.Body.Add(new AdaptiveTextBlock() { Text = "Your Question:", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveTextInput() { Style = AdaptiveTextInputStyle.Text, Id = "Question", IsMultiline = true, Placeholder = "type your question here", Height = AdaptiveHeight.Auto, });
            card.Actions.Add(new AdaptiveSubmitAction() { Title = "Submit" });
            card.Body.Add(new AdaptiveTextBlock() { Text = "By clicking submit you agree to our T&C's here https://www.quick-point.co.uk/Home/Terms", Size = AdaptiveTextSize.Small, Weight = AdaptiveTextWeight.Lighter });

            return new Microsoft.Bot.Schema.Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
        }

        private Microsoft.Bot.Schema.Attachment welcomeQCard()
        {
            var card = new AdaptiveCard("1.0");
            card.Body.Add(new AdaptiveTextBlock() { Text = "Hi, welcome to Quick-Point! Please ask a covid-related business question below", Size = AdaptiveTextSize.Medium, Wrap = true });
            card.Body.Add(new AdaptiveTextBlock() { Text = "Otherwise, you can browse some of our most popular questions from the dropdown menu below:", Size = AdaptiveTextSize.Medium, Wrap = true });
            card.Body.Add(new AdaptiveChoiceSetInput()
            {
                Id = "welcomeQCard",

                Style = AdaptiveChoiceInputStyle.Compact,
                Value = "Select Common Question",


                Choices = new List<AdaptiveChoice>(new[] {
                        new AdaptiveChoice() { Title = "How to reopen my business safely during coronavirus", Value = "1" },
                        new AdaptiveChoice() { Title = "Changes in the Coronavirus Job Retention Scheme", Value = "2" } ,
                        new AdaptiveChoice() { Title = "What if I cannot pay my Self Assessment tax bill?", Value = "3" },
                        new AdaptiveChoice() { Title = "Are there grants available for Self-Employed during virus?", Value = "4" },
                        new AdaptiveChoice() { Title = "How much money can I get as a Self Employed grant?", Value = "5" },
                        new AdaptiveChoice() { Title = "Which countries are one the quarantine list?", Value = "6" },

                })
            }) ;
            card.Actions.Add(new AdaptiveSubmitAction() { Title = "Get Answer For Selected Question" });

            return new Microsoft.Bot.Schema.Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
        }

        private Microsoft.Bot.Schema.Attachment WelcomeCard()
        {
            var card = new AdaptiveCard("1.0");
            card.Body.Add(new AdaptiveTextBlock() { Text = "Hi, welcome to Quick-Point. To optimise your visit, please type in your email and name", Size = AdaptiveTextSize.Medium, Wrap = true });
            card.Body.Add(new AdaptiveTextBlock() { Text = "This chatbot is for covid-related business advice", Size = AdaptiveTextSize.Medium, Wrap = true });

            card.Body.Add(new AdaptiveTextBlock() { Text = "Email:", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveTextInput() { Style = AdaptiveTextInputStyle.Text, Id = "Email" });
            card.Body.Add(new AdaptiveTextBlock() { Text = "Name:", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveTextInput() { Style = AdaptiveTextInputStyle.Text, Id = "Name" });
            card.Actions.Add(new AdaptiveSubmitAction() { Title = "Submit" });
            card.Body.Add(new AdaptiveTextBlock() { Text = "By clicking submit you agree to our T&C's here https://www.quick-point.co.uk/Home/Terms", Size = AdaptiveTextSize.Small, Weight = AdaptiveTextWeight.Lighter });
            card.Body.Add(new AdaptiveTextBlock() { Text = "Otherwise, please feel free to ask us your covid-related business questions below", Size = AdaptiveTextSize.Medium, Wrap = true });

            return new Microsoft.Bot.Schema.Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
        }

        private Microsoft.Bot.Schema.Attachment InsuranceCard()
        {
            var card = new AdaptiveCard("1.0");
                       card.Body.Add(new AdaptiveTextBlock() { Text = " Due to a new high court ruling, your business may be able to claim its losses due to corona virus.", Size = AdaptiveTextSize.Medium, Wrap = true });

            card.Body.Add(new AdaptiveTextBlock() { Text = "My AI engine has recognised that you are asking about insurance. Enter your details below and we will personally contact you regarding this", Size = AdaptiveTextSize.Small, Wrap = true });

            card.Body.Add(new AdaptiveTextBlock() { Text = "Email:", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveTextInput() { Style = AdaptiveTextInputStyle.Text, Id = "Email" });
            card.Body.Add(new AdaptiveTextBlock() { Text = "Name:", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveTextInput() { Style = AdaptiveTextInputStyle.Text, Id = "Name" });
            card.Actions.Add(new AdaptiveSubmitAction() { Title = "Submit" });
            card.Body.Add(new AdaptiveTextBlock() { Text = "By clicking submit you agree to our T&C's here https://www.quick-point.co.uk/Home/Terms", Size = AdaptiveTextSize.Small, Weight = AdaptiveTextWeight.Lighter });
            card.Body.Add(new AdaptiveTextBlock() { Text = "Otherwise, please feel free to continue ask us your covid-related business questions below", Size = AdaptiveTextSize.Small, Wrap = true });

            return new Microsoft.Bot.Schema.Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
        }


        private Bot.Schema.Attachment YesNoCard()
        {
            var card = new AdaptiveCard();
            card.Body.Add(new AdaptiveTextBlock() { Text = "Did this answer your question?", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveChoiceSetInput()
            {
                Id = "YesNo",
                Style = AdaptiveChoiceInputStyle.Expanded,
                Choices = new List<AdaptiveChoice>(new[] {
                        new AdaptiveChoice() { Title = "Yes", Value = "Yes" },
                        new AdaptiveChoice() { Title = "No", Value = "No" } })
            }      );
            card.Actions.Add(new AdaptiveSubmitAction() { Title = "Get the answer" });
            return new Bot.Schema.Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
        }

        private readonly IConfiguration _configuration;
        private readonly ILogger<QnABot> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        HeroCard card = new HeroCard()
        {
            Title = $"Answer not found"
        };    
              

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = ((Activity)turnContext.Activity).CreateReply();
            welcomeText.Attachments = new List<Microsoft.Bot.Schema.Attachment>() { welcomeQCard() };
            
            
            //var welcomeText = "Welcome to QuickPoint, what can we help you with today?";
            var welcomeSent = false;
            foreach (var member in membersAdded)
            {
                // welcome new users
                if (!welcomeSent && !_ids.Contains(member.Id))
                {
                    welcomeSent = true;
                    await turnContext.SendActivityAsync(welcomeText);
                 
                    _ids.Add(member.Id);
                }
              
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            
            if (turnContext.Activity.Value == null)
            {
                var typing = new Activity() { Type = ActivityTypes.Typing, Text = null, Value = null };
                await turnContext.SendActivityAsync(typing);


                var httpClient = _httpClientFactory.CreateClient(); 

                var qnaMaker = new QnAMaker(new QnAMakerEndpoint
                {
                    KnowledgeBaseId = "4368eb8d-e200-4e81-a301-31aa12e58de0",
                    Host = "https://qnamakerinstapp.azurewebsites.net/qnamaker",
                    EndpointKey = "e4bd7921-d6bc-4005-89ec-ae7a558a4da2"
                },
                null,
                httpClient); ;
                /////////////////// ******* USING TEST KB!!!!! CHANGE BEFORE DEPLOYMENT & Change email adress to sales *********** //////////////
                _logger.LogInformation("Calling QnA Maker");

                var options = new QnAMakerOptions { Top = 1 };
                // Returns no accurate answer found on any questions below 70 score
                options.ScoreThreshold = 0.6F;


               
                


                // The actual call to the QnA Maker service.
                var response = await qnaMaker.GetAnswersAsync(turnContext, options);
                
                

                if (response != null && response.Length > 0)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(response[0].Answer), cancellationToken);

                    if (Convert.ToString(turnContext.Activity.Text).Contains("Insurance") ^ Convert.ToString(turnContext.Activity.Text).Contains("insurance"))
                    {
                        if (Convert.ToString(turnContext.Activity.Text).Contains("NIC") ^ Convert.ToString(turnContext.Activity.Text).Contains("nic") ^ Convert.ToString(turnContext.Activity.Text).Contains("national") ^ Convert.ToString(turnContext.Activity.Text).Contains("National"))
                        {
                           
                        }
                        else
                        {
                            var _InsuranceCard = ((Activity)turnContext.Activity).CreateReply();
                            _InsuranceCard.Attachments = new List<Microsoft.Bot.Schema.Attachment>() { InsuranceCard() };
                            await turnContext.SendActivityAsync(_InsuranceCard);
                        }
                        
                    }
                    



                    //offer 'ddi this answer your question'
                    if (turnContext.Activity.Text.Length > 10)
                    {
                        // var reply = ((Activity)turnContext.Activity).CreateReply();
                        // reply.Attachments = new List<Microsoft.Bot.Schema.Attachment>() { YesNoCard() };
                        // await turnContext.SendActivityAsync(reply);
                        var finalmsg ="We hope this has answered your question. If it hasn't, let us know at sales@sterling-beanland.co.uk. Otherwise, feel free to ask another question.";
                       
                         await turnContext.SendActivityAsync(finalmsg);
                    }

                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("Sorry, my AI engine can't process that question. Please re-phrase your question and try again. Thank you for your patience!"), cancellationToken);
                }
                
            }
            else // value contains JSON result of card entries 
            {
                var jobj = JObject.Parse(turnContext.Activity.Value.ToString());

                if (jobj.ContainsKey("YesNo"))
                {
                    var reply = (string)jobj["YesNo"];


                    try
                    {

                        if (reply == "yes" ^ reply == "yeah" ^ reply == "yep" ^ reply == "ye" ^ reply == "y" ^ reply == "Yes")
                        {



                            string success = "We are glad to be of help. Please continue to ask another question or check out the other features on our website";
                            await turnContext.SendActivityAsync(success);
                        }
                        else if (reply == "no" ^ reply == "on" ^ reply == "narp" ^ reply == "nah" ^ reply == "nope" ^ reply == "noo" ^ reply == "No")
                        {
                            var reply_ = ((Activity)turnContext.Activity).CreateReply();
                            reply_.Attachments = new List<Microsoft.Bot.Schema.Attachment>() { wrongAnswerCard() };

                            await turnContext.SendActivityAsync(reply_);
                        }
                        else
                        {
                            string fail = "Please only enter 'yes' or 'no', or ask another question";
                            await turnContext.SendActivityAsync(fail);
                        }

                    }
                    catch
                    {
                        string fail = "Please only enter 'yes' or 'no', or ask another question";
                        await turnContext.SendActivityAsync(fail);

                    }
                }
                else if (jobj.ContainsKey("welcomeQCard"))
                {
                    var reply = (string)jobj["welcomeQCard"];


                    try
                    {

                        if (reply == "1")
                        {



                            string answer = "Employers that want to reopen their business have a legal responsibility to protect their employees and other people on site. Use this guidance to help you carry out a risk assessment and make sensible adjustments to the site and workforce. If you do not carry out a risk assessment, the Health and Safety Executive(HSE) or your local council can issue an enforcement notice. Employees can use this guidance to check what their workplace needs to do to keep people safe. This guidance is only for businesses that are allowed to reopen in England.Guidance from the Scottish Government, Welsh Government and Northern Ireland Assembly is also available. If you are in an area affected by coronavirus outbreak, check Local restrictions: areas with an outbreak of coronavirus guidance. https://www.gov.uk/coronavirus-business-reopening/y ";
                            await turnContext.SendActivityAsync(answer);
                        }
                        else if (reply == "2")
                        {



                            string answer = "The Coronavirus Job Retention Scheme will close on 31 October 2020. From 1 July, employers can bring furloughed employees back to work for any amount of time and any shift pattern, while still being able to claim CJRS grant for the hours not worked. From 1 August 2020, the level of grant will be reduced each month.To be eligible for the grant employers must pay furloughed employees 80 % of their wages, up to a cap of £2, 500 per month for the time they are being furloughed. The timetable for changes to the scheme is set out below.Wage caps are proportional to the hours an employee is furloughed. For example, an employee is entitled to 60 % of the £2, 500 cap if they are placed on furlough for 60 % of their usual hours: there are no changes to grant levels in June for June and July, the government will pay 80 % of wages up to a cap of £2, 500 for the hours the employee is on furlough, as well as employer National Insurance Contributions(ER NICS) and pension contributions for the hours the employee is on furlough. Employers will have to pay employees for the hours they work for August, the government will pay 80 % of wages up to a cap of £2, 500 for the hours an employee is on furlough and employers will pay ER NICs and pension contributions for the hours the employee is on furlough for September, the government will pay 70 % of wages up to a cap of £2, 187.50 for the hours the employee is on furlough. Employers will pay ER NICs and pension contributions and top up employees’ wages to ensure they receive 80 % of their wages up to a cap of £2, 500, for time they are furloughed for October, the government will pay 60 % of wages up to a cap of £1, 875 for the hours the employee is on furlough. Employers will pay ER NICs and pension contributions and top up employees’ wages to ensure they receive 80 % of their wages up to a cap of £2, 500, for time they are furloughed. Employers will continue to able to choose to top up employee wages above the 80 % total and £2,500 cap for the hours not worked at their own expense if they wish. Employers will have to pay their employees for the hours worked.  The table shows Government contribution, required employer contribution and amount employee receives where the employee is furloughed 100 % of the time. Wage caps are proportional to the hours not worked.";
                            await turnContext.SendActivityAsync(answer);
                        }
                        else if (reply == "3")
                        {



                            string answer = "You have the option to defer your second payment on account if you’re: registered in the UK for Self Assessment and finding it difficult to make your second payment on account by 31 July 2020 due to the impact of coronavirus You can still make the payment by 31 July 2020 as normal if you’re able to do so. The June 2020 Self Assessment statements showed 31 January 2021 as the due date for paying the July 2020 Payment on Account.This is because HMRCupdated their IT systems to prevent customers incurring late payment interest on any July 2020 Payment on Account paid between 1st August 2020 and 31 January 2021.The deferment has not been applied for all customers by HMRC and it remains optional. HMRC will not charge interest or penalties on any amount of the deferred payment on account, provided it’s paid on or before 31 January 2021. If you owe less than £10, 000 you might be able to set up a Time to Pay Arrangement online.This lets you pay your Self Assessment tax bill in instalments. Call the Self Assessment helpline if you’ve missed your payment date or you cannot use the online service. You do not need to contact HMRC if you have set up a payment plan online. Self Assessment Payment Helpline Telephone: 0300 200 3822 Monday to Friday, 8am to 4pm Find out about call charges";
                            await turnContext.SendActivityAsync(answer);
                        }
                        else if (reply == "4")
                        {



                            string answer = "You can claim if you’re a self-employed individual or a member of a partnership and your business has been adversely affected on or after 14 July 2020. Your business could be adversely affected by coronavirus if, for example: you’re unable to work because you: are shielding are self-isolating are on sick leave because of coronavirus have caring responsibilities because of coronavirus you’ve had to scale down, temporarily stop trading or incurred additional costs because: your supply chain has been interrupted you have fewer or no customers or clients your staff are unable to come in to work one or more of your contracts have been cancelled you had to buy protective equipment so you could trade following social distancing rules All of the following must also apply: you traded in the tax year 2018 to 2019 and submitted your Self Assessment tax return on or before 23 April 2020 for that year you traded in the tax year 2019 to 2020 you intend to continue to trade in the tax year 2020 to 2021 you carry on a trade which has been adversely affected by coronavirus You cannot claim the grant if you trade through a limited company or a trust. If you claim Maternity Allowance this will not affect your eligibility for the grant. To work out your eligibility we will first look at your 2018 to 2019 Self Assessment tax return. Your trading profits must be no more than £50,000 and at least equal to your non-trading income. If you’re not eligible based on the 2018 to 2019 Self Assessment tax return, we will then look at the tax years 2016 to 2017, 2017 to 2018, and 2018 to 2019. Find out how we will work out your eligibility including if we have to use other years. Grants under the Self-Employment Income Support Scheme are not counted as ‘access to public funds’, and you can claim the grant on all categories of work visa.";
                            await turnContext.SendActivityAsync(answer);
                        }
                        else if (reply == "5")
                        {


                            string answer = "You’ll get a taxable grant based on your average trading profit over the 3 tax years: 2016 to 2017, 2017 to 2018, 2018 to 2019 HMRC will work out your average trading profit by adding together your total trading profits or losses for the 3 tax years, then they will divide by 3. The second and final grant is worth 70% of your average monthly trading profits, paid out in a single instalment covering 3 months’ worth of profits, and capped at £6,570 in total. The online service will tell you how HMRC’s worked your grant out. The grant amount HMRC works out for you will be paid directly into your bank account, in one instalment. Find out how HMRC will work out your average trading profits including if you have not traded for all 3 years:https://www.gov.uk/guidance/how-hmrc-works-out-total-income-and-trading-profits-for-the-self-employment-income-support-scheme#threeyears";
                            await turnContext.SendActivityAsync(answer);
                        }
                        else if (reply == "6")
                        {

                            string answer = "You will not need to self-isolate if you are travelling from the following countries:  Akrotiri and Dhekelia  Anguilla  Antigua and Barbuda  Australia  Barbados  Bermuda  Bonaire, St Eustatius and Saba  British Antarctic Territory  British Indian Ocean Territory  British Virgin Islands  Brunei (added 11 August 2020 – if you arrived in England from Brunei before 11 August, you will need to self–isolate)  Cayman Islands  the Channel Islands  Curaçao  Cyprus  Denmark  Dominica  Estonia  Falkland Islands  Faroe Islands  Fiji  Finland  French Polynesia  Gibraltar  Germany  Greece  Greenland  Grenada  Guadeloupe  Hong Kong  Hungary  Iceland  Ireland  the Isle of Man  Italy  Japan  Latvia  Liechtenstein  Lithuania  Macao (Macau)  Malaysia (added 11 August 2020 – if you arrived in England from Malaysia before 11 August, you will need to self–isolate)  Mauritius  Montserrat  New Caledonia  New Zealand  Norway  Pitcairn, Henderson, Ducie and Oeno Islands  Poland  Portugal (added 4am, Saturday 22 August 2020 – if you arrived in England from Portugal before 4am, 22 August, you will need to self–isolate)  Reunion  San Marino  Seychelles  Slovakia  Slovenia  South Korea  South Georgia and the South Sandwich Islands  St Barthélemy  St Helena, Ascension and Tristan da Cunha  St Kitts and Nevis  St Lucia  St Pierre and Miquelon  St Vincent and the Grenadines  Switzerland  Taiwan  Turkey  Vatican City State  Vietnam ";
                            await turnContext.SendActivityAsync(answer);
                        }



                    }
                    catch
                    {
                        string fail = "Sorry, there seems to be an error on our side, we will look at this as soon as possible. For now, feel free to conintinue asking questions.";
                        await turnContext.SendActivityAsync(fail);

                    }
                }
                else
                {
                    var email = (string)jobj["Email"];
                    var name = jobj["Name"].ToString();
                    var storage = new MemoryStorage();
                    // Create the User state passing in the storage layer.
                    var userState = new UserState(storage);
                    


                    try
                    {

                        if (email != "" & email != null)
                        {

                            var dt = DateTime.Now.ToString();


                            var mailMessage = new MailMessage();
                            mailMessage.From = new
                               MailAddress("freddie.kemp@cybercom.media", "Quick Point Admin");
                            mailMessage.To.Add("sales@sterling-beanland.co.uk");
                            // mailMessage.To.Add("freddieK_02@hotmail.co.uk");
                             mailMessage.CC.Add("freddie.kemp@cybercom.media");
                            //mailMessage.CC.Add("andrew.ingpen@cybercom.media");
                            mailMessage.Subject = "Insurance request from " + name; ;
                            mailMessage.Body = dt + "\n" + "\n" + "Name:" + "\n" + name + "\n" + "\n" + "Email:" + "\n" + email;
                            mailMessage.IsBodyHtml = false;
                            SmtpClient client = new SmtpClient();
                            client.Credentials = new NetworkCredential("freddie.kemp@cybercom.media", fredpw());
                            client.Port = 587;
                            client.Host = "smtp.office365.com";
                            client.EnableSsl = true;
                            client.Send(mailMessage);

                            string success = "Thank you, "+name+". We have processed your details. Please continue to ask us a question:";

                            await turnContext.SendActivityAsync(success);
                        }
                        else
                        {
                            string fail = "Unfortunately we could not process your details. Please continue to ask us a question";
                            await turnContext.SendActivityAsync(fail);
                        }

                    }
                    catch
                    {
                        string fail = "Sorry, we couldn't process your request. Please ask a question";
                        await turnContext.SendActivityAsync(fail);

                    }
                }
                

            }
            

        }
        public string fredpw()
        {
            string p;
            p = "%";
            p = p + "^";
            p = p + "$";
            p = p + "8";
            p = p + "i";
            p = p + "j";
            p = p + "t";
            p = p + "6";
            p = p + "5";
            p = p + "$";
            p = p + "R";
            p = p + "t";
            p = p + "y";
            p = p + "u";
            p = p + "!";
            p = p + "*";
            return p;
        }
    }
}

// live agent handoff
//if (turnContext.Activity.Text.Contains("agent") ^ turnContext.Activity.Text.Contains("Agent"))
//{
//    var skillDict = new Dictionary<string, string> { { "skill", "CUSTOM" } };
//    var actionDict = new Dictionary<string, object>()
//    {
//        { "name", "TRANSFER" },
//        { "parameters", skillDict }
//    };

//    IMessageActivity message = Activity.CreateMessageActivity();
//    message.Text = $"Trying to connect to an agent";
//    message.TextFormat = "plain";
//    message.ChannelData = new Dictionary<string, object>
//    {
//        ["type"] = "message", //["type"] = "connectAgent",
//        ["text"] = "",
//        ["channelData"] = new Dictionary<string, object> { { "action", actionDict } }

//    };
//    await turnContext.SendActivityAsync(message, cancellationToken);

//    return;

//}
