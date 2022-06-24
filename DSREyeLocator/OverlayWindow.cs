﻿using ECommons.MathHelpers;
using ImGuiScene;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSREyeLocator
{
    internal class OverlayWindow : Window, IDisposable
    {
        Vector2 WinSize;
        TextureWrap imgYes, imgNo1, imgNo2;
        internal bool Correct = false;
        
        public OverlayWindow() : base("DSREye Overlay",
            ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysUseWindowPadding | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.AlwaysAutoResize |
            ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoFocusOnAppearing,
            true)
        {
            this.RespectCloseHotkey = false;
            imgYes = Svc.PluginInterface.UiBuilder.LoadImage(Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, "yes.png"));
            imgNo1 = Svc.PluginInterface.UiBuilder.LoadImage(Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, "no1.png"));
            imgNo2 = Svc.PluginInterface.UiBuilder.LoadImage(Path.Combine(Svc.PluginInterface.AssemblyLocation.DirectoryName, "no2.png"));
            this.IsOpen = true;
        }

        public override void PreDraw()
        {
            ImGuiHelpers.SetNextWindowPosRelativeMainViewport(new(ImGuiHelpers.MainViewport.Size.X / 2 - WinSize.X / 2, P.config.VerticalOffset));
        }

        public override bool DrawConditions()
        {
            return P.config.EnableBanner && 
                (P.config.Test || (Svc.ClientState.TerritoryType == 968 && (P.IsDeath() || P.IsSanctity()) && P.EyesPositions.ContainsKey(P.EyePos)));
        }

        public override void Draw()
        {
            if (P.config.Test) Correct = Svc.Targets.Target != null?!ConeHandler.IsInCone(Svc.Targets.Target.Position.ToVector2()):Environment.TickCount % 5000 > 2500;
            WinSize = ImGui.GetWindowSize();
            if (Correct)
            {
                ImGui.Image(imgYes.ImGuiHandle, new(imgYes.Width, imgYes.Height));
            }
            else
            {
                var image = P.config.BannerBlink && Environment.TickCount % 400 > 200 ? imgNo1 : imgNo2;
                ImGui.Image(image.ImGuiHandle, new(image.Width, image.Height));
            }
        }

        public void Dispose()
        {
            imgYes.Dispose();
            imgNo1.Dispose();
            imgNo2.Dispose();
        }
    }
}