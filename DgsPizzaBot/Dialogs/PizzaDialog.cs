using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Newtonsoft.Json;

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

    public class Produkt
    {
        public string Group { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
    }

    [Serializable]
    public class PizzaOrder
    {
        private static Produkt[] produkty;
        private static Produkt[] pastas;
        private static Produkt[] pizzas;
        private static Produkt[] salats;
        
        [Prompt("What kind of meal would you like?{||}")]
        public MealTypes? MealType;

        public Size? Size;

        [Prompt("Would you like additional sauce for your order? {||}")]
        public bool Sauce;

        [Prompt("Select pizza: {||}")]
        public string Pizza;

        [Prompt("Select pasta: {||}")]
        public string Pasta;

        [Prompt("Select salat: {||}")]
        public string Salat;

        public static IForm<PizzaOrder> BuildForm()
        {
            if (PizzaOrder.produkty == null)
            {
                try
                {
                    var asm = Assembly.GetExecutingAssembly();
                    var json = asm.GetManifestResourceStream("QnABot.produkty.json");
                    using (var reader = new StreamReader(json))
                    {
                        var jsonString = reader.ReadToEnd();
                        PizzaOrder.produkty = (Produkt[]) JsonConvert.DeserializeObject(jsonString, typeof(Produkt[]));
                    }
                }catch { }
                
            }

            OnCompletionAsyncDelegate<PizzaOrder> processOrder = async (context, state) =>
            {
                var name = state.Pizza ?? state.Pasta ?? state.Salat;
                var produkt = PizzaOrder.produkty.FirstOrDefault(c => c.Name == name);
                if (produkt != null)
                {
                    double val = 0;
                    var cena = produkt.Price.Split('/');
                    if (cena.Length == 5)
                    {
                        if (state.Size.HasValue)
                        {
                            if (state.Size == SimpleSandwichBot.Size.Small)
                            {
                                double.TryParse(cena[0], out val);
                            }
                            if (state.Size == SimpleSandwichBot.Size.Medium)
                            {
                                double.TryParse(cena[2], out val);
                            }
                            if (state.Size == SimpleSandwichBot.Size.Large)
                            {
                                double.TryParse(cena[4], out val);
                            }
                        }
                    }
                    else if (cena.Length == 3)
                    {
                        if (state.Size.HasValue)
                        {
                            if (state.Size == SimpleSandwichBot.Size.Small)
                            {
                                double.TryParse(cena[0], out val);
                            }
                            if (state.Size == SimpleSandwichBot.Size.Medium)
                            {
                                double.TryParse(cena[2], out val);
                            }
                            if (state.Size == SimpleSandwichBot.Size.Large)
                            {
                                double.TryParse(cena[2], out val);
                            }
                        }
                    }
                    else
                    {
                        double.TryParse(produkt.Price, out val);
                    }
                    var result = FormattableString.Invariant($"Your choice is {name}. Cost = {val} PLN");
                    await context.PostAsync(result);
                }
                
                await context.PostAsync("We are currently processing your meal. We will message you the status.");
            };


            return new FormBuilder<PizzaOrder>()
                .Message("Welcome to Pizza ordering form (In future it will completely replace Wojtek)")
                //
                // MEAL TYPE
                //
                .Field(nameof(PizzaOrder.MealType), validate: async (state, value) =>
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
                    .SetActive(state => state.MealType.HasValue && state.MealType.Value == MealTypes.Pizza)
                    .SetDefine(async (state, field) =>
                    {
                        if (PizzaOrder.pizzas == null)
                        {
                            PizzaOrder.pizzas = PizzaOrder.produkty?.Where(c => c.Group == "pizza" && !string.IsNullOrWhiteSpace(c.Price)).ToArray();
                        }

                        if (PizzaOrder.pizzas != null)
                        {
                            var tmpField = field;
                            foreach (var produkt in PizzaOrder.pizzas)
                            {
                                if (string.IsNullOrWhiteSpace(produkt.Name))
                                {
                                    continue;
                                }
                                tmpField = tmpField
                                    .AddDescription(produkt.Name, produkt.Name)
                                    .AddTerms(produkt.Name, produkt.Name);
                            }
                        }

                        return true;
                    }))
                // PASTA
                //
                .Field(new FieldReflector<PizzaOrder>(nameof(Pasta))
                    .SetType(null)
                    .SetActive(state => state.MealType.HasValue && state.MealType.Value == MealTypes.Pasta)
                    .SetDefine(async (state, field) =>
                    {
                        if (PizzaOrder.pastas == null)
                        {
                            PizzaOrder.pastas = PizzaOrder.produkty?.Where(c => c.Group == "Makaron" && !string.IsNullOrWhiteSpace(c.Price)).ToArray();
                        }

                        if (PizzaOrder.pastas != null)
                        {
                            var tmpField = field;
                            foreach (var produkt in PizzaOrder.pastas)
                            {

                                tmpField = tmpField
                                .AddDescription(produkt.Name, produkt.Name)
                                .AddTerms(produkt.Name, produkt.Name);
                            }
                        }

                        return true;
                    }))
                // PASTA
                //
                .Field(new FieldReflector<PizzaOrder>(nameof(PizzaOrder.Salat))
                    .SetType(null)
                    .SetActive(state => state.MealType.HasValue && state.MealType.Value == MealTypes.Salat)
                    .SetDefine(async (state, field) =>
                    {
                        if (PizzaOrder.salats == null)
                        {
                            PizzaOrder.salats = PizzaOrder.produkty?.Where(c => c.Group == "sałatka" && !string.IsNullOrWhiteSpace(c.Price)).ToArray();
                        }

                        if (PizzaOrder.salats != null)
                        {
                            var tmpField = field;
                            foreach (var produkt in PizzaOrder.salats)
                            {

                                tmpField = tmpField
                                    .AddDescription(produkt.Name, produkt.Name)
                                    .AddTerms(produkt.Name, produkt.Name);
                            }
                        }

                        return true;
                    }))
                //
                // SIZE
                //
                .Field(nameof(PizzaOrder.Size), validate: async (state, value) =>
                {
                    if (state.MealType == MealTypes.Pizza)
                    {
                        var result = new ValidateResult {IsValid = true, Value = value};

                        var size = (Size) value;
                        if (size == SimpleSandwichBot.Size.Small)
                        {
                            result.Feedback = "You should eat something bigger!";
                        }
                        else if (size == SimpleSandwichBot.Size.Large && state.MealType.Value == MealTypes.Pizza)
                        {
                            result.Feedback = "Are you sure you will eat large pizza? I don't think so!";
                        }
                        return result;
                    }
                    
                    return new ValidateResult { IsValid = true, Value = value };
                })
                
                //
                // ADDITIONAL SAUCE
                //
                //.Field(new FieldReflector<PizzaOrder>(nameof(PizzaOrder.Sauce))
                //    .SetType(null)
                //    .SetActive(state => state.MealType.HasValue && state.MealType.Value == MealTypes.Pizza))
                //.Confirm("Do you want to order your {Size} {MealType}?")
                .OnCompletion(processOrder)
                .Build();
        }
    }
}



