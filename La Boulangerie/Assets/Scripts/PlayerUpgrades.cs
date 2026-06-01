using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    #region SINGLETON
    public static PlayerUpgrades Instance;
    #endregion

    #region UPGRADE LEVELS
    [Header("Upgrade Levels")]
    public int healthLevel = 0;         // 0 = 3 hearts, 3 = 6 hearts
    public int baguetteLevel = 0;       // 0 = 3 slots, 3 = 6 slots
    public int staminaLevel = 0;        // 0 = default drain, 2 = slowest drain
    public int movementLevel = 0;       // 0 = default movement, 2 = fastest movement
    public int jumpLevel = 0;           // 0 = default jump, 2 = highest jump
    public int baguetteDamageLevel = 0; // 0 = default damage, 2 = +2 damage
    #endregion

    #region MAX VALUES
    public int MaxHearts => Mathf.Clamp(3 + healthLevel, 3, 6);
    public int MaxBaguettes => Mathf.Clamp(3 + baguetteLevel, 3, 6);
    public int BaguetteDamageBonus => Mathf.Clamp(baguetteDamageLevel, 0, 2);
    #endregion

    #region AWAKE
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadUpgrades();
    }
    #endregion

    #region LOAD AND SAVE
    public void LoadUpgrades()
    {
        healthLevel = PlayerPrefs.GetInt("Upgrade_Health", 0);
        baguetteLevel = PlayerPrefs.GetInt("Upgrade_Baguette", 0);
        staminaLevel = PlayerPrefs.GetInt("Upgrade_Stamina", 0);
        movementLevel = PlayerPrefs.GetInt("Upgrade_Movement", 0);
        jumpLevel = PlayerPrefs.GetInt("Upgrade_Jump", 0);
        baguetteDamageLevel = PlayerPrefs.GetInt("Upgrade_BaguetteDamage", 0);

        ClampUpgradeLevels();
    }

    public void SaveUpgrades()
    {
        ClampUpgradeLevels();

        PlayerPrefs.SetInt("Upgrade_Health", healthLevel);
        PlayerPrefs.SetInt("Upgrade_Baguette", baguetteLevel);
        PlayerPrefs.SetInt("Upgrade_Stamina", staminaLevel);
        PlayerPrefs.SetInt("Upgrade_Movement", movementLevel);
        PlayerPrefs.SetInt("Upgrade_Jump", jumpLevel);
        PlayerPrefs.SetInt("Upgrade_BaguetteDamage", baguetteDamageLevel);
        PlayerPrefs.Save();
    }

    public void ResetUpgrades()
    {
        healthLevel = 0;
        baguetteLevel = 0;
        staminaLevel = 0;
        movementLevel = 0;
        jumpLevel = 0;
        baguetteDamageLevel = 0;
        SaveUpgrades();
    }

    private void ClampUpgradeLevels()
    {
        healthLevel = Mathf.Clamp(healthLevel, 0, 3);
        baguetteLevel = Mathf.Clamp(baguetteLevel, 0, 3);
        staminaLevel = Mathf.Clamp(staminaLevel, 0, 2);
        movementLevel = Mathf.Clamp(movementLevel, 0, 2);
        jumpLevel = Mathf.Clamp(jumpLevel, 0, 2);
        baguetteDamageLevel = Mathf.Clamp(baguetteDamageLevel, 0, 2);
    }
    #endregion

    #region APPLY TO PLAYER
    public void ApplyToPlayer(Player player)
    {
        if (player == null) return;

        LoadUpgrades();
        player.ApplySavedUpgrades(true);
    }
    #endregion
}
