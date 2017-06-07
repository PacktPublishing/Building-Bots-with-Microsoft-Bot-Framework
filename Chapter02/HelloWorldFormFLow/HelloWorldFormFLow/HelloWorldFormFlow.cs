using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorldFormFLow
{
    [Serializable]
    class HelloWorldFormFlow
    {
        [Prompt("Please enter name")]
        public string UserMessage;
        public static IForm<HelloWorldFormFlow> BuildForm()
        {
            OnCompletionAsyncDelegate<HelloWorldFormFlow> userMessage = async (context, state) =>
            {
                
                await context.PostAsync("Hello World: "+state.UserMessage);
            };
            return new FormBuilder<HelloWorldFormFlow>()
                    .Field(nameof(HelloWorldFormFlow.UserMessage))
                    .OnCompletion(userMessage)
                    .Build();
        }
    };
}
