using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchTemplate.Classes;

namespace WatchTemplate.Menu
{
    internal class Buttons
    {
        public static ButtonInfo[][] buttons = new ButtonInfo[][]
        {
            new ButtonInfo[]
            {
                new ButtonInfo { ButtonText = "PlaceHolder", EnableAction = () => PhotonNetwork.Disconnect(), Togglable = false, Notification = "This Is A Test Notificaiton For The Disconnect Button" }
            }
        };
    }
}
