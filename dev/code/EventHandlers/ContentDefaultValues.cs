using System;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace code.EventHandlers
{
    internal class ContentDefaultValues
    {
        public ContentDefaultValues()
        {
            ContentService.Created += ContentService_Created;

            ContentService.Published += ContentService_Published;

            ContentService.Saved += ContentService_Saved;
        }

        private void ContentService_Created(IContentService sender, NewEventArgs<IContent> e)
        {
            //Bruges til at udføre custom-handlinger når et dokument er oprettet
            //Eksempel: sæt en default dato på egenskaben "contentDate"
            //if (e.Alias == "Kunde-Nyhed")
            //{
            //    e.Entity.SetValue(Constants.SkyConstants.Properties.ContentDate, DateTime.Now);
            //}
        }

        private void ContentService_Published(Umbraco.Core.Publishing.IPublishingStrategy sender, Umbraco.Core.Events.PublishEventArgs<IContent> e)
        {
            //Bruges til at udføre custom-handlinger når et dokument er udgivet
            
            Startup.ContentChangesGuid = Guid.NewGuid();    //clear localstorage on change
        }

        private void ContentService_Saved(IContentService sender, Umbraco.Core.Events.SaveEventArgs<IContent> e)
        {
            //Bruges til at udføre custom-handlinger når et dokument er gemt
            //Eksempel: Opretter en side under siden, hvis der ikke findes en af samme dokumentType i forvejen
            //foreach (var content in e.SavedEntities)
            //{
            //    if (content.ContentType.Alias.Equals("SOSU-Uddannelse"))
            //    {
            //        //Tjek om siden har en node af typen "SOSU-Kontainer-Uddannelse-Mediekarussel" i forvejen
            //        if (!content.Children().Any(x => x.ContentType.Alias.Equals("SOSU-Kontainer-Uddannelse-Mediekarussel")))
            //        {
            //            var mediaContainer = sender.CreateContent("Mediekarussel", content, "SOSU-Kontainer-Uddannelse-Mediekarussel");
            //            sender.SaveAndPublish(mediaContainer);
            //        }
            //    }
            //}
        }
    }
}
