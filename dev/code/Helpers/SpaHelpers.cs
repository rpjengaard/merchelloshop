using System;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;
using umbraco.presentation.preview;
using Umbraco.Web;

namespace code.Helpers {

    public static class SpaHelpers {

        public static bool SetPreviewContext(int id) {

            Guid guid = Guid.NewGuid();

            var user = umbraco.BusinessLogic.User.GetUser(0);
            Document document;
            try
            {
                document = new Document(id);
            }
            catch (Exception e)
            {
                return false;
            }
            var pc = new PreviewContent(user, guid, false);
            pc.PrepareDocument(user, document, true);
            pc.SavePreviewSet();
            pc.PreviewSet = guid;
            pc.ActivatePreviewCookie();

            UmbracoContext.Current.Security.PerformLogin(0);

            StateHelper.Cookies.Preview.SetValue(guid.ToString());
            return true;

        }

    }

}