using Ninject;

namespace AmbiLight.CrossCutting.Module
{
    public class AmbiLightKernel
    {
        private static IKernel _instance;

        public static IKernel Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new StandardKernel();
                _instance.Load("AmbiLight.*.dll");
                return _instance;
            }
        }
    }
}