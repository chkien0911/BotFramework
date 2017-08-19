using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace StupidBot.Dialogs
{
    [Serializable]
    public class HelpDialog : IDialog<string>
    {
        private string name;

        public HelpDialog(string name)
        {
            this.name = name;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync($"How may I help you, { this.name }?");

            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result as Activity;

            // convert message to lower case
            string answer = Convert.ToString(message.Text).ToLower();
            if ((answer != null) && (answer.Trim().Length > 0))
            {
                if (answer.Contains("have lunch"))
                {
                    //context.Call(new LunchDialog(), this.LunchDialogResumeAfter);

                    // connector that bot uses to communicate with user on a channel
                    var connector = new ConnectorClient(new Uri(message.ServiceUrl));

                    Activity replyToConversation = message.CreateReply("Here is menu of lunch.");
                    replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    replyToConversation.Attachments = new List<Attachment>();

                    Dictionary<string, string> cardContentList = new Dictionary<string, string>();
                    cardContentList.Add("Phở", "http://imgs.vietnamnet.vn/Images/2017/04/02/09/20170402095649-pho-1.jpg");
                    cardContentList.Add("Bún chả", "https://media.foody.vn/res/g15/140540/prof/s640x400/foody-mobile-t-1-jpg-832-635687514509475148.jpg");
                    cardContentList.Add("Bún bò", "http://static.thanhnien.com.vn/uploaded/2014/ihay.thanhnien.com.vn/pictures201410/linh_san/_nb/bunbohueohue.jpg?width=600");

                    foreach (KeyValuePair<string, string> cardContent in cardContentList)
                    {
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: cardContent.Value));

                        List<CardAction> cardButtons = new List<CardAction>();

                        CardAction plButton = new CardAction()
                        {
                            Value = $"http://www.google.com/search?source=hp&q={cardContent.Key}&oq={cardContent.Key}",
                            Type = "openUrl",
                            Title = "Google"
                        };

                        cardButtons.Add(plButton);

                        HeroCard plCard = new HeroCard()
                        {
                            Title = $"{cardContent.Key}",
                            Subtitle = $"Search for { cardContent.Key } on Google",
                            Images = cardImages,
                            Buttons = cardButtons
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                    }

                    var reply = await connector.Conversations.SendToConversationAsync(replyToConversation);

                    context.Done(message.Text);
                }
                else if (answer.Contains("have dinner"))
                {

                }
                else if (answer.Contains("have a drink"))
                {

                }
                else
                {

                }
            }
        }

        private async Task LunchDialogResumeAfter(IDialogContext context, IAwaitable<object> result)
        {
            //await this.StartAsync(context);
            var message = await result;
            context.Done<object>(null);
        }
    }
}