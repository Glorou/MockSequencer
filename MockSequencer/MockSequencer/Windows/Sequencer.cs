using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.STD.Helper;
using ImSequencer;
using ImRect = Dalamud.Bindings.ImGui.ImRect;

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

public unsafe class RampEdit : ImSequencer.ImCurveEdit.CurveContext
{
    
    public new bool focused = false;
    public Vector2 ScreenMin;
    public Vector2 ScreenMax;
    public Vector2 ScreenRange;
    public Vector2 Min;
    public Vector2 Max;
    public Vector2 Range;
    
    public RampEdit()
    {
        mPts = new List<List<Vector2>>();
        mPts.Add(new List<Vector2>
        {
            new Vector2(10f, 0f),
            new Vector2(20f,0.6f),
            new Vector2(25f, 0.2f),
            new Vector2(70f, 0.4f),
            new Vector2(120f, 1f)
        });

        mPts.Add(new List<Vector2>
        {
            new Vector2(-50f, 0.2f),
            new Vector2(33f, 0.7f),
            new Vector2(80f, 0.2f),
            new Vector2(82, 0.8f)
        });

        mPts.Add(new List<Vector2>
        {
            new Vector2(40f, 0f),
            new Vector2(60f, 0.1f),
            new Vector2(90f, 0.82f),
            new Vector2(150f, 0.24f),
            new Vector2(200f, 0.34f),
            new Vector2(250f, 0.12f)
        });
    }

    private List<List<Vector2>> mPts = [];
    
    public override int GetCurveCount()
    {
        return mPts.Count;
    }

    public override int GetPointCount(int curveIndex)
    {
        return mPts[curveIndex].Count;
    }

    public override uint GetCurveColor(int curveIndex)
    {
        switch (curveIndex)
        {
            case 0:
                return 0xFF0000FF;
            case 1:
                return 0xFF00AA00;
            case 2:
                return 0xFFFF0000;
            default:
                return 0xFF0AAA00;
        }
        
    }

    public override List<Vector2> GetPoints(int curveIndex)
    {
        return mPts[curveIndex];
    }

    public override int EditPoint(int curveIndex, int pointIndex, Vector2 value)
    {
        mPts[curveIndex][pointIndex] = value;
        return 1;
    }

    public override void AddPoint(int curveIndex, Vector2 value)
    {
        mPts[curveIndex][mPts[curveIndex].Count] = value;
    }
}


public class Sequencer : Window, IDisposable, SequenceInterface
{
    private Plugin Plugin;

    private int frameMin = 0;
    private int frameMax = 200;
    public List<Item> items = new List<Item>();
    
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
        rampEdit = new RampEdit();
    }

    public void Dispose() { }
    private int _index;
    private bool _expanded = true;
    private int _currFrame;
    private int _firstFrame;
    public RampEdit rampEdit;

    public override void Draw()
    {
        var ptr = ImGui.GetWindowDrawList();
        ImRect CustomRect = new ImRect(new Vector2(ImGui.GetWindowWidth(), ImGui.GetWindowHeight()));
        ImRect LegendRect = new ImRect(new Vector2(ImGui.GetWindowWidth() * .3f, ImGui.GetWindowHeight()));
        ImRect LegendClip = new ImRect(new Vector2(ImGui.GetWindowWidth() * .3f, ImGui.GetWindowHeight()));
        ImRect CustomClip = new ImRect(new Vector2(ImGui.GetWindowWidth(), ImGui.GetWindowHeight()));

        ImGui.PushItemWidth(75f);
        ImGui.InputInt("Frame Min", ref frameMin);
        ImGui.SameLine();
        ImGui.InputInt("Frame Max", ref frameMax);
        ImGui.Separator();
        
        ImGui.PopItemWidth();


        ImSequencer.ImSequencer.Sequencer(this, ref _currFrame , ref _expanded, ref _index, ref _firstFrame, options);
    }
    public bool focused { get; set; }
    public int GetFrameMin() => frameMin;
    public SEQUENCER_OPTIONS options = SEQUENCER_OPTIONS.SEQUENCER_ADD | SEQUENCER_OPTIONS.SEQUENCER_CHANGE_FRAME |
                                       SEQUENCER_OPTIONS.SEQUENCER_COPYPASTE | SEQUENCER_OPTIONS.SEQUENCER_EDIT_ALL;


    public int GetFrameMax() => frameMax;

    public int GetItemCount() => items.Count;

    public void BeginEdit(int index)
    {
    }

    public void EndEdit()
    {
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
    }

    public void Duplicate(int index)
    {
    }

    public void Copy()
    {
    }

    public void Paste()
    {
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
        
        items.ForEach((i) => { i.expanded = false;});
        item.expanded = true;
            
    }

    public unsafe void CustomDraw(
        int index, ImDrawListPtr drawList, ImRect customRect, ImRect legendRect, ImRect clippingRect,
        ImRect legendClippingRect)
    {
        var labels = new[] { "Translation", "Rotation" , "Scale"};
        rampEdit.Max = new Vector2(frameMax, 1f);
        rampEdit.Min = new Vector2(frameMin, 0f);
        drawList.PushClipRect(clippingRect.Min, clippingRect.Max, true);
        for (int i = 0; i < 3; i++)
        {
            Vector2 pta = new Vector2(legendRect.Min.X + 30, legendRect.Min.Y + i * 14f);
            Vector2 ptb = new Vector2(legendRect.Max.X, legendRect.Min.Y + (i + 1) * 14f);
            drawList.AddText(pta, 0xFFFFFFFF, labels[i]);
            var imRect = new ImRect(pta, ptb);
            if (imRect.Contains(ImGui.GetMousePos()) && ImGui.IsMouseClicked(0))
            {
                //rampEdit
            }

        }
        
        drawList.PopClipRect();
        drawList.PushClipRect(clippingRect.Min, clippingRect.Max, true);
        ImGui.SetCursorScreenPos(customRect.Min);
        
        ImSequencer.ImCurveEdit.ImCurveEdit.Edit(rampEdit, customRect.Max-customRect.Min,(uint)(137+ index) );
        drawList.PopClipRect();
        
    }

    public void CustomDrawCompact(int index, ImDrawListPtr drawList, ImRect customRect,  ImRect clippingRect)
    {

        drawList.PushClipRect(clippingRect.Min, clippingRect.Max, true);
        for (int i = 0; i < 3; i++)
        {
            /*for (uint j = 0; j < _rampEdit.mPointCount[i]; j++)
            {
                float p = _rampEdit.mPts[i][j].X;
                if (p < items[index].start || p > items[index].end)
                    continue;
                float r = (p - frameMin) / (float)(frameMax - frameMin);
                float x = Extensions.ImLerp(customRect.Min.X, customRect.Max.X, r);
                drawList.AddLine(new Vector2(x, customRect.Min.Y + 6), new Vector2(x, customRect.Max.Y - 4), 0xAA000000,
                    4.0f);

            }*/
        }
        drawList.PopClipRect();
    }
}
