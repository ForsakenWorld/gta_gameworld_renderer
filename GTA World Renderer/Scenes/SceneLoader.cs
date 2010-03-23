﻿using System;
using System.Collections.Generic;
using GTAWorldRenderer.Logging;
using System.IO;
using Microsoft.Xna.Framework;

namespace GTAWorldRenderer.Scenes
{

   partial class SceneLoader // TODO :: возможно, его можно сделать Static
   {
      class LoadingException : ApplicationException
      {
         public LoadingException(string msg) : base(msg)
         {
         }
      }

      /// <summary>
      /// Пишет в текущий Stage лога текст ошибки и кидает исключение LoadingException
      /// </summary>
      /// <param name="msg"></param>
      private static void TerminateWithError(string msg)
      {
         Log.Instance.Print(msg, MessageType.Error);
         throw new LoadingException(msg);
      }


      enum GtaVersion
      {
         III, ViceCity, SanAndreas
      }


      private Log Logger = Log.Instance;
      private GtaVersion gtaVersion;
      

      private Dictionary<int, SceneItemDefinition> objDefinitions = new Dictionary<int, SceneItemDefinition>();
      private List<SceneItemPlacement> objPlacements = new List<SceneItemPlacement>();



      public Scene LoadScene()
      {
         using (Logger.EnterStage("Loading scene"))
         {
            try
            {
               Logger.Print("Switching working directory to GTA folder");
               System.Environment.CurrentDirectory = Config.Instance.GTAFolderPath;

               DetermineGtaVersion();
               LoadDatFile("data/default.dat");
               LoadDatFile(GetVersionSpecificDatFile());

               Scene scene = new Scene();
               var loadedModels = new Dictionary<string, Model3D>();

               //var objs = new IPLFileLoader(@"c:\Program Files\GTAIII\data\maps\comntop\comNtop.ipl", GtaVersion.III).Load();
               //foreach (var obj in objs)
               //   objPlacements.Add(obj);

               // TODO :: temporary code with absolute paths!!!
               foreach (var obj in objPlacements)
               {
                  if (!obj.Name.StartsWith("LOD"))
                     continue;
                  if (!loadedModels.ContainsKey(obj.Name))
                     loadedModels[obj.Name] = new DffLoader(@"c:\home\tmp\root\" + obj.Name + ".dff").Load();

                  Matrix matrix = Matrix.CreateScale(obj.Scale) * Matrix.CreateFromQuaternion(obj.Rotation) * Matrix.CreateTranslation(obj.Position);

                  scene.SceneObjects.Add(new SceneObject(loadedModels[obj.Name], matrix));
                  scene.SceneObjects[scene.SceneObjects.Count - 1].ModelFilename = obj.Name;
               }

               Logger.Print("Scene loaded!");
               Logger.PrintStatistic();
               Logger.Print("Objects located on scene: " + scene.SceneObjects.Count);
               Logger.Flush();

               //scene.SceneObjects.Add(new SceneObject(new DffLoader(@"c:\home\tmp\root\LODders02.dff ").Load(), Matrix.Identity));
               //scene.SceneObjects.Add(new SceneObject(new DffLoader(@"c:\home\tmp\root\LOD_landnew2.dff ").Load(), Matrix.Identity));
               
               //scene.SceneObjects.Add(new SceneObject(new DffLoader(@"c:\Program Files\GTAIII\models\Generic\arrow.DFF").Load(), Matrix.Identity));

               return scene;

            } catch (Exception)
            {
               Logger.Print("Failed to load scene", MessageType.Error);
               throw;
            }
         }
      }


      private void DetermineGtaVersion()
      {
         Logger.Print("Determining GTA version...");
         if (File.Exists("gta3.exe"))
         {
            Logger.Print("... version is GTA III");
            gtaVersion = GtaVersion.III;
         }
         else if (File.Exists("gta-vc.exe"))
         {
            Logger.Print("... version is GTA Vice City");
            gtaVersion = GtaVersion.ViceCity;
         }
         else if (File.Exists("gta_sa.exe"))
         {
            Logger.Print("... version is GTA San Andreas");
            gtaVersion = GtaVersion.SanAndreas;
         }
         else
         {
            Logger.Print("Can not determine game version!", MessageType.Error);
            throw new LoadingException("Unknown or unsopported version of GTA.");
         }
      }


      private string GetVersionSpecificDatFile()
      {
         switch(gtaVersion)
         {
            case GtaVersion.III:
               return "data/gta3.dat";
            case GtaVersion.ViceCity:
               return "data/gta_vc.dat";
            case GtaVersion.SanAndreas:
               return "data/gra.dat";
            default:
               string msg = "Unsopported GTA version: " + gtaVersion.ToString() + ".";
               Logger.Print(msg, MessageType.Error);
               throw new LoadingException(msg);
         }
      }


      private void LoadDatFile(string path)
      {
         using (Logger.EnterStage("Reading DAT file: " + path))
         {
            using (StreamReader fin = new StreamReader(path))
            {
               string line;
               while ((line = fin.ReadLine()) != null)
               {
                  line = line.Trim();
                  if (line.Length == 0 || line.StartsWith("#"))
                     continue;

                  if (line.StartsWith("TEXDICTION"))
                  {
                     // Device->getFileSystem()->registerFileArchive(inStr.subString(11, inStr.size() - 11), true, false); // " 11 = "TEXDICTION "
                     Logger.Print("TEXDICTION: not implemented yet", MessageType.Warning);
                  }
                  else if (line.StartsWith("IDE"))
                  {
                     string fileName = line.Substring(4);
                     var objs = new IDEFileLoader(fileName, gtaVersion).Load();
                     foreach (var obj in objs)
                        objDefinitions.Add(obj.Key, obj.Value);
                  }
                  else if (line.StartsWith("IPL"))
                  {
                     string fileName = line.Substring(4);
                     var objs = new IPLFileLoader(fileName, gtaVersion).Load();
                     foreach (var obj in objs)
                        objPlacements.Add(obj);
                  }
                  else if (line.StartsWith("SPLASH") || line.StartsWith("COLFILE") || line.StartsWith("MAPZONE") || line.StartsWith("MODELFILE"))
                  {
                     // Ignoring this commands
                  }
                  else
                  {
                     int sep_idx = line.IndexOf(' ');
                     if (sep_idx == -1)
                        sep_idx = line.IndexOf('\t');
                     if (sep_idx == -1)
                        sep_idx = line.Length;
                     string command = line.Substring(0, sep_idx);
                     Logger.Print("Unsupported command in DAT file: " + command, MessageType.Error);
                  }

               }
            }
         }
      }


   }
}
