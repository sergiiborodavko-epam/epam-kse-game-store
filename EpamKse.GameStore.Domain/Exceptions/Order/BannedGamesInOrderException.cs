namespace EpamKse.GameStore.Domain.Exceptions.Order;

public class BannedGamesInOrderException(string gamesList) 
    : CustomHttpException(403, $"Order contains games banned in your region: {gamesList}");
