using System.Threading.Tasks;

namespace MenuGenerator.ViewModel;

public interface IMainPage
{
    public bool IsProcessing { get; set; }

    public Task LoadAsync();
}