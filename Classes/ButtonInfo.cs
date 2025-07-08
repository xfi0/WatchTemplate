using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchTemplate.Classes
{
    public class ButtonInfo //lowk took the idea from iidk
    {
        public string ButtonText = "PlaceHolder";
        public bool Togglable = true;
        public Action EnableAction = null;
        public Action DisableAction = null;
        public string Notification = "I forgot to add a notification";
    }
}
