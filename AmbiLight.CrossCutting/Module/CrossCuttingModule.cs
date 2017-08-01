using AmbiLight.CrossCutting.Helpers;
using Ninject.Modules;

namespace AmbiLight.CrossCutting.Module
{
    public class CrossCuttingModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ScreenHelper>().ToSelf().InSingletonScope();
            Bind<CommunicationHelper>().ToSelf().InSingletonScope();
            Bind<AudioEventHelper>().ToSelf().InSingletonScope();
        }
    }
}