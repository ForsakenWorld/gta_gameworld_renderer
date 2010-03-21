﻿using Microsoft.Xna.Framework;
using GTAWorldRenderer.Scenes;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GTAWorldRenderer.Rendering
{
   class SceneRenderer3D : CompositeRenderer
   {
      private const float rotationSpeed = 0.3f;

      public Scene SceneContent { get; set; }

      GraphicsDevice device;
      Camera camera;
      InfoPanelFor3Dview textInfoPanel;
      MouseState originalMouseState;
      Effect effect;
      Matrix projectionMatrix;

      /*
       * TODO :: 
       * в C# 4.0 появятся параметры по умолчанию.
       * Заменить набор конструкторов и метод Construct на единый конструктор с параметрами по умолчанию.
       */

      public SceneRenderer3D(ContentManager contentManager)
         : base(contentManager)
      {
         Initialize();
      }


      public SceneRenderer3D(ContentManager contentManager, Scene scene)
         : base(contentManager)
      {
         SceneContent = scene;
         Initialize();
      }


      public void Initialize()
      {
         device = GraphicsDeviceHolder.Device;
         camera = new Camera();

         textInfoPanel = new InfoPanelFor3Dview(Content);
         textInfoPanel.Camera = camera;
         AddSubRenderer(textInfoPanel);

         projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 0.1f, 200.0f);
         effect = Content.Load<Effect>("effect");

         Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
         originalMouseState = Mouse.GetState();
      }


      public override void DoUpdate(GameTime gameTime)
      {
         float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

         ProcessMouse(gameTime, timeDifference);
         ProcessKeyboard(gameTime, timeDifference);
      }


      private void ProcessMouse(GameTime gameTime, float amount)
      {
         MouseState currentMouseState = Mouse.GetState();
         if (currentMouseState != originalMouseState && currentMouseState.LeftButton == ButtonState.Pressed)
         {
            float xDifference = -currentMouseState.X + originalMouseState.X;
            float yDifference = -currentMouseState.Y + originalMouseState.Y;
            float leftrightRot = rotationSpeed * xDifference * amount;
            float updownRot = rotationSpeed * yDifference * amount;
            camera.UpdateRotation(leftrightRot, updownRot);
         }
         Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
      }


      private void ProcessKeyboard(GameTime gameTime, float amount)
      {
         Vector3 moveVector = new Vector3(0, 0, 0);
         KeyboardState keyState = Keyboard.GetState();
         if (keyState.IsKeyDown(Keys.Up))
            moveVector += Vector3.Forward;
         if (keyState.IsKeyDown(Keys.Down))
            moveVector += Vector3.Backward;
         if (keyState.IsKeyDown(Keys.Right))
            moveVector += Vector3.Right;
         if (keyState.IsKeyDown(Keys.Left))
            moveVector += Vector3.Left;
         if (keyState.IsKeyDown(Keys.PageUp))
            moveVector += Vector3.Up;
         if (keyState.IsKeyDown(Keys.PageDown))
            moveVector += Vector3.Down;
         camera.UpdatePosition(moveVector * amount);
      }


      public override void DoDraw(GameTime gameTime)
      {
         device.Clear(Color.Black);

         if (SceneContent == null)
            return;

         effect.Parameters["xView"].SetValue(camera.ViewMatrix);
         effect.Parameters["xProjection"].SetValue(projectionMatrix);
         SceneContent.Draw(effect);
      }
   }
}
