using System;

namespace GuiExampleWithImGui
{
  public class MainProgram
  {
    private static Veldrid.Sdl2.Sdl2Window window;
    private static Veldrid.GraphicsDevice gd;
    private static Veldrid.CommandList cl;
    private static Veldrid.ImGuiRenderer controller;

    private static bool showAWindow = true;
    static void Main(string[] args) {
      Veldrid.StartupUtilities.VeldridStartup.CreateWindowAndGraphicsDevice(
        new Veldrid.StartupUtilities.WindowCreateInfo(
          48, 48, 512, 384, Veldrid.WindowState.Normal, "Example"
        ),
        new Veldrid.GraphicsDeviceOptions(true, null, true),
        out window,
        out gd
      );
      window.Resized += () => {
        gd.MainSwapchain.Resize((uint)window.Width, (uint)window.Height);
        controller.WindowResized(window.Width, window.Height);
      };
      cl = gd.ResourceFactory.CreateCommandList();
      controller = new Veldrid.ImGuiRenderer(gd, gd.MainSwapchain.Framebuffer.OutputDescription, window.Width, window.Height);

      // main
      while (window.Exists) {
        Veldrid.InputSnapshot snapshot = window.PumpEvents();
        if (!window.Exists) { break; }
        controller.Update(1f / 60f, snapshot);

        // UI
        {
          ImGuiNET.ImGui.Text("Hello World!");
          ImGuiNET.ImGui.SameLine();
          ImGuiNET.ImGui.Checkbox("show another window?", ref showAWindow);
        }
        if (showAWindow) {
          ImGuiNET.ImGui.SetNextWindowSize(
            new System.Numerics.Vector2(128, 128));
          ImGuiNET.ImGui.Begin("Another window", ref showAWindow);
          ImGuiNET.ImGui.Text("Lorem ipsum");
          if (ImGuiNET.ImGui.Button("I got it")) {
            showAWindow = false;
          }
          ImGuiNET.ImGui.End();
        }

        cl.Begin();
        cl.SetFramebuffer(gd.MainSwapchain.Framebuffer);
        cl.ClearColorTarget(0, new Veldrid.RgbaFloat(160f/255, 160f/255, 192f/255, 255f));
        controller.Render(gd, cl);
        cl.End();
        gd.SubmitCommands(cl);
        gd.SwapBuffers(gd.MainSwapchain);
      }
      gd.WaitForIdle();
      controller.Dispose();
      cl.Dispose();
      gd.Dispose();
    }
  }
}
