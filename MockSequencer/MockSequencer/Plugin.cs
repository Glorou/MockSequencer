using System.Collections.Generic;

namespace MockSequencer;

using System.Reflection;
using Autofac;
using DalaMock.Host.Hosting;
using MockSequencer.Services;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Microsoft.Extensions.DependencyInjection;
using ImSequencer;

public class Plugin : HostedPlugin
{
    public Sequencer Sequencer { get; init; }
    public List<Item> items;
    public Plugin(
        IDalamudPluginInterface pluginInterface,
        IPluginLog pluginLog,
        IFramework framework,
        ICommandManager commandManager,
        IDataManager dataManager,
        ITextureProvider textureProvider,
        IChatGui chatGui,
        IDtrBar dtrBar
        )
        : base(pluginInterface, pluginLog, framework, commandManager, dataManager, textureProvider, chatGui, dtrBar)
    {
        Sequencer = new Sequencer(this);

        var temp = new Item
        {
            color = 0xFF0000,
            end = 20,
            start = 0,
            type = 0,
            expanded = false
        };
        Sequencer.items.Add(temp);
        temp.expanded = true;
        temp.end = 40;
        temp.type = ItemType.Bone;
        Sequencer.items.Add(temp);
        temp.expanded = false;
        temp.start = 35;
        temp.type = ItemType.Light;
        Sequencer.items.Add(temp);
        Sequencer.items.Add(temp);
        pluginInterface.UiBuilder.Draw += Sequencer.Draw;
        this.CreateHost();
        this.Start();
    }

    /// <summary>
    /// Configures the optional services to register automatically for use in your plugin.
    /// </summary>
    /// <returns>A HostedPluginOptions configured with the options you require.</returns>
    public override HostedPluginOptions ConfigureOptions()
    {
        return new HostedPluginOptions()
        {
            UseMediatorService = true,
        };
    }

    public override void ConfigureContainer(ContainerBuilder containerBuilder)
    {
        // While you can register services in the service collection, as long as you register a service as IHostedService(the AsImplementedInterfaces call) it will automatically be picked up by the host. This also avoids potential double registrations.
        containerBuilder.RegisterType<WindowService>().AsSelf().AsImplementedInterfaces().SingleInstance();
        containerBuilder.RegisterType<ConfigurationService>().AsSelf().AsImplementedInterfaces().SingleInstance();
        containerBuilder.RegisterType<CommandService>().AsSelf().AsImplementedInterfaces().SingleInstance();
        containerBuilder.RegisterType<InstallerWindowService>().AsSelf().AsImplementedInterfaces().SingleInstance();

        // Register every class ending in Window inside our assembly with the container
        containerBuilder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(t => t.Name.EndsWith("Window"))
            .As<Window>()
            .AsSelf()
            .AsImplementedInterfaces();

        // Register the configuration with the container so that it's loaded/created when requested.
        containerBuilder.Register(s =>
        {
            var configurationLoaderService = s.Resolve<ConfigurationService>();
            return configurationLoaderService.GetConfiguration();
        }).SingleInstance();
    }


    public override void ConfigureServices(IServiceCollection serviceCollection)
    {
    }
}