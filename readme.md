# GameJam - Snake

Juego 2D tipo **Snake** desarrollado en **Unity 6.3 LTS** como proyecto para una Game Jam.

El objetivo del proyecto es desarrollar una versión sencilla y funcional del clásico Snake, utilizando movimiento en un escenario limitado, detección de colisiones, recolección de elementos y crecimiento progresivo de la serpiente.

---

## Estado del proyecto

Actualmente se encuentran integrados los límites del escenario y la lógica básica de Snake.

### Implementado

- Proyecto creado en Unity.
- Configuración de Unity 2D.
- Fondo principal del escenario.
- Organización inicial de recursos gráficos.
- Cuatro límites para delimitar el área de juego.
- Uso de `Box Collider 2D` en las paredes.
- Tag `Pared` para identificar los límites del escenario.
- Preparación del escenario para detectar la colisión de la serpiente con los bordes.
- Serpiente con movimiento automático por cuadrícula y controles de dirección.
- Comida que aparece en celdas libres, crecimiento y puntuación de un punto por manzana.
- Game Over al alcanzar el borde o el propio cuerpo, con reinicio mediante `R`.
- Control de versiones mediante Git y GitHub.
- Rama `develop` utilizada para integrar el desarrollo.

La comprobación del juego en Play Mode queda pendiente de las pruebas manuales del equipo.

---

# Tecnologías utilizadas

| Tecnología | Uso |
|---|---|
| Unity 6.3 LTS | Motor del videojuego |
| C# | Programación de la lógica del juego |
| Universal Render Pipeline 2D | Renderizado del proyecto |
| Git | Control de versiones |
| GitHub | Repositorio remoto y colaboración |

Versión de Unity utilizada:

```text
Unity 6.3 LTS
6000.3.16f1
```

---

# Estructura del proyecto

La organización actual de los archivos principales es:

```text
GameJam/
│
├── Assets/
│   │
│   ├── Arte/
│   │   ├── Fondo/
│   │   │   ├── snake.jpg
│   │   │   └── snake.jpg.meta
│   │   │
│   │   ├── Interfaz/
│   │   └── Sprite/
│   │       └── Snake_Assets_Completo (2).aseprite
│   │
│   ├── Scenes/
│   │   └── SampleScene.unity
│   │
│   ├── Scripts/
│   │   └── Serpiente/
│   │       └── ControladorSerpiente.cs
│   │
│   └── Settings/
│
├── Packages/
│   ├── manifest.json
│   └── packages-lock.json
│
├── ProjectSettings/
│
├── .gitignore
└── README.md
```

Los archivos generados automáticamente por Unity que no son necesarios para compartir el proyecto se encuentran excluidos mediante `.gitignore`.

Entre ellos se encuentran:

```text
Library/
Temp/
Obj/
Logs/
UserSettings/
Build/
Builds/
```

La carpeta `Library` no debe almacenarse en GitHub, ya que Unity puede reconstruirla automáticamente al abrir el proyecto.

---

# Escenario

El escenario principal se encuentra actualmente en:

```text
Assets/Scenes/SampleScene.unity
```

La escena contiene los elementos principales del entorno del juego.

La organización actual es:

```text
SampleScene
│
├── Main Camera
├── Global Light 2D
├── snake
├── Paredes
│   ├── Pared Superior
│   ├── Pared Inferior
│   ├── Pared Izquierda
│   └── Pared Derecha
├── Serpiente
│   ├── Cabeza
│   ├── Segmento_01
│   ├── Segmento_02
│   └── Cola
└── Comida
```

---

# Fondo del escenario

El fondo utilizado para el tablero de Snake se encuentra almacenado en:

```text
Assets/Arte/Fondo/snake.jpg
```

En Unity, la imagen es importada como:

```text
Texture Type: Sprite (2D and UI)
Sprite Mode: Single
```

El fondo se utiliza únicamente como elemento visual y no participa directamente en las colisiones o lógica del juego.

Para evitar que interfiera visualmente con los demás elementos se recomienda utilizar un `Order in Layer` inferior al de la serpiente y la comida.

Ejemplo:

