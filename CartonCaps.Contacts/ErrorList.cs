namespace CartonCaps.Contracts;

public class ErrorList
{
    public List<string> Errors { get; set; }

    public ErrorList(List<string> errors)
    {
        Errors = errors;
    }

    public ErrorList(string error)
    {
        Errors = new List<string> { error };
    }
}
