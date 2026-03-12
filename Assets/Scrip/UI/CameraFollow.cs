using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float minX, maxX;
    public float minY, maxY;

    [Header("Targeting")]
    // El objeto que la cámara debe seguir (normalmente el Jugador)
    [SerializeField] private Transform target;

    // Tiempo aproximado de respuesta. Menos tiempo = cámara más rígida.
    [SerializeField] private float smoothTime = 0.25f;

    [Header("Settings")]
    private Vector3 offset;
    private Vector3 currentVelocity = Vector3.zero; // Referencia interna para el cálculo de inercia

    private void Start()
    {
        // Calculamos la distancia inicial para mantener la perspectiva elegida en el Editor
        if (target != null)
            offset = transform.position - target.position;
    }

    /* ¿Por qué LateUpdate?
     * En Unity, el movimiento del jugador ocurre en Update o FixedUpdate. 
     * Si moviéramos la cámara en Update, habría frames donde la cámara se mueve ANTES
     * que el jugador, causando que la imagen 'tiemble'. 
     * LateUpdate garantiza que la cámara se mueva después de que el jugador ya terminó su posición en ese frame.
     */

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculamos la posición deseada sumando el desfase original
        Vector3 targetPos = target.position + offset;

        // IMPORTANTE: En 2D/TopDown, no queremos que la cámara cambie su profundidad (Z)
        // ya que podría acercarse o alejarse del suelo y perderíamos el renderizado.
        // esto asegura que la cámara siempre esté a la misma distancia del plano de juego.

        float clampX = Mathf.Clamp(targetPos.x, minX, maxX);
        float clampY = Mathf.Clamp(targetPos.y, minY, maxY);

        Vector3 finalPos = new Vector3(clampX, clampY, transform.position.z);

        targetPos.z = transform.position.z;

        /*
         * En las propiedades del Rigidbody2D, interpolate debería estar en 'Interpolate' no en 'None'.
         * 1. La Física corre a una frecuencia fija (ej. 50Hz - FixedUpdate).
         * 2. La Cámara corre a los FPS del monitor (ej. 60Hz o 144Hz - Update/LateUpdate).
         * 3. Como los tiempos no coinciden, hay frames donde el objeto visualmente 'salta'.
         *
         * AL ACTIVAR 'INTERPOLATE' en el Rigidbody2D:
         * Unity crea una posición intermedia suavizada para el jugador basada en sus frames 
         * anteriores. Así, cuando SmoothDamp busca al jugador, este siempre está en una 
         * posición fluida, eliminando el jitter por completo.
         */

        transform.position = Vector3.SmoothDamp(
            transform.position,
            finalPos,

            ref currentVelocity,
            smoothTime
        );
    }
}