using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Microsoft.Bot.Connector;

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
        Calabrese,
        Ferrara,
        Vegetariana,
        Affumicata
    }

    public enum Pasta
    {
        SpaghettiPomodore,
        PenneQuattroFormagi,
        PenneBrocoli,
        PenneTonno,
        PenneSalmon
    }

    [Serializable]
    public class PizzaOrder
    {
        [Prompt("What kind of meal would you like?{||}")]
        public MealTypes? MealType;
        public Size? Size;
        [Prompt("Would you like additional sauce for your order? {||}")]
        public bool Sauce;

        [Prompt("Select pizza: {||}")]
        public Pizza? Pizza;

        [Prompt("Select pasta: {||}")]
        public Pasta? Pasta;

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

                    var meal = (MealTypes?)value;

                    if (meal.HasValue && meal.Value == MealTypes.Pizza)
                    {
                        result.Feedback = "Good choice";
                    }

                    return result;
                })

                //
                // PIZZA
                //
                .Field(new FieldReflector<PizzaOrder>(nameof(Pizza))
                    .SetType(null)
                    .SetActive(state => state.MealType == MealTypes.Pizza))
                //
                // PASTA
                //
                .Field(new FieldReflector<PizzaOrder>(nameof(Pasta))
                    .SetType(null)
                    .SetActive(state => state.MealType == MealTypes.Pasta))
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

                //
                // ADDITIONAL SAUCE
                //
                .Field(new FieldReflector<PizzaOrder>(nameof(Sauce))
                    .SetType(null)
                    .SetActive(state => state.MealType == MealTypes.Pizza))
                .Confirm("Do you want to order your {Size} {MealType}?")
                .AddRemainingFields()
                .OnCompletion(processOrder)
                .Build();
        }
    };
}



