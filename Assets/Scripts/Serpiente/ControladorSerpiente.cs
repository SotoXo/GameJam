using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorSerpiente : MonoBehaviour
{
    [Header("Juego")]
    [Min(0.5f)] [SerializeField] private float pasosPorSegundo = 5f;
    [Min(0.1f)] [SerializeField] private float tamanoCelda = 0.75f;

    [Header("Limites del tablero")]
    [SerializeField] private BoxCollider2D paredSuperior;
    [SerializeField] private BoxCollider2D paredInferior;
    [SerializeField] private BoxCollider2D paredIzquierda;
    [SerializeField] private BoxCollider2D paredDerecha;

    [Header("Objetos de la escena")]
    [SerializeField] private SpriteRenderer[] partesIniciales;
    [SerializeField] private SpriteRenderer comida;

    [Header("Sprites de cabeza")]
    [SerializeField] private Sprite cabezaArriba;
    [SerializeField] private Sprite cabezaAbajo;
    [SerializeField] private Sprite cabezaIzquierda;
    [SerializeField] private Sprite cabezaDerecha;

    [Header("Sprites de cuerpo y cola")]
    [SerializeField] private Sprite cuerpoHorizontal;
    [SerializeField] private Sprite cuerpoVertical;
    [SerializeField] private Sprite colaArriba;
    [SerializeField] private Sprite colaAbajo;
    [SerializeField] private Sprite colaIzquierda;
    [SerializeField] private Sprite colaDerecha;

    [Header("Comida")]
    [SerializeField] private Sprite manzanaRoja;

    private readonly List<SpriteRenderer> partes = new List<SpriteRenderer>();
    private readonly List<Vector2Int> celdas = new List<Vector2Int>();
    private Vector2 origen;
    private Vector2Int direccion = Vector2Int.right;
    private Vector2Int direccionPendiente = Vector2Int.right;
    private Vector2Int celdaComida;
    private int columnas;
    private int filas;
    private int puntos;
    private float tiempoAcumulado;
    private bool gameOver;
    private GUIStyle estiloPuntos;
    private GUIStyle estiloGameOver;

    private void Awake()
    {
        if (!PrepararTablero())
        {
            enabled = false;
            return;
        }

        Reiniciar();
    }

    private bool PrepararTablero()
    {
        if (paredSuperior == null || paredInferior == null ||
            paredIzquierda == null || paredDerecha == null ||
            comida == null || partesIniciales == null || partesIniciales.Length < 2 ||
            cabezaArriba == null || cabezaAbajo == null ||
            cabezaIzquierda == null || cabezaDerecha == null ||
            cuerpoHorizontal == null || cuerpoVertical == null ||
            colaArriba == null || colaAbajo == null ||
            colaIzquierda == null || colaDerecha == null || manzanaRoja == null)
        {
            Debug.LogError("Snake: faltan referencias en el Inspector.", this);
            return false;
        }

        for (int i = 0; i < partesIniciales.Length; i++)
        {
            if (partesIniciales[i] == null)
            {
                Debug.LogError("Snake: falta una parte inicial de la serpiente.", this);
                return false;
            }
        }

        float izquierda = paredIzquierda.bounds.max.x;
        float derecha = paredDerecha.bounds.min.x;
        float abajo = paredInferior.bounds.max.y;
        float arriba = paredSuperior.bounds.min.y;
        columnas = Mathf.FloorToInt((derecha - izquierda) / tamanoCelda);
        filas = Mathf.FloorToInt((arriba - abajo) / tamanoCelda);

        if (columnas < partesIniciales.Length * 2 || filas < 3)
        {
            Debug.LogError("Snake: no hay espacio suficiente entre las paredes.", this);
            return false;
        }

        origen = new Vector2(
            izquierda + (derecha - izquierda - columnas * tamanoCelda) * 0.5f + tamanoCelda * 0.5f,
            abajo + (arriba - abajo - filas * tamanoCelda) * 0.5f + tamanoCelda * 0.5f);
        return true;
    }

    private void Update()
    {
        Keyboard teclado = Keyboard.current;

        if (gameOver)
        {
            if (teclado != null && teclado.rKey.wasPressedThisFrame)
                Reiniciar();
            return;
        }

        if (teclado != null && direccionPendiente == direccion)
        {
            if (teclado.wKey.wasPressedThisFrame || teclado.upArrowKey.wasPressedThisFrame)
                ElegirDireccion(Vector2Int.up);
            else if (teclado.sKey.wasPressedThisFrame || teclado.downArrowKey.wasPressedThisFrame)
                ElegirDireccion(Vector2Int.down);
            else if (teclado.aKey.wasPressedThisFrame || teclado.leftArrowKey.wasPressedThisFrame)
                ElegirDireccion(Vector2Int.left);
            else if (teclado.dKey.wasPressedThisFrame || teclado.rightArrowKey.wasPressedThisFrame)
                ElegirDireccion(Vector2Int.right);
        }

        tiempoAcumulado += Time.deltaTime;
        float intervalo = 1f / Mathf.Max(0.5f, pasosPorSegundo);
        if (tiempoAcumulado >= intervalo)
        {
            tiempoAcumulado -= intervalo;
            Avanzar();
        }
    }

    private void ElegirDireccion(Vector2Int nueva)
    {
        if (nueva + direccion != Vector2Int.zero)
            direccionPendiente = nueva;
    }

    private void Avanzar()
    {
        direccion = direccionPendiente;
        Vector2Int siguiente = celdas[0] + direccion;
        if (siguiente.x < 0 || siguiente.x >= columnas ||
            siguiente.y < 0 || siguiente.y >= filas)
        {
            gameOver = true;
            return;
        }

        bool come = siguiente == celdaComida;
        int partesQueBloquean = celdas.Count - (come ? 0 : 1);
        for (int i = 0; i < partesQueBloquean; i++)
        {
            if (celdas[i] == siguiente)
            {
                gameOver = true;
                return;
            }
        }

        Vector2Int colaAnterior = celdas[celdas.Count - 1];
        for (int i = celdas.Count - 1; i > 0; i--)
            celdas[i] = celdas[i - 1];
        celdas[0] = siguiente;

        if (come)
        {
            partes[partes.Count - 1].gameObject.name = "Segmento_" + (partes.Count - 1).ToString("00");
            GameObject nuevo = new GameObject("Cola");
            nuevo.transform.SetParent(transform, false);
            nuevo.transform.localScale = partesIniciales[0].transform.localScale;
            SpriteRenderer nuevoRenderer = nuevo.AddComponent<SpriteRenderer>();
            partes.Add(nuevoRenderer);
            celdas.Add(colaAnterior);
            puntos++;
            if (!GenerarComida())
                gameOver = true;
        }

        ActualizarAspecto();
    }

    private void Reiniciar()
    {
        for (int i = partesIniciales.Length; i < partes.Count; i++)
        {
            if (partes[i] != null)
            {
                partes[i].gameObject.SetActive(false);
                Destroy(partes[i].gameObject);
            }
        }

        partesIniciales[partesIniciales.Length - 1].gameObject.name = "Cola";
        partes.Clear();
        celdas.Clear();
        direccion = Vector2Int.right;
        direccionPendiente = direccion;
        tiempoAcumulado = 0f;
        puntos = 0;
        gameOver = false;

        int xInicial = columnas / 2;
        int yInicial = filas / 2;
        for (int i = 0; i < partesIniciales.Length; i++)
        {
            partes.Add(partesIniciales[i]);
            celdas.Add(new Vector2Int(xInicial - i, yInicial));
        }

        ActualizarAspecto();
        GenerarComida();
    }

    private bool GenerarComida()
    {
        int libres = columnas * filas - celdas.Count;
        if (libres <= 0)
            return false;

        int elegida = Random.Range(0, libres);
        for (int y = 0; y < filas; y++)
        {
            for (int x = 0; x < columnas; x++)
            {
                Vector2Int celda = new Vector2Int(x, y);
                if (celdas.Contains(celda))
                    continue;

                if (elegida-- != 0)
                    continue;

                celdaComida = celda;
                comida.sortingOrder = 1;
                comida.sprite = manzanaRoja;
                comida.transform.position = Posicion(celda);
                return true;
            }
        }

        return false;
    }

    private Vector3 Posicion(Vector2Int celda)
    {
        return new Vector3(
            origen.x + celda.x * tamanoCelda,
            origen.y + celda.y * tamanoCelda,
            0f);
    }

    private void ActualizarAspecto()
    {
        for (int i = 0; i < partes.Count; i++)
        {
            SpriteRenderer parte = partes[i];
            parte.transform.position = Posicion(celdas[i]);

            if (i == 0)
            {
                parte.sprite = SpriteCabeza(direccion);
                parte.sortingOrder = 3;
            }
            else if (i == partes.Count - 1)
            {
                parte.sprite = SpriteCola(celdas[i] - celdas[i - 1]);
                parte.sortingOrder = 1;
            }
            else
            {
                bool horizontal = celdas[i - 1].x != celdas[i].x ||
                                  celdas[i + 1].x != celdas[i].x;
                parte.sprite = horizontal ? cuerpoHorizontal : cuerpoVertical;
                parte.sortingOrder = 2;
            }
        }
    }

    private Sprite SpriteCabeza(Vector2Int hacia)
    {
        if (hacia == Vector2Int.up) return cabezaArriba;
        if (hacia == Vector2Int.down) return cabezaAbajo;
        if (hacia == Vector2Int.left) return cabezaIzquierda;
        return cabezaDerecha;
    }

    private Sprite SpriteCola(Vector2Int hacia)
    {
        if (hacia == Vector2Int.up) return colaArriba;
        if (hacia == Vector2Int.down) return colaAbajo;
        if (hacia == Vector2Int.left) return colaIzquierda;
        return colaDerecha;
    }

    private void OnGUI()
    {
        if (estiloPuntos == null)
        {
            estiloPuntos = new GUIStyle(GUI.skin.label);
            estiloPuntos.fontSize = 24;
            estiloPuntos.fontStyle = FontStyle.Bold;
            estiloPuntos.normal.textColor = Color.white;

            estiloGameOver = new GUIStyle(estiloPuntos);
            estiloGameOver.fontSize = 40;
            estiloGameOver.alignment = TextAnchor.MiddleCenter;
        }

        GUI.Label(new Rect(20, 15, 280, 40), "Puntuación: " + puntos, estiloPuntos);
        if (gameOver)
        {
            GUI.Label(
                new Rect(0, Screen.height * 0.5f - 65, Screen.width, 130),
                "GAME OVER\nR para reiniciar",
                estiloGameOver);
        }
    }
}
