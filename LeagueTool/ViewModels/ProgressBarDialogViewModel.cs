using ReactiveUI;

namespace LeagueTool.ViewModels;

public class ProgressBarDialogViewModel : ViewModelBase
{
    private string _currentStep = "Initializing...";
    private bool _isFinished = false;
    private float _currentProgress = 0;

    public string CurrentStep { 
        get => _currentStep;
        set => this.RaiseAndSetIfChanged(ref _currentStep, value); 
    }
    
    public float CurrentProgress { 
        get => _currentProgress;
        set => this.RaiseAndSetIfChanged(ref _currentProgress, value); 
    }

    public bool IsFinished
    {
        get => _isFinished;
        set => this.RaiseAndSetIfChanged(ref _isFinished, value);
    }
}