```text
Fondo:
Order in Layer = -10

Comida:
Order in Layer = 0

Serpiente:
Order in Layer = 1
```

---

# Sistema de límites y colisiones

Para impedir que la serpiente pueda salir del escenario se crearon cuatro paredes alrededor del tablero.

```text
Paredes
├── Pared Superior
├── Pared Inferior
├── Pared Izquierda
└── Pared Derecha
```

Cada pared utiliza el componente:

```text
Box Collider 2D
```

Las paredes funcionan como límites físicos invisibles del escenario.

No necesitan un `Rigidbody 2D`, debido a que se utilizan como elementos estáticos del entorno.

---

## Tag de las paredes

Se creó el Tag:

```text
Pared
```

Las cuatro paredes utilizan este Tag:

```text
Pared Superior   -> Pared
Pared Inferior   -> Pared
Pared Izquierda  -> Pared
Pared Derecha    -> Pared
```

El Tag se conserva en las paredes. El controlador actual obtiene el área jugable de sus `Box Collider 2D` sin cambiar sus posiciones, tamaños ni configuración.

La configuración del Tag se almacena dentro de:

```text
ProjectSettings/TagManager.asset
```

Por esta razón, este archivo debe mantenerse dentro del repositorio.

---

# Detección de colisiones

`ControladorSerpiente` toma los bordes interiores de los colliders de las cuatro paredes y construye una cuadrícula dentro de ellos. La serpiente avanza cambiando posiciones entre celdas; no utiliza físicas para moverse.

Hay Game Over si la siguiente celda de la cabeza está fuera de esa cuadrícula o está ocupada por su cuerpo. Se permite entrar en la celda que libera la cola durante un paso normal.

---

## Controles y reglas

- `W` o flecha arriba, `S` o flecha abajo, `A` o flecha izquierda, `D` o flecha derecha: cambiar dirección. No se permite girar 180° en un paso.
- La serpiente se mueve automáticamente a intervalos regulares. `Pasos Por Segundo` y `Tamano Celda` se pueden ajustar desde el Inspector de `Serpiente`.
- La manzana roja aparece solo en celdas libres. Al comerla, la serpiente conserva la posición anterior de la cola como segmento nuevo, suma **1 punto** y genera otra manzana.
- La puntuación se muestra en pantalla. Al perder, el movimiento se detiene y aparece `GAME OVER`.
- `R` tras Game Over: vuelve a la serpiente inicial de cuatro partes, pone la puntuación a cero y genera comida nueva.

La cabeza y la cola cambian de sprite según su dirección. El cuerpo utiliza las variantes horizontal y vertical; las curvas de la hoja no se usan en esta versión sencilla. El controlador está en `Assets/Scripts/Serpiente/ControladorSerpiente.cs`.

Sprites utilizados: `Cabeza_Arriba`, `Cabeza_Abajo`, `Cabeza_Izquierda`, `Cabeza_Derecha`, `Cuerpo_Horizontal`, `Cuerpo_Vertical`, `Cola_Arriba`, `Cola_Abajo`, `Cola_Izquierda`, `Cola_Derecha` y `Manzana_Roja`.

---

# Funcionamiento del juego

El flujo implementado para Snake es:

```text
Inicio
  ↓
Aparece la serpiente
  ↓
Movimiento continuo
  ↓
El jugador cambia la dirección
  ↓
        ┌───────────────┐
        │               │
        ▼               │
Buscar comida            │
  ↓                      │
Comer alimento           │
  ↓                      │
Aumentar puntuación      │
  ↓                      │
Crecer serpiente         │
  │                      │
  └──────────────────────┘

Si colisiona con:
- una pared
- su propio cuerpo

        ↓
     Game Over
        ↓
 Reiniciar partida
```

---

# Recursos gráficos

Los recursos obtenidos de fuentes externas deben documentarse indicando su procedencia, ubicación dentro del proyecto y licencia cuando esta pueda verificarse.

Esto permite mantener un registro de qué elementos fueron desarrollados por el equipo y cuáles fueron obtenidos de fuentes externas.

---

## Hoja de sprites de Snake

