namespace Com.Apdcomms.DataGateway.LocationsService.Database
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDatabase
    {
        Task Update(Location location);

        Task Create(Location location);
        
        Task<Location> Get(string locationId);
        
		Task<IEnumerable<Location>> GetAll();
    }
}