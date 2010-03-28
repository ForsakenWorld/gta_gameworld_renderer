using System.Globalization;
using System.Threading;
using GTAWorldRenderer.Logging;
using GTAWorldRenderer.Scenes;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using GTAWorldRenderer.Rendering;
using GTAWorldRenderer.Scenes.Loaders;


namespace GTAWorldRenderer
{
   public class Main : Microsoft.Xna.Framework.Game
   {
      Renderer renderer3d;

      public Main()
      {
         Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
         
         GraphicsDeviceHolder.DeviceManager = new GraphicsDeviceManager(this);
         Content.RootDirectory = "Content";

         // ����������� ���
         Log.Instance.AddLogWriter(new FileLogWriter("log.log"));
         if (!Debugger.IsAttached)
         {
            // ��� ������������� ��������� ������-�� ������ ����� ��������� � �������,
            // ������� ���������� ConsoleWriter ������ ����� ����������� ��� ���������
            Log.Instance.AddLogWriter(ConsoleLogWriter.Instance);
         }

      }


      protected override void Initialize()
      {
         GraphicsDeviceHolder.InitDevice();
         base.Initialize();
      }


      protected override void LoadContent()
      {
         Scene scene = new SceneLoader().LoadScene();
         if (Config.Instance.FullScreenMode)
         {
            Log.Instance.Print("Switching to fullscreen mode...");
            GraphicsDeviceHolder.DeviceManager.PreferredBackBufferHeight = GraphicsDeviceHolder.Device.DisplayMode.Height;
            GraphicsDeviceHolder.DeviceManager.PreferredBackBufferWidth = GraphicsDeviceHolder.Device.DisplayMode.Width;
            GraphicsDeviceHolder.DeviceManager.IsFullScreen = true;
            GraphicsDeviceHolder.DeviceManager.ApplyChanges();
         }
         renderer3d = new SceneRenderer3D(Content, scene);
      }


      protected override void Update(GameTime gameTime)
      {
         renderer3d.Update(gameTime);
      }


      protected override void Draw(GameTime gameTime)
      {
         renderer3d.Draw(gameTime);

         base.Draw(gameTime);
      }



   }
}