La hoja se encuentra en `Assets/Arte/Sprite/Snake_Assets_Completo (2).aseprite` y su lienzo mide **1448 × 1086 píxeles**. Se importa con el importador de Aseprite de Unity en modo **Sprite Sheet**, usando Sprite Editor / SpriteRects sobre la textura generada por Unity.

Configuración de importación:

```text
Texture Type: Sprite (2D and UI)
Sprite Mode: Multiple
Filter Mode: Point (no filter)
Compression: None (Uncompressed)
```

Los 20 sprites definidos a partir de los elementos visibles del primer fotograma son:

- Cabezas: `Cabeza_Abajo`, `Cabeza_Arriba`, `Cabeza_Izquierda`, `Cabeza_Derecha`.
- Cuerpo: `Cuerpo_Horizontal`, `Cuerpo_Vertical`, `Cuerpo_Curva_01`, `Cuerpo_Curva_02`, `Cuerpo_Curva_03`, `Cuerpo_Curva_04`.
- Colas: `Cola_Arriba`, `Cola_Abajo`, `Cola_Izquierda`, `Cola_Derecha`.
- Objetos y terreno: `Manzana_Roja`, `Manzana_Amarilla`, `Bloque_Piedra`, `Bloque_Cesped`, `Corazon`, `Estrella`.

Los sprites permanecen como subrecursos de **una única textura** importada desde el archivo `.aseprite`; no se exportaron como archivos PNG separados.

---

## Fondo del escenario

### Información del recurso

- **Recurso:** Fondo utilizado para el escenario de Snake.
- **Archivo utilizado:** `snake.jpg`
- **Ruta dentro del proyecto:** `Assets/Arte/Fondo/snake.jpg`
- **Fuente consultada:** Coding Blocks Discussion Forum.
- **Publicación:** `IAM not able to find this image in Google`
- **Compartido en la publicación por:** Aarnav Jindal.
- **Fecha de publicación:** 26 de marzo de 2020.
- **Fecha de consulta:** 1 de octubre de 2026.
- **Uso dentro del proyecto:** Fondo visual del tablero principal del juego Snake.

### Fuente

https://discuss.codingblocks.com/t/iam-not-able-to-find-this-image-in-google/33643

### Licencia

La publicación consultada no especifica una licencia de uso para la imagen.

Por este motivo, la documentación únicamente registra el lugar desde el cual se obtuvo el recurso y no atribuye la autoría original de la imagen al usuario que la compartió.

Si el proyecto requiriera posteriormente una distribución pública o comercial, se recomienda verificar los derechos del recurso o reemplazarlo por uno con una licencia claramente especificada.

---

# Convención para nuevos recursos

Cada nuevo recurso externo incorporado al proyecto deberá documentarse dentro de este archivo.

Para imágenes:

```text
Nombre:
Archivo:
Ruta:
Fuente:
Autor o usuario:
Licencia:
Fecha de consulta:
Uso dentro del proyecto:
```

Para audio:

```text
Nombre:
Archivo:
Ruta:
Fuente:
Autor:
Licencia:
Fecha de consulta:
Uso dentro del proyecto:
```

No se debe indicar que un recurso es propio o libre de derechos si dicha información no puede verificarse.

---

# Control de versiones

El proyecto utiliza Git para controlar los cambios realizados durante el desarrollo.

El repositorio remoto es:

```text
https://github.com/SotoXo/GameJam.git
```

---

# Flujo de ramas

Se utiliza una estructura basada en Git Flow.

```text
main
 │
 └── develop
      │
      ├── feature/fondo-snake
      ├── feature/movimiento-serpiente
      ├── feature/comida
      ├── feature/puntuacion
      └── feature/interfaz
```

## Rama main

```text
main
```

Contiene las versiones estables del proyecto.

No se recomienda realizar desarrollo directamente sobre esta rama.

---

## Rama develop

```text
develop
```

Es la rama de integración.

Las funcionalidades desarrolladas por el equipo se incorporan aquí antes de pasar a una versión estable en `main`.

El fondo y las colisiones actuales del escenario están siendo integrados en esta rama.

---

## Ramas feature

Cada funcionalidad puede desarrollarse independientemente.

