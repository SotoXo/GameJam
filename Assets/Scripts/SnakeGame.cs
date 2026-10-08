using System.Collections.Generic;
using UnityEngine;

public class SnakeGame : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite cabezaSprite;
    public Sprite cuerpoSprite;
    public Sprite comidaSprite;

    [Header("Audio")]
    public AudioClip sonidoComer;

    [Header("Juego")]
    public float tiempoPasoInicial = 0.18f;

    private enum EstadoJuego { Menu, Instrucciones, Jugando, GameOver }
    private EstadoJuego estado = EstadoJuego.Menu;

    private readonly List<Vector2Int> segmentos = new List<Vector2Int>();
    private readonly List<GameObject> visuales = new List<GameObject>();

    private Vector2Int direccion = Vector2Int.right;
    private Vector2Int siguienteDireccion = Vector2Int.right;
    private Vector2Int comida;

    private const int minX = -8;
    private const int maxX = 8;
    private const int minY = -4;
    private const int maxY = 4;

    private float acumulado;
    private float tiempoPaso;
    private int puntaje;

    private AudioSource audioSource;
    private GameObject comidaVisual;
    private Sprite pixelSprite;

    private readonly Color colorTablero = new Color(0.08f, 0.17f, 0.10f, 1f);
    private readonly Color colorBorde = new Color(0.35f, 0.75f, 0.25f, 1f);

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        CrearPixelSprite();
        CrearTableroVisual();
    }

    void Start()
    {
        PrepararMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (estado == EstadoJuego.Jugando || estado == EstadoJuego.GameOver || estado == EstadoJuego.Instrucciones)
                PrepararMenu();
        }

        if (estado != EstadoJuego.Jugando)
            return;

        LeerEntrada();

        acumulado += Time.deltaTime;
        if (acumulado >= tiempoPaso)
        {
            acumulado -= tiempoPaso;
            Avanzar();
        }
    }

    void LeerEntrada()
    {
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && direccion != Vector2Int.down)
            siguienteDireccion = Vector2Int.up;
        else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && direccion != Vector2Int.up)
            siguienteDireccion = Vector2Int.down;
        else if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && direccion != Vector2Int.right)
            siguienteDireccion = Vector2Int.left;
        else if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && direccion != Vector2Int.left)
            siguienteDireccion = Vector2Int.right;
    }

    void IniciarJuego()
    {
        LimpiarVisuales();

        segmentos.Clear();
        segmentos.Add(new Vector2Int(0, 0));
        segmentos.Add(new Vector2Int(-1, 0));
        segmentos.Add(new Vector2Int(-2, 0));

        direccion = Vector2Int.right;
        siguienteDireccion = Vector2Int.right;
        acumulado = 0f;
        puntaje = 0;
        tiempoPaso = tiempoPasoInicial;

        estado = EstadoJuego.Jugando;

        CrearComidaVisualSiHaceFalta();
        GenerarComida();
        SincronizarVisuales();
    }

    void PrepararMenu()
    {
        estado = EstadoJuego.Menu;
        LimpiarVisuales();

        if (comidaVisual != null)
            comidaVisual.SetActive(false);
    }

    void Avanzar()
    {
        direccion = siguienteDireccion;
        Vector2Int nuevaCabeza = segmentos[0] + direccion;

        if (nuevaCabeza.x < minX || nuevaCabeza.x > maxX ||
            nuevaCabeza.y < minY || nuevaCabeza.y > maxY)
        {
            Perder();
            return;
        }

        bool come = nuevaCabeza == comida;

        int limiteColision = come ? segmentos.Count : segmentos.Count - 1;
        for (int i = 0; i < limiteColision; i++)
        {
            if (segmentos[i] == nuevaCabeza)
            {
                Perder();
                return;
            }
        }

        segmentos.Insert(0, nuevaCabeza);

        if (come)
        {
            puntaje += 10;
            tiempoPaso = Mathf.Max(0.075f, tiempoPaso * 0.975f);

            if (sonidoComer != null)
                audioSource.PlayOneShot(sonidoComer);

            GenerarComida();
        }
        else
        {
            segmentos.RemoveAt(segmentos.Count - 1);
        }

        SincronizarVisuales();
    }

    void Perder()
    {
        estado = EstadoJuego.GameOver;
    }

    void GenerarComida()
    {
        int seguridad = 0;
        do
        {
            comida = new Vector2Int(
                Random.Range(minX, maxX + 1),
                Random.Range(minY, maxY + 1)
            );

            seguridad++;
            if (seguridad > 500)
                break;

        } while (segmentos.Contains(comida));

        CrearComidaVisualSiHaceFalta();
        comidaVisual.transform.position = new Vector3(comida.x, comida.y, 0);
        comidaVisual.SetActive(true);
    }

    void CrearComidaVisualSiHaceFalta()
    {
        if (comidaVisual != null)
            return;

        comidaVisual = new GameObject("Comida");
        SpriteRenderer sr = comidaVisual.AddComponent<SpriteRenderer>();
        sr.sprite = comidaSprite;
        sr.sortingOrder = 10;
        AjustarSpriteAUnaCelda(comidaVisual, sr.sprite, 0.70f);
    }

    void SincronizarVisuales()
    {
        while (visuales.Count < segmentos.Count)
        {
            GameObject go = new GameObject("Segmento_" + visuales.Count);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 20;
            visuales.Add(go);
        }

        for (int i = 0; i < visuales.Count; i++)
        {
            bool activo = i < segmentos.Count;
            visuales[i].SetActive(activo);

            if (!activo)
                continue;

            visuales[i].transform.position = new Vector3(segmentos[i].x, segmentos[i].y, 0);

            SpriteRenderer sr = visuales[i].GetComponent<SpriteRenderer>();

            if (i == 0)
            {
                sr.sprite = cabezaSprite;
                visuales[i].name = "Cabeza";
                visuales[i].transform.rotation = Quaternion.Euler(0, 0, AnguloCabeza());
                AjustarSpriteAUnaCelda(visuales[i], sr.sprite, 0.88f);
            }
            else
            {
                sr.sprite = cuerpoSprite;
                visuales[i].name = "Cuerpo_" + i;
                visuales[i].transform.rotation = Quaternion.identity;
                AjustarSpriteAUnaCelda(visuales[i], sr.sprite, 0.82f);
            }
        }
    }

    float AnguloCabeza()
    {
        if (direccion == Vector2Int.up) return 90f;
        if (direccion == Vector2Int.left) return 180f;
        if (direccion == Vector2Int.down) return -90f;
        return 0f;
    }

    void AjustarSpriteAUnaCelda(GameObject go, Sprite sprite, float tamano)
    {
        if (sprite == null)
            return;

        Vector2 size = sprite.bounds.size;
        float mayor = Mathf.Max(size.x, size.y);
        if (mayor <= 0.0001f)
            return;

        float escala = tamano / mayor;
        go.transform.localScale = new Vector3(escala, escala, 1);
    }

    void LimpiarVisuales()
    {
        foreach (GameObject go in visuales)
        {
            if (go != null)
                Destroy(go);
        }
        visuales.Clear();
    }

    void CrearPixelSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.name = "PixelRuntime";
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();

        pixelSprite = Sprite.Create(
            tex,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f),
            1f
        );
    }

    void CrearTableroVisual()
    {
        GameObject fondo = CrearRectangulo(
            "Tablero",
            new Vector3(0, 0, 1),
            new Vector2((maxX - minX) + 1.9f, (maxY - minY) + 1.9f),
            colorTablero,
            -20
        );

        float ancho = (maxX - minX) + 2f;
        float alto = (maxY - minY) + 2f;

        CrearRectangulo("BordeSuperior", new Vector3(0, maxY + 1f, 0), new Vector2(ancho, 0.16f), colorBorde, -10);
        CrearRectangulo("BordeInferior", new Vector3(0, minY - 1f, 0), new Vector2(ancho, 0.16f), colorBorde, -10);
        CrearRectangulo("BordeIzquierdo", new Vector3(minX - 1f, 0, 0), new Vector2(0.16f, alto), colorBorde, -10);
        CrearRectangulo("BordeDerecho", new Vector3(maxX + 1f, 0, 0), new Vector2(0.16f, alto), colorBorde, -10);
    }

    GameObject CrearRectangulo(string nombre, Vector3 posicion, Vector2 escala, Color color, int orden)
    {
        GameObject go = new GameObject(nombre);
        go.transform.position = posicion;
        go.transform.localScale = new Vector3(escala.x, escala.y, 1);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = pixelSprite;
        sr.color = color;
        sr.sortingOrder = orden;

        return go;
    }

    void OnGUI()
    {
        GUIStyle titulo = new GUIStyle(GUI.skin.label);
        titulo.alignment = TextAnchor.MiddleCenter;
        titulo.fontStyle = FontStyle.Bold;
        titulo.fontSize = Mathf.RoundToInt(Mathf.Clamp(Screen.height * 0.07f, 34f, 58f));
        titulo.normal.textColor = new Color(0.75f, 1f, 0.35f);

        GUIStyle texto = new GUIStyle(GUI.skin.label);
        texto.alignment = TextAnchor.MiddleCenter;
        texto.wordWrap = true;
        texto.fontSize = Mathf.RoundToInt(Mathf.Clamp(Screen.height * 0.03f, 17f, 26f));
        texto.normal.textColor = Color.white;

        GUIStyle boton = new GUIStyle(GUI.skin.button);
        boton.fontSize = Mathf.RoundToInt(Mathf.Clamp(Screen.height * 0.032f, 18f, 28f));
        boton.fontStyle = FontStyle.Bold;

        float centroX = Screen.width * 0.5f;
        float ancho = Mathf.Min(420f, Screen.width * 0.72f);
        float x = centroX - ancho * 0.5f;

        if (estado == EstadoJuego.Menu)
        {
            GUI.Label(new Rect(x, Screen.height * 0.16f, ancho, 80), "VIBORITA", titulo);
            GUI.Label(new Rect(x, Screen.height * 0.28f, ancho, 55), "Snake básico con tus sprites pixel art", texto);

            if (GUI.Button(new Rect(x, Screen.height * 0.42f, ancho, 58), "JUGAR", boton))
                IniciarJuego();

            if (GUI.Button(new Rect(x, Screen.height * 0.53f, ancho, 58), "INSTRUCCIONES", boton))
                estado = EstadoJuego.Instrucciones;

            if (GUI.Button(new Rect(x, Screen.height * 0.64f, ancho, 58), "SALIR", boton))
                Application.Quit();

            return;
        }

        if (estado == EstadoJuego.Instrucciones)
        {
            GUI.Label(new Rect(x, Screen.height * 0.14f, ancho, 75), "CÓMO JUGAR", titulo);
            GUI.Label(
                new Rect(x, Screen.height * 0.29f, ancho, 180),
                "Muévete con las FLECHAS o W A S D.\nCome la manzana para crecer y sumar 10 puntos.\nNo choques con los bordes ni con tu propio cuerpo.\nESC vuelve al menú.",
                texto
            );

            if (GUI.Button(new Rect(x, Screen.height * 0.66f, ancho, 58), "VOLVER", boton))
                PrepararMenu();

            return;
        }

        GUIStyle hud = new GUIStyle(texto);
        hud.alignment = TextAnchor.UpperLeft;
        hud.fontStyle = FontStyle.Bold;

        GUI.Label(new Rect(20, 15, 300, 45), "PUNTAJE: " + puntaje, hud);

        GUIStyle ayuda = new GUIStyle(texto);
        ayuda.alignment = TextAnchor.UpperRight;
        ayuda.fontSize = Mathf.RoundToInt(Mathf.Clamp(Screen.height * 0.022f, 14f, 20f));
        GUI.Label(new Rect(Screen.width - 330, 18, 310, 40), "ESC = menú", ayuda);

        if (estado == EstadoJuego.GameOver)
        {
            GUI.Box(new Rect(centroX - 220, Screen.height * 0.30f, 440, 275), "");

            GUI.Label(new Rect(centroX - 200, Screen.height * 0.33f, 400, 70), "GAME OVER", titulo);
            GUI.Label(new Rect(centroX - 200, Screen.height * 0.44f, 400, 50), "Puntaje: " + puntaje, texto);

            if (GUI.Button(new Rect(centroX - 180, Screen.height * 0.54f, 360, 52), "REINTENTAR", boton))
                IniciarJuego();

            if (GUI.Button(new Rect(centroX - 180, Screen.height * 0.64f, 360, 52), "MENÚ", boton))
                PrepararMenu();
        }
    }
}
