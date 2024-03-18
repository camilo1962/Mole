using System.Collections;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class Mole : MonoBehaviour {
  [Header("Graphics")]
  [SerializeField] private Sprite mole;
  [SerializeField] private Sprite moleHardHat;
  [SerializeField] private Sprite moleHatBroken;
  [SerializeField] private Sprite moleHit;
  [SerializeField] private Sprite moleHatHit;

  [Header("GameManager")]
  [SerializeField] private GameManager gameManager;

  // The offset of the sprite to hide it.
  private Vector2 startPosition = new Vector2(0f, -2.56f);
  private Vector2 endPosition = Vector2.zero;
  // How long it takes to show a mole.
  private float showDuration = 0.5f;
  private float duration = 1f;

  private SpriteRenderer spriteRenderer;
  private Animator animator;
  private BoxCollider2D boxCollider2D;
  private Vector2 boxOffset;
  private Vector2 boxSize;
  private Vector2 boxOffsetHidden;
  private Vector2 boxSizeHidden;

  // Mole Parameters 
  private bool hittable = true;
  public GameObject prefabMartillo;
    private Vector3 posicionObjetivo;
  public enum MoleType { Standard, HardHat, Bomb };
  private MoleType moleType;
  private float hardRate = 0.15f;
  private float bombRate = 0f;
  private int lives;
  private int moleIndex = 0;

  private IEnumerator ShowHide(Vector2 start, Vector2 end) {
    // Make sure we start at the start.
    transform.localPosition = start;

    // mostrar el moñaco.
    float elapsed = 0f;
    while (elapsed < showDuration) {
      transform.localPosition = Vector2.Lerp(start, end, elapsed / showDuration);
      boxCollider2D.offset = Vector2.Lerp(boxOffsetHidden, boxOffset, elapsed / showDuration);
      boxCollider2D.size = Vector2.Lerp(boxSizeHidden, boxSize, elapsed / showDuration);
      // Actualizar a la velocidad de fotogramas máxima.
      elapsed += Time.deltaTime;
      yield return null;
    }

     // .Asegúrate de que estamos exactamente al final
    transform.localPosition = end;
    boxCollider2D.offset = boxOffset;
    boxCollider2D.size = boxSize;

    // Wait for duration to pass.
    yield return new WaitForSeconds(duration);

    // ocultar el topo.
    elapsed = 0f;
    while (elapsed < showDuration)
    {
      transform.localPosition = Vector2.Lerp(end, start, elapsed / showDuration);
      boxCollider2D.offset = Vector2.Lerp(boxOffset, boxOffsetHidden, elapsed / showDuration);
      boxCollider2D.size = Vector2.Lerp(boxSize, boxSizeHidden, elapsed / showDuration);
      // Update at max framerate.
      elapsed += Time.deltaTime;
      yield return null;
    }
    // Asegúrate de que estamos exactamente de vuelta en la posición inicial..
    transform.localPosition = start;
    boxCollider2D.offset = boxOffsetHidden;
    boxCollider2D.size = boxSizeHidden;

    // Si llegamos al final y todavía es accesible, entonces nos lo perdimos..
    if (hittable)
    {
      hittable = false;
      // Solo damos penalización de tiempo si no es una bomba.
      gameManager.Missed(moleIndex, moleType != MoleType.Bomb);
    }
  }

  public void Hide()
  {
    // Establezca los parámetros mole apropiados para ocultarlo.
    transform.localPosition = startPosition ;
    boxCollider2D.offset = boxOffsetHidden;
    boxCollider2D.size = boxSizeHidden;
  }

  private IEnumerator QuickHide()
  {
    yield return new WaitForSeconds(0.25f);
    // Mientras esperábamos, es posible que hayamos aparecido de nuevo aquí, así que solo
    // comprueba que eso no ha pasado antes de ocultarlo. Esto lo detendrá
    // parpadeando en ese caso.
    if (!hittable)
    {
      Hide();
    }
  }
    public void Update()
    {

       //posicionObjetivo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
       //posicionObjetivo.z = transform.position.z;
       

    }

    private void OnMouseDown() {

        posicionObjetivo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        posicionObjetivo.z = transform.position.z;
        if (hittable)
        {
            
           switch (moleType)
           {
             case MoleType.Standard:

               
               spriteRenderer.sprite = moleHit;
               Instantiate(prefabMartillo, posicionObjetivo, Quaternion.identity);
               gameManager.AddScore(moleIndex);
               // Stop the animation
               StopAllCoroutines();
               StartCoroutine(QuickHide());
               // Desactive hittable para que no podamos seguir tocando para obtener puntaje.
               hittable = false;
               break;
             case MoleType.HardHat:
               // Si vidas == 2 reduce, y cambia de sprite.
               if (lives == 2)
               {
                 Instantiate(prefabMartillo, posicionObjetivo, Quaternion.identity);
                 spriteRenderer.sprite = moleHatBroken;
                 lives--;
               }
               else
               {
                 spriteRenderer.sprite = moleHatHit;
                 gameManager.AddScore(moleIndex);
                 // Stop the animation
                 StopAllCoroutines();
                 StartCoroutine(QuickHide());
                 //Desactive hittable para que no podamos seguir tocando para obtener puntaje.
                 hittable = false;
               }
               break;
             case MoleType.Bomb:
               // Game over, 1 for bomb.
               Instantiate(prefabMartillo, posicionObjetivo, Quaternion.identity);
               gameManager.GameOver(1);
               break;
               default:
               break;
           }
        }
    }

  private void CreateNext() {
    float random = Random.Range(0f, 1f);
    if (random < bombRate) {
      //hacer una bomba.
      moleType = MoleType.Bomb;
      // The animator handles setting the sprite.
      animator.enabled = true;
    } else {
      animator.enabled = false;
      random = Random.Range(0f, 1f);
      if (random < hardRate) {
        //Crea uno duro
        moleType = MoleType.HardHat;
        spriteRenderer.sprite = moleHardHat;
        lives = 2;
      } else {
        // Create a standard one.
        moleType = MoleType.Standard;
        spriteRenderer.sprite = mole;
        lives = 1;
      }
    }
    // Mark as hittable so we can register an onclick event.
    hittable = true;
  }

  // As the level progresses the game gets harder.
  private void SetLevel(int level) {
    // As level increases increse the bomb rate to 0.25 at level 10.
    bombRate = Mathf.Min(level * 0.025f, 0.25f);

    // Increase the amounts of HardHats until 100% at level 40.
    hardRate = Mathf.Min(level * 0.025f, 1f);

    // Duration bounds get quicker as we progress. No cap on insanity.
    float durationMin = Mathf.Clamp(1 - level * 0.1f, 0.01f, 1f);
    float durationMax = Mathf.Clamp(2 - level * 0.1f, 0.01f, 2f);
    duration = Random.Range(durationMin, durationMax);
  }

  private void Awake() {
    // Get references to the components we'll need.
    spriteRenderer = GetComponent<SpriteRenderer>();
    animator = GetComponent<Animator>();
    boxCollider2D = GetComponent<BoxCollider2D>();
    // Work out collider values.
    boxOffset = boxCollider2D.offset;
    boxSize = boxCollider2D.size;
    boxOffsetHidden = new Vector2(boxOffset.x, -startPosition.y / 2f);
    boxSizeHidden = new Vector2(boxSize.x, 0f);
  }

  public void Activate(int level) {
    SetLevel(level);
    CreateNext();
    StartCoroutine(ShowHide(startPosition, endPosition));
  }

  // Used by the game manager to uniquely identify moles. 
  public void SetIndex(int index) {
    moleIndex = index;
  }

  // Used to freeze the game on finish.
  public void StopGame() {
    hittable = false;
    StopAllCoroutines();
  }
}
