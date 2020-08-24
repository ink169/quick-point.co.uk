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

        private Microsoft.Bot.Schema.Attachment WelcomeCard()
        {
            var card = new AdaptiveCard("1.0");
            card.Body.Add(new AdaptiveTextBlock() { Text = "Hi, welcome to Quick-Point. To optimise your visit, please type in your email and name", Size = AdaptiveTextSize.Medium, Wrap = true });
            card.Body.Add(new AdaptiveTextBlock() { Text = "Email:", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveTextInput() { Style = AdaptiveTextInputStyle.Text, Id = "Email" });
            card.Body.Add(new AdaptiveTextBlock() { Text = "Name:", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveTextInput() { Style = AdaptiveTextInputStyle.Text, Id = "Name" });
            card.Actions.Add(new AdaptiveSubmitAction() { Title = "Submit" });
            card.Body.Add(new AdaptiveTextBlock() { Text = "By clicking submit you agree to our T&C's here https://www.quick-point.co.uk/Home/Terms", Size = AdaptiveTextSize.Small, Weight = AdaptiveTextWeight.Lighter });
            card.Body.Add(new AdaptiveTextBlock() { Text = "Otherwise, please feel free to ask us your question below", Size = AdaptiveTextSize.Medium, Wrap = true });

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
            card.Actions.Add(new AdaptiveSubmitAction() { Title = "Submit" });
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
            welcomeText.Attachments = new List<Microsoft.Bot.Schema.Attachment>() { WelcomeCard() };
            
            
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
                            mailMessage.Subject = "Bot conversation details from " + name; ;
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
