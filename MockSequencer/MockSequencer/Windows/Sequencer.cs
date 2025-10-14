using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ImSequencer;

namespace MockSequencer;

public struct Item
{
    public uint color;
    public int start;
    public int end;
    public ItemType type; //switch this to enum later
    public bool expanded;

    public Item(ItemType _type, int _start, int _end, bool _expanded)
    {
        start = _start;
        end = _end;
        type = _type;
        expanded = _expanded;
    }
    //public Item(int _type, int _start, int _end, bool _expanded) => Item((ItemType)_type, _start, _end, expanded);
}

public enum ItemType
{
    Camera,
    Light,
    Bone
}

public class Sequencer : Window, IDisposable, SequenceInterface
{
    private Plugin Plugin;

    private int frameMin = 0;
    private int frameMax = 200;

    private RampEdit _rampEdit = new RampEdit();
    private ImRect _imRect;

    public List<Item> items = new List<Item>();
    private int lastExpanded = -1;
    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public Sequencer(Plugin plugin)
        : base("My Amazing Sequencer##With a hidden ID",
            ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        Plugin = plugin;
    }

    public void Dispose()
    {
    }


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
        ImGui.PushItemWidth(75f);
        ImGui.InputInt("Frame Min", ref frameMin);
        ImGui.SameLine();
        ImGui.InputInt("Frame Max", ref frameMax);
        ImGui.Separator();

        ImGui.PopItemWidth();
        ImSequencer.ImSequencer.Sequencer(this, ref currFrame, ref expanded, ref index, ref firstFrame, 0);
    }

    public bool focused { get; set; }
    public int GetFrameMin() => frameMin;


    public int GetFrameMax() => frameMax;


    public int GetItemCount() => items.Count;


    public void BeginEdit(int index)
    {
        throw new NotImplementedException();
    }

    public void EndEdit()
    {
        throw new NotImplementedException();
    }

    public int GetItemTypeCount() => Enum.GetNames(typeof(ItemType)).Length;


    public string GetItemTypeName(int index) => items[index].type.ToString();

    public string GetItemLabel(int index)
    {
        return $"[{index.ToString()}] {Enum.GetName(items[index].type)}";
    }

    public string GetCollapseFmt(int frameCount, int sequenceCount)
    {
        return $"{frameCount.ToString()} frames / {sequenceCount.ToString()} entries";
    }

    public void Get(int index, out int start, out int end, out int type, out uint color)
    {

        start = items[index].start;
        end = items[index].end;
        type = (int)items[index].type;
        color = items[index].color;
    }

    public void Add(int index)
    {
        items.Add(new Item()
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
        items.RemoveAt(index);
    }

    public void Duplicate(int index)
    {
        items.Add(items[index]);
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
        return (uint)(items[index].expanded ? 300 : 0);
    }

    public void DoubleClick(int index)
    {

        var item = items[index];
        if (item.expanded)
        {
            item.expanded = false;
            return;
        }

        items.ForEach((i) => { i.expanded = false; });
        item.expanded = true;

    }

    public void CustomDraw(
        int index, ImDrawListPtr drawList, ref ImRect customRect, ref ImRect legendRect, ref ImRect clippingRect,
        ref ImRect legendClippingRect)
    {
        var labels = new[] { "Translation", "Rotation", "Scale" };
        _rampEdit.Max = new Vector2((float)(frameMax), 1.0f);
        _rampEdit.Min = new Vector2((float)(frameMin), 0.0f);
        drawList.PushClipRect(clippingRect.Min, clippingRect.Max, true);
        for (int i = 0; i < 3; i++)
        {
            Vector2 pta = new Vector2(legendRect.Min.X + 30, legendRect.Min.Y + i * 14f);
            Vector2 ptb = new Vector2(legendRect.Max.X, legendRect.Min.Y + (i + 1) * 14f);
            drawList.AddText(pta, _rampEdit.mbVisible[i] ? 0xFFFFFFFF : 0x80FFFFFF, GetItemLabel(index));
            _imRect = new ImRect(pta, ptb);
            if (_imRect.Contains(ImGui.GetMousePos()) && ImGui.IsMouseClicked(0))
                _rampEdit.mbVisible[i] = !_rampEdit.mbVisible[i];
        }

        drawList.PopClipRect();
    }

    public void CustomDrawCompact(int index, ImDrawListPtr drawList, ref ImRect customRect, ref ImRect clippingRect)
    {
        _rampEdit.Max = new Vector2((float)(frameMax), 1.0f);
        _rampEdit.Min = new Vector2((float)(frameMin), 0.0f);
        drawList.PushClipRect(clippingRect.Min, clippingRect.Max, true);
        for (int i = 0; i < 3; i++)
        {
            for (uint j = 0; j < _rampEdit.mPointCount[i]; j++)
            {
                float p = _rampEdit.mPts[i][j].X;
                if (p < items[index].start || p > items[index].end)
                    continue;
                float r = (p - frameMin) / (float)(frameMax - frameMin);
                float x = Extensions.ImLerp(customRect.Min.X, customRect.Max.X, r);
                drawList.AddLine(new Vector2(x, customRect.Min.Y + 6), new Vector2(x, customRect.Max.Y - 4), 0xAA000000,
                    4.0f);

            }
        }
    }
}
