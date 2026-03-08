using Movie2Image.Process.Domain.Exceptions;

namespace Movie2Image.Process.Domain.Validation;

public class Validator
{

    private readonly List<string> errors = [];


    private Validator() { }


    public static Validator Create()
    {
        return new Validator();
    }

    public Validator Test(bool condition, string errorMessage)
    {
        if (!condition)
            errors.Add(errorMessage);

        return this;
    }

    public Validator IsNotNull(object obj)
    {
        return Test(obj != null, "should not be null.");
    }


    public void Validate()
    {
        if (errors.Count > 0)
            throw new ValidationException(errors);
    }

}
