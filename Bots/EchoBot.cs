// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot : ActivityHandler
    {
        ContestTask NewTask = new ContestTask();
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var replyText = $"Messages: {turnContext.Activity.Text}";
            string result = "";
            switch (turnContext.Activity.Text)
            {
                case "/random":
                    NewTask = new ContestTask();
                    result = NewTask.Condition;
                    CSharpCompiler.NowTask = NewTask;
                    break;
                case "/help":
                    string helper = "/random – random task" + Environment.NewLine + "/code - send the code to check it" + Environment.NewLine  + "/mood - shows a picture of Kermit";
                    await turnContext.SendActivityAsync(helper);
                    break;
                case "/mood":
                    var pic = MessageFactory.Text("", "");
                    var PhotoAttach = new Attachment()
                    {
                        Name = "maxresdefault.jpg",
                        ContentType = "image/jpg",
                        ContentUrl = MoodPhoto()
                    };
                    pic.Attachments = new List<Attachment>() { PhotoAttach };
                    await turnContext.SendActivityAsync(pic);
                    break;
                case "/code":
                    CSharpCompiler.BuildProject(out bool error, turnContext.Activity.Text);
                    if (!error)
                    {
                        result = CSharpCompiler.RunAndCheckProject();
                    }
                    else
                    {
                        result = "CE";
                    }
                    break;
            }

            var reply = MessageFactory.Text(result, result);
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello dear friend!" + Environment.NewLine + "This bot will help you to learn C#. " + Environment.NewLine + "Call /help to see all commands";
            var reply = MessageFactory.Text(welcomeText, welcomeText);
            var PhotoAttach = new Attachment()
            {
                Name = "maxresdefault.jpg",
                ContentType = "image/jpg",
                ContentUrl = "https://i.ytimg.com/vi/PtXVF9o8DiA/maxresdefault.jpg"
            };
            reply.Attachments = new List<Attachment>() { PhotoAttach };
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                }
            }
        }
        private static string MoodPhoto()
        {
            List<string> AllPhohtos = new List<string>()
            {
                "https://pbs.twimg.com/media/EMLMeOCXYAAztWs.jpg",
                "https://image.tmdb.org/t/p/original/rQYQByzhjG4oNU8Om1CnWUSb4yf.jpg",
                "https://i.pinimg.com/originals/72/23/e3/7223e3778b0c05dff8e197adb8e629ae.jpg",
                "https://pbs.twimg.com/media/CTFitQoWoAArebW.jpg:large",
                "https://images.fandango.com/ImageRenderer/0/0/redesign/static/img/default_poster.png/0/images/masterrepository/fandango/161864/muppetsmostwanted-mv-56.jpg",
                "https://www.thesun.co.uk/wp-content/uploads/2018/02/nintchdbpict000255319028.jpg",
                "https://c.wallhere.com/photos/c1/62/Kermit_the_Frog_flowers_Sesame_Street_The_Muppets-1336809.jpg!d",
                "https://www.dailydot.com/wp-content/uploads/26c/b1/06e160103c1974fa0fb3d133e5d38bcf.jpg",
                "https://yt3.ggpht.com/a/AATXAJycOI5UU75wO5_Rz_k-FUCuILJo6PqfWAS-Yg=s900-c-k-c0xffffffff-no-rj-mo",
                "https://i.pinimg.com/736x/fc/bf/98/fcbf98ece06d0b04dd787a7e01993923.jpg",
                "https://www.factinate.com/wp-content/uploads/2018/06/30-32.jpg"
            };
            Random random = new Random();
            int index = random.Next(AllPhohtos.Count);
            return AllPhohtos[index];
        }
    }
}