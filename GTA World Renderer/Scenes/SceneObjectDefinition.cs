﻿using Microsoft.Xna.Framework;

namespace GTAWorldRenderer.Scenes
{
   /// <summary>
   /// Определяет объект сцены (его позицию, масштаб, поворт)
   /// Данные берутся из *.ipl файлов
   /// </summary>
   class SceneItemPlacement
   {
      public int Id{ get; set; }
      public string Name{ get; set; }
      public Vector3 Position{ get; set; }
      public Quaternion Rotation { get; set; }
      public Vector3 Scale { get; set; }
   }


   /// <summary>
   /// Определяет описание объекта (текстуру и растояние отрисовки).
   /// Данные берутся из *.ide файлов
   /// </summary>
   class SceneItemDefinition
   {
      public string Name { get; set; }
      public string TextureFolder { get; set; }
      public float DrawDistance { get; set; }
   }


   /// <summary>
   /// Описание объекта на построенной сцене
   /// </summary>
   class SceneObject
   {
      public Matrix WorldMatrix { get; private set; }
      public Model3D Model { get; private set; }
      public string ModelFilename { get; set; }

      public SceneObject(Model3D model, Matrix worldMatrix)
      {
         Model = model;
         WorldMatrix = worldMatrix;
      }

   }
}
