using System;
using System.Globalization;
using System.Threading;
using GTAWorldRenderer.Logging;
using GTAWorldRenderer.Scenes;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using GTAWorldRenderer.Rendering;

namespace GTAWorldRenderer
{
   public class Main : Microsoft.Xna.Framework.Game
   {
      Scene scene;
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
         // ��������� �����
         scene = new Scene();
         scene.LoadScene();
         Log.Instance.PrintStatistic();
         GC.Collect();

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
