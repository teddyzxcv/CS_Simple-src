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
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var replyText = $"Messages: {turnContext.Activity.Text}";
            var error = CSharpCompiler.ComplieCode(turnContext.Activity.Text);
            string errorreply;
            if (error.Count > 0)
                errorreply = String.Join('\n', error);
            else
            {
                errorreply = "No error, good job!\n";

            }

            var reply = MessageFactory.Text(errorreply, errorreply);
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
