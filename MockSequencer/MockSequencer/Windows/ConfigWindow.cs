namespace MockSequencer.Windows;

using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;

public class ConfigWindow(Configuration config) : Window("MockSequencer Config")
{
    public override void Draw()
    {
        var configOption = config.ConfigOption;

        if (ImGui.Checkbox("Config Option", ref configOption))
        {
            config.ConfigOption = configOption;
        }
    }
}