using System;

[Serializable]
public struct UpgradeCost
{
    public int flour;
    public int milk;
    public int butter;
    public int totalIngredients;
    public bool useTotalIngredients;

    public UpgradeCost(int flour, int milk, int butter)
    {
        this.flour = flour;
        this.milk = milk;
        this.butter = butter;
        totalIngredients = 0;
        useTotalIngredients = false;
    }

    public UpgradeCost(int totalIngredients)
    {
        flour = 0;
        milk = 0;
        butter = 0;
        this.totalIngredients = totalIngredients;
        useTotalIngredients = true;
    }

    public string ToRequirementText()
    {
        if (useTotalIngredients)
            return "Requires " + totalIngredients + " total ingredients";

        return "Requires " + flour + " Flour, " + milk + " Milk, " + butter + " Butter";
    }
}
