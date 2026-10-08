using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class CrearViborita
{
    const string Escena = "Assets/Scenes/Main.unity";
    const string Cabeza = "Assets/Sprites/Cabeza_Derecha.png";
    const string Cuerpo = "Assets/Sprites/Cuerpo.png";
    const string Comida = "Assets/Sprites/Comida_Manzana.png";
    const string Sonido = "Assets/Audio/comer.wav";

    static CrearViborita()
    {
        EditorSettings.defaultBehaviorMode = EditorBehaviorMode.Mode2D;
        EditorApplication.delayCall += Preparar;
    }

    [MenuItem("Viborita/Recrear escena")]
    public static void Recrear()
    {
        if (File.Exists(Escena))
            AssetDatabase.DeleteAsset(Escena);

        Preparar();
    }

    static void Preparar()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            EditorApplication.delayCall += Preparar;
            return;
        }

        EditorSettings.defaultBehaviorMode = EditorBehaviorMode.Mode2D;

        ConfigurarSprite(Cabeza, 200);
        ConfigurarSprite(Cuerpo, 200);
        ConfigurarSprite(Comida, 200);

        if (File.Exists(Escena))
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(Escena, true)
            };

            if (SceneManager.GetActiveScene().path != Escena)
                EditorSceneManager.OpenScene(Escena);

            return;
        }

        Directory.CreateDirectory("Assets/Scenes");

        Sprite cabeza = AssetDatabase.LoadAssetAtPath<Sprite>(Cabeza);
        Sprite cuerpo = AssetDatabase.LoadAssetAtPath<Sprite>(Cuerpo);
        Sprite comida = AssetDatabase.LoadAssetAtPath<Sprite>(Comida);
        AudioClip audio = AssetDatabase.LoadAssetAtPath<AudioClip>(Sonido);

        if (cabeza == null || cuerpo == null || comida == null)
        {
            EditorApplication.delayCall += Preparar;
            return;
        }

        Scene escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject camara = new GameObject("Main Camera");
        camara.tag = "MainCamera";
        camara.transform.position = new Vector3(0, 0, -10);

        Camera cam = camara.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 6.2f;
        cam.backgroundColor = new Color(0.025f, 0.055f, 0.035f);

        camara.AddComponent<AudioListener>();

        GameObject juego = new GameObject("SnakeGame");
        SnakeGame snake = juego.AddComponent<SnakeGame>();
        snake.cabezaSprite = cabeza;
        snake.cuerpoSprite = cuerpo;
        snake.comidaSprite = comida;
        snake.sonidoComer = audio;

        EditorSceneManager.SaveScene(escena, Escena);

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(Escena, true)
        };

        PlayerSettings.productName = "Viborita";
        PlayerSettings.companyName = "Carla";
        PlayerSettings.defaultScreenWidth = 1280;
        PlayerSettings.defaultScreenHeight = 720;
        PlayerSettings.resizableWindow = true;

        Selection.activeGameObject = juego;

        foreach (SceneView view in SceneView.sceneViews)
        {
            view.in2DMode = true;
            view.orthographic = true;
            view.Repaint();
        }

        Debug.Log("Viborita lista. Presiona Play para probarla.");
    }

    static void ConfigurarSprite(string ruta, float ppu)
    {
        TextureImporter importer = AssetImporter.GetAtPath(ruta) as TextureImporter;
        if (importer == null)
            return;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = ppu;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.SaveAndReimport();
    }

    [MenuItem("Viborita/Construir EXE Windows")]
    public static void ConstruirWindows()
    {
        if (!File.Exists(Escena))
        {
            Debug.LogError("Primero espera a que se cree Assets/Scenes/Main.unity.");
            return;
        }

        Directory.CreateDirectory("Builds/Windows");

        BuildPlayerOptions opciones = new BuildPlayerOptions
        {
            scenes = new[] { Escena },
            locationPathName = "Builds/Windows/Viborita.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildPipeline.BuildPlayer(opciones);
        Debug.Log("Build solicitado en Builds/Windows/Viborita.exe");
    }
}
