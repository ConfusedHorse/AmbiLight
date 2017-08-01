using AmbiLight.ViewModel.Models;
using AmbiLight.ViewModel.Models.Modes;
using AmbiLight.ViewModel.Models.Modes.CustomModes.ForceOfNature;
using AmbiLight.ViewModel.Models.Modes.CustomModes.Miscellaneous;
using AmbiLight.ViewModel.Models.Modes.CustomModes.Music;
using AmbiLight.ViewModel.Models.Modes.Groups;
using Ninject.Modules;

namespace AmbiLight.ViewModel.Module
{
    public class ViewModelModule : NinjectModule
    {
        public override void Load()
        {
            Bind<AmbiLightViewModel>().ToSelf().InSingletonScope();

            // Modes
            Bind<IAmbiLightMode, SingleColorMode>().To<SingleColorMode>().InSingletonScope();
            Bind<IAmbiLightMode, AmbiLightMode>().To<AmbiLightMode>().InSingletonScope();
            Bind<IAmbiLightMode, AmbiLightSaturatedMode>().To<AmbiLightSaturatedMode>().InSingletonScope();

            Bind<IAmbiLightMode, MusicLevelSingleColorMode>().To<MusicLevelSingleColorMode>().InSingletonScope();
            Bind<IAmbiLightMode, MusicLevelRainbowMode>().To<MusicLevelRainbowMode>().InSingletonScope();
            Bind<IAmbiLightMode, MusicLevelMode>().To<MusicLevelMode>().InSingletonScope();

            Bind<IAmbiLightMode, RainbowMode>().To<RainbowMode>().InSingletonScope();
            Bind<IAmbiLightMode, FireMode>().To<FireMode>().InSingletonScope();
            Bind<IAmbiLightMode, WaterMode>().To<WaterMode>().InSingletonScope();
            Bind<IAmbiLightMode, EarthMode>().To<EarthMode>().InSingletonScope();
            Bind<IAmbiLightMode, LeafMode>().To<LeafMode>().InSingletonScope();
            Bind<IAmbiLightMode, FlashMode>().To<FlashMode>().InSingletonScope();

            // Groups
            Bind<IAmbiLightMode, MusicModeGroup>().To<MusicModeGroup>().InSingletonScope();
            Bind<IAmbiLightMode, MiscellaneousModeGroup>().To<MiscellaneousModeGroup>().InSingletonScope();
            Bind<IAmbiLightMode, ForceOfNatureModeGroup>().To<ForceOfNatureModeGroup>().InSingletonScope();
        }
    }
}