namespace MockSequencer.Windows;

using DalaMock.Shared.Interfaces;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;

public class MainWindow : Window
{
    private readonly IFont font1;

    public MainWindow(IFont font)
        : base("MockSequencer")
    {
        this.font1 = font;
    }

    public override void Draw()
    {
        ImGui.TextUnformatted("Hello, world!");

        
    }
}