Ejemplo:

```bash
git switch develop
git pull

git switch -c feature/movimiento-serpiente
```

Al finalizar:

```bash
git add .
git commit -m "feat: agregar movimiento de la serpiente"
git push -u origin feature/movimiento-serpiente
```

Posteriormente la funcionalidad puede integrarse en `develop`.

---

# Convención de commits

Se utiliza una convención sencilla para identificar el propósito de cada cambio.

```text
feat: nueva funcionalidad
fix: corrección de error
docs: documentación
refactor: reorganización de código
style: cambios visuales
chore: configuración o mantenimiento
```

Ejemplos:

```text
feat: agregar fondo del juego Snake
feat: agregar paredes y colisiones al escenario
feat: implementar movimiento de la serpiente
fix: corregir colision con pared inferior
docs: documentar recursos utilizados
style: mejorar interfaz del juego
```

---

# Archivos `.meta` de Unity

Los archivos `.meta` generados por Unity deben mantenerse en Git.

Ejemplo:

```text
snake.jpg
snake.jpg.meta
```

Unity utiliza los archivos `.meta` para mantener identificadores y referencias entre recursos.

Eliminar estos archivos puede provocar que:

- se pierdan referencias;
- desaparezcan sprites asignados;
- se rompan escenas;
- se generen nuevos identificadores incompatibles con los de otros integrantes.

Por esta razón, los archivos `.meta` forman parte del repositorio.

---

# Cómo abrir el proyecto

## 1. Clonar el repositorio

```bash
git clone https://github.com/SotoXo/GameJam.git
```

## 2. Entrar al proyecto

```bash
cd GameJam
```

## 3. Cambiar a develop

```bash
git switch develop
```

## 4. Actualizar el proyecto

```bash
git pull
```

## 5. Abrir desde Unity Hub

En Unity Hub:

```text
Add
→ Add project from disk
→ Seleccionar GameJam
```

Abrir utilizando la versión compatible de Unity 6.3 LTS.

Unity reconstruirá automáticamente las carpetas locales que no se encuentran almacenadas en Git, incluyendo:

```text
Library/
Temp/
Logs/
```

---

# Trabajo colaborativo

Antes de comenzar a trabajar se recomienda actualizar la rama correspondiente:

```bash
git switch develop
git pull
```

Al finalizar una modificación:

```bash
git status
git add <archivos>
git commit -m "tipo: descripcion del cambio"
git push
```

Se recomienda agregar únicamente los archivos relacionados con la funcionalidad desarrollada y revisar siempre:

```bash
git status
```

antes de realizar el commit.

Esto evita subir configuraciones locales o modificaciones generadas accidentalmente por Unity.

---

# Desarrollo actual del escenario

Actualmente el escenario dispone de:

```text
snake
│
└── Objeto existente del escenario

Paredes
│
├── Superior ─────── Box Collider 2D
├── Inferior ─────── Box Collider 2D
├── Izquierda ────── Box Collider 2D
└── Derecha ──────── Box Collider 2D

Serpiente
│
├── Cabeza
├── Segmento_01
├── Segmento_02
└── Cola

Comida
└── Manzana_Roja
```

Las paredes delimitan el área jugable.

`Serpiente` y `Comida` se encuentran dentro del área delimitada por las paredes. El controlador comprueba los bordes mediante la cuadrícula calculada a partir de los colliders existentes.

---

# Autoría y colaboración

Proyecto realizado como parte de una Game Jam.

### Trabajo documentado

**Limber Soto**

Participación actual:

- Integración del fondo del escenario.
- Organización de recursos gráficos.
- Implementación de los límites del tablero.
- Configuración de `Box Collider 2D`.
- Configuración del Tag `Pared`.
- Documentación del recurso gráfico utilizado.
- Gestión e integración de cambios mediante Git.

Los demás integrantes y responsabilidades pueden añadirse conforme avance el desarrollo.

---

# Notas

Este README representa el estado actual del proyecto.

Debe actualizarse cuando se incorporen nuevas funcionalidades, recursos externos, cambios importantes en la arquitectura del juego o modificaciones en el flujo de trabajo del equipo.
