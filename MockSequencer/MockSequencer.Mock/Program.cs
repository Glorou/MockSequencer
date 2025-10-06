using System.IO;
using DalaMock.Core.Mocks;

namespace MockSequencer;

using DalaMock.Core.DI;

internal static class Program
{
    private static void Main(string[] args)
    {
        var dalamudConfiguration = new MockDalamudConfiguration()
        {
            GamePath = new DirectoryInfo("D:\\Games\\FFXIV\\Game Folder\\game\\sqpack"),
            PluginSavePath = new DirectoryInfo("C:\\Users\\Karou\\AppData\\Roaming\\XIVLauncher\\pluginConfigs"),
        };
        var mockContainer = new MockContainer(dalamudConfiguration);
        var mockDalamudUi = mockContainer.GetMockUi();
        var pluginLoader = mockContainer.GetPluginLoader();
        var mockPlugin = pluginLoader.AddPlugin(typeof(MockPlugin));
        pluginLoader.StartPlugin(mockPlugin);
        mockDalamudUi.Run();
    }
}