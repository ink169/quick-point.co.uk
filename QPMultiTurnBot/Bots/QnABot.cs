﻿// Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace Microsoft.BotBuilderSamples
{
    public class QnABot : ActivityHandler
    {

        private List<String> _ids;
        public QnABot(IConfiguration configuration, ILogger<QnABot> logger, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _ids = new List<String>();
        }

        private Microsoft.Bot.Schema.Attachment CreateAdaptiveCardUsingSdk()
        {
            var card = new AdaptiveCard("1.0");
            card.Body.Add(new AdaptiveTextBlock() { Text = "Unfortunately, the Quick-Point bot couldn't find an accurate answer to your query. Please try again, using specific key words. Thanks for your patience - the ChatBot gets smarter with each question!", Size = AdaptiveTextSize.Medium, Wrap = true });
            card.Body.Add(new AdaptiveTextBlock() { Text = "If you would like a member of the team to answer your question personally, please enter your email and question", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder, Wrap = true });
            card.Body.Add(new AdaptiveTextBlock() { Text = "Your Email:", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveTextInput() { Style = AdaptiveTextInputStyle.Text, Id = "Email" });
            card.Body.Add(new AdaptiveTextBlock() { Text = "Your Name:", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveTextInput() { Style = AdaptiveTextInputStyle.Text, Id = "Name" });
            card.Body.Add(new AdaptiveTextBlock() { Text = "Your Question:", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveTextInput() { Style = AdaptiveTextInputStyle.Text, Id = "Question" });
            card.Actions.Add(new AdaptiveSubmitAction() { Title = "Submit" });

            return new Microsoft.Bot.Schema.Attachment()
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
            var welcomeText = "Welcome to QuickPoint, what can we help you with today?";
            foreach (var member in membersAdded)
            {
                // welcome new users
                if (!_ids.Contains(member.Id))
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                    _ids.Add(member.Id);
                }
              
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var typing = new Activity() { Type = ActivityTypes.Typing };
            await turnContext.SendActivityAsync(typing);
            if (turnContext.Activity.Value == null)
            {

                // live agent handoff
                if (turnContext.Activity.Text.Contains("agent") ^ turnContext.Activity.Text.Contains("Agent"))
                {
                    var skillDict = new Dictionary<string, string> { { "skill", "CUSTOM" } };
                    var actionDict = new Dictionary<string, object>()
                    {
                        { "name", "TRANSFER" },
                        { "parameters", skillDict }
                    };

                    IMessageActivity message = Activity.CreateMessageActivity();
                    message.Text = $"Trying to connect to an agent";
                    message.TextFormat = "plain";
                    message.ChannelData = new Dictionary<string, object>
                    {
                        ["type"] = "message", //["type"] = "connectAgent",
                        ["text"] = "",
                        ["channelData"] = new Dictionary<string, object> { { "action", actionDict } }

                    };
                    await turnContext.SendActivityAsync(message, cancellationToken);

                    return;

                }

                var httpClient = _httpClientFactory.CreateClient();

                /////////////////// ******* USING TEST KB!!!!! CHANGE BEFORE DEPLOYMENT *********** //////////////

                var qnaMaker = new QnAMaker(new QnAMakerEndpoint
                {/*
                    //Practise Details
                    KnowledgeBaseId = "9c87bf00-637f-4ce8-88e0-829c96a96ebb",
                    Host = "https://qpqnamakerapp1406.azurewebsites.net/qnamaker",
                    EndpointKey = "a8460833-f441-4247-bb18-cad2bf2672fa"
                    // Live Details
                    */
                    KnowledgeBaseId = "cb3bd2f1-94fb-4190-bddd-07f025baa3a3",
                    Host = "https://qpqnamakerapp1406.azurewebsites.net/qnamaker",
                    EndpointKey = "a8460833-f441-4247-bb18-cad2bf2672fa"
                    
                },
                null,
                httpClient); ;
                /////////////////// ******* USING TEST KB!!!!! CHANGE BEFORE DEPLOYMENT & Change email adress to sales *********** //////////////
                _logger.LogInformation("Calling QnA Maker");

                var options = new QnAMakerOptions { Top = 1 };
                // Returns no accurate answer found on any questions below 70 score
                options.ScoreThreshold = 0.4F;



                // The actual call to the QnA Maker service.
                var response = await qnaMaker.GetAnswersAsync(turnContext, options);
                if (response != null && response.Length > 0)
                {

                    await turnContext.SendActivityAsync(MessageFactory.Text(response[0].Answer), cancellationToken);
                }
                else
                {
                    //await turnContext.SendActivityAsync(MessageFactory.Text("No QnA Maker answers were found."), cancellationToken);
                    var reply = ((Activity)turnContext.Activity).CreateReply();
                    reply.Attachments = new List<Microsoft.Bot.Schema.Attachment>() { CreateAdaptiveCardUsingSdk() };

                    await turnContext.SendActivityAsync(reply);

                }
            }
            else   // value contains JSON result of card entries 
            {
                var jobj = JObject.Parse(turnContext.Activity.Value.ToString());
                var email = (string)jobj["Email"];
                var name = jobj["Name"].ToString();
                var question = jobj["Question"].ToString();

                try
                {

                    if (question != "")
                    {

                        var dt = DateTime.Now.ToString();


                        var mailMessage = new MailMessage();
                        mailMessage.From = new
                           MailAddress("freddie.kemp@cybercom.media", "Quick Point Admin");
                        mailMessage.To.Add("sales@sterling-beanland.co.uk");
                        //mailMessage.To.Add("freddieK_02@hotmail.co.uk");
                        mailMessage.CC.Add("freddieK_02@hotmail.co.uk");
                        mailMessage.CC.Add("andrew.ingpen@cybercom.media");
                        mailMessage.Subject = "Unanswered Question from " + name; ;
                        mailMessage.Body = dt + "\n" + "\n" + "Name:" + "\n" + name + "\n" + "\n" + "Email:" + "\n" + email + "\n" + "\n" + "Question:" + "\n" + question;
                        mailMessage.IsBodyHtml = false;
                        SmtpClient client = new SmtpClient();
                        client.Credentials = new NetworkCredential("freddie.kemp@cybercom.media", fredpw());
                        client.Port = 587;
                        client.Host = "smtp.office365.com";
                        client.EnableSsl = true;
                        client.Send(mailMessage);

                        string success = "Thank you, we will be in touch. Please feel free to ask another question in the meantime";

                        await turnContext.SendActivityAsync(success);
                    }
                    else
                    {
                        string fail = "Unfortunately we could not process your details. Please make sure at least the question is entered and try again.";
                        await turnContext.SendActivityAsync(fail);
                    }

                }
                catch
                {
                    string fail = "Unfortunately we could not process your details. Please make sure at least the question is entered and try again.";
                    await turnContext.SendActivityAsync(fail);

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
