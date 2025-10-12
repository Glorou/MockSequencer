using System;
using System.Numerics;

namespace MockSequencer.Windows;

using DalaMock.Shared.Interfaces;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using Dalamud.Bindings.ImGuizmo;

public class MainWindow : Window
{
    private readonly IFont font1;

    public float[] Cameraview = {
        1f, 0f, 0f, 0f,
        0f, 1f, 0f, 0f,
        0f, 0f, 1f, 0f,
        0f, 0f, 0f, 1f
    }; 
    public float[] Cameraprojection = {
        1f, 0f, 0f, 0f,
        0f, 1f, 0f, 0f,
        0f, 0f, 1f, 0f,
        0f, 0f, 0f, 1f
    }; 
    
    public float[][] objectMatrix ={
        new float[] {1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1 },

        new float[] {1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1 },

        new float[] {1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1 },

        new float[] {1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1 }
    };
    public MainWindow(IFont font)
        : base("MockSequencer")
    {
        this.font1 = font;
    }

    
    public unsafe override void Draw()
    {

        float* pCameraview = stackalloc float[16];
        float* pCameraprojection = stackalloc float[16];
        float* pobjectMatrix = stackalloc float[64];
        
        for(var i = 0; i < 16; i++)
        {
            pCameraprojection[i] = Cameraprojection[i];
            pCameraview[i] = Cameraview[i];
            for (var j = 0; j < 4; j++)
            {
                pobjectMatrix[i] = objectMatrix[j][i];
            }
        }

        
        ImGuizmo.DrawCubes(pCameraview, pCameraprojection, pobjectMatrix, 4);
        
        //ImViewGuizmo.ImViewGuizmo.;

        //ImGuizmo.DrawCubes()
        
        
    }
    
}