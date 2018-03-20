using System;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System.Text.RegularExpressions;
public enum Ecc { Esi = 1, Autre };
public enum Gendr { Garçon = 1, Fille };

namespace HelloFormFlowBot
{
    [Serializable]
    public class ProfileForm
    {
        
        public string Name { get; set; }
        public string Email { get; set; }
        public Gendr Sexe { get; set; }
        public Ecc Choice { get; set; }
        public string Studie { get; set; }
        public string Motivation { get; set; }
        public string WhatWeEarn { get; set; }
        public string centreInteret { get; set; }
        

        // This method 'builds' the form 
        // This method will be called by code we will place
        // in the MakeRootDialog method of the MessagesControlller.cs file
        public static IForm<ProfileForm> BuildForm()
        {
            Regex rgx = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            return new FormBuilder<ProfileForm>()
           .Message("Tu veux rejoindre le CSE ? Pas de problèmes. Mais avant toute chose, j’aimerais bien qu’on fasse connaissance, ça te dit?")
            .Message("Moi je suis le CSE bot")
            .Field(nameof(Name), "Et toi tu t’appelles comment?")
            .Message("Ravi {Name}")
            .Message("Fille ou un garçon ?")
            .Field(nameof(Sexe))
            .Message("Enchanté !")
            .Field(nameof(centreInteret), "Je voudrais en savoir un peu plus sur toi ! Dis-moi, quels sont tes centres d’intérêts ?")
            .Message("Passionnant !")
            .Message("Moi j’aime bien tout ce qui est informatique.")
            .Message("En parlant d’informatique, je suis de l’ESI, et toi {Name} tu étudies où?")
            .Field(nameof(Choice))
            .Message("Ah super :D !")
            .Field(nameof(Studie), "Et tu es en quelle année? {||}")
            .Field(nameof(Motivation), "Tu sais, on essaye d’améliorer la vie étudientine à l’ESI, comment penses tu nous aider?")
            .Message("Je vois Cool! :D")
            .Field(nameof(WhatWeEarn), "Une dernière question, le plus important, qu’attends tu de nous?")
            .Message("On va faire de notre mieux pour ne pas te décevoir :D !")
            .Message("Super!")
            .Field(nameof(Email), "Hey avant de partir, je voudrais avoir ton e-mail afin qu’on reste en contact!",
            validate: async (state, value) =>
            {
                var result = new ValidateResult { IsValid = true, Value = value };
                var address = (value as string);

                if (!rgx.IsMatch(address))
                {
                    result.Feedback = "Incorrect e-mail";
                    result.IsValid = false;
                }

                return result;
            })
            .AddRemainingFields()
            .Message("Tous-est correct?")
            .Confirm("Si vous voulez changez quelque chose entrez Non ,Sinon entrez Oui")
            .Message("Merci {Name} à très bientôt pour de nouvelles aventures!")
            .Message("entrez n'importe quoi pour valider votre réponse")
            .OnCompletion(async (context, profileForm) =>
                    {
                        // Set BotUserData
                        context.PrivateConversationData.SetValue<bool>(
                            "ProfileComplete", true);
                        context.PrivateConversationData.SetValue<string>(
                            "Name", profileForm.Name);
                        context.PrivateConversationData.SetValue<string>(
                            "Email", profileForm.Email);
                        context.PrivateConversationData.SetValue<string>(
                            "Sexe", profileForm.Sexe.ToString());
                        context.PrivateConversationData.SetValue<string>(
                           "Choice", profileForm.Choice.ToString());
                        context.PrivateConversationData.SetValue<string>(
                            "Studie", profileForm.Studie);
                        context.PrivateConversationData.SetValue<string>(
                           "Motivation", profileForm.Motivation);
                        context.PrivateConversationData.SetValue<string>(
                           "WhatWeEarn", profileForm.WhatWeEarn);
                        context.PrivateConversationData.SetValue<string>(
                           "centreInteret", profileForm.centreInteret);



                        // Tell the user that the form is complete
                    })
                    .Build();
        }
    }

    // This enum provides the possible valuse for the 
    // Gender property in the ProfileForm class
    // Notice we start the options at 1 
    
}