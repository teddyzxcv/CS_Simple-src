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
                default:
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
            var welcomeText = "Hello there!";
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
    }
}