using System.Text;

namespace EpamKse.GameStore.Services.Services.License;
using Domain.Entities;

public class LicenseBuilder : ILicenseBuilder
{
    public LicenseBuilder() {}

    public byte[] Build(License license)
    {
        var order = license.Order;
        var user = order.User;
        var gamesList = GetGamesList(order.Games);
        
        var licenseTxt = _pattern.Replace("{licenseId}", license.Id.ToString())
            .Replace("{orderId}", order.Id.ToString())
            .Replace("{userFullName}", user.FullName)
            .Replace("{userEmail}", user.Email)
            .Replace("{gamesList}", string.Join("\n", gamesList))
            .Replace("{orderTotalSum}", order.TotalSum.ToString())
            .Replace("{licenseKey}", license.Key);
        
        return Encoding.UTF8.GetBytes(licenseTxt);
    }
    
    private List<string> GetGamesList(IEnumerable<Game> games)
    {
        var gamesTxt = new List<string>();
        var index = 1;
        foreach (var game in games)
        {
            var line = $"{index}. {game.Title}; Price: {game.Price}";
            gamesTxt.Add(line);
            index++;
        }
        return gamesTxt;
    }
    
    private readonly string _pattern = """
                          License №{licenseId} for order №{orderId}
                          Owner: {userFullName}
                          Email: {userEmail}

                          Contents:
                          {gamesList}
                          Total sum: {orderTotalSum}

                          License key: {licenseKey}
                          """;
}