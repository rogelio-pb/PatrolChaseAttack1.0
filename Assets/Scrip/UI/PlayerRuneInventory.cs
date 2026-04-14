using UnityEngine;
using TMPro;

public class PlayerRuneInventory : MonoBehaviour
{
    [Header("Rune Settings")]
    [SerializeField] private int maxRunes = 3;
    [SerializeField] private int currentRunes = 0;
    [SerializeField] private int totalRunesDelivered = 0;
    [SerializeField] private int runesToWin = 10;

    [Header("Score")]
    [SerializeField] private int pointsPerRune = 10;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI runeText;


    private PlayerStateManager player;

    private void Start()
    {
        player = GetComponent<PlayerStateManager>();
        UpdateUI();
    }

    // =========================
    // RECOGER RUNA
    // =========================
    public bool AddRune()
    {
        if (currentRunes >= maxRunes)
        {
            Debug.Log("Inventario lleno");
            return false;
        }

        currentRunes++;
        UpdateUI();

        Debug.Log("Runas cargadas: " + currentRunes);
        return true;
    }
    // =========================
    // ACCEDER A LOS PUNTOS
    // =========================
    public int GetTotalRunesDelivered()
    {
        return totalRunesDelivered;
    }

    // =========================
    // ENTREGAR RUNAS
    // =========================
    public void DeliverRunes()
    {
        if (currentRunes <= 0) return;

        int totalPoints = currentRunes * pointsPerRune;

        player.AddPoints(totalPoints);

       
        totalRunesDelivered += currentRunes;

        Debug.Log("Runas entregadas: " + currentRunes);
        Debug.Log("Total entregadas: " + totalRunesDelivered);

        currentRunes = 0;
        UpdateUI();

      
        if (totalRunesDelivered >= runesToWin)
        {
            player.WinFromRunes(); 
        }
    }

    private void UpdateUI()
    {
        if (runeText != null)
            runeText.text = $"Runas: {currentRunes}/{maxRunes}";
    }
}