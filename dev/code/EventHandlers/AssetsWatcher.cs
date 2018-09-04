using System;
using System.IO;

namespace code.EventHandlers
{
    internal class AssetsWatcher{
        private FileSystemWatcher _watcher;

        public AssetsWatcher(){
            //_watcher = new FileSystemWatcher();
            //_watcher.Path = System.Web.HttpContext.Current.Server.MapPath("~/scripts/0000xx/");
            //_watcher.NotifyFilter = NotifyFilters.LastWrite;
            //_watcher.Filter = "*.js";
            //_watcher.Changed += new FileSystemEventHandler(OnChanged);
            //_watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e){
            Startup.JsChangesGuid = Guid.NewGuid();
        }
    }
}
