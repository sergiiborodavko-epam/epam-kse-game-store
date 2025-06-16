using System.Text;

namespace EpamKse.GameStore.Services.Helpers.License;
using Domain.Entities;

public class LicenseBuilder
{
    public LicenseBuilder(License license)
    {
        _license = license;
    }

    public byte[] Build()
    {
        var order = _license.Order;
        var user = order.User;
        var gamesList = GetGamesList(order.Games);
        
        var licenseTxt = _pattern.Replace("{licenseId}", _license.Id.ToString())
            .Replace("{orderId}", order.Id.ToString())
            .Replace("{userFullName}", user.FullName)
            .Replace("{userEmail}", user.Email)
            .Replace("{gamesList}", string.Join("\n", gamesList))
            .Replace("{orderTotalSum}", order.TotalSum.ToString())
            .Replace("{licenseKey}", _license.Key);
        
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
    
    private readonly License _license;
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