using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gma.System.MouseKeyHook;

namespace OverlayClickinterceptor
{
    // Singleton design pattern to prevent multiple instances. All forms should share this one instance.
    public static class GlobalHooks
    {
        public static IKeyboardMouseEvents GlobalHookEvents
        {
            get
            {
                if (_globalHookEvents == null)
                {
                    _globalHookEvents = Hook.GlobalEvents();
                }
                return _globalHookEvents;
            }
        }
        private static IKeyboardMouseEvents _globalHookEvents = null;

        public static void Dispose()
        {
            if (_globalHookEvents != null)
            {
                _globalHookEvents.Dispose();
                _globalHookEvents = null;
            }
        }
    }
}
