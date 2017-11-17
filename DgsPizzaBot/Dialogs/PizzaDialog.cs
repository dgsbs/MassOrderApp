using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Microsoft.Bot.Connector;

namespace Microsoft.Bot.Sample.QnABot
{
    [Serializable]
    public class PizzaDialog : IDialog<object>
    {
        private readonly BasicQnAMakerDialog defaultDialog = new BasicQnAMakerDialog();

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            if (message.Text.Contains("pizza"))
            {
                await context.PostAsync("Do you want to order pizza? I recommend order from PERUGIA. You can visit http://perugia.pl/ and make your order.");
                context.Wait(MessageReceivedAsync);
                return;
            }
            else
            {
                await this.defaultDialog.MessageReceivedAsync(context, argument);
            }
            
        }
    }

    namespace Microsoft.Bot.Sample.SimpleSandwichBot
    {
        public enum MealTypes
        {
            Salat, Pasta, Pizza
        }

        public enum Size
        {
            Small, Medium, Large
        }

        public enum Pizza
        {
            CosaNostra,
            Pollo,
            Romana,
            Peperoni,
            Crudo,
            Diavola,
            Mexicana,
            Mafia,
            Calabrese
        }

        [Serializable]
        public class PizzaOrder
        {
            [Prompt("What kind of meal would you like?{||}")]
            public MealTypes? MealType;
            public Size? Size;
            [Optional]
            [Prompt("Would you like additional sauce for your order? {||}")]
            public bool? Sauce;

            [Prompt("Select pizza: {||}")]
            public Pizza? Pizza;

            public static IForm<PizzaOrder> BuildForm()
            {
                OnCompletionAsyncDelegate<PizzaOrder> processOrder = async (context, state) =>
                {
                    await context.PostAsync("We are currently processing your order :)");
                };

                return new FormBuilder<PizzaOrder>()
                    .Message("Welcome to Pizza ordering form")
                    //
                    // MEAL TYPE
                    //
                    .Field(nameof(MealType), validate: async (state, value) =>
                    {
                        var result = new ValidateResult { IsValid = true, Value = value };

                        var meal = (MealTypes?) value;

                        if (meal.HasValue && meal.Value == MealTypes.Pizza)
                        {
                            result.Feedback = "Good choice";
                        }

                        return result;
                    })
                    //
                    // SIZE
                    //
                    .Field(nameof(Size), validate: async (state, value) =>
                    {
                        var result = new ValidateResult { IsValid = true, Value = value };

                        var size = (SimpleSandwichBot.Size)value;
                        if (size == SimpleSandwichBot.Size.Small)
                        {
                            result.Feedback = "You should eat something bigger!";
                        }
                        else if (size == SimpleSandwichBot.Size.Large && state.MealType.Value == MealTypes.Pizza)
                        {
                            result.Feedback = "Are you sure you will eat large pizza? I don't think so!";
                        }

                        return result;
                    })
                    .Field(new FieldReflector<PizzaOrder>(nameof(Pizza))
                        .SetType(null)
                        .SetActive((state) => state.MealType == MealTypes.Pizza))
                        //.SetDefine(async (state, field) =>
                        //{
                        //    field
                        //        .AddDescription("cookie", "Free cookie")
                        //        .AddTerms("cookie", "cookie", "free cookie")
                        //        .AddDescription("drink", "Free large drink")
                        //        .AddTerms("drink", "drink", "free drink");
                        //    return true;
                        //}))

                    //
                    // ADDITIONAL SAUCE
                    //
                    .Field(nameof(PizzaOrder.Sauce))
                    .Confirm("Do you want to order your {Size} {MealType}?")
                    .AddRemainingFields()
                    .OnCompletion(processOrder)
                    .Build();
            }
        };
    }



}