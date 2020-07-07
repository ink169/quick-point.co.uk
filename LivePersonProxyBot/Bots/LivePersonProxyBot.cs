// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LivePersonConnector;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using AdaptiveCards;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using System.Net.Mail;
using System.Net;

namespace LivePersonProxyBot.Bots
{
    public class LivePersonProxyBot : ActivityHandler
    {

        private Microsoft.Bot.Schema.Attachment CreateAdaptiveCardUsingSdk()
        {
            var card = new AdaptiveCard();
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

        private readonly BotState _conversationState;
        private readonly ILivePersonCredentialsProvider _creds;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LivePersonProxyBot> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        HeroCard card = new HeroCard()
        {
            Title = $"Answer not found"
        };


        public LivePersonProxyBot(ConversationState conversationState, ILivePersonCredentialsProvider creds, IConfiguration configuration, ILogger<LivePersonProxyBot> logger, IHttpClientFactory httpClientFactory)
        {
            _conversationState = conversationState;
            _creds = creds;
            _configuration = configuration;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }



        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Welcome to QuickPoint, what can we help you with today?";

            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var conversationStateAccessors = _conversationState.CreateProperty<LoggingConversationData>(nameof(LoggingConversationData));
            var conversationData = await conversationStateAccessors.GetAsync(turnContext, () => new LoggingConversationData());

            
            if (turnContext.Activity.Text.Contains("agent") ^ turnContext.Activity.Text.Contains("Agent"))
            {
                await turnContext.SendActivityAsync("Your request will be escalated to a human agent");

                var transcript = new Transcript(conversationData.ConversationLog.Where(a => a.Type == ActivityTypes.Message).ToList());

                var evnt = EventFactory.CreateHandoffInitiation(turnContext,
                    new { Skill = "chat",
                          EngagementAttributes = new EngagementAttribute[]
                          {
                              new EngagementAttribute { Type = "ctmrinfo", CustomerType = "vip", SocialId = "123456789"},
                              new EngagementAttribute { Type = "personal", FirstName = turnContext.Activity.From.Name }
                          }
                    },
                    transcript);

                await turnContext.SendActivityAsync(evnt);
            }
            else if (turnContext.Activity.Value == null)
            {

                var httpClient = _httpClientFactory.CreateClient();

                /////////////////// ******* USING TEST KB!!!!! CHANGE BEFORE DEPLOYMENT *********** //////////////

                var qnaMaker = new QnAMaker(new QnAMakerEndpoint
                {
                    //Practise Details
                    KnowledgeBaseId = "9c87bf00-637f-4ce8-88e0-829c96a96ebb",
                    Host = "https://qpqnamakerapp1406.azurewebsites.net/qnamaker",
                    EndpointKey = "a8460833-f441-4247-bb18-cad2bf2672fa"
                    // Live Details
                    /*
                    KnowledgeBaseId = "cb3bd2f1-94fb-4190-bddd-07f025baa3a3",
                    Host = "https://qpqnamakerapp1406.azurewebsites.net/qnamaker",
                    EndpointKey = "a8460833-f441-4247-bb18-cad2bf2672fa"
                    */
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
                      //  mailMessage.To.Add("sales@sterling-beanland.co.uk");
                        mailMessage.To.Add("freddie.kemp@cybercom.media");
                        mailMessage.CC.Add("freddie.kemp@cybercom.media");
                        //mailMessage.CC.Add("andrew.ingpen@cybercom.media");
                        mailMessage.Subject = "Unanswered Question from " + name; ;
                        mailMessage.Body = dt + "\n" + "\n" + "Name:" + "\n" + name + "\n" + "\n" + "Email:" + "\n" + email + "\n" + "\n" + "Question:" + "\n" + question;
                        mailMessage.IsBodyHtml = false;
                        SmtpClient client = new SmtpClient();
                        client.Credentials = new NetworkCredential("freddie.kemp@cybercom.media", "145fred89jk!*.");
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

        protected override async Task OnEventAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            if(turnContext.Activity.Name == "handoff.status")
            {
                var conversationStateAccessors = _conversationState.CreateProperty<LoggingConversationData>(nameof(LoggingConversationData));
                var conversationData = await conversationStateAccessors.GetAsync(turnContext, () => new LoggingConversationData());

                Activity replyActivity;
                var state = (turnContext.Activity.Value as JObject)?.Value<string>("state");
                if (state == "typing")
                {
                    replyActivity = new Activity
                    {
                        Type = ActivityTypes.Typing,
                        Text = "agent is typing",
                    };
                }
                else if (state == "accepted")
                {
                    replyActivity = MessageFactory.Text("An agent has accepted the conversation and will respond shortly.");
                    await _conversationState.SaveChangesAsync(turnContext);
                }
                else if (state == "completed")
                {
                    replyActivity = MessageFactory.Text("The agent has closed the conversation.");
                }
                else
                {
                    replyActivity = MessageFactory.Text($"Conversation status changed to '{state}'");
                }

                await turnContext.SendActivityAsync(replyActivity);
            }

            await base.OnEventAsync(turnContext, cancellationToken);
        }
    }
}
