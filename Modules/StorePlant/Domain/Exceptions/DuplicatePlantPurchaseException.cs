

namespace PlantDomain.Exceptions
{
    public class DuplicatePlantPurchaseException: Exception
    {
        public DuplicatePlantPurchaseException(Guid userId, Guid plantId)
      : base($"User '{userId}' already owns plant '{plantId}'.") { }
    }
}
