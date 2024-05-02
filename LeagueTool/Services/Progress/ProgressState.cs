namespace LeagueTool.Services.Progress;

public class ProgressState
{
    private float _progress = 0;

    public ProgressState(string state, float progress)
    {
        State = state;
        Progress = progress;
    }
    
    public string State { get; set; } = "Initializing...";
    public float Progress
    {
        get { return _progress; }
        set
        {
            _progress = value;
            if (_progress >= 100 && FinishState == ProgressFinishState.Running)
            {
                FinishState = ProgressFinishState.Finished;
            }
        }
    }
    
    public ProgressFinishState FinishState { get; set; } = ProgressFinishState.Running;
}

public enum ProgressFinishState
{
    Running,
    Finished,
    Failed
}