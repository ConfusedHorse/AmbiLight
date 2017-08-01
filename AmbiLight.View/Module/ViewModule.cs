using AmbiLight.View.Controls;
using Ninject.Modules;

namespace AmbiLight.View.Module
{
    public class ViewModule : NinjectModule
    {
        public override void Load()
        {
            Bind<AmbiLightTray>().ToSelf().InSingletonScope();
            Bind<AmbiLightSubModes>().ToSelf().InSingletonScope();
            Bind<AmbiLightOptions>().ToSelf().InSingletonScope();
        }
    }
}