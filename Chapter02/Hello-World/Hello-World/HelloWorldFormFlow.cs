using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello_World
{
    [Serializable]
    class HelloWorldFormFlow
    {
        public static IForm<HelloWorldFormFlow> BuildForm()
        {
            return new FormBuilder<HelloWorldFormFlow>()
                    .Message("Hello World!")
                    .Build();
        }
    };
}
