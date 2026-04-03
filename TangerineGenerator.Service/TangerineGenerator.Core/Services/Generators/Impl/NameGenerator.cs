using TangerineAuction.Shared.Enums;

namespace TangerineGenerator.Core.Services.Generators.Impl;

internal class NameGenerator : INameGenerator
{
    
    public string Generate(TangerineQuality quality)
    {
        return quality switch
        {
            TangerineQuality.Common => GenerateRussianName([
                "Сочная", "Тёплая", "Оранжевая", "Лёгкая"
            ]),

            TangerineQuality.Uncommon => GenerateRussianName([
                "Отборная", "Золотистая", "Нежная", "Ароматная"
            ]),

            TangerineQuality.Rare => GenerateRareName(),

            TangerineQuality.Legendary => GenerateLegendaryCode(),

            _ => throw new NotImplementedException($"Не обработан вариант {quality}")
        };
    }

    private string GenerateRussianName(string[] adjectives)
        => $"{adjectives[Random.Shared.Next(adjectives.Length)]} мандаринка";

    private string GenerateRareName()
    {
        var rareNames = new[]
        {
            "Éclatante mandarine",
            "Sublime mandarine",
            "Bellissima mandarino",
            "Splendida mandarino",
            "Wunderschöne Mandarine",
            "Herrliche Mandarine",
            "Radiante mandarina",
            "Hermosa mandarina"
        };

        return rareNames[Random.Shared.Next(rareNames.Length)];
    }

    private string GenerateLegendaryCode()
    {
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        var letter = letters[Random.Shared.Next(letters.Length)];
        var number1 = Random.Shared.Next(0, 100).ToString("D2");
        var number2 = Random.Shared.Next(0, 10);

        return $"{letter}{number1}-{number2}";
    }
    
}