namespace PlantDomain.Exceptions
{
    public class UserPlantNotFoundException:Exception
    {
        public UserPlantNotFoundException(Guid id)
        : base($"UserPlant with ID '{id}' was not found.") { }

    }
}
