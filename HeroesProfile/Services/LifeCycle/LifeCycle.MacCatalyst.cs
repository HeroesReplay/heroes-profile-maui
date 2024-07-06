using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.LifecycleEvents;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroesProfile.UI.Services.LifeCycle;

public static partial class Lifecycle
{
    public static void Configure(IiOSLifecycleBuilder configure)
    {

    }

    public static void AddPlatformEvents(ILifecycleBuilder lifecycle)
    {
        lifecycle.AddiOS(Configure);
    }
}
