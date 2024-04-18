using System.Threading.Tasks;
using Avalonia.Interactivity;
using AvaloniaDialogs.Views;

namespace LeagueTool.Views.Dialogs;

public partial class ProgressBarDialog : BaseDialog<bool>
{
    
    public ProgressBarDialog()
    {
        InitializeComponent();
    }
    
    private void ButtonConfirm_Click(object? sender, RoutedEventArgs e)
    {
        Close(true);
    }
}
