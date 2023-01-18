using SixMinApi.Models;

namespace SixMinApi.Data;
public interface ICommandRepo
{
    Task SaveChangesAsync();
    Task<Command?> GetCommandByIdAsync(int id);
    Task<IEnumerable<Command>> GetAllCommandsAsync();    
    Task CreateCommandAsync(Command cmd);

    void DeleteCommand(Command cmd);
}