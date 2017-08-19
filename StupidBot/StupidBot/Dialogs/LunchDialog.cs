using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StupidBot.Dialogs
{
    [Serializable]
    public class LunchDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
            //await this.MessageReceivedAsync(context, null);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result as Activity;

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

            context.Wait(this.MessageReceivedAsyncDone);
        }

        private async Task MessageReceivedAsyncDone(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            context.Done(message.Text);
        }
    }
}