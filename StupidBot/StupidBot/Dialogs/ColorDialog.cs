using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StupidBot.Dialogs
{
    [Serializable]
    public class ColorDialog : IDialog<object>
    {
        private int attempts = 3;

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("May I ask your favorite color?");

            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if ((activity != null) & !string.IsNullOrWhiteSpace(activity.Text))
            {
                string answer = Convert.ToString(activity.Text).ToLower();
                if (answer.Contains("yes"))
                {
                    // connector that bot uses to communicate with user on a channel
                    var connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                    // add a suggestion
                    var reply = activity.CreateReply("I have colors in mind, but need your help to choose the best one.");
                    reply.Type = ActivityTypes.Message;
                    reply.TextFormat = TextFormatTypes.Plain;

                    reply.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                        {
                            new CardAction(){ Title = "Blue", Type=ActionTypes.ImBack, Value="Blue" },
                            new CardAction(){ Title = "Red", Type=ActionTypes.ImBack, Value="Red" },
                            new CardAction(){ Title = "Green", Type=ActionTypes.ImBack, Value="Green" }
                        }
                    };

                    await connector.Conversations.ReplyToActivityAsync(reply);

                    context.Wait(this.MessageReceivedAsyncDone);
                }
                else if (answer.Contains("no"))
                {
                    context.Done<object>(null);
                }
            }
            else
            {
                --attempts;
                if (attempts > 0)
                {
                    await context.PostAsync("I'm sorry, I don't understand your reply.");
                }
                else
                {
                    context.Fail(new TooManyAttemptsException("Answer is invalid."));
                }
            }
        }

        private async Task MessageReceivedAsyncDone(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            context.Done(message.Text);
        }
    }
}