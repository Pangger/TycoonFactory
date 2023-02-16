using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Plugin;
using MvvmCross.ViewModels;
using TycoonFactory.Core.Managers;
using TycoonFactory.Core.Managers.Interfaces;
using TycoonFactory.DAL.Repositories;
using TycoonFactory.DAL.Repositories.Interfaces;

namespace TycoonFactory.ViewModels;

public class Setup : MvxApplication
{
    public override void Initialize()
    {
        RegisterTypes();

        RegisterAppStart<MainViewModel>();
    }

    public override void LoadPlugins(IMvxPluginManager pluginManager)
    {
        pluginManager.EnsurePluginLoaded<MvvmCross.Plugin.Messenger.Plugin>();
    }

    private void RegisterTypes()
    {
        Mvx.IoCProvider.RegisterType<IActivityRepository, ActivityRepository>();
        Mvx.IoCProvider.RegisterType<IAndroidRepository, AndroidRepository>();
        Mvx.IoCProvider.RegisterType<IActivityManager, ActivityManager>();
        Mvx.IoCProvider.RegisterType<IAndroidManager, AndroidManager>();
    }
}