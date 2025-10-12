using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

using ImSequencer;

namespace MockSequencer;

public struct Sequence
{
    public uint color;
    public int start;
    public int end;
    public int type; //switch this to enum later
    public bool expanded;

}
public class Sequencer : Window, IDisposable, SequenceInterface
{
    private Plugin Plugin;


    
    
    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public Sequencer(Plugin plugin)
        : base("My Amazing Sequencer##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        
        Plugin = plugin;
    }

    public void Dispose() { }


    public override void Draw()
    {
        var ptr = ImGui.GetWindowDrawList();
        ImRect CustomRect = new ImRect(new Vector2(ImGui.GetWindowWidth(), ImGui.GetWindowHeight()));
        ImRect LegendRect = new ImRect(new Vector2(ImGui.GetWindowWidth() * .3f, ImGui.GetWindowHeight()));
        ImRect LegendClip = new ImRect(new Vector2(ImGui.GetWindowWidth() * .3f, ImGui.GetWindowHeight()));
        ImRect CustomClip = new ImRect(new Vector2(ImGui.GetWindowWidth(), ImGui.GetWindowHeight()));
        var index = 0;
        bool expanded = true;
        var currFrame = 0;
        var firstFrame = 0;
        ImSequencer.ImSequencer.Sequencer(this, ref currFrame , ref expanded, ref index, ref firstFrame, 0 );
    }
    public bool focused { get; set; }
    public int GetFrameMin()
    {
        return 0;
    }

    public int GetFrameMax()
    {
        return 200;
    }

    public int GetItemCount()
    {
        return Plugin.items.Count;
    }

    public void BeginEdit(int index)
    {
        throw new NotImplementedException();
    }

    public void EndEdit()
    {
        throw new NotImplementedException();
    }

    public int GetItemTypeCount()
    {
        return 1;
    }

    public string GetItemTypeName(int index)
    {
        return "testing";
    }

    public string GetItemLabel(int index)
    {
        return "testing";
    }

    public string GetCollapseFmt()
    {
        throw new NotImplementedException();
    }

    public void Get(int index, out int start, out int end, out int type, out uint color)
    {

        start = Plugin.items[index].start;
        end = Plugin.items[index].end;
        type = Plugin.items[index].type;
        color = Plugin.items[index].color;
    }

    public void Add(int index)
    {
        Plugin.items.Add(new Sequence()
        {
            color = 0000000,
            end = 60,
            expanded = true,
            start = 40,
            type = 0
        });
    }

    public void Del(int index)
    {
        throw new NotImplementedException();
    }

    public void Duplicate(int index)
    {
        throw new NotImplementedException();
    }

    public void Copy()
    {
        throw new NotImplementedException();
    }

    public void Paste()
    {
        throw new NotImplementedException();
    }

    public uint GetCustomHeight(int index)
    {
        return (uint)(Plugin.items[index].expanded ? 300 : 0);
    }

    public void DoubleClick(int index)
    {

        var item = Plugin.items[index];
        if (item.expanded)
        {
            item.expanded = false;
            return;
        }
        
        Plugin.items.ForEach((i) => { i.expanded = false;});
        item.expanded = true;
            
    }

    public void CustomDraw(
        int index, ImDrawListPtr drawList, ImRect customRect, ImRect legendRect, ImRect clippingRect,
        ImRect legendClippingRect)
    {
        var labels = new[] { "Translation", "Rotation" , "Scale"};

        drawList.PushClipRect(clippingRect.Min, clippingRect.Max, true);
        for (int i = 0; i < 3; i++)
        {
            Vector2 pta = new Vector2(legendRect.Min.X + 30, legendRect.Min.Y + i * 14f);
            Vector2 ptb = new Vector2(legendRect.Max.X, legendRect.Min.Y + (i + 1) * 14f);
            drawList.AddText(pta, 0xFFFFFFFF, "Testing" );
        }
        
        drawList.PopClipRect();
    }

    public void CustomDrawCompact(int index, ImDrawListPtr drawList, ImRect customRect, ImRect clippingRect)
    {
        throw new NotImplementedException();
    }
}
