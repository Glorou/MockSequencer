using System;
using System.Collections.Generic;
using System.Numerics;
using ImSequencer.ImCurveEdit;
namespace MockSequencer;

public class RampEdit : CurveContext
{
    public RampEdit()
    {
        mPts[0][0] = new Vector2(0, 0);
        mPts[0][1] = new Vector2(0.25f, 0.25f);
        mPointCount[0] = 2;

        mbVisible[0] = true;
        Max = new Vector2(1.0f, 1.0f);
        Min = new Vector2(0.0f, 0.0f);
    }


    public override int GetCurveCount()
    {
        return 1;
    }

    public override bool IsVisible(int curveIndex)
    {
        return mbVisible[curveIndex];
    }

    public override int GetPointCount(int curveIndex)
    {
        return mPointCount[curveIndex];
    }

    public override uint GetCurveColor(int curveIndex)
    {
        uint[] cols = { 0xFF0000FF, 0xFF00FF00, 0xFFFF0000 };
        return cols[curveIndex];
    }

    public override Span<Vector2> GetPoints(int curveIndex)
    {
        return mPts[curveIndex];
    }

    public override CurveType GetCurveType(int curveIndex)
    {
        return CurveType.CurveSmooth;
    }

    private int InsertPoint(int curveIndex, Vector2 value)
    {
        var points = mPts[curveIndex];
        var count = mPointCount[curveIndex]++;

        var insertIndex = Array.BinarySearch(points, 0, count, value, Comparer.Instance);
        if (insertIndex < 0) insertIndex = ~insertIndex;

        Array.Copy(points, insertIndex, points, insertIndex + 1, count - insertIndex);

        points[insertIndex] = value;
        return insertIndex;
    }

    public override int EditPoint(int curveIndex, int pointIndex, Vector2 value)
    {
        var points = mPts[curveIndex];
        var count = mPointCount[curveIndex];

        var canStay = (pointIndex == 0 || value.X >= points[pointIndex - 1].X) &&
                      (pointIndex == count - 1 || value.X <= points[pointIndex + 1].X);

        if (canStay)
        {
            points[pointIndex] = value;
            return pointIndex;
        }

        var insertIndex = Array.BinarySearch(points, 0, mPointCount[curveIndex], value, Comparer.Instance);
        if (insertIndex < 0) insertIndex = ~insertIndex;

        if (insertIndex < pointIndex)
        {
            Array.Copy(points, insertIndex, points, insertIndex + 1, pointIndex - insertIndex);
        }
        else
        {
            Array.Copy(points, pointIndex + 1, points, pointIndex, insertIndex - pointIndex - 1);
            insertIndex--;
        }

        points[insertIndex] = value;
        return insertIndex;
    }

    public override void AddPoint(int curveIndex, Vector2 value)
    {
        if (mPointCount[curveIndex] >= 8)
            return;
        InsertPoint(curveIndex, value);
    }

    public override uint GetBackgroundColor()
    {
        return 0;
    }

    public Vector2[][] mPts = [new Vector2[8]];
    public int[] mPointCount = new int[3];
    public bool[] mbVisible = new bool[3];

    private class Comparer : IComparer<Vector2>
    {
        public static readonly Comparer Instance = new();

        public int Compare(Vector2 a, Vector2 b)
        {
            return a.X.CompareTo(b.X);
        }
    }

    private void SortValues(int curveIndex)
    {
        Array.Sort(mPts[curveIndex], Comparer.Instance);
    }
}