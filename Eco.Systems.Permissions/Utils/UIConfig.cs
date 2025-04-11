using Eco.Core.Controller;
using Eco.Core.Serialization;
using Eco.Core.Utils;
using Eco.Gameplay.Items;
using Eco.Gameplay.Players;
using Eco.Gameplay.PropertyHandling;
using Eco.Gameplay.Utils;
using Eco.Shared.Networking;
using Eco.Shared.Serialization;
using Eco.Shared.Utils;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco.Systems.Permissions.Utils
{
    [Serialized]
    public class EPSData : Singleton<EPSData>, IStorage
    {
        [Serialized] public ThreadSafeDictionary<int, ModConfig> ModConfiguration = new();
        public IPersistent? StorageHandle { get; set; }
    }

    [Serialized]
    public class ModConfig
    {
        [Serialized] public bool WipeWorldOnReset { get; set; }

        public EPSConfigUI ToUI()
        {
            EPSConfigUI ui = new()
            {
                WipeWorldOnReset = Plugin.Obj.Config.WipeGroupsFileOnFreshWorld
            };
            return ui;
        }

        public void UpdateFromUI(EPSConfigUI config)
        {
            Plugin.Obj.Config.WipeGroupsFileOnFreshWorld = config.WipeWorldOnReset;
        }
    }

    public class EPSConfigUI : IController, INotifyPropertyChanged, Eco.Core.PropertyHandling.INotifyPropertyChangedInvoker, IHasClientControlledContainers
    {
        [Eco] public bool WipeWorldOnReset { get; set; }


        public EPSConfigUI()
        {
            WipeWorldOnReset = Plugin.Obj.Config.WipeGroupsFileOnFreshWorld;
        }


        #region IController
        public event PropertyChangedEventHandler? PropertyChanged;
        int controllerID;
        [DoNotNotify] public ref int ControllerID => ref controllerID;

        public void InvokePropertyChanged(PropertyChangedEventArgs eventArgs)
        {
            if (PropertyChanged == null)
                return;
            PropertyChanged(this, eventArgs);
        }

        protected void OnPropertyChanged(string propertyName, object before, object after) => PropertyChangedNotificationInterceptor.Intercept(this, propertyName, before, after);
        #endregion
    }
}