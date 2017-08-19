using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace StupidBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        // user name
        private string name;

        // age of user
        private int age;

        // color
        private object color;

        public async Task StartAsync(IDialogContext context)
        {
            // Root dialog initiates and waits for the next message from the user. 
            // When a message arrives, call MessageReceivedAsync.
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            // Get a message
            var message = await result;

            await this.WelcomeMessageAsync(context);
        }

        private async Task WelcomeMessageAsync(IDialogContext context)
        {
            await context.PostAsync("Hi, I am Stupid Bot. Let's get started.");

            context.Call(new NameDialog(), this.NameDialogResumeAfter);
        }

        private async Task NameDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                this.name = await result;

                //context.Call(new AgeDialog(this.name), this.AgeDialogResumeAfter);
                context.Call(new HelpDialog(this.name), this.HelpDialogResumeAfter);
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");

                await this.WelcomeMessageAsync(context);
            }
        }

        // Help dialog from bot
        private async Task HelpDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            context.Call(new AgeDialog(this.name), this.AgeDialogResumeAfter);
        }

        private async Task AgeDialogResumeAfter(IDialogContext context, IAwaitable<int> result)
        {
            try
            {
                this.age = await result;

                await context.PostAsync($"Your name is { name } and your age is { age }.");
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");
            }
            finally
            {
                //await this.WelcomeMessageAsync(context);
                context.Call(new ColorDialog(), this.ColorDialogResumeAfter);
            }
        }

        private async Task ColorDialogResumeAfter(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                //this.color = await result as Activity;
                this.color = await result;

                await context.PostAsync($"At the age of { age }, { name }'s favorite color is  { color }.");
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("Opps! An error has been occured.");
            }
            finally
            {
                await this.WelcomeMessageAsync(context);
            }
        }
    }
}