// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Net.Http;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AdaptiveCards;

namespace Microsoft.BotBuilderSamples
{
    public class QnABot : ActivityHandler
    {
        private Attachment CreateAdaptiveCardUsingSdk()
        {
            var card = new AdaptiveCard();
            card.Body.Add(new AdaptiveTextBlock() { Text = "Colour", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveChoiceSetInput()
            {
                Id = "Colour",
                Style = AdaptiveChoiceInputStyle.Compact,
                Choices = new List<AdaptiveChoice>(new[] {
                        new AdaptiveChoice() { Title = "Red", Value = "RED" },
                        new AdaptiveChoice() { Title = "Green", Value = "GREEN" },
                        new AdaptiveChoice() { Title = "Blue", Value = "BLUE" } })
            });
            card.Body.Add(new AdaptiveTextBlock() { Text = "Registration number:", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveTextInput() { Style = AdaptiveTextInputStyle.Text, Id = "RegistrationNumber" });
            card.Actions.Add(new AdaptiveSubmitAction() { Title = "Submit" });
            return new Attachment()
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
            Title = $"Appointment with doctor"
        };
        


        public QnABot(IConfiguration configuration, ILogger<QnABot> logger, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }


        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Welcome to QuickPoint, what can we help you with today  !";
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
            var httpClient = _httpClientFactory.CreateClient();

            var qnaMaker = new QnAMaker(new QnAMakerEndpoint
            {
                KnowledgeBaseId = "cb3bd2f1-94fb-4190-bddd-07f025baa3a3",
                Host = "https://qpqnamakerapp1406.azurewebsites.net/qnamaker",
                EndpointKey = "a8460833-f441-4247-bb18-cad2bf2672fa"
                //KnowledgeBaseId = _configuration["QnAKnowledgebaseId"],
                //EndpointKey = _configuration["QnAEndpointKey"],
                //Host = _configuration["QnAEndpointHostName"]




            },
            null,
            httpClient); ;

            _logger.LogInformation("Calling QnA Maker");

            var options = new QnAMakerOptions { Top = 1 };

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
                reply.Attachments = new List<Attachment>() { CreateAdaptiveCardUsingSdk() };

                await turnContext.SendActivityAsync(reply);

            }
        }
    }
}
